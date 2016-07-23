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

namespace gobeam
{

    public partial class Form1 : Form
    {

        // Declaring one joystick (Device id 1) and a position structure. 
        static public vJoy joystick;
        static public vJoy.JoystickState iReport;

        //Create a key binding list
        public KeyBind[] keys = { };

        public IntPtr processHandleWindow;
        IntPtr handle;
        private Process sourceProcess;
        private pointerset scoreps = new pointerset();
        private pointerset ballps = new pointerset();

        private Channel channel;
        private RobotServiceClient client;

        public Form1()
        {
            InitializeComponent();
        }

        private static Form1 instance;
        public static Form1 GetInstance()
        {
            return instance;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            instance = this;
            keys = new KeyBind[]{
               /* new KeyBind(0, "W", "Tactile", "W", 0x57),
                new KeyBind(1, "A", "Tactile", "A", 0x41),
                new KeyBind(2, "D", "Tactile", "D", 0x44),
                new KeyBind(3, "LEFT", "Tactile", "LEFT", 0x25),
                new KeyBind(4, "RIGHT", "Tactile", "RIGHT", 0x27),
                new KeyBind(5, "UP", "Tactile", "UP", 0x26),
                new KeyBind(6, "4", "Tactile", "4", 0x64),
                new KeyBind(7, "8", "Tactile", "8", 0x68),
                new KeyBind(8, "6", "Tactile", "6", 0x66),*/
                new KeyBind(0, "Joy", "Joystick", "1", 1),
            };
            foreach (var key in keys)
            {
                grdControls.Rows.Add(key.Index, key.Label, key.Type, key.KeyName);
            }
            joystick = new vJoy();
            iReport = new vJoy.JoystickState();
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
            this.Text = "GoBeam [" + txtProcess.Text + "]";
            btnTestControls.Enabled = true;
            btnTestJoystick.Enabled = true;
            lblStatus.Text = "Attached to " + txtProcess.Text + ".";
        }


        private void detachProcess()
        {
            handle = IntPtr.Zero;
            processHandleWindow = IntPtr.Zero;
            this.Text = "GoBeam Client";
            txtProcess.Enabled = true;
            btnAttach.Text = "Attach";
            btnAttach.Enabled = true;
            btnTestControls.Enabled = false;
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

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (client != null && channel != null)
            {
                channel.ShutdownAsync();
                client = null;
                btnConnect.Text = "Connect";
                channel = null;
                return;
            }

            try
            {
                SetStatus("Connecting to " + txtAddr.Text + "...");
                //Create a connection
                var channel = new Channel(txtAddr.Text, ChannelCredentials.Insecure);
                var rClient = new RobotService.RobotServiceClient(channel);
                client = new RobotServiceClient(rClient);
                client.StreamReport();
                btnConnect.Text = "Disconnect";
                SetStatus("Connected to " + txtAddr.Text + ".");
            }
            catch (Exception err)
            {
                MessageBox.Show("Failed to connect to gRPC client:", err.Message);
                btnConnect.Text = "Connect";
                SetStatus("Disconnected from " + txtAddr.Text + ".");
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
            if (grdControls.Rows[e.RowIndex].Cells[2].Value.ToString() == "Joystick")
            {
                string id = Microsoft.VisualBasic.Interaction.InputBox("Joystick ID", "Enter Joystick ID # (1 is default)", "1");
                if (Byte.Parse(id) == 0)
                {
                    MessageBox.Show("Invalid ID: " + id + ", discarding");
                    return;
                }
                bool isFound = false;
                foreach (var key in keys)
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
                grdControls.Rows[e.RowIndex].Cells[3].Value = id;
                return;
            }

            lastKeyIndex = -1;
            foreach (var key in keys)
            {
                if (key.Index == (int)grdControls.Rows[e.RowIndex].Cells[0].Value)
                {
                    lastKeyIndex = key.Index;
                    break;
                }
            }
            if (lastKeyIndex == -1)
            {
                MessageBox.Show("An internal error occured");
                return;
            }
            if (inputKey == null) inputKey = new InputKey();
            inputKey.Show();
            this.Enabled = false;
        }

        int lastKeyIndex;
        public void SetFocusedKey(string label, byte key)
        {
            for (var i = 0; i < grdControls.Rows.Count - 1; i++)
            {
                var row = grdControls.Rows[i];
                if (row.Cells[0].Value.ToString() == lastKeyIndex.ToString())
                {
                    row.Cells[3].Value = label;
                    for (var j = 0; j < keys.Count(); j++)
                    {
                        if (keys[j].Index == lastKeyIndex)
                        {
                            keys[j].KeyName = label;
                            keys[j].KeyCode = key;
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

            
            if (!joystick.vJoyEnabled())
            {
                btnTestJoystick.Text = "Detect Joystick";
                MessageBox.Show("Joystick is not enabled.");
                return;
            }
            if (btnTestJoystick.Text == "Detect Joystick")
            {
                foreach (var key in keys)
                {
                    UInt32 id = key.KeyCode;
                    if (key.Type != "Joystick") continue;
                    if (key.KeyCode < 1)
                    {
                        MessageBox.Show("Key #" + key.Index + " has an invalid joystick set: " + key.KeyCode + " (Must be greater than 0)");
                        return;
                    }

                    if (!joystick.vJoyEnabled())
                    {
                        SetStatus("vJoy was not detected");
                        MessageBox.Show("vJoy driver not enabled. This likely means you need to install vJoy drivers, or reboot if you recently installed them.", "vJoy driver not enabled.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnTestJoystick.Text = "Detect Joystick";
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
                //MessageBox.Show("Joysticks are configured successfully. Press Test Joystick if you wish to test them.");
                btnTestJoystick.Text = "Test Joystick";
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

            if (keys[testIndex].Type != "Joystick")
            {
                SetStatus("Skipping #" + keys[testIndex].Index + " " + keys[testIndex].KeyName + " (Not joystick)");
                testIndex++;
                return;
            }            
            string direction = "";
            UInt32 id = keys[testIndex].KeyCode;
            //Get max value
            long maxval = 0;
            joystick.GetVJDAxisMax(id, HID_USAGES.HID_USAGE_X, ref maxval);
            // Reset this device to default values
            joystick.ResetVJD(id);

            if (testJoystickDirection == 0)
            {
                direction = "100% up";
                joystick.SetAxis((int)maxval, keys[testIndex].KeyCode, HID_USAGES.HID_USAGE_Y);
            }
            if (testJoystickDirection == 1)
            {
                direction = "100% down";
                joystick.SetAxis(0, keys[testIndex].KeyCode, HID_USAGES.HID_USAGE_Y);
            }

            if (testJoystickDirection == 2)
            {
                direction = "50% up";
                joystick.SetAxis((int)(maxval*0.75f), keys[testIndex].KeyCode, HID_USAGES.HID_USAGE_Y);
            }
            if (testJoystickDirection == 3)
            {
                direction = "50% down";
                joystick.SetAxis((int)(maxval * 0.25f), keys[testIndex].KeyCode, HID_USAGES.HID_USAGE_Y);
            }

            if (testJoystickDirection == 4)
            {
                direction = "100% right";
                joystick.SetAxis((int)(maxval * 1f), keys[testIndex].KeyCode, HID_USAGES.HID_USAGE_X);
            }
            if (testJoystickDirection == 5)
            {
                direction = "100% left";
                joystick.SetAxis((int)(maxval * 0f), keys[testIndex].KeyCode, HID_USAGES.HID_USAGE_X);
            }
            if (testJoystickDirection == 6)
            {
                direction = "50% right";
                joystick.SetAxis((int)(maxval * 0.75f), keys[testIndex].KeyCode, HID_USAGES.HID_USAGE_X);
            }
            if (testJoystickDirection == 7)
            {
                direction = "50% left";
                joystick.SetAxis((int)(maxval * 0.25f), keys[testIndex].KeyCode, HID_USAGES.HID_USAGE_X);
            }
            if (testJoystickDirection == 8)
            {
                direction = "25% down-right";
                joystick.SetAxis((int)(maxval * 0.62f), keys[testIndex].KeyCode, HID_USAGES.HID_USAGE_X);
                joystick.SetAxis((int)(maxval * 0.62f), keys[testIndex].KeyCode, HID_USAGES.HID_USAGE_Y);
            }
            if (testJoystickDirection == 9)
            {
                direction = "25% up-left";
                joystick.SetAxis((int)(maxval * 0.38f), keys[testIndex].KeyCode, HID_USAGES.HID_USAGE_X);
                joystick.SetAxis((int)(maxval * 0.38f), keys[testIndex].KeyCode, HID_USAGES.HID_USAGE_Y);
            }

            SetStatus("Pressing #"+keys[testIndex].Index+" "+ keys[testIndex].KeyName+": "+direction+" ("+maxval+")");

            testJoystickDirection++;
            if (testJoystickDirection > 10)
            {
                testJoystickIndex++;
                testJoystickDirection = 0;
            }

        }
    }

    public class KeyBind
    {
        public int Index;
        public string Label;
        public string Type;
        public string KeyName;
        public byte KeyCode;
        public bool IsPressed;
        public bool IsEnabled; //This is a flag primarily for joysticks
        public KeyBind(int index, string label, string type, string keyname, byte keycode)
        {
            Index = index;
            Label = label;
            Type = type;
            KeyName = keyname;
            KeyCode = keycode;
        }
    }
    public class RobotServiceClient
    {

        readonly RobotService.RobotServiceClient client;
        
        public RobotServiceClient(RobotService.RobotServiceClient client)
        {
            this.client = client;
        }

        //Handle Stream of Reports
        public async Task StreamReport()
        {
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
                        Report report = responseStream.Current;
                        //Ignore report if window isn't focused
                        //Console.WriteLine(report);
                        if (Form1.GetInstance().processHandleWindow != w32.GetForegroundWindow())
                        {
                            Form1.GetInstance().SetStatus("Window not visible");
                            continue;
                        } else
                        {
                            Form1.GetInstance().SetStatus("");
                        }
                        //iterate touches
                        foreach(var touch in report.Tactile)
                        {
                            //iterate keymap
                            foreach (var key in Form1.GetInstance().keys)
                            {                                
                                if (key.Index != touch.Id) continue;

                                Console.WriteLine("Key " + touch.Id);
                                //Press or release key based on report
                                if ((touch.PressFrequency != 0 && key.IsPressed) ||
                                    (touch.ReleaseFrequency != 0 && !key.IsPressed))
                                {                                                                        
                                    key.IsPressed = !key.IsPressed;
                                    Console.WriteLine("KeyPress " + touch.Id + "," + key.IsPressed);
                                    w32.keybd_event(key.KeyCode, 0, (key.IsPressed) ? 0 : w32.KEYEVENTF_KEYUP, 0);
                                }      
                            }
                        }
                    }
                }

               
                                
            }
            catch (RpcException e)
            {
                MessageBox.Show("RPC Failed: " + e);
               throw;
            }
        }
    }
}
