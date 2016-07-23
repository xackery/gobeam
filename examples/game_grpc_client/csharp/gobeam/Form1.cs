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

        //Create a key binding list
        public KeyBind[] keys = { };

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

        private void Form1_Load(object sender, EventArgs e)
        {
            instance = this;
            String strAppDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);

            openFileDialog1.InitialDirectory = strAppDir;
            saveFileDialog1.InitialDirectory = strAppDir;

            keys = new KeyBind[]{
                new KeyBind(8, "W", "Tactile", "W", 0x57),
                new KeyBind(9, "A", "Tactile", "A", 0x41),
                new KeyBind(11, "D", "Tactile", "D", 0x44),
                new KeyBind(10, "S", "Tactile", "S", 0x53),
                new KeyBind(4, "CU", "Tactile", "U", 0x55),
                new KeyBind(5, "CL", "Tactile", "H", 0x48),
                new KeyBind(6, "CD", "Tactile", "J", 0x4A),
                new KeyBind(7, "CR", "Tactile", "K", 0x4B),
                new KeyBind(2, "A", "Tactile", "Z", 0x5A),
                new KeyBind(3, "B", "Tactile", "X", 0x58),
                new KeyBind(0, "Joy", "Joystick", "1", 1),
                new KeyBind(1, "Enter", "Tactile", "Enter", 0x0D),
            };
            foreach (var key in keys)
            {
                grdControls.Rows.Add(key.Index, key.Label, key.Type, key.KeyName);
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
            btnTestJoystick.Enabled = false;
            btnTestJoystick.Text = "Detect Joystick";
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
            if (client != null && channel != null)
            {
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

                    var joystick = key.joystick;
             
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
                SetStatus("Joysticks configured");
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
            
            if (keys[testJoystickIndex].Type != "Joystick")
            {
                SetStatus("Skipping #" + keys[testJoystickIndex].Index + " " + keys[testJoystickIndex].KeyName + " (Not joystick)");
                testJoystickIndex++;
                return;
            }            
            string direction = "";
            UInt32 id = keys[testJoystickIndex].KeyCode;
            var joystick = keys[testJoystickIndex].joystick;
            //Get max value
            long maxval = 0;            
            joystick.GetVJDAxisMax(id, HID_USAGES.HID_USAGE_X, ref maxval);
            // Reset this device to default values
            joystick.ResetVJD(id);

            if (testJoystickDirection == 0)
            {
                direction = "100% up";
                joystick.SetAxis((int)maxval, keys[testJoystickIndex].KeyCode, HID_USAGES.HID_USAGE_Y);
            }
            if (testJoystickDirection == 1)
            {
                direction = "100% down";
                joystick.SetAxis(0, keys[testJoystickIndex].KeyCode, HID_USAGES.HID_USAGE_Y);
            }

            if (testJoystickDirection == 2)
            {
                direction = "50% up";
                Console.Write(" Y: " + maxval + "," + (int)(maxval * 0.75f));
                joystick.SetAxis((int)(maxval*0.75f), keys[testJoystickIndex].KeyCode, HID_USAGES.HID_USAGE_Y);
            }
            if (testJoystickDirection == 3)
            {
                direction = "50% down";
                joystick.SetAxis((int)(maxval * 0.25f), keys[testJoystickIndex].KeyCode, HID_USAGES.HID_USAGE_Y);
            }

            if (testJoystickDirection == 4)
            {
                direction = "100% right";
                joystick.SetAxis((int)(maxval * 1f), keys[testJoystickIndex].KeyCode, HID_USAGES.HID_USAGE_X);
            }
            if (testJoystickDirection == 5)
            {
                direction = "100% left";
                joystick.SetAxis((int)(maxval * 0f), keys[testJoystickIndex].KeyCode, HID_USAGES.HID_USAGE_X);
            }
            if (testJoystickDirection == 6)
            {
                direction = "50% right";
                joystick.SetAxis((int)(maxval * 0.75f), keys[testJoystickIndex].KeyCode, HID_USAGES.HID_USAGE_X);
            }
            if (testJoystickDirection == 7)
            {
                direction = "50% left";
                joystick.SetAxis((int)(maxval * 0.25f), keys[testJoystickIndex].KeyCode, HID_USAGES.HID_USAGE_X);
            }
            if (testJoystickDirection == 8)
            {
                direction = "25% down-right";
                joystick.SetAxis((int)(maxval * 0.62f), keys[testJoystickIndex].KeyCode, HID_USAGES.HID_USAGE_X);
                joystick.SetAxis((int)(maxval * 0.62f), keys[testJoystickIndex].KeyCode, HID_USAGES.HID_USAGE_Y);
            }
            if (testJoystickDirection == 9)
            {
                direction = "25% up-left";
                joystick.SetAxis((int)(maxval * 0.38f), keys[testJoystickIndex].KeyCode, HID_USAGES.HID_USAGE_X);
                joystick.SetAxis((int)(maxval * 0.38f), keys[testJoystickIndex].KeyCode, HID_USAGES.HID_USAGE_Y);
            }

            SetStatus("Pressing #"+keys[testJoystickIndex].Index+" "+ keys[testJoystickIndex].KeyName+": "+direction+" ("+maxval+")");
            Console.WriteLine(joystick);
            testJoystickDirection++;
            if (testJoystickDirection > 10)
            {
                testJoystickIndex++;
                testJoystickDirection = 0;
            }

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
            try
            {
                File.WriteAllText(Path.GetFullPath(saveFileDialog1.FileName), JsonConvert.SerializeObject(config, Formatting.Indented));
                SetStatus(Path.GetFileName(saveFileDialog1.FileName) + " saved.");
            } catch (Exception err)
            {
                MessageBox.Show(err.Message, "Failed to save", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void loadKeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Load new configuration without saving?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes) return;

            openFileDialog1.Filter = "JSON Files|*.json";
            openFileDialog1.DefaultExt = ".json";
            result = openFileDialog1.ShowDialog();
            if (result != DialogResult.OK) return;

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
            var result = MessageBox.Show("Clear all settings without saving?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes) return;

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
            ResetDefaults();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            
           
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Written by Xackery", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnTestgRPC_Click(object sender, EventArgs e)
        {
            var prg = new ProgressUpdate();
            var tac = new ProgressUpdate.Types.TactileUpdate();
            tac.Id = 0;
            tac.Fired = true;
            tac.Progress = 1;
            prg.Tactile.Add(tac);
            client.SendProgressUpdate(prg);
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
        public bool IsPressed;
        [JsonIgnore]
        public bool IsEnabled; //This is a flag primarily for joysticks
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
            
            long maxval = 32767;
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
                            //Form1.GetInstance().SetStatus("Window not visible");
                            continue;
                        } else
                        {
                            //Form1.GetInstance().SetStatus("");
                        }
                        //iterate keymap
                        foreach (var key in Form1.GetInstance().keys)
                        {
                            //iterate touches
                            foreach (var touch in report.Tactile)
                            {    
                                if (key.Index != touch.Id) continue;

                                if (touch.Holding > 0 || touch.PressFrequency > 0 || touch.ReleaseFrequency > 0)
                                {
                                    Console.WriteLine(touch.Id + ": " + touch.Holding + ", "+ touch.PressFrequency+ "," + touch.ReleaseFrequency);
                                }
                                //Press or release key based on report
                                if ((touch.PressFrequency != 0 && !key.IsPressed) ||
                                    (touch.ReleaseFrequency != 0 && key.IsPressed))
                                {
                                    key.IsPressed = !key.IsPressed;
                                    Console.WriteLine("KeyPress " + touch.Id + "," + key.IsPressed);
                                    w32.keybd_event(key.KeyCode, 0, (key.IsPressed) ? 0 : w32.KEYEVENTF_KEYUP, 0);
                                    var tac = new ProgressUpdate.Types.TactileUpdate();
                                    tac.Id = (UInt32)key.Index;
                                    tac.Fired = key.IsPressed;                                    
                                    tac.Progress = (key.IsPressed) ? 1 : 0;
                                    prg.Tactile.Add(tac);
                                    isProgressUpdated = true;                                                                   
                                }
                            }
                            
                            foreach (var joy in report.Joystick)
                            {
                                
                                if (key.Index != joy.Id) continue;
                                if (key.joystick == null) continue;
                                if (!key.IsEnabled) continue;
                                jid = (UInt32)key.KeyCode;

                                //Console.WriteLine("joy " + joy.Id + ": " + joy.CoordMean.X + ", " + joy.CoordMean.Y);
                                if (joy.CoordMean.X != 0 && !key.IsPressed ||
                                    joy.CoordMean.Y != 0 && !key.IsPressed ||
                                    joy.CoordMean.X == 0 && key.IsPressed ||
                                    joy.CoordMean.Y == 0 && key.IsPressed)
                                {
                                    
                                    Form1.GetInstance().SetStatus("Joystick");

                                    key.IsPressed = !key.IsPressed;
                                    var ju = new ProgressUpdate.Types.JoystickUpdate();
                                    ju.Id = jid;
                                    ju.Intensity = (key.IsPressed) ? 1 : 0;
                                    prg.Joystick.Add(ju);
                                    isProgressUpdated = true;                                   
                                }
                                //Console.Write(joy.CoordMean.X + "," + joy.CoordMean.Y);

                                if (joy.CoordMean.X == 0 && joy.CoordMean.Y == 0)
                                {
                                    key.joystick.ResetVJD(jid);
                                } else
                                {
                                    if (double.Equals(joy.CoordMean.X,double.NaN))
                                    {
                                        key.joystick.SetAxis((int)(maxval * 0.5f), jid, HID_USAGES.HID_USAGE_X);
                                    }
                                    else
                                    {
                                        Console.Write(" X: " + ", " + joy.CoordMean.X);
                                        key.joystick.SetAxis((int)(maxval * ((joy.CoordMean.X + 1.0f) / 2.0f)), jid, HID_USAGES.HID_USAGE_X);
                                    }

                                    if (double.Equals(joy.CoordMean.Y, double.NaN))
                                    {
                                        key.joystick.SetAxis((int)(maxval * 0.5f), jid, HID_USAGES.HID_USAGE_Y);
                                    }
                                    else
                                    {
                                        Console.Write(" Y: " + joy.CoordMean.Y);
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
