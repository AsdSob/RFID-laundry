using PlcControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        FinsTcp FinsTcp1, FinsTcp2, FinsTcp3;
        public Form1()
        {
            InitializeComponent();
            FinsTcp1 = new FinsTcp(localip.Text, remoteip1.Text, 9600);
            FinsTcp2 = new FinsTcp(localip.Text, remoteip2.Text, 9600);
            FinsTcp3 = new FinsTcp(localip.Text, remoteip3.Text, 9600);
            label9.Text = label10.Text = label11.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button.Name=="button1")
            {
                if (!FinsTcp1.connected)
                   label9.Text = FinsTcp1.conn(localip.Text, remoteip1.Text, 9600).ToString();
            }
            else if (button.Name=="button2")
            {
                if (!FinsTcp2.connected)
                    label10.Text = FinsTcp2.conn(localip.Text, remoteip2.Text, 9600).ToString();
            }
            else if (button.Name == "button3")
            {
                if (!FinsTcp3.connected)
                    label11.Text = FinsTcp3.conn(localip.Text, remoteip3.Text, 9600).ToString();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!FinsTcp1.connected)
                label9.Text = FinsTcp1.conn(localip.Text, remoteip1.Text, 9600).ToString();
            if (!FinsTcp2.connected)
                label10.Text = FinsTcp2.conn(localip.Text, remoteip2.Text, 9600).ToString();
            if (!FinsTcp3.connected)
                label11.Text = FinsTcp3.conn(localip.Text, remoteip3.Text, 9600).ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button.Name == "button5")  //start
            {
                if (!FinsTcp2.connected)
                    label10.Text = FinsTcp2.conn(localip.Text, remoteip2.Text, 9600).ToString();
                FinsTcp2.Start();
                if (!FinsTcp3.connected)
                    label11.Text = FinsTcp3.conn(localip.Text, remoteip3.Text, 9600).ToString();
                FinsTcp3.Start();
            }
            else if (button.Name == "button6") //stop
            {
                if (!FinsTcp2.connected)
                    label10.Text = FinsTcp2.conn(localip.Text, remoteip2.Text, 9600).ToString();
                FinsTcp2.Stop();
                if (!FinsTcp3.connected)
                    label11.Text = FinsTcp3.conn(localip.Text, remoteip3.Text, 9600).ToString();
                FinsTcp3.Stop();
            }
            else if (button.Name == "button7") //stop
            {
                if (!FinsTcp2.connected)
                    label10.Text = FinsTcp2.conn(localip.Text, remoteip2.Text, 9600).ToString();
                FinsTcp2.Reset();
                if (!FinsTcp3.connected)
                    label11.Text = FinsTcp3.conn(localip.Text, remoteip3.Text, 9600).ToString();
                FinsTcp3.Reset();
            }
            else if (button.Name == "button8") //Clear
            {
                if (!FinsTcp2.connected)
                    label10.Text = FinsTcp2.conn(localip.Text, remoteip2.Text, 9600).ToString();
                FinsTcp2.Clear();
                if (!FinsTcp3.connected)
                    label11.Text = FinsTcp3.conn(localip.Text, remoteip3.Text, 9600).ToString();
                FinsTcp3.Clear();
            } 

        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (!FinsTcp1.connected)
                label9.Text = FinsTcp1.conn(localip.Text, remoteip1.Text, 9600).ToString();
            bool ClotheReady = FinsTcp1.GetClotheReady();
            if (ClotheReady)
            {
                button10.Enabled = button11.Enabled = true;
            }

        }

        private void button10_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (!FinsTcp1.connected)
                label9.Text = FinsTcp1.conn(localip.Text, remoteip1.Text, 9600).ToString();
            //The above is to check the connection status of the device.
            if (button.Name == "button10")
                FinsTcp1.Sorting(1);   //This is the order to the line 1
            else if (button.Name == "button11")
                FinsTcp1.Sorting(2); //This is the order to the line 2
            button10.Enabled = button11.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button10.Enabled = button11.Enabled = false;
            label12.Size = new Size(397, 473);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            label12.Visible = !label12.Visible;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            if (button.Name == "button13")
            {
                if (!FinsTcp2.connected)
                    label10.Text = FinsTcp2.conn(localip.Text, remoteip2.Text, 9600).ToString();

                FinsTcp2.HangUpToPoint(int.Parse(textBox1.Text));
            }

            else if (button.Name == "button14")
            {
                if (!FinsTcp3.connected)
                    label11.Text = FinsTcp3.conn(localip.Text, remoteip3.Text, 9600).ToString();
                FinsTcp3.HangUpToPoint(int.Parse(textBox2.Text));
            }
            
        }

        private void button16_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            if (button.Name == "button16")
            {
                if (!FinsTcp2.connected)
                    label10.Text = FinsTcp2.conn(localip.Text, remoteip2.Text, 9600).ToString();

                FinsTcp2.TakeOutClothes(textBox4.Text.Trim());   //HangUpToPoint(int.Parse(textBox1.Text));
            }

            else if (button.Name == "button15")
            {
                if (!FinsTcp3.connected)
                    label11.Text = FinsTcp3.conn(localip.Text, remoteip3.Text, 9600).ToString();
                FinsTcp3.TakeOutClothes(textBox3.Text.Trim());
            }
        }

        private void rbauto_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.Name == "rb1auto")
            {
                if (!FinsTcp2.connected)
                    label10.Text = FinsTcp2.conn(localip.Text, remoteip2.Text, 9600).ToString();
                FinsTcp2.SetModel(1);
            }
            else if (rb.Name == "rb1pc")
            {
                if (!FinsTcp2.connected)
                    label10.Text = FinsTcp2.conn(localip.Text, remoteip2.Text, 9600).ToString();
                FinsTcp2.SetModel(0);
            }
            else if (rb.Name == "rb2auto")
            {
                if (!FinsTcp3.connected)
                    label11.Text = FinsTcp3.conn(localip.Text, remoteip3.Text, 9600).ToString();
                FinsTcp3.SetModel(1);
            }
            else if (rb.Name == "rb2pc")
            {
                if (!FinsTcp3.connected)
                    label11.Text = FinsTcp3.conn(localip.Text, remoteip3.Text, 9600).ToString();
                FinsTcp3.SetModel(0);
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button.Name == "button17")
            {
                if (!FinsTcp2.connected)
                    label10.Text = FinsTcp2.conn(localip.Text, remoteip2.Text, 9600).ToString();
                lb1nowpoint.Text = FinsTcp2.GetNowPoint().ToString();
            }
            else if (button.Name == "button18")
            {
                if (!FinsTcp3.connected)
                    label11.Text = FinsTcp3.conn(localip.Text, remoteip3.Text, 9600).ToString();
                lb2nowpoint.Text = FinsTcp3.GetNowPoint().ToString();
            }
            
        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (!FinsTcp1.connected)
                label9.Text = FinsTcp1.conn(localip.Text, remoteip1.Text, 9600).ToString();
            FinsTcp1.Packclothes();
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (!FinsTcp1.connected)
                label9.Text = FinsTcp1.conn(localip.Text, remoteip1.Text, 9600).ToString();
            FinsTcp1.ResetWaitHangNum();
        }

        private void button21_Click(object sender, EventArgs e)
        {
            if (!FinsTcp1.connected)
                label9.Text = FinsTcp1.conn(localip.Text, remoteip1.Text, 9600).ToString();
            label26.Text=FinsTcp1.GetWaitHangNum().ToString();
        }

        private void rb1pc_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rb1pc_CheckedChanged_1(object sender, EventArgs e)
        {

        }

    }
}
