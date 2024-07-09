using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using System.Reflection.Emit;
using System.Globalization;
using MySql.Data.MySqlClient;

namespace Project_4
{
    public partial class Form_inv_add : Form
    {
        public Form_inv_add()
        {
            InitializeComponent();
        }

        private void Form_inv_add_Load(object sender, EventArgs e)
        {
            label1.Font = new Font("G마켓 산스 TTF Light", 16);
            label2.Font = new Font("G마켓 산스 TTF Light", 16);
            label3.Font = new Font("G마켓 산스 TTF Light", 16);
            label4.Font = new Font("G마켓 산스 TTF Light", 14);
            label5.Font = new Font("G마켓 산스 TTF Light", 16);
            label6.Font = new Font("G마켓 산스 TTF Light", 16);
            label7.Font = new Font("G마켓 산스 TTF Light", 16);
            button1.Font = new Font("G마켓 산스 TTF Light", 16);
            textBox1.Text = null;
            textBox2.Text = null;
            textBox3.Text = null;
        }
        
        private async void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "") 
            {
                Inv_add.Inew();
                textBox1.Text = null;
                textBox2.Text = null;
                textBox3.Text = null;
            }
            else
            {
                label4.Text = "잘못된 값입니다.";
                await Task.Delay(1000);                     // 일정시간후 에러메시지 삭제
                label4.Text = null;
            }
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

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            button1.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);

        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.Font = new Font("G마켓 산스 TTF Light", 16);

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label5.Text = textBox1.Text;
        }

        private void label5_Click(object sender, EventArgs e)
        {
            label5.Text = null;
            textBox1.Focus();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            label6.Text = textBox2.Text;
        }

        private void label6_Click(object sender, EventArgs e)
        {
            label6.Text = null;
            textBox2.Focus();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            label7.Text = textBox3.Text;
        }

        private void label7_Click(object sender, EventArgs e)
        {
            label7.Text = null;
            textBox3.Focus();
        }
    }
    class Inv_add
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
        private static bool Isnumber(string input)
        {
            foreach (char c in input)
            {
                if (char.IsDigit(c))
                {
                    return true;
                }
            }
            return false;
        }
        private static bool Isupper(string input)
        {
            foreach (char c in input)
            {
                if (char.IsUpper(input[0]))     // 제품 코드의 첫 글자가 대문자가 아닐때
                {
                    return true;
                }
            }
            return false;
        }
        private static bool Iskorean(string input)
        {
            foreach (char c in input)
            {
                if (char.GetUnicodeCategory(c) == UnicodeCategory.OtherLetter)     // 제품 명이 한글이 아닐때
                {
                    return true;
                }
            }
            return false;
        }
        public async static void Inew()
        {
            string connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};";
            conn = new MySqlConnection(connectionString);
            string Icode = Form_main.form_inv_add.textBox1.Text;          // 신규제품 코드
            string Iname = Form_main.form_inv_add.textBox2.Text;          // 신규제품 이름
            int Icount = Int32.Parse(Form_main.form_inv_add.textBox3.Text);         // 신규제품 재고
            if (make_connection())
            {
                string inv = "SELECT * FROM inventory;";
                MySqlCommand cmd = new MySqlCommand(inv, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                int dupli = 0;
                if (Isupper(Icode) && Iskorean(Iname) && Isnumber(Icount.ToString()))
                {
                    while (reader.Read())
                    {
                        if (reader[0].ToString() == Icode || reader[1].ToString() == Iname)
                        {
                            Form_main.form_inv_add.label4.Text = "이미 포함되어 있는 제품입니다.";
                            dupli++;
                        }
                    }
                    reader.Close();
                    if (dupli == 0)
                    {
                        string insertTable = $"INSERT INTO inventory (I_code, I_name, I_count) VALUES ('{Icode}','{Iname}','{Icount}')";
                        MySqlCommand cmd_i = new MySqlCommand(insertTable, conn);
                        cmd_i.ExecuteNonQuery();
                        MessageBox.Show("등록되었습니다.");
                    }
                }
                else if (!Isupper(Icode))
                {
                    Form_main.form_inv_add.label4.Text = "제품코드 형식이 잘못되었습니다.";
                }
                else if (!Iskorean(Iname))
                {
                    Form_main.form_inv_add.label4.Text = "제품명 형식이 잘못되었습니다.";
                }
                else if (!Isnumber(Icount.ToString()))
                {
                    Form_main.form_inv_add.label4.Text = "수량 형식이 잘못되었습니다.";
                }
                await Task.Delay(1000);                     // 일정시간후 에러메시지 삭제
                Form_main.form_inv_add.label4.Text = null;
            }
            conn.Close();
        }
    }
}