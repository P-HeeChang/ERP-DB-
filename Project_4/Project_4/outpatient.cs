using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;


namespace Project_4
{
    public partial class outpatient : Form
    {
        private static int countClinic1 = 0; // 진료실1 카운트를 저장하는 정적 변수
        private static int countClinic2 = 0;

        private static MySqlConnection conn;
        private static string server = "192.168.31.147";//"127.0.0.1";"192.168.31.147"
        private static string database = "team4";
        private static string uid = "root";
        private static string password = "0000";

        private static string pname = "";
        private static string birth = "";

        static Random random = new Random();   // 랜덤 수 생성

        //time tick
        private Timer timer;
        public outpatient(string[] rowData)
        {
            InitializeComponent();


            dataGridView1.Rows.Add(rowData);//outpatient폼에 그리드뷰안에 폼1의 데이터 기입




        }

        private void outpatient_Load(object sender, EventArgs e)
        {

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = comboBox2.Items.Count - 1;
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            textBox1.Text = null;
        }
        private void button2_Click(object sender, EventArgs e)//--------------20240315 박희창
        {
            DataGridViewRow selectedRow2 = dataGridView1.SelectedRows[0];
            string[] rowData = new string[selectedRow2.Cells.Count];

            // DataGridView에서 선택된 행의 데이터를 rowData 배열에 복사합니다.
            for (int i = 0; i < selectedRow2.Cells.Count; i++)
            {
                rowData[i] = selectedRow2.Cells[i].Value.ToString();
            }
            pname = rowData[0];
            birth = rowData[2];

            // Form_login에 있는 패널 내에 있는 모든 컨트롤을 검색합니다.
            Control[] controls1 = Form_login.form_main.panel106.Controls.Find("newTextBox", true);
            Control[] controls2 = Form_login.form_main.panel107.Controls.Find("newTextBox", true);
            // TextBox인 컨트롤만 필터링하여 검사합니다.
            foreach (Control control in controls1)
            {
                if (control is System.Windows.Forms.TextBox textBox)
                {
                    string[] lines = textBox.Text.Split('\n');
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split('/');


                        if (parts.Length >= 3)
                        {
                            string name = parts[0].Trim();
                            string dateOfBirth = parts[2].Trim();

                            // 선택된 행의 데이터와 TextBox의 데이터를 비교하여 중복을 체크합니다.
                            if (name == rowData[0] && dateOfBirth == rowData[2])
                            {
                                MessageBox.Show("이미 접수 된 환자입니다.");
                                Close();
                                return; // 중복이 발견되면 함수를 종료합니다.
                            }
                        }
                    }
                }

            }
            foreach (Control control in controls2)
            {
                if (control is System.Windows.Forms.TextBox textBox)
                {
                    string[] lines = textBox.Text.Split('\n');
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split('/');
                        MessageBox.Show(parts.ToString());
                        if (parts.Length >= 3)
                        {
                            string name = parts[0].Trim();
                            string dateOfBirth = parts[2].Trim();

                            pname = parts[0].Trim();
                            birth = parts[2].Trim();
                            MessageBox.Show(pname);
                            MessageBox.Show(birth);

                            // 선택된 행의 데이터와 TextBox의 데이터를 비교하여 중복을 체크합니다.
                            if (name == rowData[0] && dateOfBirth == rowData[2])
                            {
                                MessageBox.Show("이미 접수 된 환자입니다.");
                                Close();
                                return; // 중복이 발견되면 함수를 종료합니다.
                            }
                        }
                    }
                }
            }

            NewTB(rowData);

            string connectionString = $"SERVER = {server}; DATABASE = {database}; UID={uid}; PASSWORD={password};";//--240314 메모장
            conn = new MySqlConnection(connectionString);

            string createtable = "CREATE TABLE IF NOT EXISTS registration (name VARCHAR(5) NOT NULL,BIRTH VARCHAR(10) NOT NULL,TREATMENT VARCHAR(10) NOT NULL,OPINION VARCHAR(1000));";
            string opinion = textBox1.Text;
            string treatment = DateTime.Now.ToString("yyyy-MM-dd");

            string previous = $"INSERT INTO registration VALUES('{pname}','{birth}','{treatment}','{opinion}');";

            string find = $"SELECT * FORM REGISTRATION WHERE NAME = {pname};";

            if (make_connection())
            {
                MySqlCommand cmd = new MySqlCommand(createtable, conn);
                cmd.ExecuteNonQuery();  //테이블 생성

                MySqlCommand pre = new MySqlCommand(previous, conn);
                pre.ExecuteNonQuery();

                conn.Close();//conn이라는 DB연결 객체를 해제

            }
            else
            {
            }





            // 차트 초기화
            Form_login.form_main.chart1.Series[0].Points.Clear();
            Form_login.form_main.UpdateChart();
            Form_login.form_main.chart1.Series[1].Points.Clear();
            Form_login.form_main.chart();
            Form_login.form_main.GetThisWeekTotalMedicalLinesCount();
           

            this.Close();



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


        private void button3_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
        private System.Windows.Forms.TextBox FindTextBoxByButton(System.Windows.Forms.Button button)
        {
            Control textBoxControl = button.Parent;
            if (textBoxControl is System.Windows.Forms.TextBox)
            {
                return (System.Windows.Forms.TextBox)textBoxControl;
            }
            return null;
        }
        private void Remove(System.Windows.Forms.TextBox textBox)//진료완료로 넘어가서 생긴 빈공간 없애기
        {
            int panel7Y = 6;
            int panel9Y = 6;


            foreach (Control control in Form_login.form_main.panel106.Controls)
            {
                if (control is System.Windows.Forms.TextBox)
                {

                    System.Windows.Forms.TextBox textBox1 = (System.Windows.Forms.TextBox)control;
                    textBox1.Location = new System.Drawing.Point(6, panel7Y);
                    panel7Y += textBox1.Height + 20;//생성할때마다 20간격두고 생성
                }
            }


            foreach (Control control in Form_login.form_main.panel107 .Controls)
            {
                if (control is System.Windows.Forms.TextBox)
                {

                    System.Windows.Forms.TextBox textBox2 = (System.Windows.Forms.TextBox)control;
                    textBox2.Location = new System.Drawing.Point(6, panel9Y);
                    panel9Y += textBox2.Height + 20;
                }
            }
        }

        private void TabPage2(System.Windows.Forms.TextBox textBox)
        {
            Form_login.form_main.tabPage1.Controls.Remove(textBox);

            // 탭 페이지 2에서 마지막으로 생성된 컨트롤의 위치를 기준으로 여백을 추가하여 위치를 지정합니다.
            int lastControlBottom = 0;
            foreach (Control control in Form_login.form_main.tabPage2.Controls)
            {
                if (control.Bottom > lastControlBottom)
                {
                    lastControlBottom = control.Bottom;
                }
            }

            // 여백을 추가하여 새로운 텍스트 박스의 위치를 지정합니다.
            int newLocationY = lastControlBottom + 20;
            textBox.Location = new System.Drawing.Point(6, newLocationY);

            Form_login.form_main.tabPage2.Controls.Add(textBox);
        }
        private void Count(System.Windows.Forms.TextBox textBox)
        {
            string text = textBox.Text;

            // "진료1실"-Regex.Matches(찾을데이터유형,찾을데이터) 사용해 카운트
            countClinic1 -= Regex.Matches(text, "진료1실").Count;
            Form_login.form_main.label55.Text = "진료  1실 대기인원 : " + countClinic1.ToString();
            //진료2실
            countClinic2 -= Regex.Matches(text, "진료2실").Count;
            Form_login.form_main.label56.Text = "진료 2실 대기인원 : " + countClinic2.ToString();
            if (countClinic1 == 0 || countClinic2 == 0)
            {
                Form_login.form_main.label55.Text = "진료  1실 대기인원 : " + countClinic1.ToString();
                Form_login.form_main.label56.Text = "진료 2실 대기인원 : " + countClinic2.ToString();
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {


            Alarm alarm = new Alarm();
            alarm.Text = null;
            timer.Stop();

            Check();
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button clickedButton = (System.Windows.Forms.Button)sender;


            if (clickedButton.Text == "진료대기")
            {
                System.Windows.Forms.TextBox targetTextBox = FindTextBoxByButton(clickedButton);
                Count(targetTextBox);
                clickedButton.Text = "진료중";

                Random random = new Random();
                timer = new Timer();
                timer.Interval = random.Next(15000, 25000);
                timer.Tick += Timer_Tick;
                timer.Start();

                // 진료 중 버튼이 클릭되면, 진료 대기 버튼을 다른 텍스트박스에서 비활성화합니다.
                DisableWaitingButtons(targetTextBox);
            }
            else if (clickedButton.Text == "진료중")
            {
                clickedButton.Text = "진료완료";
                System.Windows.Forms.TextBox targetTextBox = FindTextBoxByButton(clickedButton);
                System.Windows.Forms.Button button = (System.Windows.Forms.Button)sender;

                // 텍스트박스가 존재하고, 탭 페이지를 변경할 수 있는 조건을 확인
                if (targetTextBox != null)
                {
                    // 탭 페이지 2로 텍스트박스 이동
                    TabPage2(targetTextBox);

                }

                Remove(targetTextBox);
                //textBoxCount--;

                // 진료 완료 시 다른 텍스트박스에서 진료 대기 버튼을 다시 활성화합니다.
                EnableWaitingButtons(targetTextBox);


            }
        }
        private void DisableWaitingButtons(System.Windows.Forms.TextBox currentTextBox)
        {            // 현재 진료 중인 텍스트박스의 진료실 번호를 가져옵니다.
            string currentClinic = currentTextBox.Text.Split('/')[0];
            // 모든 텍스트박스를 순회하며 진료 대기 버튼을 처리합니다.
            foreach (Control control in Form_login.form_main.panel106.Controls)
            {
                if (control is System.Windows.Forms.TextBox)
                {
                    System.Windows.Forms.TextBox textBox = (System.Windows.Forms.TextBox)control;
                    System.Windows.Forms.Button button = (System.Windows.Forms.Button)textBox.Controls[0]; // 텍스트박스 내부에 있는 버튼을 가져옵니다.
                    string clinic = textBox.Text.Split('/')[0]; // 현재 텍스트박스의 진료실 번호를 가져옵니다.
                    // 현재 진료실과 같은 진료실의 버튼만 잠깁니다.
                    if (clinic == currentClinic && button.Text == "진료대기")
                    {
                        button.Enabled = false;
                    }
                    if (clinic == currentClinic && button.Text == "진료중")
                    {
                        button.Enabled = false;
                    }
                }
            }
            foreach (Control control in Form_login.form_main.panel107.Controls)
            {
                if (control is System.Windows.Forms.TextBox)
                {
                    System.Windows.Forms.TextBox textBox = (System.Windows.Forms.TextBox)control;
                    System.Windows.Forms.Button button = (System.Windows.Forms.Button)textBox.Controls[0]; // 텍스트박스 내부에 있는 버튼을 가져옵니다.
                    string clinic = textBox.Text.Split('/')[0]; // 현재 텍스트박스의 진료실 번호를 가져옵니다.
                    // 현재 진료실과 같은 진료실의 버튼만 잠깁니다.
                    if (clinic == currentClinic && button.Text == "진료대기")
                    {
                        button.Enabled = false;
                    }
                    if (clinic == currentClinic && button.Text == "진료중")
                    {
                        button.Enabled = false;
                    }
                }
            }
        }
        private void EnableWaitingButtons(System.Windows.Forms.TextBox currentTextBox)
        {
            // 모든 텍스트박스를 순회하며 현재 진료 중인 텍스트박스와 동일한 진료실을 가지고 있는 진료 대기 버튼을 활성화합니다.
            foreach (Control control in Form_login.form_main.panel106.Controls)
            {
                if (control is System.Windows.Forms.TextBox && control != currentTextBox)
                {
                    System.Windows.Forms.TextBox textBox = (System.Windows.Forms.TextBox)control;
                    System.Windows.Forms.Button button = (System.Windows.Forms.Button)textBox.Controls[0]; // 텍스트박스 내부에 있는 버튼을 가져옵니다.
                    if (textBox.Text.Contains(currentTextBox.Text.Split('/')[0]) && button.Text == "진료대기")
                    {

                        button.Enabled = true;
                    }
                }
            }
            foreach (Control control in Form_login.form_main.panel107.Controls)
            {
                if (control is System.Windows.Forms.TextBox && control != currentTextBox)
                {
                    System.Windows.Forms.TextBox textBox = (System.Windows.Forms.TextBox)control;
                    System.Windows.Forms.Button button = (System.Windows.Forms.Button)textBox.Controls[0]; // 텍스트박스 내부에 있는 버튼을 가져옵니다.

                    if (textBox.Text.Contains(currentTextBox.Text.Split('/')[0]) && button.Text == "진료대기")
                    {


                        button.Enabled = true;
                    }
                }
            }
        }
        private void NewTB(string[] rowData)
        {
            // 텍스트 박스 생성
            System.Windows.Forms.TextBox newTextBox = new System.Windows.Forms.TextBox();
            newTextBox.Multiline = true;
            newTextBox.ReadOnly = true;
            newTextBox.Name = "newTextBox";
            newTextBox.Size = new System.Drawing.Size(260, 100); // 크기 지정
            newTextBox.Cursor = Cursors.Default;//240310

            newTextBox.Font = new Font("G마켓 산스 TTF Light", 12, FontStyle.Bold);  // 폰트 지정

            if (comboBox1.Text == "진료1실")
            {
                newTextBox.Location = new System.Drawing.Point(6, 6 + countClinic1 * 120); // 생성 위치 지정
                Form_login.form_main.panel106.Controls.Add(newTextBox);
            }
            else if (comboBox1.Text == "진료2실")
            {

                newTextBox.Location = new System.Drawing.Point(6, 6 + countClinic2 * 120); // 생성 위치 지정
                Form_login.form_main.panel107.Controls.Add(newTextBox);
            }
            //Form_login.form_main.tabPage1.Controls.Add(newTextBox); // 탭 페이지에 텍스트 박스 추가

            // 버튼 생성
            System.Windows.Forms.Button newButton = new System.Windows.Forms.Button();
            newButton.Text = "진료대기";
            newButton.Name = "newButton";
            newButton.Size = new Size(70, 30);
            newButton.Font = new Font("G마켓 산스 TTF Light", 9, FontStyle.Bold);
            newButton.Location = new Point(newTextBox.Width - newButton.Width - 5, 0);
            newButton.Click += new EventHandler(NewButton_Click);
            newButton.Cursor = Cursors.Hand;//240310

            newTextBox.Controls.Add(newButton);//텍스트박스에 버튼추가

            // 텍스트 설정
            newTextBox.Text = comboBox1.Text + "/" + comboBox2.Text + Environment.NewLine + Environment.NewLine;
            for (int i = 0; i <= 2; i++)
            {
                newTextBox.Text += rowData[i];

                if (i != 2)
                {
                    newTextBox.Text += "/";

                }

            }
            //이윤서
            string str = "";



            int RanNum = random.Next(1000, 200000);
            int RanNum2 = (RanNum / 100) * 100;
            str = string.Format("{0:#,###}", RanNum2);

            string apeendDateTime = DateTime.Now.ToString("yyyy-MM-dd");
            string connectionString = $"SERVER = {server}; DATABASE = {database}; UID={uid}; PASSWORD={password};";
            conn = new MySqlConnection(connectionString); // MYSQL로 만든 생성자 함수 conn이라 부름.
            string rv = $"SELECT * FROM medical";
            // string pd = $"SELECT * FROM {table2}";

            string reservation = $"INSERT INTO medical VALUES ('{rowData[0]}' ,'{rowData[2]}', '{rowData[1]}', '{apeendDateTime}', '{str}', false)";  // 넣으려는 SQL문
            if (make_connection())
            {
                MySqlCommand cmd = new MySqlCommand(reservation, conn);

                cmd.ExecuteNonQuery();  //SQL문 실행
             
                conn.Close();//conn이라는 DB연결 객체를 해제
            }


            // 접수 시간 추가
            string formattedDateTime = DateTime.Now.ToString("yy.MM.dd-HH:mm");
            newTextBox.Text += Environment.NewLine + "접수시간 : " + formattedDateTime;

           

            // 대기실 인원 카운트
            WaitCount();
        }
        private void WaitCount()
        {
            if (comboBox1.Text == "진료1실")
            {
                countClinic1++;
                Form_login.form_main.label55.Text = "진료 1실 대기인원 : " + countClinic1.ToString();
            }
            else if (comboBox1.Text == "진료2실")
            {
                countClinic2++;
                Form_login.form_main.label56.Text = "진료 2실 대기인원 : " + countClinic2.ToString();
            }
        }
        private string GetFirstWordOfParentTextBoxText(System.Windows.Forms.TextBox textBox)
        {
            // 텍스트박스의 텍스트를 공백을 기준으로 분할하여 첫 번째 단어를 추출합니다.
            string[] words = textBox.Text.Split('/');
            if (words.Length > 0)
            {
                return words[0];
            }
            return string.Empty; // 단어가 없을 경우 빈 문자열 반환
        }
        private void Check()
        {
            Alarm alarm = new Alarm();
            foreach (Control control1 in Form_login.form_main.panel106.Controls)
            {
                if (control1 is System.Windows.Forms.TextBox)
                {
                    System.Windows.Forms.TextBox textBox = (System.Windows.Forms.TextBox)control1;
                    // 텍스트박스 내부의 버튼을 찾음
                    foreach (Control innerControl in textBox.Controls)
                    {
                        if (innerControl is System.Windows.Forms.Button)
                        {
                            System.Windows.Forms.Button button = (System.Windows.Forms.Button)innerControl;

                            //버튼의 텍스트가 진료중인지 확인하는 조건문
                            if (button.Text.Contains("진료중"))
                            {
                                string firstWord = GetFirstWordOfParentTextBoxText(textBox);

                                alarm.label1.Text = firstWord + "\n" + "진료 완료";

                                alarm.ShowDialog();

                                //DialogResult result = MessageBox.Show("진료실 진료완료", "알림", MessageBoxButtons.OK);
                                //timer.Stop();
                                ClickButton();
                                return;
                            }
                        }
                    }
                }
            }
            foreach (Control control in Form_login.form_main.panel107.Controls)
            {
                if (control is System.Windows.Forms.TextBox)
                {
                    System.Windows.Forms.TextBox textBox = (System.Windows.Forms.TextBox)control;
                    // 텍스트박스 내부의 버튼을 찾음
                    foreach (Control innerControl in textBox.Controls)
                    {
                        if (innerControl is System.Windows.Forms.Button)
                        {
                            System.Windows.Forms.Button button = (System.Windows.Forms.Button)innerControl;

                            //버튼의 텍스트가 진료중인지 확인하는 조건문
                            if (button.Text.Contains("진료중"))
                            {
                                string firstWord = GetFirstWordOfParentTextBoxText(textBox);

                                alarm.label1.Text = firstWord + "\n" + "진료 완료";

                                alarm.ShowDialog();

                                //DialogResult result = MessageBox.Show("진료실 진료완료", "알림", MessageBoxButtons.OK);
                                //timer.Stop();
                                ClickButton();
                                return; // 버튼을 찾고 클릭했으므로 더 이상의 탐색은 종료
                            }
                        }
                    }
                }

            }
        }
        private void ClickButton()
        {
            // 탭페이지 내의 모든 컨트롤을 탐색하여 동적으로 생성된 텍스트박스의 버튼을 찾음

            foreach (Control control1 in Form_login.form_main.panel106.Controls)
            {
                if (control1 is System.Windows.Forms.TextBox)
                {
                    System.Windows.Forms.TextBox textBox = (System.Windows.Forms.TextBox)control1;
                    // 텍스트박스 내부의 버튼을 찾음
                    foreach (Control innerControl in textBox.Controls)
                    {
                        if (innerControl is System.Windows.Forms.Button)
                        {
                            System.Windows.Forms.Button button = (System.Windows.Forms.Button)innerControl;

                            //버튼의 텍스트가 진료중인지 확인하는 조건문
                            if (button.Text.Contains("진료중"))
                            {
                                button.Enabled = true;
                                // 찾은 버튼을 클릭하도록 함
                                button.PerformClick();//패널을 안보고있으면 안눌림

                                return; // 버튼을 찾고 클릭했으므로 더 이상의 탐색은 종료
                            }
                        }
                    }
                }
            }
            foreach (Control control in Form_login.form_main.panel107.Controls)
            {
                if (control is System.Windows.Forms.TextBox)
                {
                    System.Windows.Forms.TextBox textBox = (System.Windows.Forms.TextBox)control;
                    // 텍스트박스 내부의 버튼을 찾음
                    foreach (Control innerControl in textBox.Controls)
                    {
                        if (innerControl is System.Windows.Forms.Button)
                        {
                            System.Windows.Forms.Button button = (System.Windows.Forms.Button)innerControl;

                            //버튼의 텍스트가 진료중인지 확인하는 조건문
                            if (button.Text.Contains("진료중"))
                            {
                                button.Enabled = true;
                                // 찾은 버튼을 클릭하도록 함

                                button.PerformClick();//패널을 안보고있으면 안눌림

                                return; // 버튼을 찾고 클릭했으므로 더 이상의 탐색은 종료
                            }
                        }
                    }
                }
            }

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Color borderColor = Color.FromArgb(203, 216, 242);
            ControlPaint.DrawBorder(e.Graphics, this.panel1.ClientRectangle, borderColor, ButtonBorderStyle.Solid);
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            Color borderColor = Color.FromArgb(203, 216, 242);
            ControlPaint.DrawBorder(e.Graphics, this.panel2.ClientRectangle, borderColor, ButtonBorderStyle.Solid);
        }

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            button1.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.Font = new Font("G마켓 산스 TTF Light", 16);
        }

        private void button2_MouseMove(object sender, MouseEventArgs e)
        {
            button2.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.Font = new Font("G마켓 산스 TTF Light", 16);
        }

        private void button3_MouseMove(object sender, MouseEventArgs e)
        {
            button3.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            button3.Font = new Font("G마켓 산스 TTF Light", 16);
        }
    }

    
}
