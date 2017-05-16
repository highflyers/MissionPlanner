using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSender;

namespace MissionPlanner.GCSViews
{
    public partial class WebSocketConfig : UserControl
    {
        public WebSocketConfig()
        {
            InitializeComponent();
        }

        private void button_connect_Click(object sender, EventArgs e)
        {
            Console.Out.WriteLine("Clicked. IP = " + this.textBox_IP.Text);
            WebSocketSender.MessageManager.Instance.SetConnectionParameters(textBox_IP.Text, textBox_port.Text);
        }

        private void button_update_Click(object sender, EventArgs e)
        {
            label_queue_size.Text = WebSocketSender.MessageManager.Instance.QueueSize().ToString();
        }
    }
}
