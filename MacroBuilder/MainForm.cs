using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MacroBuilder {
    public partial class MainForm : Form {
        private NoxInstance _noxInstance;
        private bool _mouseDown;

        public MainForm() {
            InitializeComponent();

            attachToNoxToolStripMenuItem_Click(null, new EventArgs());
        }

        private void btnTest_Click(object sender, EventArgs e) {
            if (pictureBox1 != null) { pictureBox1.Image = _noxInstance?.TakeScreenshot(); }
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
            if (_mouseDown) {
                _noxInstance?.TouchMove(e.X, e.Y);
            }
        }
    }
}
