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
            if (cmbInputType.Text != "Keyboard") return;
            Form1.GetInstance().SetFocusedKey("Tactile", e.KeyData.ToString(), (byte)e.KeyValue);
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbInputType.Text != "Joystick")
            {
                MessageBox.Show("Only joystick buttons can be saved.");
                return;
            }
            var key = Byte.Parse(cmbKey.Text);
            if (key == 0)
            {
                MessageBox.Show("Invalid Key");
                return;
            }
            Form1.GetInstance().SetFocusedKey("JoystickKey", cmbKey.Text, key);
            CloseWindow();
        }

        private void cmbInputType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbInputType.Text == "Joystick")
            {
                lblPressKey.Visible = false;
                cmbKey.Enabled = true;
                cmbJoystickID.Enabled = true;
                                
                btnSave.Enabled = true;
            } else
            {
                lblPressKey.Visible = true;
                cmbJoystickID.Enabled = false;
                cmbKey.Enabled = false;
                btnSave.Enabled = false;
            }
        }

        private void cmbKey_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbInputType_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void cmbInputType_KeyDown(object sender, KeyEventArgs e)
        {
            DetectKey(e);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
