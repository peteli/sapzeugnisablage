using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace sapzeugnisablage
{
    /// <summary>
    /// This class will serve as controller and model in model-view-controller model
    /// </summary>
    public class SapCertDirectory
    {
        // constructor
        public SapCertDirectory(string fullPath)
        {
            // make sure the path exists on this machine
            CertRootFolderString = Directory.Exists(fullPath) ? fullPath : Properties.Settings.Default.certificateRootFolder;
            CertRootFolderString = Directory.Exists(CertRootFolderString) ? CertRootFolderString : Environment.SpecialFolder.Personal.ToString();
        }

        // event definitions
        public event EventHandler<TaskProgressedEventArgs> TaskProgressed;
        protected virtual void OnTaskProgressed(TaskProgressedEventArgs e)
        {
            TaskProgressed?.Invoke(this, e);
        }

        // properties
        public string CertRootFolderString { get; set; } //root directory of sap certificates folders
        public DirectoryInfo CertRootFolder { get { return new DirectoryInfo(CertRootFolderString); } }
        public List<DirectoryInfo> SubDirFolders { get { return new List<DirectoryInfo>(CertRootFolder.EnumerateDirectories()); } }
        public List<DirectoryInfo> SubDirCertFolders { get { return SubDirFolders.FindAll(obj => IsCertificateFolder(obj)); } }
        public List<FileInfo> CertFiles
        {
            get
            {
                List<FileInfo> certfiles = new List<FileInfo>();
                SubDirCertFolders.ForEach(certfolder =>
                {
                    certfiles.AddRange(certfolder.EnumerateFiles());
                });
                //Console.WriteLine("Es gibt {0} Zertifikate", certfiles.Count);
                return certfiles;
            }
        }
        public List<FileInfo> CertFilesProcessed { get { return CertFiles.FindAll(obj => IsProcessedCertificateFile(obj)); } }
        public List<FileInfo> CertFilesUnprocessed { get { return CertFiles.Except(CertFilesProcessed, new FileInfoComparer()).ToList(); } }
        public List<FileInfo> CertFilesPDF { get { return CertFiles.FindAll(obj => (obj.Extension.ToLower() == ".pdf")); } }
        public List<FileInfo> CertFilesPDFunprocessed { get { return CertFilesPDF.Except(CertFilesProcessed,new FileInfoComparer()).ToList(); } }
        public List<string> SubDirCertFolderNames { get { return SubDirCertFolders.ConvertAll(obj => obj.Name); } }
        public List<uint> SubDirCertFolderNumbers { get { return SubDirCertFolders.ConvertAll(obj => Convert.ToUInt32(obj.Name)); } }
        public uint CertNumberRangeStart { get { return Properties.Settings.Default.certificateNumberRangeStart; } }
        public uint CertNumberCycleEndingDigits { get { return Properties.Settings.Default.certificateCreateCycleEndingDigits; } }
        public uint CertNumberCycleSafe { get { return PowerOfTenSafe(CertNumberRangeStart, CertNumberCycleEndingDigits); } }
        public uint CertNumberCycleNext { get { return GetMaxCertNumberNextCycle(MaxCertNumber, CertNumberCycleSafe); } }
        //public uint CertNumberRangeStart { get { return Properties.Settings.Default.certificateNumberRangeStart; } }
        public uint MaxCertNumber { get { return (SubDirCertFolderNumbers.Count > 0) ? SubDirCertFolderNumbers.Max() : this.CertNumberRangeStart - 1; } }
        public uint MinCertNumber { get { return (SubDirCertFolderNumbers.Count > 0) ? SubDirCertFolderNumbers.Min() : this.CertNumberRangeStart - 1; } }

        // function/methods

        // create subdirectories thus user can put certificates into them
        public List<DirectoryInfo> CreateSubDirectories(uint startnumber, uint endnumber)
        {
            List<DirectoryInfo> NewSubCertFolders = new List<DirectoryInfo>();
            for (uint certnum = startnumber; certnum <= endnumber; certnum++)
            {
                OnTaskProgressed(new TaskProgressedEventArgs(startnumber, endnumber, certnum, "create folders"));

                try
                {
                    var newCertFolder = this.CertRootFolder.CreateSubdirectory(certnum.ToString());
                    NewSubCertFolders.Add(newCertFolder);
                }
                catch (IOException e)
                {
                    Console.Write(e.Data.ToString());
                }
            }

            //little insert here to put some pdf into the new folders
            PutFilesPDFintoFolder(NewSubCertFolders);
            //end of little insert - delete later

            return NewSubCertFolders;
        }

        // for test with put same random files into each certificate folders
        private void PutFilesPDFintoFolder(List<DirectoryInfo> myFolders)
        {
            myFolders.ForEach(obj =>
            {
                Console.WriteLine("bin gerade bei: {0}", obj.FullName);
                //get fileinfos auf files in cert root directory

                List<FileInfo> FileTemplates = new List<FileInfo>(CertRootFolder.EnumerateFiles());

                FileTemplates.ForEach(f => f.CopyTo(obj.FullName + "\\" + f.Name));

            });
        }

        // process certificate files -> rename + processing PDFs
        public void ProcessCertificateFile(List<FileInfo> files2process)
        {
            // first processing pdf files

            // second rename file
            string delimiterSign = Properties.Settings.Default.certificateFileNameSeparator;
            files2process.ForEach((f) =>
            {
                StringBuilder newFileName = new StringBuilder(f.DirectoryName)
                .Append(@"\")
                .Append(f.Directory.Name)
                .Append(delimiterSign)
                .Append(f.Name);

                // rename file by moving is (like linux :-)
                f.MoveTo(newFileName.ToString());
            });
        }



        private uint GetMaxCertNumberNextCycle(uint maxCertNumber, uint certNumberCycleSafe)
        {
            var nextCertNumber = maxCertNumber + 1;
            while (!IsDigitMatch(nextCertNumber, certNumberCycleSafe))
            {
                nextCertNumber++;
                // raise event
                OnTaskProgressed(new TaskProgressedEventArgs(maxCertNumber, (maxCertNumber + certNumberCycleSafe + 1), nextCertNumber, "seeking next max cert"));
            }
            OnTaskProgressed(new TaskProgressedEventArgs(0, 0, 0, "ready"));
            return nextCertNumber;
        }

        private static bool IsDigitMatch(uint nextCertNumber, uint certNumberCycle)
        {
            // substract cyclenumber from nextcertnumber

            var num = nextCertNumber - certNumberCycle;
            var certNumberDigits = (int)Math.Floor((Math.Log10(certNumberCycle)) + 1); //get number of digits

            return ((num % Math.Pow(10, certNumberDigits)) == 0);
        }

        private bool IsProcessedCertificateFile(FileInfo testFile)
        {
            if (Regex.IsMatch(testFile.Name, Properties.Settings.Default.certificateNumberFileNameRegExp)) { return true; } else { return false; }
        }
        private bool IsCertificateFolder(DirectoryInfo obj)
        {
            if (Regex.IsMatch(obj.Name, Properties.Settings.Default.certificateNumberRegExp)) { return true; } else { return false; }
        }

        /// <summary>
        /// makes sure that cycle is always one digit smaller than number
        /// </summary>
        /// <param name="number"></param>
        /// <param name="cycle"></param>
        /// <returns>assured cycle</returns>
        public static uint PowerOfTenSafe(uint number, uint cycle)
        {
            if (Math.Log10(Math.Abs(number)) <= Math.Log10(Math.Abs(cycle)))
            {
                int powerDiff = (int)Math.Floor((Math.Log10(number))) - (int)Math.Floor((Math.Log10(cycle))) - 1;
                double newPower4Cycle = Math.Log10(cycle) + powerDiff;
                return (uint)Math.Pow(10, newPower4Cycle);
            }
            else
            {
                return cycle;
            }
        }


    }

    // event argument definition
    public class TaskProgressedEventArgs : EventArgs
    {
        //constructor
        public TaskProgressedEventArgs(uint start, uint stop, uint val, string text)
        {
            this.Start = start;
            this.End = stop;
            this.Val = val;
            this.Text = text;
        }

        public uint Start { get; set; }
        public uint End { get; set; }
        public uint Val { get; set; }
        public string Text { get; set; }
    }

    // serves for FileInfo object comparison
    public class FileInfoComparer : IEqualityComparer<FileInfo>
    {
        // implement equal function
        public bool Equals(FileInfo x, FileInfo y)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.FullName == y.FullName;
        }


        //  implement get hashcode function
        public int GetHashCode(FileInfo f)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(f, null)) return 0;

            return f.FullName.GetHashCode();
        }
    }
}

