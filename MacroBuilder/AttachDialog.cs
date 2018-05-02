using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MacroBuilder {
    public partial class AttachDialog : Form {
        public AttachDialog(List<NoxInstance> instances) {
            InitializeComponent();

            comboBox1.DataSource = instances;
        }

        public NoxInstance SelectedInstance => comboBox1.SelectedItem as NoxInstance;
    }
}
