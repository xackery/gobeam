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
        private static InputKey instance;

        public static InputKey GetInstance()
        {
            return instance;
        }
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
            e.Cancel = true;
        }

        private void InputKey_KeyDown(object sender, KeyEventArgs e)
        {
            DetectKey(e);
        }

        private void DetectKey(KeyEventArgs e)
        {
            if (cmbInputType.Text != "Tactile") return;
            Form1.GetInstance().SetFocusedKey("Tactile", e.KeyData.ToString(), (byte)e.KeyValue, 0);
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
            if (instance != null && instance != this)
            {
                Close();
                return;
            }
            instance = this;
        }

        public static void SetJoystickID(int joystickIndex)
        {
            GetInstance().cmbJoystickID.Text = joystickIndex.ToString();
        }

        public static void SetKeyButton(byte keyCode)
        {
            GetInstance().cmbKey.Text = keyCode.ToString();
        }

        public static void SetType(string type)
        {
            GetInstance().cmbInputType.Text = type;
            GetInstance().DetectComboBoxStatus();
        }

        public static void SetIndex(string index)
        {
            GetInstance().Text = "InputKey #"+index;
        }
        
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbInputType.Text != "JoystickKey")
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
            var joystickIndex = Byte.Parse(cmbJoystickID.Text);
            if (joystickIndex == 0)
            {
                MessageBox.Show("Invalid Joystick ID");
                return;
            }

            Form1.GetInstance().SetFocusedKey("JoystickKey", cmbKey.Text, key, joystickIndex);
            CloseWindow();
        }


        private void DetectComboBoxStatus()
        {
            if (cmbInputType.Text != "Tactile")
            {
                lblPressKey.Visible = false;
                cmbKey.Enabled = true;
                cmbJoystickID.Enabled = true;
                btnSave.Enabled = true;
            }
            else
            {
                lblPressKey.Visible = true;
                cmbJoystickID.Enabled = false;
                cmbKey.Enabled = false;
                btnSave.Enabled = false;
            }
        }

        private void cmbInputType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DetectComboBoxStatus();
                
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
