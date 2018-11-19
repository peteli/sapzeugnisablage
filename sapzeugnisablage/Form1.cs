using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using System.Windows.Forms;


namespace sapzeugnisablage
{
    public partial class Form1 : Form
    {
        //root directory of certificates
        private SapCertDirectory CertDirectory;// = new SapCertDirectory(Properties.Settings.Default.certificateRootFolder);

        // fullpathstring of root directory
        private string SapCertFullPath
        {
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
                    InvokeUpdateProgressBar(null, new TaskProgressedEventArgs(0, 0, 0, ""));
                    return false;
                });
            };
            this.button3.Click += (o, data) =>
            {
                if (!backgroundWorkerCertificateProcessing.IsBusy)
                {
                    backgroundWorkerCertificateProcessing.RunWorkerAsync();
                }
            };

        }



        //public delegate void UpdateProgressBar(TaskProgressedEventArgs e);

        private void InvokeUpdateProgressBar(object sender, TaskProgressedEventArgs e)
        {
            //progressBar1.Invoke(new UpdateProgressBar(UpdateProgressBarControl), new object[] { e }) ;

            toolStripProgressBar1.GetCurrentParent().Invoke(new MethodInvoker(delegate
            {
                toolStripProgressBar1.Minimum = (int)e.Start;
                toolStripProgressBar1.Maximum = (int)e.End;
                toolStripProgressBar1.Value = (int)e.Val;
                //toolStripProgressBar1.ToolTipText = e.Text;
            }), new object[] { e });

        }


        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            this.toolStripStatusLabel1.Text = CertFolderStatusStrip;
        }

        private void backgroundWorkerCertificateProcessing_DoWork(object sender, DoWorkEventArgs e)
        {
            CertDirectory.ProcessCertificateFiles(CertDirectory.CertFilesUnprocessed, CertDirectory.CertFilesPDFunprocessed);
            
        }

        private void backgroundWorkerCertificateProcessing_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Zertifikate wurden verarbeitet");
        }
    }
}