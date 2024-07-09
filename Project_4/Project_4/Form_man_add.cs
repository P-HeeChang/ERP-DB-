using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace Project_4
{
    public partial class Form_man_add : Form
    {
        public Form_man_add()
        {
            InitializeComponent();
        }

        private void Form_man_add_Load(object sender, EventArgs e)
        {
            button2.Font = new Font("G마켓 산스 TTF Light", 16);
            label1.Font = new Font("G마켓 산스 TTF Light", 15);
            label2.Font = new Font("G마켓 산스 TTF Light", 15);
            label3.Font = new Font("G마켓 산스 TTF Light", 15);
            label3.Font = new Font("G마켓 산스 TTF Light", 15);
            label4.Font = new Font("G마켓 산스 TTF Light", 15);
            label5.Font = new Font("G마켓 산스 TTF Light", 15);
            label6.Font = new Font("G마켓 산스 TTF Light", 15);
            label7.Font = new Font("G마켓 산스 TTF Light", 15);
            label8.Font = new Font("G마켓 산스 TTF Light", 15);
            label9.Font = new Font("G마켓 산스 TTF Light", 15);
            label10.Font = new Font("G마켓 산스 TTF Light", 15);
            textBox1.Font = new Font("G마켓 산스 TTF Light", 15);
            textBox2.Font = new Font("G마켓 산스 TTF Light", 15);
            textBox3.Font = new Font("G마켓 산스 TTF Light", 15);
            textBox4.Font = new Font("G마켓 산스 TTF Light", 15);
            textBox5.Font = new Font("G마켓 산스 TTF Light", 15);
            textBox6.Font = new Font("G마켓 산스 TTF Light", 15);
            textBox7.Font = new Font("G마켓 산스 TTF Light", 15);
            textBox8.Font = new Font("G마켓 산스 TTF Light", 15);
            comboBox1.Font = new Font("G마켓 산스 TTF Light", 15);
            comboBox1.Items.Add("");
            comboBox1.Items.Add("농협");
            comboBox1.Items.Add("국민");
            comboBox1.Items.Add("신한");
            comboBox1.Items.Add("우리");
            comboBox1.Items.Add("카카오");
            comboBox1.SelectedIndex = 0;
            button1.BackgroundImage = Properties.Resources.folder;
            button1.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (open.ShowDialog() == DialogResult.OK)
            {
                string file = open.FileName;
                string name = Path.GetFileName(file);
                label5.Text = name;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Man_add.Mnew();
        }

        private void label1_1_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void label1_2_Click(object sender, EventArgs e)
        {
            textBox2.Focus();
        }

        private void label1_3_Click(object sender, EventArgs e)
        {
            textBox3.Focus();
        }

        private void label1_4_Click(object sender, EventArgs e)
        {
            textBox4.Focus();
        }

        private void label1_5_Click(object sender, EventArgs e)
        {
            textBox5.Focus();
        }

        private void label1_6_Click(object sender, EventArgs e)
        {
            textBox6.Focus();
        }

        private void label1_7_Click(object sender, EventArgs e)
        {
            textBox7.Focus();
        }

        private void label1_8_Click(object sender, EventArgs e)
        {
            textBox8.Focus();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label1_1.Text = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            label1_2.Text = textBox2.Text;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            label1_3.Text = textBox3.Text;
        }
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            label1_4.Text = textBox4.Text;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            label1_5.Text = textBox5.Text;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            label1_6.Text = textBox6.Text;
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            label1_7.Text = textBox7.Text;
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            label1_8.Text = textBox8.Text;
        }

        private void button2_MouseMove(object sender, MouseEventArgs e)
        {
            button2.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.Font = new Font("G마켓 산스 TTF Light", 16);
        }


    }
    class Man_add
    {
        private static MySqlConnection conn;
        private static string server = "192.168.31.147";
        private static string database = "team4";
        private static string uid = "root";
        private static string password = "0000";
        private static bool make_connection()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }
        public static void Mnew()
        {
            string connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};";
            conn = new MySqlConnection(connectionString);
            string Mname = Form_main.form_man_add.textBox1.Text;
            string Mbirth = Form_main.form_man_add.textBox2.Text;
            string Maddress = Form_main.form_man_add.textBox3.Text;
            string Mphone = Form_main.form_man_add.textBox4.Text;
            string Mbankname = Form_main.form_man_add.comboBox1.SelectedItem.ToString();    // 개체 참조 에러
            string Mbanknum = Form_main.form_man_add.textBox6.Text;
            string Mday = DateTime.Now.ToString("yy.MM");
            string Memail = Form_main.form_man_add.textBox5.Text;
            string Mid = Form_main.form_man_add.textBox7.Text;
            string Mpw = Form_main.form_man_add.textBox8.Text;
            string Mpicture = Form_main.form_man_add.label5.Text;
            string set = "SELECT * FROM info;";
            MySqlCommand cmd = new MySqlCommand(set, conn);
            int count = 0;
            if (make_connection())
            {
                if (Mname != "" && Mbirth != "" && Maddress != "" && Mphone != "" && Memail != "" && Mbankname != "" && Mbanknum != "" && Mid != "" && Mpw != "" && Mpicture != "")
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader[1].ToString() == Mbirth)
                        {
                            count++;
                            MessageBox.Show("이미 등록된 사람입니다.");
                        }
                    }
                    reader.Close();
                    if(count == 0)
                    {
                        if (Mid.Length == 5)
                        {
                            string Madmin = "X";
                            string Mrank = "간호사";
                            string Nnew = $"INSERT INTO info VALUES('{Mname}','{Mbirth}','{Maddress}','{Mphone}','{Mbankname}','{Mbanknum}','{Mday}','{Memail}','{Mid}','{Mpw}','{Madmin}',15,'{Mrank}','{Mpicture}')";
                            MySqlCommand cmd_n = new MySqlCommand(Nnew, conn);
                            cmd_n.ExecuteNonQuery();      // SQL문 실행
                        }
                        else if (Mid.Length == 8)
                        {
                            string Madmin = "O";
                            string Mrank = "관리자";
                            string Mnew = $"INSERT INTO info VALUES('{Mname}','{Mbirth}','{Maddress}','{Mphone}','{Mbankname}','{Mbanknum}','{Mday}','{Memail}','{Mid}','{Mpw}','{Madmin}',0,'{Mrank}','{Mpicture}')";
                            MySqlCommand cmd_m = new MySqlCommand(Mnew, conn);
                            cmd_m.ExecuteNonQuery();      // SQL문 실행
                        }
                        MessageBox.Show("등록되었습니다.");
                    }
                }
            }
            conn.Close();
        }
    }
}
