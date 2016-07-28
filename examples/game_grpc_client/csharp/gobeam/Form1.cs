using Grpc.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gobeam;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using vJoyInterfaceWrap;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;

namespace gobeam
{

    public partial class Form1 : Form
    {
        long maxval = 32767;
        public IntPtr processHandleWindow;
        IntPtr handle;
        private Process sourceProcess;
        private pointerset scoreps = new pointerset();
        private pointerset ballps = new pointerset();

        private Channel channel;
        private RobotServiceClient client;

        public static Config config;

        public Form1()
        {
            InitializeComponent();
        }

        private static Form1 instance;
        public static Form1 GetInstance()
        {
            return instance;
        }
        public static void SetLeftProgress(int val)
        {
            Form1.GetInstance().prgLeft.Value = val;
        }

        public static void SetRightProgress(int val)
        {
            Form1.GetInstance().prgRight.Value = val;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text = "GoBeam v" + Application.ProductVersion;
            //var js = "{\"reportInterval\":50,\"tactiles\":[{\"id\":0,\"type\":\"tactiles\",\"blueprint\":[{\"width\":1,\"height\":1,\"grid\":\"large\",\"state\":\"default\",\"x\":7,\"y\":0},{\"width\":1,\"height\":1,\"grid\":\"small\",\"state\":\"default\",\"x\":1,\"y\":2},{\"width\":1,\"height\":1,\"grid\":\"medium\",\"state\":\"default\",\"x\":3,\"y\":0}],\"analysis\":{\"holding\":true,\"frequency\":true},\"cost\":{\"press\":{\"cost\":0}},\"cooldown\":{\"press\":0},\"text\":\"Left\",\"key\":81,\"help\":\"Left Paddle\"},{\"id\":1,\"type\":\"tactiles\",\"blueprint\":[{\"width\":1,\"height\":1,\"grid\":\"large\",\"state\":\"default\",\"x\":8,\"y\":0},{\"width\":1,\"height\":1,\"grid\":\"medium\",\"state\":\"default\",\"x\":5,\"y\":0},{\"width\":1,\"height\":1,\"grid\":\"small\",\"state\":\"default\",\"x\":5,\"y\":0}],\"analysis\":{\"holding\":true,\"frequency\":true},\"key\":87,\"text\":\"Right\",\"help\":\"Right Paddle\",\"cost\":{\"press\":{\"cost\":0}},\"cooldown\":{\"press\":0}}],\"joysticks\":[],\"screens\":[]}";
            //BeamJson beamJson = Newtonsoft.Json.JsonConvert.DeserializeObject<BeamJson>(js);         
            //prgLeft.ProgressBar.ForeColor = Color.Blue;
            //prgRight.ProgressBar.ForeColor = Color.Red;
            instance = this;
            String strAppDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);

            openFileDialog1.InitialDirectory = strAppDir;
            saveFileDialog1.InitialDirectory = strAppDir;            
        }

        public static void RebuildKeys(KeyBind[] newKeys)
        {
            config.keys = newKeys;
            GetInstance().grdControls.Rows.Clear();
            foreach (var key in newKeys)
            {
                GetInstance().grdControls.Rows.Add(key.Index, key.Label, key.Type, key.joystickIndex, key.KeyName);
            }
            
        }

        private void attachProcess()
        {
            if (txtProcess.Text == "")
            {
                MessageBox.Show("Empty process name is invalid");
                return;
            }
            handle = IntPtr.Zero;
            var processes = w32.GetProcessList(txtProcess.Text);
            if (processes.Length == 0)
            {
                MessageBox.Show("No " + txtProcess.Text + " process found");
                txtProcess.Enabled = true;
                btnTestControls.Enabled = false;
                btnAttach.Text = "Attach";
                return;
            }
            else if (processes.Length > 1)
            {
                MessageBox.Show("More than 1 " + txtProcess.Text + " process found.");
                txtProcess.Enabled = true;
                btnTestControls.Enabled = false;
                btnAttach.Text = "Attach";
                return;
            }

            handle = w32.AttachProcess(processes[0]);
            sourceProcess = processes[0];
            processHandleWindow = processes[0].MainWindowHandle;
            txtProcess.Enabled = false;
            btnAttach.Text = "Detach";
            Text = "["+txtProcess.Text+"] GoBeam v" + Application.ProductVersion;            
            btnTestControls.Enabled = true;
            btnTestJoystick.Enabled = true;
            lblStatus.Text = "Attached to " + txtProcess.Text + ".";
        }


        private void detachProcess()
        {
            handle = IntPtr.Zero;
            processHandleWindow = IntPtr.Zero;
            Text = "GoBeam v" + Application.ProductVersion;
            txtProcess.Enabled = true;
            btnAttach.Text = "Attach";
            btnAttach.Enabled = true;
            btnTestControls.Enabled = false;
            btnTestJoystick.Enabled = false;
            btnTestJoystick.Text = "Detect Joystick";
            chkJoyUp.Enabled = false;
            btnTestControls.Text = "Test Controls";
            lblStatus.Text = "Detached from " + txtProcess.Text + ".";
        }

        public void SetStatus(string message)
        {
            lblStatus.Text = message;
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            if (processHandleWindow != w32.GetForegroundWindow())
            {
                return;
            }
            //Any iteration refresh stuff   
        }


        private void btnAttach_Click(object sender, EventArgs e)
        {
            if (handle == IntPtr.Zero)
            {
                attachProcess();
            }
            else
            {
                detachProcess();
            }

        }

        public void DisconnectRPC()
        {
            btnConnect.Text = "Connect";
            SetStatus("Disconnected from " + txtAddr.Text + ".");
            if (client != null && channel != null)
            {
                channel.ShutdownAsync();
                client = null;                
                channel = null;         
            }
            btnConnect.Text = "Connect";
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            
            if (config == null || config.keys == null || config.keys.Length < 1)
            {
                MessageBox.Show("First load a key config before connecting.", "No keys are mapped", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            bool hasJoystick = false;
            foreach (var key in config.keys)
            {
                if (key.Type == "Joystick")
                {
                    hasJoystick = true;
                    break;
                }
            }
            if (hasJoystick && btnTestJoystick.Text == "Detect Joystick")
            {
                MessageBox.Show("There are uninitialized joysticks. Press Detect Joystick before connecting.", "Joysticks uninitialized.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

           

            if (client != null && channel != null)
            {
                if (tmrProgressUpdate.Enabled)
                {
                    tmrProgressUpdate.Enabled = false;
                    ProgressUpdateMassSet(false, 0);
                }
                DisconnectRPC();
                return;
            }

            try
            {
                
                SetStatus("Connecting to " + txtAddr.Text + "...");
                //Create a connection
                channel = new Channel(txtAddr.Text, ChannelCredentials.Insecure);
                var rClient = new RobotService.RobotServiceClient(channel);
                client = new RobotServiceClient(rClient);
                client.StreamReport();
                btnConnect.Text = "Disconnect";
                SetStatus("Connected to " + txtAddr.Text + ".");
                btnTestgRPC.Enabled = true;
            }
            catch (Exception err)
            {
                DisconnectRPC();
                MessageBox.Show("Failed to connect to gRPC client:", err.Message);                
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void grdControls_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnTestControls_Click(object sender, EventArgs e)
        {
            if (handle == IntPtr.Zero)
            {
                SetStatus("Process must be attached to run test");
                return;
            }

            if (tmrTestJoystick.Enabled == true || tmrTestControls.Enabled == true)
            {
                SetStatus("Test is already running");
                return;
            }
            testIndex = 0;
            tmrTestControls.Enabled = true;
        }

        private int testIndex;
        private bool testKeyDown;
        private void tmrTestControls_Tick(object sender, EventArgs e)
        {

            if (processHandleWindow != w32.GetForegroundWindow())
            {
                SetStatus("Focus the process to test controls!");
                return;
            }

            //Release any previously pressed keys
            var keys = config.keys;
            if (testKeyDown)
            {
                SetStatus("Releasing key " + keys[testIndex - 1].KeyName);
                w32.keybd_event(keys[testIndex - 1].KeyCode, 0, w32.KEYEVENTF_KEYUP, 0);
                testKeyDown = false;
                return;
            }

            if (grdControls.Rows.Count - 2 < testIndex)
            {
                tmrTestControls.Enabled = false;
                SetStatus("Test Complete");
                return;
            }

            if (keys[testIndex].Type != "Tactile")
            {
                SetStatus("Skipping key " + keys[testIndex].KeyName + " (Not key)");
                testIndex++;
                return;
            }

            testKeyDown = true;
            w32.keybd_event(keys[testIndex].KeyCode, 0, 0, 0);
            SetStatus("Pressing key " + keys[testIndex].KeyName);
            testIndex++;
        }

        InputKey inputKey;
        private void grdControls_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (grdControls.Rows[e.RowIndex] == null || grdControls.Rows[e.RowIndex].Cells[2] == null || grdControls.Rows[e.RowIndex].Cells[2].Value == null) return;
            if (grdControls.Rows[e.RowIndex].Cells[2].Value.ToString() == "Joystick")
            {
                string id = Microsoft.VisualBasic.Interaction.InputBox("Joystick ID", "Enter Joystick ID # (1 is default)", "1");
                    
                if (Byte.Parse(id) == 0)
                {
                    MessageBox.Show("Invalid ID: " + id + ", discarding");
                    return;
                }
                bool isFound = false;
                foreach (var key in config.keys)
                {
                    if (key.Index == (int)grdControls.Rows[e.RowIndex].Cells[0].Value)
                    {
                        key.KeyName = id;
                        key.KeyCode = Byte.Parse(id);
                        isFound = true;
                        break;
                    }
                }
                if (!isFound)
                {
                    MessageBox.Show("There was an error setting joystick id, try again later.");
                    return;
                }
                grdControls.Rows[e.RowIndex].Cells[4].Value = id;
                return;
            }

            lastKeyIndex = -1;
            foreach (var key in config.keys)
            {
                if (key.Index == (int)grdControls.Rows[e.RowIndex].Cells[0].Value)
                {
                    lastKeyIndex = key.Index;
                    if (inputKey == null) inputKey = new InputKey();
                    inputKey.Show();

                    InputKey.SetJoystickID(key.joystickIndex);
                    InputKey.SetKeyButton(key.KeyCode);
                    InputKey.SetType(key.Type);
                    InputKey.SetIndex(key.Index.ToString());
                    
                    this.Enabled = false;
                    return;
                }
            }
            if (lastKeyIndex == -1)
            {
                MessageBox.Show("An internal error occured");
                return;
            }
            
        }

        int lastKeyIndex;
        public void SetFocusedKey(string type, string label, byte key, byte joystickIndex)
        {
            for (var i = 0; i < grdControls.Rows.Count - 1; i++)
            {
                var row = grdControls.Rows[i];
                if (row.Cells[0].Value.ToString() == lastKeyIndex.ToString())
                {

                    row.Cells[2].Value = type;
                    row.Cells[3].Value = joystickIndex.ToString();
                    row.Cells[4].Value = label;                    
                    for (var j = 0; j < config.keys.Count(); j++)
                    {
                        if (config.keys[j].Index == lastKeyIndex)
                        {
                            config.keys[j].Type = type;
                            config.keys[j].KeyName = label;
                            config.keys[j].KeyCode = key;
                            config.keys[j].joystickIndex = joystickIndex;
                            return;
                        }
                    }
                    MessageBox.Show("Could not find key with index " + lastKeyIndex);
                }
            }
            MessageBox.Show("Could not find cell with index " + lastKeyIndex);
            
        }

        private void btnTestJoystick_Click(object sender, EventArgs e)
        {

            if (btnTestJoystick.Text == "Detect Joystick")
            {
                foreach (var key in config.keys)
                {
                    UInt32 id = key.KeyCode;
                    if (key.Type != "Joystick") continue;
                    if (key.KeyCode < 1)
                    {
                        MessageBox.Show("Key #" + key.Index + " has an invalid joystick set: " + key.KeyCode + " (Must be greater than 0)");
                        return;
                    }

                    var joystick = key.joystick;
             
                    if (!joystick.vJoyEnabled())
                    {
                        SetStatus("vJoy was not detected");
                        MessageBox.Show("vJoy driver not enabled. This likely means you need to install vJoy drivers, or reboot if you recently installed them.", "vJoy driver not enabled.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnTestJoystick.Text = "Detect Joystick";
                        chkJoyUp.Enabled = false;
                        return;
                    }
                    Console.WriteLine("Vendor: {0}\nProduct :{1}\nVersion Number:{2}\n", joystick.GetvJoyManufacturerString(), joystick.GetvJoyProductString(), joystick.GetvJoySerialNumberString());

                    // Get the state of the requested device
                    VjdStat status = joystick.GetVJDStatus(id);
                    switch (status)
                    {
                        case VjdStat.VJD_STAT_OWN:
                            key.IsEnabled = true; 
                            Console.WriteLine("vJoy Device {0} is already owned by this feeder\n", id);
                            break;
                        case VjdStat.VJD_STAT_FREE:
                            key.IsEnabled = true; 
                            Console.WriteLine("vJoy Device {0} is free\n", id);
                            break;
                        case VjdStat.VJD_STAT_BUSY:
                            MessageBox.Show("Joystick " + key.KeyCode + " is being used by another feeder, please close any applications controlling this and try again.");
                            Console.WriteLine("vJoy Device {0} is already owned by another feeder\nCannot continue\n", id);
                            return;
                        case VjdStat.VJD_STAT_MISS:
                            MessageBox.Show("Joystick " + key.KeyCode + " is not installed or disabled, please remedy and try again.");
                            Console.WriteLine("vJoy Device {0} is not installed or disabled\nCannot continue\n", id);
                            return;
                        default:
                            MessageBox.Show("Joystick " + key.KeyCode + " caused a general error that isn't known. Please adjust settings outside of program and try again.");
                            Console.WriteLine("vJoy Device {0} general error\nCannot continue\n", id);
                            return;
                    };

                    // Check which axes are supported
                    if (!joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_X))
                    {
                        MessageBox.Show("Joystick " + key.KeyCode + " does not support X axis, this may cause challenges. (Use Configure vJoy app to fix)");
                    }
                    if (!joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Y))
                    {
                        MessageBox.Show("Joystick " + key.KeyCode + " does not support X axis, this may cause challenges. (Use Configure vJoy app to fix)");
                    }
                    if (!joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Z))
                    {
                        MessageBox.Show("Joystick " + key.KeyCode + " does not support X axis, this may cause challenges. (Use Configure vJoy app to fix)");
                    }
                    //bool AxisX = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_X);
                    //bool AxisY = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Y);
                    //bool AxisZ = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Z);
                    //bool AxisRX = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RX);
                    //bool AxisRZ = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RZ);
                    // Get the number of buttons and POV Hat switchessupported by this vJoy device
                    //  int nButtons = joystick.GetVJDButtonNumber(id);
                    // int ContPovNumber = joystick.GetVJDContPovNumber(id);
                    // int DiscPovNumber = joystick.GetVJDDiscPovNumber(id);

                    // Print results                    
                    // Test if DLL matches the driver
                    UInt32 DllVer = 0, DrvVer = 0;
                    bool match = joystick.DriverMatch(ref DllVer, ref DrvVer);
                    if (!match)
                    {
                        MessageBox.Show("vJoy driver detected with version " + DrvVer + ", however this client only works with " + DllVer);
                        return;
                    }

                    // Acquire the target
                    if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!joystick.AcquireVJD(id))))
                    {
                        MessageBox.Show("There was an error while trying to acquire the vJoy device for joystick " + key.KeyCode + ".");
                        return;
                    }
                }
                SetStatus("Joysticks configured");
                //MessageBox.Show("Joysticks are configured successfully. Press Test Joystick if you wish to test them.");
                btnTestJoystick.Text = "Test Joystick";
                chkJoyUp.Enabled = true;
                return;
            } else if (btnTestJoystick.Text == "Test Joystick")
            {
                if (handle == IntPtr.Zero)
                {
                    SetStatus("Process must be attached to run test");
                    return;
                }

                if (tmrTestJoystick.Enabled == true || tmrTestControls.Enabled == true)
                {
                    SetStatus("A Test is already running");
                    return;
                }
                
                testJoystickIndex = 0;
                testJoystickDirection = 0;            
                tmrTestJoystick.Enabled = true;
            }
        }

        private int testJoystickIndex;
        private int testJoystickDirection;
        private void tmrTestJoystick_Tick(object sender, EventArgs e)
        {
            if (processHandleWindow != w32.GetForegroundWindow())
            {
                SetStatus("Focus the process to test controls!");
               return;
            }

            if (testJoystickDirection == 0 && grdControls.Rows.Count - 2 < testJoystickIndex)
            {
                tmrTestJoystick.Enabled = false;
                SetStatus("Test Completed.");
                return;
            }
            
            if (config.keys[testJoystickIndex].Type != "Joystick" && config.keys[testJoystickIndex].Type != "JoystickKey")
            {
                SetStatus("Skipping #" + config.keys[testJoystickIndex].Index + " " + config.keys[testJoystickIndex].KeyName + " (Not joystick)");
                testJoystickIndex++;
                return;
            }

            string direction = "";
            UInt32 id = config.keys[testJoystickIndex].KeyCode;
            vJoy joystick = null;
            if (config.keys[testJoystickIndex].Type == "JoystickKey")
            {
                foreach (var joy in config.keys)
                {
                    if (joy.KeyCode == config.keys[testJoystickIndex].joystickIndex && joy.Type == "Joystick" && joy.joystick != null)
                    {
                        joystick = joy.joystick;
                        break;
                    }
                }
                
            } else
            {
                joystick = config.keys[testJoystickIndex].joystick;
            }

            if (joystick == null)
            {
                MessageBox.Show("There was an error finding joystick on index " + testJoystickIndex);
                tmrTestJoystick.Enabled = false;
                return;
            }
                
            //Get max value
            //joystick.GetVJDAxisMax(id, HID_USAGES.HID_USAGE_X, ref maxval);
            // Reset this device to default values
            joystick.ResetVJD(id);
            Form1.JoystickIdle(joystick, (byte)id, (int)maxval);            

            
            if (testJoystickDirection == 0)
            {
                if (config.keys[testJoystickIndex].Type == "JoystickKey")
                {
                    direction = config.keys[testJoystickIndex].KeyCode.ToString();
                    joystick.SetBtn(true, id, config.keys[testJoystickIndex].KeyCode);
                }
                else
                {
                    direction = "100% up";
                    joystick.SetAxis((int)maxval, config.keys[testJoystickIndex].KeyCode, HID_USAGES.HID_USAGE_Y);
                }
            }
            if (testJoystickDirection == 1)
            {
                if (config.keys[testJoystickIndex].Type == "JoystickKey")
                {
                    direction = "Releasing "+config.keys[testJoystickIndex].KeyCode.ToString();
                    joystick.SetBtn(false, id, config.keys[testJoystickIndex].KeyCode);
                }
                else
                {
                    direction = "100% down";
                    joystick.SetAxis(0, config.keys[testJoystickIndex].KeyCode, HID_USAGES.HID_USAGE_Y);
                }
            }

            if (testJoystickDirection == 2)
            {
                direction = "50% up";
                Console.Write(" Y: " + maxval + "," + (int)(maxval * 0.75f));
                joystick.SetAxis((int)(maxval*0.75f), config.keys[testJoystickIndex].KeyCode, HID_USAGES.HID_USAGE_Y);
            }
            if (testJoystickDirection == 3)
            {
                direction = "50% down";
                joystick.SetAxis((int)(maxval * 0.25f), config.keys[testJoystickIndex].KeyCode, HID_USAGES.HID_USAGE_Y);
            }

            if (testJoystickDirection == 4)
            {
                direction = "100% right";
                joystick.SetAxis((int)(maxval * 1f), config.keys[testJoystickIndex].KeyCode, HID_USAGES.HID_USAGE_X);
            }
            if (testJoystickDirection == 5)
            {
                direction = "100% left";
                joystick.SetAxis((int)(maxval * 0f), config.keys[testJoystickIndex].KeyCode, HID_USAGES.HID_USAGE_X);
            }
            if (testJoystickDirection == 6)
            {
                direction = "50% right";
                joystick.SetAxis((int)(maxval * 0.75f), config.keys[testJoystickIndex].KeyCode, HID_USAGES.HID_USAGE_X);
            }
            if (testJoystickDirection == 7)
            {
                direction = "50% left";
                joystick.SetAxis((int)(maxval * 0.25f), config.keys[testJoystickIndex].KeyCode, HID_USAGES.HID_USAGE_X);
            }
            if (testJoystickDirection == 8)
            {
                direction = "25% down-right";
                joystick.SetAxis((int)(maxval * 0.62f), config.keys[testJoystickIndex].KeyCode, HID_USAGES.HID_USAGE_X);
                joystick.SetAxis((int)(maxval * 0.62f), config.keys[testJoystickIndex].KeyCode, HID_USAGES.HID_USAGE_Y);
            }
            if (testJoystickDirection == 9)
            {
                direction = "25% up-left";
                joystick.SetAxis((int)(maxval * 0.38f), config.keys[testJoystickIndex].KeyCode, HID_USAGES.HID_USAGE_X);
                joystick.SetAxis((int)(maxval * 0.38f), config.keys[testJoystickIndex].KeyCode, HID_USAGES.HID_USAGE_Y);
            }

            SetStatus("Pressing #"+ config.keys[testJoystickIndex].Index+" "+ config.keys[testJoystickIndex].KeyName+": "+direction+" ("+maxval+")");
            Console.WriteLine(joystick);
            testJoystickDirection++;
            if (testJoystickDirection > 10 || (config.keys[testJoystickIndex].Type == "JoystickKey" && testJoystickDirection > 1))
            {
                testJoystickIndex++;
                testJoystickDirection = 0;
            }

        }
        public static void JoystickButtonIdle(vJoy joystick, byte jid)
        {
            if (joystick == null) return;
            for (int i = 1; i < 32; i++)
            {
                if (joystick.GetVJDButtonNumber(jid) < i) break;
                joystick.SetBtn(false, jid, (UInt32)i);
            }
        }

        public static void JoystickIdle(vJoy joystick, byte id, int maxval)
        {
            if (id != 0)
            {
                if (joystick == null)
                {
                    MessageBox.Show("Cannot reset joystick, null joystick passed");
                    return;
                }
                if (joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Z)) joystick.SetAxis((int)(maxval * 0.5f), id, HID_USAGES.HID_USAGE_Z);
                if (joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RX)) joystick.SetAxis((int)(maxval * 0.5f), id, HID_USAGES.HID_USAGE_RX);
                if (joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RY)) joystick.SetAxis((int)(maxval * 0.5f), id, HID_USAGES.HID_USAGE_RY);
                if (joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RZ)) joystick.SetAxis((int)(maxval * 0.5f), id, HID_USAGES.HID_USAGE_RZ);
                if (joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_SL0)) joystick.SetAxis((int)(maxval * 0.5f), id, HID_USAGES.HID_USAGE_SL0);
                if (joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_SL1)) joystick.SetAxis((int)(maxval * 0.5f), id, HID_USAGES.HID_USAGE_SL1);
                if (joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_POV)) joystick.SetAxis((int)(maxval * 0.5f), id, HID_USAGES.HID_USAGE_POV);
                return;
            }

            for (byte i = 1; i < 5; i++)
            {
                foreach (var key in config.keys)
                {
                    if (key.joystick != null && key.joystickIndex == i)
                    {
                        joystick = key.joystick;
                        if (joystick.GetVJDAxisExist(i, HID_USAGES.HID_USAGE_Z)) joystick.SetAxis((int)(maxval * 0.5f), i, HID_USAGES.HID_USAGE_Z);
                        if (joystick.GetVJDAxisExist(i, HID_USAGES.HID_USAGE_RX)) joystick.SetAxis((int)(maxval * 0.5f), i, HID_USAGES.HID_USAGE_RX);
                        if (joystick.GetVJDAxisExist(i, HID_USAGES.HID_USAGE_RY)) joystick.SetAxis((int)(maxval * 0.5f), i, HID_USAGES.HID_USAGE_RY);
                        if (joystick.GetVJDAxisExist(i, HID_USAGES.HID_USAGE_RZ)) joystick.SetAxis((int)(maxval * 0.5f), i, HID_USAGES.HID_USAGE_RZ);
                        if (joystick.GetVJDAxisExist(i, HID_USAGES.HID_USAGE_SL0)) joystick.SetAxis((int)(maxval * 0.5f), i, HID_USAGES.HID_USAGE_SL0);
                        if (joystick.GetVJDAxisExist(i, HID_USAGES.HID_USAGE_SL1)) joystick.SetAxis((int)(maxval * 0.5f), i, HID_USAGES.HID_USAGE_SL1);
                        if (joystick.GetVJDAxisExist(i, HID_USAGES.HID_USAGE_POV)) joystick.SetAxis((int)(maxval * 0.5f), i, HID_USAGES.HID_USAGE_POV);
                        break;
                    }
                }
            }
            //bool AxisX = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_X);
            //bool AxisY = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Y);
            //bool AxisZ = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Z);
            //bool AxisRX = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RX);
            //bool AxisRZ = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RZ);
            // Get the number of buttons and POV Hat switchessupported by this vJoy device
            //  int nButtons = joystick.GetVJDButtonNumber(id);
            // int ContPovNumber = joystick.GetVJDContPovNumber(id);
            // int DiscPovNumber = joystick.GetVJDDiscPovNumber(id);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void saveConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {           
            saveFileDialog1.Filter = "JSON Files|*.json";
            saveFileDialog1.DefaultExt = ".json";
            saveFileDialog1.CreatePrompt = false;
            saveFileDialog1.OverwritePrompt = true;
            var result = saveFileDialog1.ShowDialog();
            if (result != DialogResult.OK) return;

            //update config with form changes
            config.processName = txtProcess.Text;
            config.grpcAddress = txtAddr.Text;

            try
            {
                File.WriteAllText(Path.GetFullPath(saveFileDialog1.FileName), JsonConvert.SerializeObject(config, Formatting.Indented));
                SetStatus(Path.GetFileName(saveFileDialog1.FileName) + " saved.");
            } catch (Exception err)
            {
                MessageBox.Show(err.Message, "Failed to save", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public string GetProcessName()
        {
            return txtProcess.Text;
        }

        private void loadKeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (btnAttach.Text == "Detach")
            {
                MessageBox.Show("First Detach from Process before loading a config", "Detach First!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (btnConnect.Text == "Disconnect")
            {
                MessageBox.Show("First Disconnect from gRPC before loading a config", "Disconnect First!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DialogResult result;
            if (config != null && config.keys != null && config.keys.Length > 0)
            {
                result = MessageBox.Show("Clear key settings without saving?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes) return;
            }

            openFileDialog1.Filter = "JSON Files|*.json";
            openFileDialog1.DefaultExt = ".json";
            result = openFileDialog1.ShowDialog();
            if (result != DialogResult.OK) return;
            try
            {               
                var newConfig = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.GetFullPath(openFileDialog1.FileName)));
                if (newConfig == null)
                {
                    MessageBox.Show("Config failed to load.", "Failed to load", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                config = newConfig;
                RebuildKeys(config.keys);
                txtAddr.Text = config.grpcAddress;
                txtProcess.Text = config.processName;
                SetStatus(Path.GetFileName(openFileDialog1.FileName) + " loaded.");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Failed to load", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void LoadConfig(string path, bool isResetOnFail)
        {
            try
            {
                config = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"poetionbot.ini"));
            }
            catch (System.IO.FileNotFoundException)
            {                
                MessageBox.Show("Could not find file: " + path, "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            if (config == null && isResetOnFail) ResetDefaults();
        }

        private void ResetDefaults()
        {
            if (config != null && config.keys != null && config.keys.Length > 0)
            {
                var result = MessageBox.Show("Clear key settings without saving?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes) return;
            }            

            //Make sure disconnected
            DisconnectRPC();
            detachProcess();            
            var newConfig = new Config();
            //Set default values

            SetStatus("Reset config to default");
            config = newConfig;
        }

        private void newConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (btnAttach.Text == "Detach")
            {
                MessageBox.Show("First Detach from Process before loading a config", "Detach First!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (btnConnect.Text == "Disconnect")
            {
                MessageBox.Show("First Disconnect from gRPC before loading a config", "Disconnect First!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            ResetDefaults();
            Hide();
            var js = new FormJsonLoader();
            js.Show();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            
           
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Written by Xackery", "Version "+Application.ProductVersion, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnTestgRPC_Click(object sender, EventArgs e)
        {
            if (client == null || btnConnect.Text == "Connect")
            {
                btnTestgRPC.Enabled = false;
                SetStatus("This is only available while connected.");
                return;
            }
            ProgressUpdateMassSet(false, 0);
            if (tmrProgressUpdate.Enabled)
            {
                tmrProgressUpdate.Enabled = false;
                SetStatus("Disabled progress update check");
            } else
            {
                tmrProgressUpdate.Enabled = true;
                SetStatus("Testing progress updates...");
            }
        }

        private void txtProcess_TextChanged(object sender, EventArgs e)
        {

        }

        bool isJoyPressPressing;
        private void tmrJoyPress_Tick(object sender, EventArgs e)
        {
          
            if (btnTestJoystick.Text == "Detect Joystick") return;
            foreach (var key in config.keys)
            {
                if (key.Type != "Joystick" || key.joystickIndex.ToString() != cmbJoyID.Text || key.joystick == null)
                {
                    continue;
                }
                var joystick = key.joystick;
                var jid = key.KeyCode;
                //joystick.GetVJDAxisMax(jid, HID_USAGES.HID_USAGE_X, ref maxval);

                Form1.JoystickIdle(joystick, 0, (int)maxval);
                Form1.JoystickButtonIdle(joystick, jid);

                isJoyPressPressing = !isJoyPressPressing;
                if (isJoyPressPressing)
                {
                    joystick.ResetVJD(jid);
                    joystick.SetAxis((int)(maxval * 0.5f), jid, HID_USAGES.HID_USAGE_X);
                    joystick.SetAxis((int)(maxval * 0.5f), jid, HID_USAGES.HID_USAGE_Y);                    
                    SetStatus("Releasing");
                    return;
                }
                
                
                if (cmbJoystick.Text == "Up")
                {
                    joystick.SetAxis((int)0, jid, HID_USAGES.HID_USAGE_Y);
                    joystick.SetAxis((int)(maxval * 0.5f), jid, HID_USAGES.HID_USAGE_X);
                }
                else if (cmbJoystick.Text == "Down") 
                {
                    joystick.SetAxis((int)maxval, jid, HID_USAGES.HID_USAGE_Y);
                    joystick.SetAxis((int)(maxval * 0.5f), jid, HID_USAGES.HID_USAGE_X);
                }
                else if (cmbJoystick.Text == "Left")
                {
                    joystick.SetAxis((int)0, jid, HID_USAGES.HID_USAGE_X);
                    joystick.SetAxis((int)(maxval * 0.5f), jid, HID_USAGES.HID_USAGE_Y);
                }
                else if (cmbJoystick.Text == "Right")
                {
                    joystick.SetAxis((int)maxval, jid, HID_USAGES.HID_USAGE_X);
                    joystick.SetAxis((int)(maxval * 0.5f), jid, HID_USAGES.HID_USAGE_Y);
                }
                else
                {
                    uint btn;
                    UInt32.TryParse(cmbJoystick.Text, out btn);
                    if (btn < 1) return;                     
                    joystick.SetBtn(true, jid, btn);
                }
                SetStatus("Pressing " + cmbJoystick.Text);

                break;
            }

        }

        private void chkJoyUp_CheckedChanged(object sender, EventArgs e)
        {
            if (chkJoyUp.Checked)
            {
                tmrJoyPress.Enabled = true;
            } else
            {
                foreach (var key in config.keys)
                {
                    if (key.Type != "Joystick") continue;
                    JoystickIdle(key.joystick, (byte)key.KeyCode, 32767);
                    tmrJoyPress.Enabled = false;
                }
                SetStatus("No longer pressing keys");                                
            }
        }

        private void cmbJoystick_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void toolStripProgressBar1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripProgressBar2_Click(object sender, EventArgs e)
        {

        }

        private void ProgressUpdateMassSet(bool isPressed, double progress)
        {
            var prg = new ProgressUpdate();
            foreach (var key in config.keys)
            {
                if (key.Type == "JoystickKey" || key.Type == "Tactile")
                {
                    var tac = new ProgressUpdate.Types.TactileUpdate();
                    tac.Id = 0;
                    tac.Fired = isPressed;
                    tac.Progress = progress;
                    prg.Tactile.Add(tac);
                }

                if (key.Type == "Joystick")
                {
                    var joy = new ProgressUpdate.Types.JoystickUpdate();
                    joy.Id = 0;
                    joy.Intensity = progress;
                    prg.Joystick.Add(joy);
                }
            }

            client.SendProgressUpdate(prg);
        }
        
        bool puTestPressed;
        double puTestIntensity;
        private void tmrProgressUpdate_Tick(object sender, EventArgs e)
        {
            if (config.keys.Length == 0)
            {
                MessageBox.Show("No keys are configured, cannot test");
                return;
            }
            puTestIntensity += 0.1D;
            puTestPressed = true;
            if (puTestIntensity > 1D)
            {
                puTestIntensity = 0D;
                puTestPressed = false;
            }
            Console.WriteLine(puTestIntensity);
            SetStatus((puTestPressed) ? "Pressing all buttons " : "Releasing all buttons " + puTestIntensity.ToString());
            ProgressUpdateMassSet(puTestPressed, puTestIntensity);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            lblStatus.Width = Width - 76;
        }
    }

    public class KeyBind
    {
        public int Index;
        public string Label;
        public string Type;
        public string KeyName;
        public byte KeyCode;
        [JsonIgnore]
        public int IdleTime;
        [JsonIgnore]
        public bool IsPressed;
        [JsonIgnore]
        public bool IsEnabled; //This is a flag primarily for joysticks
        //Used by JoystickKey, mapping which joystick to press
        public int joystickIndex;
        [JsonIgnore]
        public vJoy joystick;
        [JsonIgnore]
        public vJoy.JoystickState joystickReport;
        public KeyBind(int index, string label, string type, string keyname, byte keycode)
        {
            Index = index;
            Label = label;
            Type = type;
            KeyName = keyname;
            KeyCode = keycode;
            if (type == "Joystick")
            {
                joystick = new vJoy();
                joystickReport = new vJoy.JoystickState();
            }
        }
    }
    public class RobotServiceClient
    {
        long maxval = 32767;
        readonly RobotService.RobotServiceClient client;
        
        public RobotServiceClient(RobotService.RobotServiceClient client)
        {
            this.client = client;
        }

        public void SendProgressUpdate(ProgressUpdate progressUpdate)
        {
            try
            {
                var req = new ProgressUpdateRequest();
                req.ProgressUpdate = progressUpdate;
                ProgressUpdateResponse resp = client.ProgressUpdate(req);
            } catch (RpcException e)
            {
                MessageBox.Show("Rpc ProgressUpdate Failed: " + e.Message);
            }
        }

        //Handle Stream of Reports
        public async Task StreamReport()
        {
                       
            UInt32 jid;
            //Start Stream Report
            try
            {
                //Prepare a Stream Report for usage
                using (var call = client.StreamReport(new StreamRequest()))
                {
                    var responseStream = call.ResponseStream;
                    //Loop the stream, consuming data
                    while (await responseStream.MoveNext())
                    {
                        ProgressUpdate prg = new ProgressUpdate();
                        bool isProgressUpdated = false;
                        Report report = responseStream.Current;
                        //Ignore report if window isn't focused
                        //Console.WriteLine(report);
                        if (Form1.GetInstance().processHandleWindow != w32.GetForegroundWindow())
                        {
                            Form1.GetInstance().Text = "[" + Form1.GetInstance().GetProcessName() + "] GoBeam v" + Application.ProductVersion;
                            //Form1.GetInstance().SetStatus("Window not visible");
                            //continue;
                        } else
                        {
                            Form1.GetInstance().Text = ">[" + Form1.GetInstance().GetProcessName() + "]< GoBeam v" + Application.ProductVersion;
                            //Form1.GetInstance().SetStatus("");
                        }
                        //iterate keymap
                        foreach (var key in Form1.config.keys)
                        {
                            bool isAnyButtonPressed = false;
                            //iterate touches
                            foreach (var touch in report.Tactile)
                            {    
                                if (key.Index != touch.Id) continue;
                                if (Form1.GetInstance().processHandleWindow != w32.GetForegroundWindow()) continue;

                                if (touch.Holding > 0 || touch.PressFrequency > 0 || touch.ReleaseFrequency > 0)
                                {
                                    //Console.WriteLine(touch.Id + ": " + touch.Holding + ", "+ touch.PressFrequency+ "," + touch.ReleaseFrequency);
                                }

                                if (!key.IsPressed)
                                {
                                    if (key.IdleTime < 2) key.IdleTime++;                                    
                                }

                                //Press or release key based on report
                                if ((touch.PressFrequency != 0 && !key.IsPressed) ||
                                    (touch.ReleaseFrequency != 0 && key.IsPressed))
                                {
                                    key.IsPressed = !key.IsPressed;                                    

                                    if (key.IsPressed)
                                    {
                                        key.IdleTime = 0;
                                        isAnyButtonPressed = true;
                                        Form1.SetLeftProgress(1);
                                    }
                                    if (key.Type == "Tactile")
                                    {
                                        //Console.WriteLine("TactileKey" + touch.Id + " ("+key.KeyName+"), " + key.IsPressed);
                                        w32.keybd_event(key.KeyCode, 0, (key.IsPressed) ? 0 : w32.KEYEVENTF_KEYUP, 0);
                                    } else if (key.Type == "JoystickKey")
                                    {
                                        if (key.joystickIndex < 1)
                                        {
                                            continue;
                                        }
                                        Console.WriteLine("JoystickKey " + touch.Id + " (J: "+key.joystickIndex+", "+key.KeyName+"), " + key.IsPressed);
                                        foreach (var jKey in Form1.config.keys)
                                        {
                                            if (jKey.IsEnabled && 
                                                jKey.joystick != null && 
                                                jKey.joystickIndex == key.joystickIndex && 
                                                jKey.KeyCode == key.KeyCode)
                                            {
                                                jKey.joystick.SetBtn(key.IsPressed, (UInt32)key.joystickIndex, key.KeyCode);
                                                break;
                                            }
                                        }
                                        
                                     //   w32.keybd_event(key.KeyCode, 0, (key.IsPressed) ? 0 : w32.KEYEVENTF_KEYUP, 0);
                                    }
                                    var tac = new ProgressUpdate.Types.TactileUpdate();
                                    tac.Id = (UInt32)key.Index;
                                    tac.Fired = key.IsPressed;
                                    tac.Progress = (double)((key.IsPressed) ? 1.0 : 0.0);
                                    prg.Tactile.Add(tac);
                                    isProgressUpdated = true;                                  
                                }
                            }

                            if (!isAnyButtonPressed) Form1.SetLeftProgress(0);
                            
                            foreach (var joy in report.Joystick)
                            {
                                
                                if (key.Index != joy.Id) continue;
                                if (key.joystick == null) continue;
                                if (!key.IsEnabled) continue;
                                jid = (UInt32)key.KeyCode;
                                
                                //Console.WriteLine("joy " + joy.Id + ": " + joy.CoordMean.X + ", " + joy.CoordMean.Y);
                                if ((!double.Equals(joy.CoordMean.X, double.NaN) && !key.IsPressed) ||
                                    (!double.Equals(joy.CoordMean.Y, double.NaN) && !key.IsPressed) ||
                                    (double.Equals(joy.CoordMean.X, double.NaN) && key.IsPressed) ||
                                  ( double.Equals(joy.CoordMean.Y, double.NaN) && key.IsPressed))
                                {
                                    
                                    //Form1.GetInstance().SetStatus("Joystick");

                                    key.IsPressed = !key.IsPressed;
                                    var ju = new ProgressUpdate.Types.JoystickUpdate();
                                    ju.Id = jid;
                                    ju.Intensity = (double)((key.IsPressed) ? 1.0 : 0.0);
                                    prg.Joystick.Add(ju);
                                    isProgressUpdated = true;
                                }
                                //Console.Write(joy.CoordMean.X + "," + joy.CoordMean.Y);

                                //Form1.JoystickIdle(key.joystick, (byte)jid, (int)maxval);
                                
                                if (double.Equals(joy.CoordMean.X, double.NaN) && double.Equals(joy.CoordMean.Y, double.NaN))
                                {                                   
                                    if (key.IdleTime < 3) key.IdleTime++;

                                    if (key.IdleTime > 2)
                                    {
                                        //Console.WriteLine("idle");
                                        //Form1.GetInstance().SetStatus("Idle");                                      
                                        key.joystick.ResetVJD(jid);
                                        Form1.JoystickIdle(key.joystick, (byte)jid, (int)maxval);
                                        Form1.SetRightProgress(0);
                                        key.joystick.SetAxis((int)(maxval * 0.5f), jid, HID_USAGES.HID_USAGE_X);
                                        key.joystick.SetAxis((int)(maxval * 0.5f), jid, HID_USAGES.HID_USAGE_Y);
                                    }
                                } else
                                {
                                    Form1.SetRightProgress(1);
                                    key.IdleTime = 0;
                                    if (double.Equals(joy.CoordMean.X,double.NaN))
                                    {
                                        key.joystick.SetAxis((int)(maxval * 0.5f), jid, HID_USAGES.HID_USAGE_X);
                                    }
                                    else
                                    {
                                        //Console.Write(" X: " + ", " + joy.CoordMean.X);
                                        key.joystick.SetAxis((int)(maxval * ((joy.CoordMean.X + 1.0f) / 2.0f)), jid, HID_USAGES.HID_USAGE_X);
                                    }

                                    if (double.Equals(joy.CoordMean.Y, double.NaN))
                                    {
                                        key.joystick.SetAxis((int)(maxval * 0.5f), jid, HID_USAGES.HID_USAGE_Y);
                                    }
                                    else
                                    {
                                        //Console.Write(" Y: " + joy.CoordMean.Y);
                                        key.joystick.SetAxis((int)(maxval * ((joy.CoordMean.Y + 1.0f) / 2.0f)), jid, HID_USAGES.HID_USAGE_Y);
                                    }

                                   // Console.WriteLine(key.joystick);
                                }
                                
                               
                            }
                        }


                        if (isProgressUpdated)
                        {
                            //Console.WriteLine("Progress Update");
                            SendProgressUpdate(prg);
                        }
                    }
                }

               
                                
            }
            catch (RpcException e)
            {
                Form1.GetInstance().DisconnectRPC();
                MessageBox.Show("RPC Failed: " + e.Message);
                
               throw;
            }
        }
    }
}
