using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace L31
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            RefreshProcessList();
        }

        private void RefreshProcessList()
        {
            processListView.Items.Clear();
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                ListViewItem item = new ListViewItem(process.ProcessName);
                item.SubItems.Add(process.Id.ToString());
                item.SubItems.Add(process.MainWindowTitle);
                processListView.Items.Add(item);
            }
        }

        private void viewDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (processListView.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = processListView.SelectedItems[0];
                string processName = selectedItem.Text;
                int processId = int.Parse(selectedItem.SubItems[1].Text);

                Process selectedProcess = Process.GetProcessById(processId);
                MessageBox.Show($"Process Name: {processName}\nProcess ID: {processId}\nMain Window Title: {selectedProcess.MainWindowTitle}");
            }
        }

        private void terminateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (processListView.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = processListView.SelectedItems[0];
                int processId = int.Parse(selectedItem.SubItems[1].Text);

                try
                {
                    Process selectedProcess = Process.GetProcessById(processId);
                    selectedProcess.Kill();
                    selectedItem.Remove();
                    MessageBox.Show("Process terminated successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error terminating process: {ex.Message}");
                }
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshProcessList();
        }

        private void exportToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        foreach (ListViewItem item in processListView.Items)
                        {
                            string processName = item.Text;
                            string processId = item.SubItems[1].Text;
                            string mainWindowTitle = item.SubItems[2].Text;

                            writer.WriteLine($"Process Name: {processName}\tProcess ID: {processId}\tMain Window Title: {mainWindowTitle}");
                        }
                    }
                    MessageBox.Show("Process list exported successfully.");
                }
            }
        }

        private void viewThreadsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (processListView.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = processListView.SelectedItems[0];
                int processId = int.Parse(selectedItem.SubItems[1].Text);

                Process selectedProcess = Process.GetProcessById(processId);

                string threadsInfo = "Threads:\n";
                foreach (ProcessThread thread in selectedProcess.Threads)
                {
                    threadsInfo += $"Thread ID: {thread.Id}\tStart Time: {thread.StartTime}\n";
                }

                string modulesInfo = "Modules:\n";
                foreach (ProcessModule module in selectedProcess.Modules)
                {
                    modulesInfo += $"Module Name: {module.ModuleName}\tBase Address: {module.BaseAddress}\n";
                }

                MessageBox.Show($"{threadsInfo}\n{modulesInfo}");
            }
        }
    }
}
