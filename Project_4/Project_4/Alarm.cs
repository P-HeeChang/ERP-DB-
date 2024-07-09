using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Project_4
{

 
    public partial class Alarm : Form
    {
        private System.Windows.Forms.Timer timer;
        private Color[] colors = { Color.Red, Color.Yellow }; // 원하는 색상 배열
        public Alarm()
        {
            InitializeComponent();
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 250;
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            ChangePanelColor();

        }
        private void Alarm_Load(object sender, EventArgs e)
        {

        }

        private void ChangePanelColor()
        {
            Random random = new Random();
            int index = random.Next(colors.Length);
            panel1.BackColor = colors[index];
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            button1.Font= new Font("G마켓 산스 TTF Bold", 28, FontStyle.Underline);
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.Font = new Font("G마켓 산스 TTF Bold", 28);
        }
    }
}
