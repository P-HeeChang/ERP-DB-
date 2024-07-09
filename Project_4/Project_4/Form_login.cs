using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Project_4
{

    public partial class Form_login : Form
    {
        public static Form_main form_main;
        public static Form_pay form_pay;
        public string UserName { get; private set; }
        public string UserBirth { get; private set; }
        public string UserRent { get; private set; }


        private static MySqlConnection conn;
        private static string server = "192.168.31.147";//"127.0.0.1";"192.168.31.147"
        private static string database = "team4";
        private static string uid = "root";
        private static string root_password = "0000";


        public Form_login()
        {
            InitializeComponent();
            
            this.BackgroundImage = Properties.Resources.kk;

            this.FormBorderStyle = FormBorderStyle.FixedSingle; // 화면 사이즈 변경 불가능
        }


        private void Form_login_Load(object sender, EventArgs e)
        {
            label1.BackColor = Color.Transparent; // label1 배경화면 투명
            label2.BackColor = Color.Transparent; // label2 배경화면 투명
            this.BackgroundImageLayout = ImageLayout.Stretch;
            button1.BackgroundImage = Image.FromFile("login.png");
            this.button1.BackgroundImageLayout = ImageLayout.Stretch;
            this.button3.BackgroundImageLayout = ImageLayout.Stretch;           // button사이즈에 맞춰 이미지 추가
            this.button4.BackgroundImageLayout = ImageLayout.Stretch;
            MaximizeBox = false;                                                // 최대화 불가능
            // this.ActiveControl = null;          // 포커스 해제
        }

/*        private void label1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
        }*/

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = $"SERVER = {server}; DATABASE = {database}; UID={uid}; PASSWORD={root_password};";
            conn = new MySqlConnection(connectionString); 
            string FindTableQuery = $"SELECT * FROM INFO;";
            if (make_connection())
            {
                MySqlCommand cmd = new MySqlCommand(FindTableQuery, conn);
                cmd.ExecuteNonQuery();  //SQL문 실행
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string[] rowValues = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        rowValues[i] = reader[i].ToString();
                    }
                    if (rowValues[8] == textBox1.Text && rowValues[9] == textBox2.Text)
                    {
                        bool isAdmin = rowValues[10] == "O";  // 권한이 O일때 관리자      
                        UserName = rowValues[0];
                        UserBirth = rowValues[1];
                        UserRent = rowValues[11];
                        form_main = new Form_main(this, isAdmin, UserName, UserBirth, UserRent);
                        this.Hide();

                        //form_main.Visible = true;
                        label3.Text = null;
                        label4.Text = null;
                        form_main.ShowDialog();
                        conn.Close();//conn이라는 DB연결 객체를 해제
                        this.Close();  //form_main 창이 없어지면 login창도 없어진다. 
                        return;
                    }
                }
            }
            textBox1.Text = ""; // textbox1 비워주기
            textBox2.Text = ""; // textbox2 비워주기
            label3.Text = "";   // label3 비워주기
            label4.Text = "";   // label4 비워주기
            textBox1.Focus();
        }
        private bool make_connection()
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

        private void label3_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
            textBox1.MaxLength = 15;
            label3.Text = null;
            textBox1.Text = null;
        }

        private void label4_Click(object sender, EventArgs e)
        {
            textBox2.Focus();
            textBox2.MaxLength = 15;
            label4.Text = null;
            textBox2.Text = null;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label3.Text = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            label4.Text = textBox2.Text;
            label4.Text = new string('*', textBox2.Text.Length);
        }
        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            label4.Text = string.Empty;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }
    }
}