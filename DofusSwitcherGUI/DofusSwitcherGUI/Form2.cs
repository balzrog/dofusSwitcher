using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DofusSwitcherGUI {
    public partial class Form2 : Form {
        Form1 instance;
        int index;
        public Form2(Form1 _instance, int _index) {
            InitializeComponent();
            instance = _instance;
            index = _index;
        }

        private void Form2_Load(object sender, EventArgs e) {
            listBox1.Items.Add(new ComboboxItem("Feca"));
            listBox1.Items.Add(new ComboboxItem("Eni"));
            listBox1.Items.Add(new ComboboxItem("Cra"));
            listBox1.Items.Add(new ComboboxItem("Iop"));
            listBox1.Items.Add(new ComboboxItem("Panda"));
            listBox1.Items.Add(new ComboboxItem("Enu"));
            listBox1.Items.Add(new ComboboxItem("Sacri"));
            listBox1.Items.Add(new ComboboxItem("Eca"));
        }

        private void listBox1_DoubleClick(object sender, EventArgs e) {

            int indexClasse = listBox1.SelectedIndex;
            ComboboxItem item = new ComboboxItem();
            item = (ComboboxItem)instance.getListbox().Items[index];
            if(indexClasse != -1) {
                item.Text = ((ComboboxItem)listBox1.Items[indexClasse]).Text;
                instance.getListbox().Items.Remove((ComboboxItem)instance.getListbox().Items[index]);
                instance.getListbox().Items.Add(item);
                instance.getListbox().Update();
                this.Close();
            }
        }
    }
}
