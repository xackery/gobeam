using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gobeam
{
    public partial class InputKey : Form
    {
        public InputKey()
        {
            InitializeComponent();
        }

        private void CloseWindow()
        {            
            Hide();
            Form1.GetInstance().Enabled = true;
            Form1.GetInstance().Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            CloseWindow();
        }

        private void InputKey_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseWindow();
        }

        private void InputKey_KeyDown(object sender, KeyEventArgs e)
        {
            DetectKey(e);
        }

        private void DetectKey(KeyEventArgs e)
        {
            Form1.GetInstance().SetFocusedKey(e.KeyData.ToString(), (byte)e.KeyValue);
            Form1.GetInstance().SetStatus("Mapped key " + e.KeyValue);
            CloseWindow();
            //MessageBox.Show(e.KeyCode + ", " + e.KeyData + ", " + e.KeyValue);
        }

        private void btnCancel_KeyDown(object sender, KeyEventArgs e)
        {
            DetectKey(e);
        }

        private void InputKey_Load(object sender, EventArgs e)
        {

        }
    }
}
