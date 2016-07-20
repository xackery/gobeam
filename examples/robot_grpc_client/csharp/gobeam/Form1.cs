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
                        //Echo out the report information
                        Console.WriteLine(report);
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
    }
}
