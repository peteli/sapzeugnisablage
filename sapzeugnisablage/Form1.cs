using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace sapzeugnisablage
{
    public partial class Form1 : Form
    {
        //root directory of certificates
        private SapCertDirectory CertDirectory = new SapCertDirectory(Properties.Settings.Default.certificateRootFolder);

        // fullpathstring of root directory
        private string SapCertFullPath {
            get { return CertDirectory.CertRootFolderString; }
            set
            {
                CertDirectory.CertRootFolderString = value;
                textBox1.Text = value;
            }
        }
        private string CertFolderStatusStrip
        {
            get { return new StringBuilder("Certificate folders from: " + CertDirectory.MinCertNumber + " to " + CertDirectory.MaxCertNumber).ToString(); }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                SapCertFullPath = folderBrowserDialog1.SelectedPath;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            CertDirectory = new SapCertDirectory(Properties.Settings.Default.certificateRootFolder);
            this.propertyGrid1.SelectedObject = CertDirectory;
            this.textBox1.Text = CertDirectory.CertRootFolderString;
            this.folderBrowserDialog1.SelectedPath = CertDirectory.CertRootFolderString;
            this.toolStripStatusLabel1.Text = CertFolderStatusStrip;

            //handler for progress bar
            CertDirectory.TaskProgressed += InvokeUpdateProgressBar;

            //handler for button create folders
            this.button2.Click += async (o, data) =>
            {
                var result = await Task.Run(() =>
                {
                    CertDirectory.CreateSubDirectories(CertDirectory.MaxCertNumber + 1, CertDirectory.CertNumberCycleNext);
                    InvokeUpdateProgressBar(null, new TaskProgressedEventArgs(0, 0, 0,""));
                    return false;
                });
            };
            
        }



        public delegate void UpdateProgressBar(TaskProgressedEventArgs e);

        private void InvokeUpdateProgressBar(object sender, TaskProgressedEventArgs e)
        {
            //progressBar1.Invoke(new UpdateProgressBar(UpdateProgressBarControl), new object[] { e }) ;

            toolStripProgressBar1.GetCurrentParent().Invoke(new MethodInvoker(delegate
            {
                toolStripProgressBar1.Minimum = (int)e.Start;
                toolStripProgressBar1.Maximum = (int)e.End;
                toolStripProgressBar1.Value = (int)e.Val;
                //toolStripProgressBar1.ToolTipText = e.Text;
            }),new object[] { e });

        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            //CertDirectory.CreateSubDirectories(CertDirectory.MaxCertNumber+1, CertDirectory.CertNumberCycleNext);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            this.toolStripStatusLabel1.Text = CertFolderStatusStrip;
        }

        private void propertyGrid1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }

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
        public List<DirectoryInfo> SubDirCertFolders { get { return SubDirFolders.FindAll(obj => Regex.IsMatch(obj.Name, Properties.Settings.Default.certificateNumberRegExp)); } }
        public List<FileInfo> CertFiles { get
            {
                List<FileInfo> certfiles = new List<FileInfo>();
                SubDirCertFolders.ForEach(certfolder =>
                {
                    certfiles.AddRange(certfolder.EnumerateFiles());
                });
                Console.WriteLine("Es gibt {0} Zertifikate", certfiles.Count);
                return certfiles;
            } }
        public List<FileInfo> CertFilesPDF { get { return CertFiles.FindAll(obj => (obj.Extension.ToLower() == ".pdf")); } }
        public List<string> SubDirCertFolderNames { get { return SubDirCertFolders.ConvertAll(obj => obj.Name); } }
        public List<uint> SubDirCertFolderNumbers { get { return SubDirCertFolders.ConvertAll(obj => Convert.ToUInt32(obj.Name)); } }
        public uint CertNumberRangeStart { get { return Properties.Settings.Default.certificateNumberRangeStart; } }
        public uint CertNumberCycleEndingDigits { get { return Properties.Settings.Default.certificateCreateCycleEndingDigits; } }
        public uint CertNumberCycleSafe { get { return PowerOfTenSafe(CertNumberRangeStart,CertNumberCycleEndingDigits); } }
        public uint CertNumberCycleNext { get { return GetMaxCertNumberNextCycle(MaxCertNumber, CertNumberCycleSafe); } }
        //public uint CertNumberRangeStart { get { return Properties.Settings.Default.certificateNumberRangeStart; } }
        public uint MaxCertNumber { get { return (SubDirCertFolderNumbers.Count > 1) ? SubDirCertFolderNumbers.Max() : this.CertNumberRangeStart - 1; } }
        public uint MinCertNumber { get { return (SubDirCertFolderNumbers.Count > 1) ? SubDirCertFolderNumbers.Min() : this.CertNumberRangeStart - 1; } }

        // function/methods
        public List<DirectoryInfo> CreateSubDirectories(uint startnumber, uint endnumber)
        {
            List<DirectoryInfo> NewSubCertFolders = new List<DirectoryInfo>();
            for(uint certnum = startnumber; certnum <= endnumber; certnum++)
            {
                OnTaskProgressed(new TaskProgressedEventArgs(startnumber,endnumber,certnum,"create folders"));

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

        private void PutFilesPDFintoFolder(List<DirectoryInfo> myFolders)
        {
            myFolders.ForEach(obj => 
            {
                Console.WriteLine("bin gerade bei: {0}", obj.FullName);
                //get fileinfos auf files in cert root directory

                List<FileInfo> FileTemplates = new List<FileInfo>(CertRootFolder.EnumerateFiles());

                FileTemplates.ForEach(f => 
                {
                    FileInfo newFile = f.CopyTo(obj.FullName + "\\" + f.Name);
                    if (newFile.Extension.ToLower() == ".pdf")
                    {
                        PDFactory.LabelonAllPages(newFile);
                    }
                });

            });
        }

        private uint GetMaxCertNumberNextCycle(uint maxCertNumber, uint certNumberCycleSafe)
        {
            var nextCertNumber = maxCertNumber + 1;
            while (!IsDigitMatch(nextCertNumber, certNumberCycleSafe)){
                nextCertNumber++;
                // raise event
                OnTaskProgressed(new TaskProgressedEventArgs(maxCertNumber, (maxCertNumber + certNumberCycleSafe + 1), nextCertNumber,"seeking next max cert"));
            }
            OnTaskProgressed(new TaskProgressedEventArgs(0, 0, 0, "ready"));
            return nextCertNumber;
        }

        private static bool IsDigitMatch(uint nextCertNumber, uint certNumberCycle)
        {
            // substract cyclenumber from nextcertnumber

            var num = nextCertNumber - certNumberCycle;
            var certNumberDigits = (int)Math.Floor((Math.Log10(certNumberCycle))+1); //get number of digits

            return ((num % Math.Pow(10, certNumberDigits)) == 0);
        }


        private bool FindCertificateFolder(DirectoryInfo obj)
        {
            if (Regex.IsMatch(obj.Name, Properties.Settings.Default.certificateNumberRegExp)) { return true; } else { return false; }
        }

        /// <summary>
        /// makes sure that cycle is always one digit smaller than number
        /// </summary>
        /// <param name="number"></param>
        /// <param name="cycle"></param>
        /// <returns>assured cycle</returns>
        public static uint PowerOfTenSafe(uint number,uint cycle)
        {
            if (Math.Log10(Math.Abs(number)) <= Math.Log10(Math.Abs(cycle))){
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

    public class TaskProgressedEventArgs : EventArgs
    {
        //constructor
        public TaskProgressedEventArgs(uint start,uint stop, uint val,string text)
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
}
