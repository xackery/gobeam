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

namespace gobeam
{
    public class RobotServiceClient
    {
        readonly RobotService.RobotServiceClient client;

        public RobotServiceClient(RobotService.RobotServiceClient client)
        {
            this.client = client;
        }


        /// <summary>
        /// Server-streaming example. Calls listFeatures with a rectangle of interest. Prints each response feature as it arrives.
        /// </summary>
        public async Task StreamReport()
        {
            Console.WriteLine("StreamReport!");
            try
            {
                using (var call = client.StreamReport(new StreamRequest()))
                {                    
                    var responseStream = call.ResponseStream;
                    Console.WriteLine("Trying");
                    while (await responseStream.MoveNext())
                    {
                        Console.WriteLine("Loop report");
                        Report report = responseStream.Current;
                        Console.WriteLine(report);
                    }
                    Console.WriteLine("Ending");
                }
            }
            catch (RpcException e)
            {
                Console.WriteLine("Fail");
                MessageBox.Show("RPC Failed: " + e);
                throw;
            }
            Console.WriteLine("End");
        }
    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                var channel = new Channel("192.168.1.109:50051", ChannelCredentials.Insecure);
                var rClient = new RobotService.RobotServiceClient(channel);
                var client = new RobotServiceClient(rClient);
                Console.WriteLine("Loaded");
                client.StreamReport().Wait();
            }
            catch (Exception err)
            {
                MessageBox.Show("Failed to load:", err.Message);
            }
        }
    }
}
