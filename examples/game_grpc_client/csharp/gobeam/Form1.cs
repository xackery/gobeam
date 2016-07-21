﻿using Grpc.Core;
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


namespace gobeam
{

    public partial class Form1 : Form
    {

        public IntPtr processHandleWindow;
        IntPtr handle;
        private Process sourceProcess;
        private pointerset scoreps = new pointerset();
        private pointerset ballps = new pointerset();

        private int score;
        private int lastScore;
        private int ball;
        private int lastBall;

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
            attachProcess();

            try
            {
                //Create a connection
                var channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);
                var rClient = new RobotService.RobotServiceClient(channel);
                var client = new RobotServiceClient(rClient);
                Console.WriteLine("Loaded");
                client.StreamReport();
            }
            catch (Exception err)
            {
                MessageBox.Show("Failed to load:", err.Message);
            }
        }

        private void attachProcess()
        {
            var processes = w32.GetProcessList(txtProcess.Text);
            if (processes.Length == 0)
            {
                lblStatus.Text = "No "+txtProcess.Text+" process found";
                return;
            }
            else if (processes.Length > 1)
            {
                lblStatus.Text = "More than 1 "+txtProcess.Text+" process found.";
                return;
            }

            handle = w32.AttachProcess(processes[0]);
            sourceProcess = processes[0];
            processHandleWindow = processes[0].MainWindowHandle;
            txtProcess.Enabled = false;
        }

        private void detachProcess()
        {
            if (handle != null) {
                this.Text = "GoBeam Client";
                txtProcess.Enabled = true;

            }
        }

        public void SetStatus(string message)
        {
            lblStatus.Text = message;
        }

        private void startRound(int interval)
        {
            refreshTimer.Enabled = false;
            newRoundTimer.Interval = interval;
            newRoundTimer.Enabled = true;
            lastBall = ball;
            lastScore = score;
        }
        

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            if (processHandleWindow != w32.GetForegroundWindow())
            {
                return;
            }
            //Any iteration refresh stuff   
        }

        private void newRoundTimer_Tick(object sender, EventArgs e)
        {    
            refreshTimer.Enabled = true;
            newRoundTimer.Enabled = false;
        }

        private void btnAttach_Click(object sender, EventArgs e)
        {
            if (handle == null)
            {
                detachProcess();
            } else
            {
                attachProcess();
            }

        }
    }

    public class KeyBind
    {
        public bool IsPressed;
        public byte KeyCode;
        public KeyBind(byte key)
        {
            KeyCode = key;
        }
    }
    public class RobotServiceClient
    {
        //Create a key binding list
        private KeyBind[] keys = { new KeyBind(0x53), new KeyBind(w32.VK_D) };

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
                        
                        foreach(var touch in report.Tactile)
                        {                            
                            //Ignore presses of keys not mapped
                            if (touch.Id >= keys.Length) continue;
                            KeyBind key = keys[touch.Id];
                            Console.WriteLine("Key "+touch.Id);
                            //Press or release key based on report
                            if ((touch.Holding == 0 && key.IsPressed) ||
                                (touch.Holding == 1 && !key.IsPressed))
                            {
                                key.IsPressed = !key.IsPressed;
                            Console.WriteLine("KeyPress "+touch.Id+","+key.IsPressed);
                            w32.keybd_event(key.KeyCode, 0, (key.IsPressed) ? 0 : w32.KEYEVENTF_KEYUP, 0);
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
