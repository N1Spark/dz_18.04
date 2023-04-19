using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace dz_18._04
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            listView1.Columns.Add("Имя файла", 200);
            listView1.Columns.Add("Размер", 100);
            listView1.Columns.Add("Дата изменения", 150);
            string[] drives = System.IO.Directory.GetLogicalDrives();
            foreach (string drive in drives)
                comboBox1.Items.Add(drive);
        }
        private CancellationTokenSource cancellationTokenSource;
        private void button1_Click(object sender, EventArgs e)
        {
            string selectedDrive = comboBox1.SelectedItem.ToString();
            string searchPattern = textBox1.Text;
            string rootFolderPath = selectedDrive + "\\";
            listView1.Items.Clear();
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            Thread searchThread = new Thread(() =>
            {
                SearchFiles(rootFolderPath, searchPattern, cancellationToken);
            });
            searchThread.Start();
        }
        private void SearchFiles(string rootFolderPath, string searchPattern, CancellationToken cancellationToken)
        {
            try
            {
                DirectoryInfo rootFolder = new DirectoryInfo(rootFolderPath);
                foreach (DirectoryInfo subFolder in rootFolder.GetDirectories())
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;
                    try
                    {
                        SearchFiles(subFolder.FullName, searchPattern, cancellationToken);
                    }
                    catch (UnauthorizedAccessException)
                    { }
                }

                foreach (FileInfo file in rootFolder.GetFiles(searchPattern))
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;
                    ListViewItem item = new ListViewItem(file.Name);
                    item.SubItems.Add(file.Length.ToString());
                    item.SubItems.Add(file.LastWriteTime.ToString());
                    listView1.Items.Add(item);
                }
            }
            catch (UnauthorizedAccessException)
            { }
        }
        private void button2_Click(object sender, EventArgs e) => cancellationTokenSource.Cancel();

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
