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

namespace Project_4
{
    public partial class Form_man_modify : Form
    {
        private static MySqlConnection conn;
        private static string server = "192.168.31.147";
        private static string database = "team4";
        private static string uid = "root";
        private static string password = "0000";
        public Form_man_modify()
        {
            InitializeComponent();
        }

        private void Form_man_modify_Load(object sender, EventArgs e)
        {
            button2.Font = new Font("G마켓 산스 TTF Light", 16);
            label1.Font = new Font("G마켓 산스 TTF Light", 15);
            label3.Font = new Font("G마켓 산스 TTF Light", 15);
            label3.Font = new Font("G마켓 산스 TTF Light", 15);
            label4.Font = new Font("G마켓 산스 TTF Light", 15);
            label5.Font = new Font("G마켓 산스 TTF Light", 15);
            label6.Font = new Font("G마켓 산스 TTF Light", 15);
            label7.Font = new Font("G마켓 산스 TTF Light", 15);
            label8.Font = new Font("G마켓 산스 TTF Light", 15);
            label9.Font = new Font("G마켓 산스 TTF Light", 15);
            label10.Font = new Font("G마켓 산스 TTF Light", 15);
            comboBox1.Font = new Font("G마켓 산스 TTF Light", 15);
            comboBox1.Items.Add("농협");
            comboBox1.Items.Add("국민");
            comboBox1.Items.Add("신한");
            comboBox1.Items.Add("우리");
            comboBox1.Items.Add("카카오");
            button1.BackgroundImage = Properties.Resources.folder;
            button1.BackgroundImageLayout = ImageLayout.Stretch;

            string connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};";
            conn = new MySqlConnection(connectionString);
            string name = Form_login.form_main.label4_1_1.Text;
            string insert = $"SELECT name,birth,city,phone,EMAIL,bankname,bankaddr,ID,PW,IMAGE_DATA FROM info WHERE name = '{name}';";
            if (make_connection())
            {
                MySqlCommand cmd = new MySqlCommand(insert, conn);
                cmd.ExecuteNonQuery();      // SQL문 실행
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    textBox1.Text = reader[0].ToString();
                    textBox3.Text = reader[2].ToString();
                    textBox4.Text = reader[3].ToString();
                    comboBox1.SelectedItem = reader[5].ToString();
                    textBox5.Text = reader[4].ToString();
                    textBox6.Text = reader[6].ToString();
                    textBox7.Text = reader[7].ToString();
                    textBox8.Text = reader[8].ToString();
                    label5.Text = reader[9].ToString();
                }
                conn.Close();
            }
        }
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
            Man_modify.Mmodify();
        }

        private void label1_1_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
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
        class Man_modify
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
            public static void Mmodify()
            {
                string connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};";
                conn = new MySqlConnection(connectionString);
                string u_name = Form_main.form_man_modify.textBox1.Text;
                string u_phone = Form_main.form_man_modify.textBox4.Text;
                string u_address = Form_main.form_man_modify.textBox3.Text;
                string u_email = Form_main.form_man_modify.textBox5.Text;
                string u_bankname = Form_main.form_man_modify.comboBox1.SelectedItem.ToString();
                string u_banknum = Form_main.form_man_modify.textBox6.Text;
                string u_id = Form_main.form_man_modify.textBox7.Text;
                string u_password = Form_main.form_man_modify.textBox8.Text;
                string u_picture = Form_main.form_man_modify.label5.Text;
                string u_birth = Form_login.form_main.label4_1_2.Text;
                if (make_connection())
                {
                    string update = $"UPDATE info SET name='{u_name}',phone = '{u_phone}',city='{u_address}',bankname = '{u_bankname}', bankaddr = '{u_banknum}',EMAIL = '{u_email}',ID = '{u_id}',PW = '{u_password}',IMAGE_DATA = '{u_picture}' WHERE birth = '{u_birth}';";
                    MySqlCommand cmd = new MySqlCommand(update, conn);
                    cmd.ExecuteNonQuery();      // SQL문 실행
                    conn.Close();
                    MessageBox.Show("수정되었습니다.");
                }
            }
        }

    }
}
