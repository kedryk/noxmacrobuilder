using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace MacroBuilder {
    public partial class MainForm : Form {
        private NoxInstance _noxInstance;
        private NoxApp _app;

        private bool _mouseDown;

        public MainForm() {
            InitializeComponent();

            attachToNoxToolStripMenuItem_Click(null, new EventArgs());
        }

        private void btnTest_Click(object sender, EventArgs e) {
            pictureBox1.Image = _noxInstance?.TakeScreenshot();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e) => Close();

        private void attachToNoxToolStripMenuItem_Click(object sender, EventArgs e) {
            _noxInstance?.RemoveHooks();

            var noxProcesses = Process.GetProcessesByName("Nox").Select(p => new NoxInstance(p)).ToList();
            if (!noxProcesses.Any()) {
                toolStripStatusLabel1.Text = "No Instances Found";
                _noxInstance = null;
            }
            else if (noxProcesses.Count == 1) {
                _noxInstance = noxProcesses.First();
                _noxInstance.AddHooks();
                toolStripStatusLabel1.Text = _noxInstance.Name;
            }
            else {
                var dlg = new AttachDialog(noxProcesses);
                if (dlg.ShowDialog() == DialogResult.OK) { _noxInstance = dlg.SelectedInstance; }
                _noxInstance.AddHooks();
                toolStripStatusLabel1.Text = _noxInstance.Name;
            }
            pictureBox1.Image = _noxInstance?.TakeScreenshot();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e) {
            _mouseDown = true;
            _noxInstance?.TouchDown(e.X, e.Y);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e) {
            _mouseDown = false;
            _noxInstance?.TouchUp(e.X, e.Y);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e) {
            lblX.Text = e.X.ToString();
            lblY.Text = e.Y.ToString();
            if (_mouseDown) {
                _noxInstance?.TouchMove(e.X, e.Y);
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e) {
            _app = new NoxApp { NoxInstance = _noxInstance };
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e) { openAppDialog.ShowDialog(); }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            if (string.IsNullOrWhiteSpace(_app.Filename)) {
                saveAsToolStripMenuItem_Click(sender, e);
                return;
            }
            try {
                File.WriteAllText(_app.Filename, JsonConvert.SerializeObject(_app));
            }
            catch (Exception ex) {
                MessageBox.Show($"Could not save App: {ex.Message} at {ex.StackTrace}", "Error Saving App", MessageBoxButtons.OK);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) { saveAppDialog.ShowDialog(); }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e) {
            try {
                _app = JsonConvert.DeserializeObject<NoxApp>(File.ReadAllText(openAppDialog.FileName));
                _app.NoxInstance = _noxInstance;
            }
            catch (Exception ex) {
                _app = new NoxApp { NoxInstance = _noxInstance };
                MessageBox.Show($"Could not open App: {ex.Message} at {ex.StackTrace}", "Error Opening App", MessageBoxButtons.OK);
            }
        }

        private void saveAppDialog_FileOk(object sender, CancelEventArgs e) {
            try {
                _app.Filename = saveAppDialog.FileName;
                File.WriteAllText(_app.Filename, JsonConvert.SerializeObject(_app));
            }
            catch (Exception ex) {
                MessageBox.Show($"Could not save App: {ex.Message} at {ex.StackTrace}", "Error Saving App", MessageBoxButtons.OK);
            }
        }
    }
}