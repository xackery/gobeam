using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace gobeam
{
    public partial class FormJsonLoader : Form
    {
        public FormJsonLoader()
        {
            InitializeComponent();
        }

        private BeamJson beamJson;
        private void FormJsonLoader_Load(object sender, EventArgs e)
        {

        }

        private void txtJson_TextChanged(object sender, EventArgs e)
        {
            if (txtJson.Text.Length < 5) return;
            try
            {
                beamJson = Newtonsoft.Json.JsonConvert.DeserializeObject<BeamJson>(txtJson.Text);
                btnLoad.Enabled = true;
                btnLoad.Text = "Load";
            } catch (Exception err)
            {
                Console.WriteLine(err.Message);
                btnLoad.Enabled = false;
                btnLoad.Text = "Invalid JSON";
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            //Make keys
            var keys = new KeyBind[beamJson.tactiles.Length+beamJson.joysticks.Length];
            var i = 0;
            foreach (var tac in beamJson.tactiles)
            {
                Keys keyData = (Keys)tac.Key;
                var key = new KeyBind(tac.id, tac.Text, "Tactile", keyData.ToString(), (byte)tac.Key);
                keys[i] = key;
                i++;
            }
            byte joyIndex = 1;
            foreach (var joy in beamJson.joysticks)
            {
                var key = new KeyBind(joy.id, "Joystick", "Joystick", joyIndex.ToString(), joyIndex);
                keys[i] = key;
                i++;
                joyIndex++;
            }
            Form1.RebuildKeys(keys);            
            this.Close();            
        }

        private void FormJsonLoader_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form1.GetInstance().Show();
            Form1.GetInstance().Focus();
        }
    }

    public class BeamJson
    {
        public int reportInterval;
        public BeamTactile[] tactiles;
        public BeamJoystick[] joysticks;
        public BeamScreen[] screens;

    }
    public class BeamTactile
    {
        public int id;
        public string type;
        public BeamBlueprint[] blueprint;
        public BeamTactileAnalysis analysis;
        public BeamCost cost;
        public BeamCooldown cooldown;
        public string Text;
        public int Key;
        public string Help;

    }

    public class BeamBlueprint {
        public int width;
        public int height;
        public string grid;
        public string state;
        public int x;
        public int y;

    }
    public class BeamTactileAnalysis
    {
        public bool holding;
        public bool frequency;
    }

    public class BeamCost
    {
        public BeamPress press;
    }

    public class BeamPress
    {
        public int cost;
    }
    public class BeamCooldown
    {
        public int press;
    }
    public class BeamJoystick
    {
        public int id;
        public string type;
        public BeamBlueprint[] blueprint;
        public BeamJoystickAnalysis analysis;
    }

    public class BeamJoystickAnalysis
    {
        public BeamJoystickAnalysisCoords coords;
    }

    public class BeamJoystickAnalysisCoords
    {
        public bool mean;
        public bool stdDev;
    }
    public class BeamScreen
    {
        public int id;
        public string type;
        public BeamBlueprint[] blueprint;
        public BeamJoystickAnalysis analysis;
        public bool clicks;
    }
}