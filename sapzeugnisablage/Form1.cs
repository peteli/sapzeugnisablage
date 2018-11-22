using System;
using System.ComponentModel;
using System.Text;
using System.IO;
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
            //this.propertyGrid1.SelectedObject = CertDirectory;
            this.textBox1.Text = CertDirectory.CertRootFolderString;
            this.folderBrowserDialog1.SelectedPath = CertDirectory.CertRootFolderString;
            this.toolStripStatusLabel1.Text = CertFolderStatusStrip;

            //Console.SetOut(new ConsoleWriter(textBoxConsoleOutput));
            //Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));

            //handler for progress bar
            CertDirectory.TaskProgressed += UpdateProgressBar;

            //handler for button create folders
            this.toolStripButtonCreateFolder.Click += async (o, data) =>
            {
                await Task.Run(() =>
                {
                    CertDirectory.CreateSubDirectories();
                });
            };
            
            // handler for button Process Certificate File (especially PDF)
            this.toolStripButtonProcessCertificate.Click += (o, data) =>
            {
                if (!backgroundWorkerCertificateProcessing.IsBusy)
                {
                    backgroundWorkerCertificateProcessing.RunWorkerAsync();
                }
            };

            //event handler for picking root directory of certificates
            this.toolStripButtonPickDirectory.Click += (o, data) =>
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    SapCertFullPath = folderBrowserDialog1.SelectedPath;
                }

            };

            //PopulateListView();

            listBox1.DataSource = CertDirectory.SubDirCertFolder;
            listBox1.DisplayMember = Name.ToString();
        }

        private void PopulateListView()
        {
            CertDirectory.SubDirCertFolder.ForEach(obj =>
            {
                ListViewItem newItem = new ListViewItem(obj);
                //listView1.Items.Add(newItem);
            });
        }

        // delegate for thread safe update of progress bar
        private delegate void UpdateProgressBarDelegate(object o, TaskProgressedEventArgs e);
        // update progress bar thread safe
        private void UpdateProgressBar(object o, TaskProgressedEventArgs e)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (statusStrip1.InvokeRequired)
            {
                //create new delegate
                UpdateProgressBarDelegate d = new UpdateProgressBarDelegate(UpdateProgressBar);// use the very same function to call it again in correct and save thread
                this.Invoke(d, new object[] {o,e});
            }
            else
            {
                toolStripProgressBar1.Minimum = (int)e.Start;
                toolStripProgressBar1.Maximum = (int)e.End;
                toolStripProgressBar1.Value = (int)e.Val;
                toolStripStatusLabel1.Text = string.IsNullOrEmpty(e.Text) ? CertFolderStatusStrip: e.Text ;
            }
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
            CertDirectory.ProcessCertificateFiles();
        }

        private void backgroundWorkerCertificateProcessing_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Zertifikate wurden verarbeitet","Hinweis", MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void toolStripProgressBar1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}