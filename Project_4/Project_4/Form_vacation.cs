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
using System.Reflection.Emit;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
namespace Project_4
{
    public partial class Form_vacation : Form
    {
        private MySqlConnection conn;
        private string server;
        private string database;
        private string uid;
        private string password;
        private Timer timer;
        private Form_login form_login;
        string name;
        string birth;
        string Rent;
        public Form_vacation(Form_login form_login, string name, string birth, string Rent)
        {
            InitializeComponent();
            this.form_login = form_login;
            this.name = form_login.UserName;
            this.birth = birth;
            this.Rent = Rent;
            LoadDisplay(); // 월차/반차 신청을 새로고침 
            Vac_count(); //월차/반차 남은 갯수를 새로고침
            Check_vac();
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            if (monthCalendar1.SelectionRange.Start == monthCalendar1.SelectionRange.End) //달력에서 선택된 날을 텍스트박스에 삽입
                textBox1.Text = monthCalendar1.SelectionRange.Start.ToString("yyyy-MM-dd");
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItem = comboBox1.SelectedItem.ToString(); //콤보박스에서 선택한것

            // 만약 특정 항목을 선택한 경우에만 두 번째 콤보 박스를 보이게 합니다.
            if (selectedItem == "반차")
            {
                comboBox2.Visible = true;
            }
            else
            {
                comboBox2.Visible = false;
                comboBox2.Text = "";
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            DateTime selecteDate = monthCalendar1.SelectionStart;//달력에서 선택한 날을 삽입
            DateTime today = DateTime.Now;
            server = "192.168.31.147";//"192.168.31.147";
            database = "team4";
            uid = "root";
            password = "0000";
            string name = this.name;
            string birth = this.birth;
            string month_half = comboBox1.Text.ToString();
            string etc = comboBox2.Text.ToString();
            string date = selecteDate.ToString("yyyy-MM-dd");
            // string createTableQuery = "CREATE TABLE IF NOT EXISTS VAC_REQ (NAME VARCHAR(5) NOT NULL,BIRTH VARCHAR(11) NOT NULL,MONTH_HALF_DAY VARCHAR(5) NOT NULL, ETC VARCHAR(3) ,Date VARCHAR(20)";
            var vac = $"INSERT INTO VAC_REQ VALUES('{name}', '{birth}', '{month_half}', '{etc}', '{date}');";
            string connectionString = $"SERVER ={server};DATABASE={database};UID={uid};PASSWORD={password};";
            conn = new MySqlConnection(connectionString);
            string Vac = $"SELECT * FROM VAC_REQ WHERE NAME = '{name}' AND Date = '{date}'";
            int monthRent;
            string monthsupdate;
            if (selecteDate > DateTime.Now) //선택한 날짜가 오늘날보다 크면
            {

                string month_count = $"SELECT monthrent FROM INFO WHERE name = '{this.name}'"; //로그인한 사람의 월차의 갯수를 가져옴
                string month_count_min = "";
                if (make_connection())
                {
                    MySqlCommand month_count_cmd = new MySqlCommand(month_count, conn);
                    MySqlDataReader reader = month_count_cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        month_count_min = reader["monthrent"].ToString();
                    }
                    reader.Close();
                    conn.Close();
                }
                if (month_half == "월차")
                {
                    if (int.Parse(month_count_min) >= 2)
                    {


                        if (make_connection())
                        {
                            try
                            {
                                MySqlCommand checkCmd = new MySqlCommand(Vac, conn);
                                MySqlDataReader reader = checkCmd.ExecuteReader();
                                bool isDuplicate = false;
                                while (reader.Read())
                                {
                                    string existingName = reader.GetString("NAME");
                                    string existingDate = reader.GetDateTime("Date").ToString("yyyy-MM-dd");

                                    if (existingName == name && existingDate == date)
                                    {
                                        MessageBox.Show("이미 신청한 날짜입니다.");
                                        isDuplicate = true;
                                        break;
                                    }
                                }
                                if (!isDuplicate)
                                {
                                    reader.Close(); // 리더 객체를 닫음
                                    MySqlCommand insertCmd = new MySqlCommand(vac, conn);
                                    monthRent = int.Parse(month_count_min) - 2;
                                    monthsupdate = $"UPDATE INFO SET monthrent = '{monthRent}' Where name = '{this.name}'";
                                    MessageBox.Show("월차가 신청되었습니다");
                                    insertCmd.ExecuteNonQuery();
                                    MySqlCommand cmd = new MySqlCommand(monthsupdate, conn);
                                    cmd.ExecuteNonQuery(); //SQL문 실행
                                    conn.Close(); //conn이라는 db연결 객체를 해제
                                }

                                conn.Close();
                            }
                            catch
                            {

                            }
                            finally
                            {
                                conn.Close();
                            }
                        }
                    }
                    if (int.Parse(month_count_min) <= 0)
                    {
                        MessageBox.Show("월차의 갯수가 모자랍니다");
                    }
                }

                else if (month_half == "반차")
                {
                    if (int.Parse(month_count_min) >= 1)
                    {
                        if (make_connection())
                        {
                            try
                            {
                                MySqlCommand checkCmd = new MySqlCommand(Vac, conn);
                                MySqlDataReader reader = checkCmd.ExecuteReader();
                                bool check = false;
                                while (reader.Read())
                                {
                                    string existingName = reader.GetString("NAME");
                                    string existingDate = reader.GetDateTime("Date").ToString("yyyy-MM-dd");

                                    if (existingName == name && existingDate == date)
                                    {

                                        MessageBox.Show("이미 신청한 날짜입니다.");
                                        check = true;
                                        break;
                                    }
                                }
                                if (!check)
                                {
                                    reader.Close(); // 리더 객체를 닫음
                                    MySqlCommand insertCmd = new MySqlCommand(vac, conn);
                                    monthRent = int.Parse(month_count_min) - 1;
                                    monthsupdate = $"UPDATE INFO SET monthrent = '{monthRent}' Where name = '{this.name}'";
                                    MessageBox.Show("반차가 신청되었습니다");
                                    insertCmd.ExecuteNonQuery();
                                    MySqlCommand VacCmd = new MySqlCommand(monthsupdate, conn);
                                    VacCmd.ExecuteNonQuery();
                                    conn.Close();

                                }
                                conn.Close();
                            }



                            catch
                            {

                            }
                            finally
                            {
                                conn.Close();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("반차의갯수가 모자랍니다");
                    }
                }
            }
            else
            {
                MessageBox.Show("이미 지난 날짜입니다");
            }
        }

        //월차/반차 구분할 필요없음 콤보박스에서 선택한다. 오전인지 오후인지도 선택 안해도된다. 콤보박스가 해준다. -> 해야 할것 info에서 월차의 개수를 가져오는것

        private bool make_connection()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open(); // 연결이 닫혀 있다면 연결을 엽니다.
                }
                return true;
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }
        private void Check_vac()
        {
            DateTime selecteDate = monthCalendar1.SelectionStart;//달력에서 선택한 날을 삽입
            DateTime today = DateTime.Now;
            server = "192.168.31.147";//"192.168.31.147";
            database = "team4";
            uid = "root";
            password = "0000";
            string connectionString = $"SERVER ={server};DATABASE={database};UID={uid};PASSWORD={password};";
            conn = new MySqlConnection(connectionString);
            string Vac = $"SELECT * FROM VAC_Ok";
            if (make_connection())
            {
                MySqlCommand cmd = new MySqlCommand(Vac, conn);
                cmd.ExecuteNonQuery();  //SQL문 실행
                MySqlDataReader reader = cmd.ExecuteReader();
                dataGridView3.Rows.Clear();
                while (reader.Read())
                {
                    dataGridView3.Rows.Add(reader[0], reader[1], reader[2], reader[3], reader[4]);
                }
                conn.Close();//conn이라는 DB연결 객체를 해제
            }
        }
        private void LoadDisplay()
        {
            DateTime selecteDate = monthCalendar1.SelectionStart;//달력에서 선택한 날을 삽입
            DateTime today = DateTime.Now;
            server = "192.168.31.147";//"192.168.31.147";
            database = "team4";
            uid = "root";
            password = "0000";
            string connectionString = $"SERVER ={server};DATABASE={database};UID={uid};PASSWORD={password};";
            conn = new MySqlConnection(connectionString);
            string Vac = $"SELECT * FROM VAC_REQ";
            if (make_connection())
            {
                MySqlCommand cmd = new MySqlCommand(Vac, conn);
                cmd.ExecuteNonQuery();  //SQL문 실행
                MySqlDataReader reader = cmd.ExecuteReader();
                dataGridView1.Rows.Clear();
                while (reader.Read())
                {
                    dataGridView1.Rows.Add(reader[0], reader[1], reader[2], reader[3], reader[4]);
                }
                conn.Close();//conn이라는 DB연결 객체를 해제
            }

        }
        public void Vac_count()
        {
            DateTime selecteDate = monthCalendar1.SelectionStart;//달력에서 선택한 날을 삽입
            DateTime today = DateTime.Now;
            server = "192.168.31.147";//"192.168.31.147";
            database = "team4";
            uid = "root";
            password = "0000";
            string connectionString = $"SERVER ={server};DATABASE={database};UID={uid};PASSWORD={password};";
            conn = new MySqlConnection(connectionString);
            string Vac = $"SELECT * FROM VAC_REQ";
            if (make_connection())
            {
                MySqlCommand cmd = new MySqlCommand(Vac, conn);
                cmd.ExecuteNonQuery();  //SQL문 실행
                MySqlDataReader reader = cmd.ExecuteReader();
                dataGridView1.Rows.Clear();
                while (reader.Read())
                {
                    dataGridView1.Rows.Add(reader[0], reader[1], reader[2], reader[3], reader[4]);
                }
                conn.Close();//conn이라는 DB연결 객체를 해제
            }
        }


        private void label58_Click(object sender, EventArgs e)
        {
            textBox1.Focus();

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label58.Text = textBox1.Text;
        }

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            button1.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.Font = new Font("G마켓 산스 TTF Light", 16);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form_vacation_Load(object sender, EventArgs e)
        {

        }
    }
    class vacation
    {
        public static List<string> vac_ok_info()
        {
            string P_path = "Info.txt";      // 환자 정보 텍스트 파일 txt주석
            string[] lines = File.ReadAllLines(P_path);       // 파일의 모든 줄을 읽어옴
            List<string> vacList = lines.ToList();
            return vacList;
        }
        public static List<List<string>> Line_vac_info()
        {
            List<List<string>> lines = new List<List<string>>();

            // 파일의 각 줄을 읽어와 리스트 형태로 변환
            foreach (string line in vac_ok())
            {
                List<string> data = line.Split('\t').ToList(); // 각 줄을 탭으로 구분하여 리스트로 변환
                lines.Add(data);
            }

            return lines;
        }
        public static List<string> vac_ok()
        {
            string P_path = "Vac_Okay.txt";      // 환자 정보 텍스트 파일 txt주석
            string[] lines = File.ReadAllLines(P_path);       // 파일의 모든 줄을 읽어옴
            List<string> vacList = lines.ToList();
            return vacList;
        }
        public static List<List<string>> Line_vac()
        {
            List<List<string>> lines = new List<List<string>>();

            // 파일의 각 줄을 읽어와 리스트 형태로 변환
            foreach (string line in vac_ok())
            {
                List<string> data = line.Split('\t').ToList(); // 각 줄을 탭으로 구분하여 리스트로 변환
                lines.Add(data);
            }

            return lines;
        }
        public static List<string> text_vac()
        {
            string P_path = "vac_req.txt";      // 환자 정보 텍스트 파일 txt주석
            string[] lines = File.ReadAllLines(P_path);       // 파일의 모든 줄을 읽어옴
            List<string> vacList = lines.ToList();
            return vacList;
        }

        public static List<List<string>> Line()
        {
            List<List<string>> lines = new List<List<string>>();

            // 파일의 각 줄을 읽어와 리스트 형태로 변환
            foreach (string line in text_vac())
            {
                List<string> data = line.Split('\t').ToList(); // 각 줄을 탭으로 구분하여 리스트로 변환
                lines.Add(data);
            }

            return lines;
        }
        public static List<List<string>> Linz()
        {
            List<List<string>> lines = new List<List<string>>();

            // 파일의 각 줄을 읽어와 리스트 형태로 변환
            foreach (string line in vac_ok_info())
            {
                List<string> data = line.Split('\t').ToList(); // 각 줄을 탭으로 구분하여 리스트로 변환
                lines.Add(data);
            }

            return lines;
        }
    }
}

