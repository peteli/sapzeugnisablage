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
        SapCertDirectory CertDirectory = new SapCertDirectory(Properties.Settings.Default.certificateRootFolder);
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            CertDirectory = new SapCertDirectory(Properties.Settings.Default.certificateRootFolder);
            this.textBox1.Text = CertDirectory.CertRootFolderString;
            this.folderBrowserDialog1.SelectedPath = CertDirectory.CertRootFolderString;
            this.GetSub();
        }

        private void GetSub()
        {
            
            //DirectoryInfo myDir = new DirectoryInfo(textBox1.Text);

            //List<DirectoryInfo> subDirNames = new List<DirectoryInfo>(myDir.EnumerateDirectories());

            //List<DirectoryInfo> subDirCertFolders = subDirNames.FindAll(obj => Regex.IsMatch(obj.Name, Properties.Settings.Default.certificateNumberRegExp));

            CertDirectory.SubDirFolders.ForEach(obj => Console.WriteLine("found:" + obj.FullName));
            
            CertDirectory.SubDirCertFolders.ForEach(obj => Console.WriteLine("cert found:" + obj.FullName));

            //List<uint> subDirCertNames = subDirCertFolders.ConvertAll(obj => Convert.ToUInt32(obj.Name));

            CertDirectory.SubDirCertFolderNumbers.ForEach(obj => Console.WriteLine("cert name: " + obj));
        }

    }

    /// <summary>
    /// This class will serve as controller and model in model-view-controller model
    /// </summary>
    public class SapCertDirectory
    {
        public string CertRootFolderString; //root directory of sap certificates folders
        public DirectoryInfo CertRootFolder { get { return new DirectoryInfo(CertRootFolderString); } }
        public List<DirectoryInfo> SubDirFolders { get { return new List<DirectoryInfo>(CertRootFolder.EnumerateDirectories()); } }
        public List<DirectoryInfo> SubDirCertFolders { get { return SubDirFolders.FindAll(obj => Regex.IsMatch(obj.Name, Properties.Settings.Default.certificateNumberRegExp)); } }
        public List<string> SubDirCertFolderNames { get { return SubDirCertFolders.ConvertAll(obj => obj.Name); } }
        public List<uint> SubDirCertFolderNumbers { get { return SubDirCertFolders.ConvertAll(obj => Convert.ToUInt32(obj.Name)); } }
        public SapCertDirectory(string fullPath)
        {
            // make sure the path exists on this machine
            CertRootFolderString = Directory.Exists(fullPath) ? fullPath : Properties.Settings.Default.certificateRootFolder;
            CertRootFolderString = Directory.Exists(CertRootFolderString) ? CertRootFolderString : Environment.SpecialFolder.Personal.ToString();
        }


        private bool FindCertificateFolder(DirectoryInfo obj)
        {
            if (Regex.IsMatch(obj.Name, Properties.Settings.Default.certificateNumberRegExp)) { return true; } else { return false; }
        }

    }
}
