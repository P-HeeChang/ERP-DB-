using Project_4;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Linq;
using static Project_4.Program;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using Mysqlx.Resultset;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using System.Runtime.Remoting.Messaging;
using System.Diagnostics.Metrics;
// panel1: 초기화면-환자
// panel1_1: 당일 예약 환자
// panel1_2: 환자 신규 등록
// panel1_3: 그래프

// panel2: 초기화면-근태
// panel3: 초기화면-재고
// panel4: 초기화면-관리자
// panel5: 대기자 화면
// panel6: 진료실1
// panel7: 진료실2

namespace Project_4
{


    public partial class Form_main : Form
    {

        private static MySqlConnection conn;

        private static string server = "192.168.31.147";
        private static string database = "team4";
        private static string uid = "root";
        private static string password = "0000";

        public static Form_inv_add form_inv_add;
        private Form_login form_login;
        public static Form_man_add form_man_add;
        public static Form_man_modify form_man_modify;
        private DateTime clickedDatetime;
        private UserControldays clickedpanel;
        public static outpatient outpatient;
        public static Form_pay form_pay;
        int month, year;
        string name;
        string workcount = "0";
        int totalworkcount = 0;
        int[] weeklyVisitors = new int[5];
        private Timer timer;
        private static DateTime clickedDateTime;


        public Form_main(Form_login form_login, bool auth, string Name, string Birth, string Rent)
        {


            InitializeComponent();


            //
            //간호사와 관리자 구분 [메인 사이드 왼쪽]
            this.name = Name;
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // 화면 사이즈 변경 불가능
            MaximizeBox = false; // 최대화 불가능
            button4.Visible = auth;     // 관리자계정일때 보여주기
            if (auth == false)       // 간호사 계정일때
            {
                label1.Text = Name + " 간호사님 환영합니다.";
            }
            else
            {
                label1.Text = Name + " 님 환영합니다.";
            }
            this.form_login = form_login;


            //
            // 당일 예약 환자 [메인 가운데]
            string connectionString = $"SERVER = {server}; DATABASE = {database}; UID={uid}; PASSWORD={password};";
            conn = new MySqlConnection(connectionString); // MYSQL로 만든 생성자 함수 conn이라 부름.
            string reservation = $"SELECT * FROM reservation";

            if (make_connection())
            {
                MySqlCommand cmd = new MySqlCommand(reservation, conn);
                cmd.ExecuteNonQuery();  //SQL문 실행
                MySqlDataReader reader = cmd.ExecuteReader();
                
                dataGridView1.Rows.Clear();
                while (reader.Read())
                {
                    if (reader[5].ToString() == DateTime.Now.ToString("yyyy-MM-dd"))
                    {
                        dataGridView1.Rows.Add(reader[0], reader[1], reader[2], reader[3], reader[4]);
                    }

                }
                conn.Close();//conn이라는 DB연결 객체를 해제
            }

            //
            //달력 나타내기
            DisplaDays();
            DisplaDays1();

            //
            // DateTimePicker에서 시간이 선택되었을 때 이벤트 핸들러 등록 -> 예약 시간
            dateTimePicker1.ValueChanged += DateTimePicker_ValueChanged;

        }

        //
        //달력 -> 근태관리 달력
        private void DisplaDays()
        {
            DateTime now = DateTime.Now;
            month = now.Month;
            year = now.Year;
            string monthname = DateTimeFormatInfo.CurrentInfo.GetMonthName(month);// 현재 월에 해당하는 달의 이름을 가져와서 라벨에 표시
            label13.Text = year + "년 " + monthname;

            DateTime startofthemonth = new DateTime(year, month, 1);// 현재 연도와 월에 해당하는 달의 시작 요일과 날짜 수를 계산
            int days = DateTime.DaysInMonth(year, month);
            int dayoftheweek = Convert.ToInt32(startofthemonth.DayOfWeek.ToString("d")) + 3;
            for (int i = 0; i < dayoftheweek; i++)  // 달력의 시작 부분에 공백을 추가
            {
                UserControl ucblank = new UserControlBlank();
                Daycontainer.Controls.Add(ucblank);
            }
            for (int i = 1; i <= days; i++)// 해당 월의 각 날짜를 패널에 추가
            {
                UserControldays ucdays = new UserControldays();
                ucdays.days(i);
                Daycontainer.Controls.Add(ucdays);
                ucdays.OnPanelClick += UserControlDays_OnPanelClick;// 클릭 이벤트 핸들러를 등록
            }

        }
        //
        // 달력 -> 예약관리 달력
        private void DisplaDays1()
        {
            DateTime now = DateTime.Now;
            month = now.Month;
            year = now.Year;
            string monthname = DateTimeFormatInfo.CurrentInfo.GetMonthName(month);// 현재 월에 해당하는 달의 이름을 가져와서 라벨에 표시
            label36.Text = year + "년 " + monthname;

            DateTime startofthemonth = new DateTime(year, month, 1);// 현재 연도와 월에 해당하는 달의 시작 요일과 날짜 수를 계산
            int days = DateTime.DaysInMonth(year, month);
            int dayoftheweek = Convert.ToInt32(startofthemonth.DayOfWeek.ToString("d")) + 3;
            for (int i = 0; i < dayoftheweek; i++)  // 달력의 시작 부분에 공백을 추가
            {
                UserControl ucblank = new UserControlBlank();
                Daycontainer1.Controls.Add(ucblank);
            }
            for (int i = 1; i <= days; i++)// 해당 월의 각 날짜를 패널에 추가
            {
                UserControldays ucdays = new UserControldays();
                ucdays.days(i);
                Daycontainer1.Controls.Add(ucdays);
                ucdays.OnPanelClick += UserControlDays_OnPanelClick;// 클릭 이벤트 핸들러를 등록
            }

        }

        //
        //
        public void UserControlDays_OnPanelClick(object sender, EventArgs e)
        {
            UserControldays clickedPanel = sender as UserControldays;//클릭한 패널 가져오기
            DateTime today = DateTime.Now;

            if (clickedPanel != null)
            {
                string clickedDate = clickedPanel.DayLabelText; // 클릭한 패널에서 날짜 정보 추출
                clickedDateTime = new DateTime(year, month, int.Parse(clickedDate)); // 클릭된 패널의 날짜 설정
            }
            else
            {
                clickedDateTime = today; // 클릭한 패널이 없는 경우에는 오늘 날짜를 선택
            }
            clickedDatetime = clickedDateTime;

            dataGridView6.Rows.Clear();
            dataGridView3.Rows.Clear();

            string reconnectionString = $"SERVER = {server}; DATABASE = {database}; UID={uid}; PASSWORD={password};";
            conn = new MySqlConnection(reconnectionString); // MYSQL로 만든 생성자 함수 conn이라 부름.
            string reservation = $"SELECT * FROM reservation";
            if (make_connection())
            {
                MySqlCommand cmds = new MySqlCommand(reservation, conn);
                cmds.ExecuteNonQuery();  //SQL문 실행
                MySqlDataReader readery = cmds.ExecuteReader();

                dataGridView3.Rows.Clear();
                while (readery.Read())
                {
                    if (readery[5].ToString() == clickedDateTime.ToString("yyyy-MM-dd"))
                    {
                        dataGridView3.Rows.Add(readery[1], readery[2], readery[3], readery[4], readery[5]);
                    }

                }
                conn.Close();
            }
            LoadDisplay();


            //이윤서
            //
            // 달력 클릭시 예약 날짜가 자동으로 들어간다.
            string connectionString = $"SERVER = {server}; DATABASE = {database}; UID={uid}; PASSWORD={password};";
            conn = new MySqlConnection(connectionString); // MYSQL로 만든 생성자 함수 conn이라 부름.

            string rv = $"SELECT * FROM reservation";

            if (make_connection())
            {
                MySqlCommand cmd = new MySqlCommand(rv, conn);
                cmd.ExecuteNonQuery();  //SQL문 실행
                MySqlDataReader reader = cmd.ExecuteReader();
                Form_login.form_main.dataGridView4.Rows.Clear();
                while (reader.Read())
                {
                    if (reader[5].ToString() == clickedDateTime.ToString("yyyy-MM-dd"))
                    {
                        Form_login.form_main.dataGridView4.Rows.Add(reader[0], reader[1], reader[2], reader[3], reader[4]);
                    }
                    textBox2.Text = clickedDateTime.ToString("yyyy-MM-dd");
                }
                conn.Close();//conn이라는 DB연결 객체를 해제
            }
          
            //
            //
            dataGridView2.Rows.Clear(); // 그리드뷰의 모든 행 제거
                                        // DataGridView2 설정
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

        }

        //
        //
        public void SetDayLabel(string day)
        {
            label13.Text = $"{DateTime.Now.Year}년 {DateTime.Now.Month}월 {day}일";// UserControlDays로부터 전달된 날짜 값을 받아서 라벨에 설정
        }

        private void Form_main_Load(object sender, EventArgs e)
        {
            //진료실번호 기본값설정
            label54.Font = new System.Drawing.Font(label54.Font, label54.Font.Style | System.Drawing.FontStyle.Underline);
            label53.Font = new System.Drawing.Font(label53.Font, label53.Font.Style & ~System.Drawing.FontStyle.Underline);
            //재고 관리
            button3_1_3.Font = new Font("G마켓 산스 TTF Light", 16);
            button3_2_1.Font = new Font("G마켓 산스 TTF Light", 16);
            button3_2_2.Font = new Font("G마켓 산스 TTF Light", 16);
            button3_3_3.Font = new Font("G마켓 산스 TTF Light", 16);
            button3_3_1.BackgroundImage = Properties.Resources.minus3;
            button3_3_2.BackgroundImage = Properties.Resources.plus;
            button3_3_1.BackgroundImageLayout = ImageLayout.Stretch;
            button3_3_2.BackgroundImageLayout = ImageLayout.Stretch;
            dataGridView3_2_1.ColumnHeadersDefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 15);
            dataGridView3_3_1.ColumnHeadersDefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 15);
            dataGridView3_2_1.DefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 14);
            dataGridView3_3_1.DefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 14);
            dataGridView3_2_1.RowTemplate.Height = 25;
            dataGridView3_3_1.RowTemplate.Height = 25;
            label3_1_2.Font = new Font("G마켓 산스 TTF Light", 16);
            label3_1_3.Font = new Font("G마켓 산스 TTF Light", 16);
            label3_1_4.Font = new Font("G마켓 산스 TTF Light", 16);
            label3_1_5.Font = new Font("G마켓 산스 TTF Light", 16);
            label3_2_2.Font = new Font("G마켓 산스 TTF Light", 16);
            label3_3_2.Font = new Font("G마켓 산스 TTF Light", 16);
            // 관리자 폰트
            tabPage3.Font = new Font("G마켓 산스 TTF Light", 12);       // 적용안됨
            tabPage4.Font = new Font("G마켓 산스 TTF Light", 12);       // 적용안됨
            tabPage5.Font = new Font("G마켓 산스 TTF Light", 12);       // 적용안됨
            tabPage6.Font = new Font("G마켓 산스 TTF Light", 12);       // 적용안됨
            tabPage7.Font = new Font("G마켓 산스 TTF Light", 12);       // 적용안됨
            button4_1.Font = new Font("G마켓 산스 TTF Light", 16);
            button4_1_1.Font = new Font("G마켓 산스 TTF Light", 16);
            button4_3.Font = new Font("G마켓 산스 TTF Light", 16);
            button4_5.Font = new Font("G마켓 산스 TTF Light", 16);
            label4_1.Font = new Font("G마켓 산스 TTF Light", 16);
            label4_2.Font = new Font("G마켓 산스 TTF Light", 16);
            label4_3.Font = new Font("G마켓 산스 TTF Light", 16);
            label4_4.Font = new Font("G마켓 산스 TTF Light", 16);
            label4_5.Font = new Font("G마켓 산스 TTF Light", 16);
            label4_6.Font = new Font("G마켓 산스 TTF Light", 16);
            label4_1_1.Font = new Font("G마켓 산스 TTF Light", 14);
            label4_1_2.Font = new Font("G마켓 산스 TTF Light", 14);
            label4_1_3.Font = new Font("G마켓 산스 TTF Light", 14);
            label4_1_4.Font = new Font("G마켓 산스 TTF Light", 14);
            label4_1_5.Font = new Font("G마켓 산스 TTF Light", 14);
            label4_1_6.Font = new Font("G마켓 산스 TTF Light", 14);
            label29.Font = new Font("G마켓 산스 TTF Light", 16);
            label23.Font = new Font("G마켓 산스 TTF Light", 16);
            dataGridView4_1.ColumnHeadersDefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 15);
            dataGridView4_1.DefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 14);
            dataGridView4_2.ColumnHeadersDefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 15);
            dataGridView4_2.DefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 14);
            dataGridView4_6.ColumnHeadersDefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 15);
            dataGridView4_6.DefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 14);
            dataGridView4_7.ColumnHeadersDefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 15);
            dataGridView4_7.DefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 14);
            dataGridView4_1.RowTemplate.Height = 25;
            dataGridView4_2.RowTemplate.Height = 25;
            dataGridView4_6.RowTemplate.Height = 25;
            dataGridView4_7.RowTemplate.Height = 25;
            // 관리자 재직 증명서
            label48.Text = DateTime.Now.ToString("yyyy" + "년 " + "MM" + "월 " + "dd" + "일");
            label22.Font = new Font("G마켓 산스 TTF Light", 16);
            label24.Font = new Font("G마켓 산스 TTF Light", 16);
            label25.Font = new Font("G마켓 산스 TTF Light", 16);
            label26.Font = new Font("G마켓 산스 TTF Light", 16);
            label27.Font = new Font("G마켓 산스 TTF Light", 16);
            label28.Font = new Font("G마켓 산스 TTF Light", 16);
            label30.Font = new Font("G마켓 산스 TTF Light", 16);
            label71.Font = new Font("G마켓 산스 TTF Bold", 24);
            label31.Font = new Font("G마켓 산스 TTF Light", 16);
            label32.Font = new Font("G마켓 산스 TTF Light", 16);
            label33.Font = new Font("G마켓 산스 TTF Light", 16);
            label48.Font = new Font("G마켓 산스 TTF Light", 16);
            label50.Font = new Font("G마켓 산스 TTF Light", 16);
            label51.Font = new Font("G마켓 산스 TTF Light", 16);
            button4_2.Font = new Font("G마켓 산스 TTF Light", 16);
            //예약 패널
            panel57.Visible = false;

            //
            //데이터그리드 첫 번째 줄 안 눌려있게 -> 당일 예약 환자[가운데 중앙]
            dataGridView1.ClearSelection();
            dataGridView1.DefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 14); // 원하는 폰트 및 크기로 변경

            //데이터 그리드 폰트 변경
            dataGridView6.DefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 14); // 원하는 폰트 및 크기로 변경
            dataGridView3.DefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 14); // 원하는 폰트 및 크기로 변경
            dataGridView4_1.DefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 14); // 원하는 폰트 및 크기로 변경

            //
            // 로드 후 첫 환자버튼은 언더라인 처리
            Font ft2 = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
            button1.Font = ft2;

            //
            //타이머
            timer1.Interval = 100; // 타이머 간격 100ms
            timer1.Start();  // 타이머 시작  

            //
            //이윤서
            //차트 
            chart1.Series.Clear();
            chart1.Series.Add("이번주 방문객 수");

            //차트 추가
            chart1.Series.Add("연령별");
            chart1.Series[1].ChartArea = "ChartArea2";

            //Grid 없애기
            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            chart1.ChartAreas[1].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[1].AxisY.MajorGrid.Enabled = false;


            // y축 설정
            this.chart1.ChartAreas[0].AxisY.Minimum = 0;
            this.chart1.ChartAreas[0].AxisY.Maximum = 30;

            this.chart1.ChartAreas[1].AxisY.Minimum = 0;
            this.chart1.ChartAreas[1].AxisY.Maximum = 30;

            //
            //차트 폰트

            // 축 레이블 폰트 변경
            chart1.ChartAreas[0].AxisX.LabelStyle.Font = new Font("G마켓 산스 TTF Light", 10);
            chart1.ChartAreas[0].AxisY.LabelStyle.Font = new Font("G마켓 산스 TTF Light", 10);
            chart1.ChartAreas[1].AxisX.LabelStyle.Font = new Font("G마켓 산스 TTF Light", 10);
            chart1.ChartAreas[1].AxisY.LabelStyle.Font = new Font("G마켓 산스 TTF Light", 10);


            // 범례 폰트 변경
            chart1.Legends[0].Font = new Font("G마켓 산스 TTF Light", 12);

            // 이번 주 월요일부터 금요일까지의 요일 설정
            DateTime thisMonday = GetThisWeekMonday();

            // 차트 초기화
            Form_login.form_main.chart1.Series[0].Points.Clear();

            chart1.ChartAreas[0].AxisX.Interval = 1; // 각 요일 간격
            chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Auto; // 요일 간격 설정
            // x축 설정 ChartArea2

            chart1.ChartAreas[1].AxisX.Interval = 1; // 각 연령 간격
            chart1.ChartAreas[1].AxisX.IntervalType = DateTimeIntervalType.Auto; // 연령 간격 설정

            UpdateChart();
            chart();
            GetThisWeekTotalMedicalLinesCount();
        }

        private void DateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            // 시간이 선택되었을 때 수행할 작업을 여기에 추가
            DateTimePicker dateTimePicker = (DateTimePicker)sender;
            textBox11.Text = dateTimePicker.Value.ToString("HH:mm");

        }


        public void UpdateChart()//240310----각요일별 누적수 보고싶으면 chart1.Series[0].Points[0~4].Label= weeklyVisitors[0~4].ToString();
        {
            // 이번 주 월요일부터 금요일까지의 요일 설정
            DateTime thisMonday = GetThisWeekMonday();
            for (int i = 0; i < 5; i++)
            {
                DateTime currentDay = thisMonday.AddDays(i);

                // 해당 날짜의 누적 방문자 수를 가져옴
                int medicalLinesCount = GetMedicalLinesCountForDay(currentDay);

                // 해당 요일의 누적 방문자 수 업데이트
                weeklyVisitors[i] = medicalLinesCount;

                // 차트에 x축에 월요일부터 금요일까지 고정하면서 해당 요일의 누적 방문자 수를 표시
                chart1.Series[0].Points.AddXY(currentDay.ToString("ddd"), medicalLinesCount);

            }
            for (int i = 0; i < 5; i++)
            {
                if (weeklyVisitors[i].ToString() != "0")
                {
                    chart1.Series[0].Points[i].Label = weeklyVisitors[i].ToString();
                }
            }

        }
        public void chart()
        {

            var chart = Form_login.form_main.chart1;
            int[] cumulativeCounts = new int[6]; // 누적 수를 저장할 배열 생성

            // 각 연령 그룹에 대한 누적 수 계산 및 데이터 포인트 추가
            cumulativeCounts[0] = GetAgeGroupCount10();
            chart.Series[1].Points.AddXY("~19", cumulativeCounts[0]);

            cumulativeCounts[1] = GetAgeGroupCount20();
            chart.Series[1].Points.AddXY("~29", cumulativeCounts[1]);

            cumulativeCounts[2] = GetAgeGroupCount30();
            chart.Series[1].Points.AddXY("~39", cumulativeCounts[2]);

            cumulativeCounts[3] = GetAgeGroupCount40();
            chart.Series[1].Points.AddXY("~49", cumulativeCounts[3]);

            cumulativeCounts[4] = GetAgeGroupCount50();
            chart.Series[1].Points.AddXY("~59", cumulativeCounts[4]);

            cumulativeCounts[5] = GetAgeGroupCount60();
            chart.Series[1].Points.AddXY("60~", cumulativeCounts[5]);

            int x = cumulativeCounts[0] + cumulativeCounts[1] + cumulativeCounts[2] + cumulativeCounts[3] + cumulativeCounts[4] + cumulativeCounts[5];
            label72.Text = x.ToString();

            // 각 막대 위에 누적 수 라벨 표시
            for (int i = 0; i < chart.Series[1].Points.Count; i++)
            {
                if (cumulativeCounts[i].ToString() != "0")
                {
                    chart.Series[1].Points[i].Label = cumulativeCounts[i].ToString(); // 누적 수를 라벨로 설정

                }
            }
        }



        // 해당 날짜의 medical_lines 수를 가져오는 함수 정의
        public int GetMedicalLinesCountForDay(DateTime day)
        {
            string connectionString = $"SERVER = {server}; DATABASE = {database}; UID={uid}; PASSWORD={password};";
            conn = new MySqlConnection(connectionString); // MYSQL로 만든 생성자 함수 conn이라 부름.
            string medical = $"SELECT * FROM medical";
            if (make_connection())
            {
                MySqlCommand cmd = new MySqlCommand(medical, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                int count = 0;

                while (reader.Read())
                {
                    // 파일 데이터의 날짜와 비교하여 오늘 날짜와 일치하면 해당하는 누적 방문자 수를 반환
                    if (reader[3].ToString() == day.ToString("yyyy-MM-dd"))
                    {
                        count++;
                    }
                }

                reader.Close();
                conn.Close(); // conn이라는 DB연결 객체를 해제
                return count;
            }
            else
            {
                conn.Close(); // 연결을 닫아야 합니다.
                return 0; // 연결을 만들 수 없는 경우 0을 반환합니다.
            }
        }


        public int GetThisWeekTotalMedicalLinesCount()
        {
            DateTime thisMonday = GetThisWeekMonday();
            DateTime nextMonday = thisMonday.AddDays(7);

            string connectionString = $"SERVER = {server}; DATABASE = {database}; UID={uid}; PASSWORD={password};";
            conn = new MySqlConnection(connectionString); // MYSQL로 만든 생성자 함수 conn이라 부름.
            string medical = $"SELECT * FROM medical";
            if (make_connection())
            {
                MySqlCommand cmd = new MySqlCommand(medical, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                int count = 0;

                while (reader.Read())
                {
                    // 파일 데이터의 날짜와 비교하여 이번 주에 해당하는지 확인
                    DateTime medicalDate = DateTime.ParseExact(reader[3].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    if (medicalDate >= thisMonday && medicalDate < nextMonday)
                    {
                        count++;
                    }
                }
                label35.Text = count.ToString();

                reader.Close();
                conn.Close(); // conn이라는 DB연결 객체를 해제
                return count;
            }
            else
            {
                conn.Close(); // 연결을 닫아야 합니다.
                return 0; // 연결을 만들 수 없는 경우 0을 반환합니다.
            }
        }

      


        //연령별 인원 수 
        public static int GetAgeGroupCount10()
        {

            string connectionString = $"SERVER = {server}; DATABASE = {database}; UID={uid}; PASSWORD={password};";
            conn = new MySqlConnection(connectionString); // MYSQL로 만든 생성자 함수 conn이라 부름.
            string medical = $"SELECT * FROM medical";
            if (make_connection())
            {
                MySqlCommand cmd = new MySqlCommand(medical, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                int count = 0;

                while (reader.Read())
                {
                    // 파일 데이터의 날짜와 비교하여 오늘 날짜와 일치하면 해당하는 누적 방문자 수를 반환
                    if (Convert.ToInt32(reader[2]) < 20 && DateTime.Parse(reader["date"].ToString()).Date == DateTime.Now.Date)
                    {
                        count++;
                    }
                }
                reader.Close();
                conn.Close(); // conn이라는 DB연결 객체를 해제
                return count;
            }
            else
            {
                conn.Close(); // 연결을 닫아야 합니다.
                return 0; // 연결을 만들 수 없는 경우 0을 반환합니다.
            }


        }

        public static int GetAgeGroupCount20()
        {



            string connectionString = $"SERVER = {server}; DATABASE = {database}; UID={uid}; PASSWORD={password};";
            conn = new MySqlConnection(connectionString); // MYSQL로 만든 생성자 함수 conn이라 부름.
            string medical = $"SELECT * FROM medical";
            if (make_connection())
            {
                MySqlCommand cmd = new MySqlCommand(medical, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                int count = 0;

                while (reader.Read())
                {
                    // 파일 데이터의 날짜와 비교하여 오늘 날짜와 일치하면 해당하는 누적 방문자 수를 반환
                    if (20 <= Convert.ToInt32(reader[2]) && Convert.ToInt32(reader[2]) < 30 && DateTime.Parse(reader["date"].ToString()).Date == DateTime.Now.Date)
                    {
                        count++;
                    }
                }

                reader.Close();
                conn.Close(); // conn이라는 DB연결 객체를 해제
                return count;
            }
            else
            {
                conn.Close(); // 연결을 닫아야 합니다.
                return 0; // 연결을 만들 수 없는 경우 0을 반환합니다.
            }

          
        }
        public static int GetAgeGroupCount30()
        {

            string connectionString = $"SERVER = {server}; DATABASE = {database}; UID={uid}; PASSWORD={password};";
            conn = new MySqlConnection(connectionString); // MYSQL로 만든 생성자 함수 conn이라 부름.
            string medical = $"SELECT * FROM medical";
            if (make_connection())
            {
                MySqlCommand cmd = new MySqlCommand(medical, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                int count = 0;

                while (reader.Read())
                {
                    // 파일 데이터의 날짜와 비교하여 오늘 날짜와 일치하면 해당하는 누적 방문자 수를 반환
                    if (30 <= Convert.ToInt32(reader[2]) && Convert.ToInt32(reader[2]) < 40 && DateTime.Parse(reader["date"].ToString()).Date == DateTime.Now.Date)
                    {
                        count++;
                    }
                }

                reader.Close();
                conn.Close(); // conn이라는 DB연결 객체를 해제
                return count;
            }
            else
            {
                conn.Close(); // 연결을 닫아야 합니다.
                return 0; // 연결을 만들 수 없는 경우 0을 반환합니다.
            }

        }
        public static int GetAgeGroupCount40()
        {

            string connectionString = $"SERVER = {server}; DATABASE = {database}; UID={uid}; PASSWORD={password};";
            conn = new MySqlConnection(connectionString); // MYSQL로 만든 생성자 함수 conn이라 부름.
            string medical = $"SELECT * FROM medical";
            if (make_connection())
            {
                MySqlCommand cmd = new MySqlCommand(medical, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                int count = 0;

                while (reader.Read())
                {
                    // 파일 데이터의 날짜와 비교하여 오늘 날짜와 일치하면 해당하는 누적 방문자 수를 반환
                    if (40 <= Convert.ToInt32(reader[2]) && Convert.ToInt32(reader[2]) < 50 && DateTime.Parse(reader["date"].ToString()).Date == DateTime.Now.Date)
                    {
                        count++;
                    }
                }

                reader.Close();
                conn.Close(); // conn이라는 DB연결 객체를 해제
                return count;
            }
            else
            {
                conn.Close(); // 연결을 닫아야 합니다.
                return 0; // 연결을 만들 수 없는 경우 0을 반환합니다.
            }

        
        }
        public static int GetAgeGroupCount50()
        {

            string connectionString = $"SERVER = {server}; DATABASE = {database}; UID={uid}; PASSWORD={password};";
            conn = new MySqlConnection(connectionString); // MYSQL로 만든 생성자 함수 conn이라 부름.
            string medical = $"SELECT * FROM medical";
            if (make_connection())
            {
                MySqlCommand cmd = new MySqlCommand(medical, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                int count = 0;

                while (reader.Read())
                {
                    // 파일 데이터의 날짜와 비교하여 오늘 날짜와 일치하면 해당하는 누적 방문자 수를 반환
                    if (50 <= Convert.ToInt32(reader[2]) && Convert.ToInt32(reader[2]) < 60 && DateTime.Parse(reader["date"].ToString()).Date == DateTime.Now.Date)
                    {
                        count++;
                    }
                }

                reader.Close();
                conn.Close(); // conn이라는 DB연결 객체를 해제
                return count;
            }
            else
            {
                conn.Close(); // 연결을 닫아야 합니다.
                return 0; // 연결을 만들 수 없는 경우 0을 반환합니다.
            }

        }
        public static int GetAgeGroupCount60()
        {

            string connectionString = $"SERVER = {server}; DATABASE = {database}; UID={uid}; PASSWORD={password};";
            conn = new MySqlConnection(connectionString); // MYSQL로 만든 생성자 함수 conn이라 부름.
            string medical = $"SELECT * FROM medical";
            if (make_connection())
            {
                MySqlCommand cmd = new MySqlCommand(medical, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                int count = 0;

                while (reader.Read())
                {
                    // 파일 데이터의 날짜와 비교하여 오늘 날짜와 일치하면 해당하는 누적 방문자 수를 반환
                    if (60 <= Convert.ToInt32(reader[2]) && DateTime.Parse(reader["date"].ToString()).Date == DateTime.Now.Date)
                    {
                        count++;
                    }
                }

                reader.Close();
                conn.Close(); // conn이라는 DB연결 객체를 해제
                return count;
            }
            else
            {
                conn.Close(); // 연결을 닫아야 합니다.
                return 0; // 연결을 만들 수 없는 경우 0을 반환합니다.
            }

     

        }

        // 현재 주의 월요일을 가져오는 메서드
        private DateTime GetThisWeekMonday()
        {
            DateTime today = DateTime.Today;
            int diff = today.DayOfWeek - DayOfWeek.Monday;
            return today.AddDays(-diff);
        }



        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            panel1_1.Visible = true;
            panel1_2.Visible = true;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            panel8.Visible = false;
            panel57.Visible = false;

            Font ft1 = new Font("G마켓 산스 TTF Light", 10);

            Font ft2 = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
            button1.Font = ft2;
            button2.Font = ft1;
            button3.Font = ft1;
            button4.Font = ft1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel1_1.Visible = false;
            panel1_2.Visible = false;
            panel2.Visible = true;
            panel3.Visible = false;
            panel4.Visible = false;
            panel8.Visible = false;
            panel57.Visible = false;
            Font ft1 = new Font("G마켓 산스 TTF Light", 10);

            Font ft2 = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
            button2.Font = ft2;
            button1.Font = ft1;
            button3.Font = ft1;
            button4.Font = ft1;
            clickedDateTime = DateTime.Now;
            LoadDisplay();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            panel1.Visible = false;
            panel1_1.Visible = false;
            panel1_2.Visible = false;
            panel2.Visible = false;
            panel3.Visible = true;
            panel4.Visible = false;
            panel8.Visible = false;
            panel57.Visible = false;
            textBox3_1_1.Focus();
            Font ft1 = new Font("G마켓 산스 TTF Light", 10);

            Font ft2 = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
            button3.Font = ft2;
            button2.Font = ft1;
            button1.Font = ft1;
            button4.Font = ft1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel1_1.Visible = false;
            panel1_2.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = true;
            panel8.Visible = false;
            panel57.Visible = false;
            Font ft1 = new Font("G마켓 산스 TTF Light", 10);

            Font ft2 = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
            button4.Font = ft2;
            button2.Font = ft1;
            button3.Font = ft1;
            button1.Font = ft1;
            //함수
            dataGridView4_1.Rows.Clear();
            dataGridView4_2.Rows.Clear();
            dataGridView4_5.Rows.Clear();
            dataGridView4_6.Rows.Clear();
            dataGridView4_7.Rows.Clear();
            Manage.Management();        // 간호사 이름과 직급
            Manage.m_app_inv();           // 결재올린 제품들
            Manage.vac_check();
            Manage.vac_check_mang();
            label4_1_1.Visible = false;
            label4_1_2.Visible = false;
            label4_1_3.Visible = false;
            label4_1_4.Visible = false;
            label4_1_5.Visible = false;
            label4_1_6.Visible = false;
            panel4_1_1.Visible = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = DateTime.Now.ToString("F"); // label1에 현재날짜시간 표시, F:자세한 전체 날짜/시간
        }


        private void main_textBox_Click(object sender, EventArgs e)
        {
            panel8.Visible = true;
        }

        private void main_textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Patient.Psearch();
            }
        }

        private void button8_Click_2(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                // 선택된 행의 데이터를 가져오기
                DataGridViewRow selectedRow = dataGridView2.SelectedRows[0];

                // 선택된 행의 데이터를 가져와서 문자열 배열로 변환
                string[] rowData = new string[selectedRow.Cells.Count];
                for (int i = 0; i < selectedRow.Cells.Count; i++)
                {
                    rowData[i] = selectedRow.Cells[i].Value.ToString();
                }

                // 폼2(outpatient)의 인스턴스 생성 및 데이터 전달
                outpatient formOutpatient = new outpatient(rowData);

                formOutpatient.Show();
            }
            else
            {
                MessageBox.Show("접수환자의 정보가 없습니다.");
            }
        }


        //주민번호 19991111이런식으로 입력하면 1999-11-11로 변환해서 저장되게 수정할것
        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            //숫자와 백스페이스와 '-'만 입력 형식에 맞지않게써도 뒤에 사용할 데이트타임변환에서 막혀서 메모장으로 안들어감
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back) || e.KeyChar == '-'))
            {
                e.Handled = true;
                label10.Visible = true;
            }
            else
            {
                label10.Visible = false;
            }
        }

        //한글입력시에도 경고문 뜨게 수정
        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back) || e.KeyChar == '-'))
            {
                e.Handled = true;
                label10.Visible = true;
            }
            else
            {
                label10.Visible = false;
            }
        }

        private void textBox4_Click(object sender, EventArgs e)
        {
            label10.Visible = false;

        }

        private void textBox5_Click(object sender, EventArgs e)
        {
            label10.Visible = false;
        }

        private void textBox6_Click(object sender, EventArgs e)
        {
            label10.Visible = false;
        }

        private void textBox7_Click(object sender, EventArgs e)
        {
            label10.Visible = false;
        }

        private void textBox4_Enter(object sender, EventArgs e)
        {
            label3.Text = string.Empty;
        }

        private void textBox5_Enter(object sender, EventArgs e)
        {
            label4.Text = string.Empty;
        }

        private void textBox6_Enter(object sender, EventArgs e)
        {
            label11.Text = string.Empty;
        }

        private void textBox7_Enter(object sender, EventArgs e)
        {
            label12.Text = string.Empty;
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button6_Click(sender, e);
            }

        }

        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button6_Click(sender, e);
            }
        }

        private void textBox6_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button6_Click(sender, e);
            }
        }

        private void textBox7_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button6_Click(sender, e);
            }
        }

        private void button13_Click(object sender, EventArgs e)  // 수납버튼 클릭시 
        {
            Form_pay form_Pay = new Form_pay();
            form_Pay.Show();
        }

        private void button17_Click_1(object sender, EventArgs e)
        {
            Daycontainer.Controls.Clear();
            month++;
            if (month > 12)
            {
                month = 1;
                year++;
            }
            DateTime startofthemonth = new DateTime(year, month, 1);// 다음달 첫날의 정보를 가져옴
            int days = DateTime.DaysInMonth(year, month); //다음달 일 수 계산
            int dayoftheweek = Convert.ToInt32(startofthemonth.DayOfWeek.ToString("d")); // 다음 달의 시작 요일을 계산
            for (int i = 0; i < dayoftheweek; i++)  // 다음 달의 시작 요일까지 공백 패널을 추가
            {
                UserControl ucblank = new UserControlBlank(); //공백 패널 생성
                Daycontainer.Controls.Add(ucblank); //공백 패널 추가
            }
            for (int i = 1; i <= days; i++)// 다음 달의 각 날짜에 해당하는 패널을 추가
            {
                UserControldays ucdays = new UserControldays(); //날짜 패널 생성
                ucdays.days(i); //패널에 날짜 설정
                Daycontainer.Controls.Add(ucdays); //날짜 패널 추가
            }
            label13.Text = $"{year}년 {month}월"; // 라벨에 다음 달의 연도와 월을 표시
            foreach (Control control in Daycontainer.Controls) // 패널에 있는 UserControlDays의 클릭 이벤트 핸들러를 등록
            {
                if (control is UserControldays)
                {
                    (control as UserControldays).OnPanelClick += UserControlDays_OnPanelClick; // 해당 패널의 클릭 이벤트 핸들러 등록
                }
            }
        }

        private void button18_Click_1(object sender, EventArgs e)
        {
            Daycontainer.Controls.Clear();
            month--;
            if (month < 1)
            {
                month = 12;
                year--;
            }
            DateTime startofthemonth = new DateTime(year, month, 1); //지난달 첫날의 정보를 가져옴
            int days = DateTime.DaysInMonth(year, month);//지난달 일 수 계산
            int dayoftheweek = Convert.ToInt32(startofthemonth.DayOfWeek.ToString("d"));// 지난 달의 시작 요일을 계산
            for (int i = 0; i < dayoftheweek; i++)// 지난 달의 시작 요일까지 공백 패널을 추가
            {
                UserControl ucblank = new UserControlBlank();//공백 패널 생성
                Daycontainer.Controls.Add(ucblank); //공백 패널 추가
            }
            for (int i = 1; i <= days; i++)// 지난 달의 각 날짜에 해당하는 패널을 추가
            {
                UserControldays ucdays = new UserControldays();//날짜 패널 생성
                ucdays.days(i);//날짜 패널 설정
                Daycontainer.Controls.Add(ucdays);//날짜 패널 추가
            }
            label13.Text = $"{year}년 {month}월";// 라벨에 지난 달의 연도와 월을 표시
            foreach (Control control in Daycontainer.Controls) // 패널에 있는 UserControlDays의 클릭 이벤트 핸들러를 등록
            {
                if (control is UserControldays)
                {
                    (control as UserControldays).OnPanelClick += UserControlDays_OnPanelClick; // 해당 패널의 클릭 이벤트 핸들러 등록
                }
            }
        }

        private void button7_Click(object sender, EventArgs e) //출근버튼
        {
            DateTime today = DateTime.Now;
            string name = this.name;
            string goworktime = today.ToString("HH:mm:ss");
            string outworktime = textBox3.Text;
            string goworkcheck = (goworktime.Contains(":")) ? "O" : textBox4.Text;
            string outworkcheck = textBox5.Text;
            int workcount;
            if (string.IsNullOrEmpty(textBox6.Text))
            {
                workcount = 0; // 혹은 다른 기본값으로 설정
            }
            else
            {
                workcount = int.Parse(textBox6.Text);
            }
            string worktime = textBox7.Text;
            string worktimecount = textBox8.Text;
            string workdate = today.ToString("yyyy-MM-dd");

            string connectionString = $"SERVER ={server};DATABASE={database};UID={uid};PASSWORD={password};";
            conn = new MySqlConnection(connectionString);


            var sql = $"INSERT INTO SCHEDULE VALUES('{name}', '{goworktime}', '{outworktime}', '{goworkcheck}', '{outworkcheck}', '{workcount}', '{worktime}', '{worktimecount}', '{workdate}');";
            string check = "SELECT * FROM SCHEDULE";

            if (make_connection())
            {
                try
                {
                    MySqlCommand checkCmd = new MySqlCommand(check, conn);
                    MySqlDataReader reader = checkCmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string existingName = reader.GetString("NAME");
                        string existingDate = reader.GetDateTime("workdate").ToString("yyyy-MM-dd");

                        if (existingName == name && existingDate == workdate)
                        {
                            MessageBox.Show("이미 출근한 날짜입니다.");
                            return;
                        }
                    }

                    reader.Close();

                    // 출근 기록 삽입 쿼리 실행
                    MySqlCommand insertCmd = new MySqlCommand(sql, conn);
                    insertCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("쿼리 전송 실패: " + ex.Message);
                }
                finally
                {
                    conn.Close(); // 항상 연결을 닫음
                }
            }
            LoadDisplay();
        }
        private void button19_Click(object sender, EventArgs e) //월차 버튼
        {
            Form_vacation formVacation = new Form_vacation(form_login, form_login.Name, form_login.UserBirth, form_login.UserRent);
            // 생성된 인스턴스를 보여줌
            formVacation.Show();
        }

        private void button11_Click(object sender, EventArgs e) //퇴근
        {
            DateTime today = DateTime.Now;
            DateTime todaycheck = DateTime.Today;
            DateTime formattedDate = DateTime.ParseExact(todaycheck.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            string time = today.ToString("HH:mm:ss");
            string box = textBox1.Text;
            server = "192.168.31.147";
            database = "team4";
            uid = "root";
            password = "0000";
            string name = textBox1.Text;
            string goworktime = DateTime.Now.ToString("HH:mm:ss");
            string outworktime = today.ToString("HH:mm:ss");
            int workcount = 0;
            if (string.IsNullOrEmpty(textBox6.Text))
            {
                workcount = workcount + 1; // 혹은 다른 기본값으로 설정
            }
            else
            {
                workcount = int.Parse(textBox6.Text);
            }
            string worktime = textBox7.Text;
            string worktimecount = textBox8.Text;
            string workdate = today.ToString("yyyy-MM-dd");
            string connectionString = $"SERVER ={server};DATABASE={database};UID={uid};PASSWORD={password};";
            conn = new MySqlConnection(connectionString);
            string gowork_time_query = $"SELECT gowork FROM SCHEDULE WHERE name = '{this.name}'AND workdate = '{todaycheck.Date.ToString("yyyy-MM-dd")}';";
            string gowork_time = "";

            if (connection())
            {
                MySqlCommand gowork_cmd = new MySqlCommand(gowork_time_query, conn);
                // gowork_time_query를 실행하여 결과를 받아옴
                MySqlDataReader reader = gowork_cmd.ExecuteReader();
                if (reader.Read())
                {
                    gowork_time = reader["gowork"].ToString(); // gowork 시간을 문자열로 저장
                }
                reader.Close();
                conn.Close();
            }
            string check_outwork = $"SELECT outworkcheck FROM SCHEDULE WHERE name = '{this.name}' AND workdate = '{todaycheck.Date.ToString("yyyy-MM-dd")}';";
            string re_check_outwork = "";
            if (connection())
            {
                MySqlCommand check_outwork_cmd = new MySqlCommand(check_outwork, conn);
                MySqlDataReader reader = check_outwork_cmd.ExecuteReader();
                if (reader.Read())
                {
                    re_check_outwork = reader["outworkcheck"].ToString(); // gowork 시간을 문자열로 저장
                    MessageBox.Show(re_check_outwork);
                }
                reader.Close();
                conn.Close();
            }
            //string total_worktime = $"SELECT worktimecount FROM SCHEDULE WHERE name = '김강도'AND workdate <= CURDATE() ORDER BY worktimecount DESC LIMIT 1';";
            string total_worktime = $"SELECT worktimecount FROM SCHEDULE WHERE name = '{this.name}' AND workdate <= CURDATE() ORDER BY workdate DESC LIMIT 1";
            string new_total_worktime = "";
            if (connection())
            {
                MySqlCommand total_worktime_cmd = new MySqlCommand(total_worktime, conn);
                MySqlDataReader reader = total_worktime_cmd.ExecuteReader();
                if (reader.Read())
                {
                    new_total_worktime = reader["worktimecount"].ToString(); // gowork 시간을 문자열로 저장
                    MessageBox.Show(new_total_worktime);
                }
                reader.Close();
                conn.Close();
            }
            string workdate_bring = $"SELECT workdate FROM SCHEDULE WHERE name = '{this.name}'AND workdate = '{todaycheck.Date.ToString("yyyy-MM-dd")}';";
            string workdate_space = "";
            if (connection())
            {
                MySqlCommand workdate_bring_cmd = new MySqlCommand(workdate_bring, conn);
                MySqlDataReader reader = workdate_bring_cmd.ExecuteReader();
                if (reader.Read())
                {
                    workdate_space = reader["workdate"].ToString(); // gowork 시간을 문자열로 저장
                }
                reader.Close();
                conn.Close();
            }
            string workcount_bring = $"SELECT workcount FROM SCHEDULE WHERE name = '{this.name}' AND workdate <= CURDATE() ORDER BY workdate DESC LIMIT 1";
            string workcount_space = "";
            if (connection())
            {
                MySqlCommand workcount_bring_cmd = new MySqlCommand(workcount_bring, conn);
                MySqlDataReader reader = workcount_bring_cmd.ExecuteReader();
                if (reader.Read())
                {
                    workcount_space = reader["workcount"].ToString(); // gowork 시간을 문자열로 저장
                }
                reader.Close();
                conn.Close();
            }
            DateTime goworkDateTime = DateTime.Parse(gowork_time);
            DateTime outworkDateTime = DateTime.Parse(outworktime);
            TimeSpan worktimeDate = TimeSpan.Parse(new_total_worktime);

            // 근무 시간 계산
            TimeSpan workDuration = outworkDateTime - goworkDateTime; // 오늘 일한 시간
            TimeSpan totalWorkTime = worktimeDate + workDuration; // 총 근무 시간
            TimeSpan totalworktimeminustoday = worktimeDate - workDuration; ; //총근무시간 - 오늘 일한시간
            int workcount_plus = int.Parse(workcount_space) + 1;
            int workcount_min;
            if (int.Parse(workcount_space) > 0) //가져온 근무 누적일수의 값이 0보다 크면 1을뺀다
            {
                workcount_min = int.Parse(workcount_space) - 1;
            }
            else ////가져온 근무 누적일수의 값이 0작거나 같으면  0을 할당해준다.
            {
                workcount_min = 0;
            }
            // 오늘 일한 시간을 제외한 총 근무 시간 계산
            DateTime workspace = DateTime.Parse(workdate_space); //가져온 일한 날짜
            string workDurationString = workDuration.ToString(@"hh\:mm\:ss"); //오늘 일한 시간 -> 퇴근 누른 근무시간
            string worktotal_time = totalWorkTime.ToString(@"hh\:mm\:ss"); //오늘 퇴근 누른 다음 총 근무시간 + 오늘 일한 시간 -> 퇴근한 총 근무시간
            string totalworkminus = totalworktimeminustoday.ToString((@"hh\:mm\:ss"));//오늘 퇴근누른다음 합친 총 근무시간 - 오늘 일한시간 -> 퇴근 취소후 총 근무시간
            //string workdur = workDuration.ToString((@"hh\:mm\:ss")); //퇴근 취소한 근무시간
            string sql = "";
            if (re_check_outwork == "O" && todaycheck == formattedDate) //formattedate는 오늘 날짜 ex) 03-13 < 03-14
            {
                sql = $"UPDATE SCHEDULE SET outworkcheck = 'X', outwork = '{null}', worktime = '{null}', worktimecount = '{totalworkminus}',workcount = '{workcount_min}' WHERE name = '{this.name}'AND workdate = '{todaycheck.Date.ToString("yyyy-MM-dd")}';"; //일한시간 = 퇴근시간-출근시간, 워크 토탈 =>
            }
            else
            {
                sql = $"UPDATE SCHEDULE SET outworkcheck = 'O', outwork = '{outworktime}', worktime = '{workDurationString}', worktimecount = '{worktotal_time}',workcount = '{workcount_min}' WHERE name = '{this.name}'AND workdate =  '{todaycheck.Date.ToString("yyyy-MM-dd")}';";
            }

            if (!string.IsNullOrEmpty(sql))
            {
                if (connection())
                {
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery(); //SQL문 실행

                    conn.Close(); //conn이라는 db연결 객체를 해제
                }
                else
                {

                }
                LoadDisplay();
            }
        }
        private bool connection()
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
        public void LoadDisplay() 
        {
            dataGridView6.Rows.Clear();

            string connectionString = $"SERVER ={server};DATABASE={database};UID={uid};PASSWORD={password};";
            conn = new MySqlConnection(connectionString);
            string check = "SELECT * FROM SCHEDULE";
            if (make_connection())
            {
                try
                {
                    MySqlCommand checkCmd = new MySqlCommand(check, conn);
                    MySqlDataReader reader = checkCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        try
                        {
                            string dbDate = reader.GetDateTime(8).ToString("yyyy-MM-dd");
                            if (dbDate == clickedDateTime.ToString("yyyy-MM-dd"))
                            {
                                dataGridView6.Rows.Add(reader[0], reader[1], reader[2], reader[3], reader[4]);
                            }
                        }
                        catch { }
                    }
                    reader.Close();
                }
                catch { }
            }
        }

        private void button1_1_Click(object sender, EventArgs e)
        {
            
            form_login.textBox1.Text = null;
            form_login.textBox2.Text = null;
            
            form_login.label3.Text = "ID를 입력해주세요.";
            form_login.label4.Text = "PW를 입력해주세요.";
            form_login.textBox1.Focus();
            this.Hide();
            form_login.ShowDialog();
            
            this.Close();


        }


        //
        //환자 검색 후 환자 더블 클릭시

        private void button9_Click(object sender, EventArgs e)
        {
            Daycontainer1.Controls.Clear();
            month--;
            if (month < 1)
            {
                month = 12;
                year--;
            }
            DateTime startofthemonth = new DateTime(year, month, 1); //지난달 첫날의 정보를 가져옴
            int days = DateTime.DaysInMonth(year, month);//지난달 일 수 계산
            int dayoftheweek = Convert.ToInt32(startofthemonth.DayOfWeek.ToString("d"));// 지난 달의 시작 요일을 계산
            for (int i = 0; i < dayoftheweek; i++)// 지난 달의 시작 요일까지 공백 패널을 추가
            {
                UserControl ucblank = new UserControlBlank();//공백 패널 생성
                Daycontainer1.Controls.Add(ucblank); //공백 패널 추가
            }
            for (int i = 1; i <= days; i++)// 지난 달의 각 날짜에 해당하는 패널을 추가
            {
                UserControldays ucdays = new UserControldays();//날짜 패널 생성
                ucdays.days(i);//날짜 패널 설정
                Daycontainer1.Controls.Add(ucdays);//날짜 패널 추가
            }
            label36.Text = $"{year}년 {month}월";// 라벨에 지난 달의 연도와 월을 표시
            foreach (Control control in Daycontainer1.Controls) // 패널에 있는 UserControlDays의 클릭 이벤트 핸들러를 등록
            {
                if (control is UserControldays)
                {
                    (control as UserControldays).OnPanelClick += UserControlDays_OnPanelClick; // 해당 패널의 클릭 이벤트 핸들러 등록
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Daycontainer1.Controls.Clear();
            month++;
            if (month > 12)
            {
                month = 1;
                year++;
            }
            DateTime startofthemonth = new DateTime(year, month, 1);// 다음달 첫날의 정보를 가져옴
            int days = DateTime.DaysInMonth(year, month); //다음달 일 수 계산
            int dayoftheweek = Convert.ToInt32(startofthemonth.DayOfWeek.ToString("d")); // 다음 달의 시작 요일을 계산
            for (int i = 0; i < dayoftheweek; i++)  // 다음 달의 시작 요일까지 공백 패널을 추가
            {
                UserControl ucblank = new UserControlBlank(); //공백 패널 생성
                Daycontainer1.Controls.Add(ucblank); //공백 패널 추가
            }
            for (int i = 1; i <= days; i++)// 다음 달의 각 날짜에 해당하는 패널을 추가
            {
                UserControldays ucdays = new UserControldays(); //날짜 패널 생성
                ucdays.days(i); //패널에 날짜 설정
                Daycontainer1.Controls.Add(ucdays); //날짜 패널 추가
            }
            label36.Text = $"{year}년 {month}월"; // 라벨에 다음 달의 연도와 월을 표시
            foreach (Control control in Daycontainer1.Controls) // 패널에 있는 UserControlDays의 클릭 이벤트 핸들러를 등록
            {
                if (control is UserControldays)
                {
                    (control as UserControldays).OnPanelClick += UserControlDays_OnPanelClick; // 해당 패널의 클릭 이벤트 핸들러 등록
                }
            }
        }

        //
        //환자 검색 더블 클릭 시
        //
        private void dataGridView2_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)//폼메인---
        {
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            panel8.Visible = false;
            panel1_1.Visible = false;
            panel1_2.Visible = false;
            panel57.Visible = true;
            panel8.BringToFront();
            DataGridViewRow selectedRow = dataGridView2.Rows[e.RowIndex];

            string name = selectedRow.Cells[0].Value.ToString();
            string birthday = selectedRow.Cells[2].Value.ToString();
            string phoneNumber = selectedRow.Cells[3].Value.ToString();
            string city = selectedRow.Cells[4].Value.ToString();


            textBox1.Text = name;
            textBox8.Text = birthday;
            textBox9.Text = phoneNumber;
            textBox10.Text = city;

            //registration


            string connectionString = $"SERVER = {server}; DATABASE = {database}; UID={uid}; PASSWORD={password};";
            conn = new MySqlConnection(connectionString);
            string registration = "SELECT * FROM registration;";

            dataGridView5.Rows.Clear();


            if (make_connection())
            {

                MySqlCommand find = new MySqlCommand(registration, conn);

                find.ExecuteNonQuery();  //SQL문 실행

                MySqlDataReader reader = find.ExecuteReader();

                Form_login.form_main.dataGridView5.Rows.Clear();
                while (reader.Read())
                {

                    string[] rowValues = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {

                        rowValues[i] = reader[i].ToString();


                    }
                    if (name == rowValues[0] && birthday == rowValues[1])
                    {

                        if (rowValues[2] != DateTime.Now.ToString("yyyy-MM-dd"))
                        {
                            dataGridView5.Rows.Add(rowValues);

                        }
                    }
                }
            }

        }

        //
        //이윤서
        //예약하기
        private void button20_Click(object sender, EventArgs e)
        {
            try
            {
                string name = textBox1.Text;
                string birth = textBox8.Text;
                string phone = textBox9.Text;
                string city = textBox10.Text;
                string day = textBox2.Text; // 날짜 입력
                string time = textBox11.Text; // 시간 입력

                string date = clickedDateTime.ToString("yyyy-MM-dd");


                string connectionString = $"SERVER = {server}; DATABASE = {database}; UID={uid}; PASSWORD={password};";
                conn = new MySqlConnection(connectionString); // MYSQL로 만든 생성자 함수 conn이라 부름.
                string rv = $"SELECT * FROM reservation";

                if (make_connection())
                {

                    MySqlCommand cmd2 = new MySqlCommand(rv, conn);
                    cmd2.ExecuteNonQuery();

                    MySqlDataReader reader = cmd2.ExecuteReader();
                    DateTime previous = DateTime.Parse(day);

                    // 예약 날짜가 오늘 날짜보다 미래인 경우
                    if (previous > DateTime.Now)
                    {
                        bool isAppointmentAvailable = true; // 예약 가능 여부
                        while (reader.Read())
                        {
                            // 동일한 날짜와 시간이 있는지 확인
                            if (reader[0].ToString() == time && reader[5].ToString() == day)
                            {
                                MessageBox.Show("동일한 날짜와 시간에 이미 예약이 있습니다.");
                                isAppointmentAvailable = false; // 예약 불가능으로 설정
                            }

                        }
                        reader.Close();

                        if (isAppointmentAvailable)
                        {
                            if (textBox11.Text == "")
                            {
                                MessageBox.Show("입력을 확인해주세요.");
                            }
                            else
                            {
                                string reservation = $"INSERT INTO reservation VALUES ('{time}' ,'{name}', '{birth}', '{phone}', '{city}', '{date}')";  // 넣으려는 SQL문

                                MySqlCommand cmd = new MySqlCommand(reservation, conn);
                                cmd.ExecuteNonQuery();  //SQL문 실행


                                // 예약 가능한 경우, 예약 정보 추가
                                MessageBox.Show("예약되었습니다.");

                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("예약 할 수 없는 날짜입니다.");
                    }

                    // 입력 필드 초기화
                    textBox2.Text = null;
                    textBox11.Text = null;
                    conn.Close();//conn이라는 DB연결 객체를 해제
                }
            }

                        catch 
            {
                MessageBox.Show("날짜를 선택해주세요");
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

        //예약 삭제
        private void button16_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = $"SERVER = {server}; DATABASE = {database}; UID={uid}; PASSWORD={password};";
                conn = new MySqlConnection(connectionString); // MYSQL로 만든 생성자 함수 conn이라 부름.
                string rv = $"SELECT * FROM reservation";

                DataGridViewRow selectedRow = dataGridView4.SelectedRows[0];
                string time = selectedRow.Cells[0].Value.ToString();
                string name = selectedRow.Cells[1].Value.ToString();
                string birth = selectedRow.Cells[2].Value.ToString();
                string phone = selectedRow.Cells[3].Value.ToString();
                string city = selectedRow.Cells[4].Value.ToString();

                if (make_connection())
                {
                    MySqlCommand cmd = new MySqlCommand(rv, conn);
                    cmd.ExecuteNonQuery();  //SQL문 실행

                    string reservation = $"DELETE FROM reservation where (time = '{time}') and (name = '{name}') and (birth = '{birth}') and (phone = '{phone}') and (city ='{city}');";  // 넣으려는 SQL문
                    MySqlCommand cmd2 = new MySqlCommand(reservation, conn);
                    cmd2.ExecuteNonQuery();
                    MessageBox.Show("예약이 삭제되었습니다.");
                    conn.Close();//conn이라는 DB연결 객체를 해제  

                }
            }
            catch 
            {
                MessageBox.Show("날짜를 선택해주세요");
            }

        }

        //이전 진료 기록 더블 클릭시
        private void dataGridView5_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)//240310
        {
            if (dataGridView5.SelectedCells.Count > 0)
            {
                int selectedRowIndex = dataGridView5.SelectedCells[0].RowIndex;

                // 선택한 행의 메모장 정보 가져오기
                string selectedMemoInfo = dataGridView5.Rows[selectedRowIndex].Cells[0].Value.ToString();

                // 파일 이름 추출
                string fileName = Path.GetFileName(selectedMemoInfo);

                // 메모장 파일 경로 생성
                string folderPath = Path.Combine(System.Windows.Forms.Application.StartupPath, "patientData");
                string filePath = Path.Combine(folderPath, fileName);

                // 메모장 파일이 존재하는지 확인하고, 존재한다면 새 창에서 열기
                if (File.Exists(filePath))
                {
                    try
                    {
                        Process.Start("notepad.exe", filePath);//새창에서 텍스트파일 여는 코드(메모장,파일명)
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"파일을 열던 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show($"선택한 메모 파일 '{fileName}'이(가) 존재하지 않습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        //
        // 패널 테두리 색
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Color borderColor = Color.FromArgb(203, 216, 242);
            ControlPaint.DrawBorder(e.Graphics, this.panel1.ClientRectangle, borderColor, ButtonBorderStyle.Solid);
        }
        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            Color borderColor = Color.FromArgb(203, 216, 242);
            ControlPaint.DrawBorder(e.Graphics, this.panel1.ClientRectangle, borderColor, ButtonBorderStyle.Solid);
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            Color borderColor = Color.FromArgb(203, 216, 242);
            ControlPaint.DrawBorder(e.Graphics, this.panel1.ClientRectangle, borderColor, ButtonBorderStyle.Solid);
        }
        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            Color borderColor = Color.FromArgb(203, 216, 242);
            ControlPaint.DrawBorder(e.Graphics, this.panel1.ClientRectangle, borderColor, ButtonBorderStyle.Solid);
        }

        private void panel57_Paint(object sender, PaintEventArgs e)
        {
            Color borderColor = Color.FromArgb(203, 216, 242);
            ControlPaint.DrawBorder(e.Graphics, this.panel1.ClientRectangle, borderColor, ButtonBorderStyle.Solid);
        }


        //
        //마우스 언더라인
        private void button1_1_MouseMove(object sender, MouseEventArgs e)
        {
            Font ft2 = new Font("G마켓 산스 TTF Light", 14, FontStyle.Underline);
            button1_1.Font = ft2;
        }
        private void button1_1_MouseLeave(object sender, EventArgs e)
        {
            Font ft2 = new Font("G마켓 산스 TTF Light", 14);
            button1_1.Font = ft2;
        }
        private void button6_MouseMove(object sender, MouseEventArgs e)
        {
            Font ft2 = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
            button6.Font = ft2;
        }
        private void button6_MouseLeave(object sender, EventArgs e)
        {
            Font ft2 = new Font("G마켓 산스 TTF Light", 16);
            button6.Font = ft2;
        }
        private void button13_MouseMove(object sender, MouseEventArgs e)
        {
            Font ft2 = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
            button13.Font = ft2;
        }
        private void button13_MouseLeave(object sender, EventArgs e)
        {
            Font ft2 = new Font("G마켓 산스 TTF Light", 16);
            button13.Font = ft2;
        }
        private void button8_MouseMove(object sender, MouseEventArgs e)
        {
            Font ft2 = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
            button8.Font = ft2;
        }
        private void button8_MouseLeave(object sender, EventArgs e)
        {
            Font ft2 = new Font("G마켓 산스 TTF Light", 16);
            button8.Font = ft2;
        }
        private void button12_MouseMove(object sender, MouseEventArgs e)
        {
            Font ft2 = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
            button12.Font = ft2;
        }
        private void button12_MouseLeave(object sender, EventArgs e)
        {
            button12.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button11_MouseMove(object sender, MouseEventArgs e)
        {
            button11.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button11_MouseLeave(object sender, EventArgs e)
        {
            button11.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button7_MouseMove(object sender, MouseEventArgs e)
        {
            button7.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button7_MouseLeave(object sender, EventArgs e)
        {
            button7.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button19_MouseMove(object sender, MouseEventArgs e)
        {
            button19.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button19_MouseLeave(object sender, EventArgs e)
        {
            button19.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button17_MouseMove(object sender, MouseEventArgs e)
        {
            button17.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button17_MouseLeave(object sender, EventArgs e)
        {
            button17.Font = new Font("G마켓 산스 TTF Light", 14);
        }
        private void button18_MouseMove(object sender, MouseEventArgs e)
        {
            button18.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button18_MouseLeave(object sender, EventArgs e)
        {
            button18.Font = new Font("G마켓 산스 TTF Light", 14);
        }
        private void button9_MouseMove(object sender, MouseEventArgs e)
        {
            button9.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button9_MouseLeave(object sender, EventArgs e)
        {
            button9.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button10_MouseMove(object sender, MouseEventArgs e)
        {
            button10.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button10_MouseLeave(object sender, EventArgs e)
        {
            button10.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button21_MouseMove(object sender, MouseEventArgs e)
        {
            button21.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button21_MouseLeave(object sender, EventArgs e)
        {
            button21.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button16_MouseMove(object sender, MouseEventArgs e)
        {
            button16.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button16_MouseLeave(object sender, EventArgs e)
        {
            button16.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button20_MouseMove(object sender, MouseEventArgs e)
        {
            button20.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button20_MouseLeave(object sender, EventArgs e)
        {
            button20.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button5_MouseMove(object sender, MouseEventArgs e)
        {
            button5.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button5_MouseLeave(object sender, EventArgs e)
        {
            button5.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button14_MouseMove(object sender, MouseEventArgs e)
        {
            button14.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button14_MouseLeave(object sender, EventArgs e)
        {
            button14.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button4_2_MouseMove(object sender, MouseEventArgs e)
        {
            button4_2.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button4_2_MouseLeave(object sender, EventArgs e)
        {
            button4_2.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button4_3_MouseMove(object sender, MouseEventArgs e)
        {
            button4_3.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button4_3_MouseLeave(object sender, EventArgs e)
        {
            button4_3.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button4_5_MouseMove(object sender, MouseEventArgs e)
        {
            button4_5.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button4_5_MouseLeave(object sender, EventArgs e)
        {
            button4_5.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button4_1_1_MouseMove_1(object sender, MouseEventArgs e)
        {
            button4_1_1.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }

        private void button4_1_1_MouseLeave_1(object sender, EventArgs e)
        {
            button4_1_1.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button4_4_MouseMove(object sender, MouseEventArgs e)
        {
            button4_4.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);

        }
        private void button4_4_MouseLeave(object sender, EventArgs e)
        {
            button4_4.Font = new Font("G마켓 산스 TTF Light", 16);

        }

        private void button15_MouseMove(object sender, MouseEventArgs e)
        {
            button15.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }

        private void button15_MouseLeave(object sender, EventArgs e)
        {
            button15.Font = new Font("G마켓 산스 TTF Light", 16);
        }


        //신규 등록 Message 박스와 label
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            label3.Text = textBox4.Text;
            char[] inputchars = textBox4.Text.ToCharArray();          //한글만 들어가게
            var sb = new StringBuilder();

            foreach (var item in inputchars)
            {
                if (char.GetUnicodeCategory(item) == UnicodeCategory.OtherLetter)
                {
                    sb.Append(item);
                    label10.Visible = false;
                }
                else
                {
                    label10.Visible = true;
                }
            }
            textBox4.Text = sb.ToString().Trim();

        }

        //
        // Message박스와 label
        private void label3_Click(object sender, EventArgs e)
        {
            textBox4.Focus();
            textBox4.MaxLength = 7;
            label3.Text = null;
            textBox4.Text = null;
        }
        private void label4_Click(object sender, EventArgs e)
        {
            textBox5.Focus();
            textBox5.MaxLength = 10;
            label4.Text = null;
            textBox5.Text = null;
        }
        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            label4.Text = textBox5.Text;
        }
        private void label11_Click(object sender, EventArgs e)
        {
            textBox6.Focus();
            textBox6.MaxLength = 13;
            label11.Text = null;
            textBox6.Text = null;
        }
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            label11.Text = textBox6.Text;
        }
        private void label12_Click(object sender, EventArgs e)
        {
            textBox7.Focus();
            textBox7.MaxLength = 13;
            label12.Text = null;
            textBox7.Text = null;
        }
        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            label12.Text = textBox7.Text;
        }
        private void label34_Click(object sender, EventArgs e)
        {
            main_textBox.Focus();
            label34.Text = null;
        }
        private void main_textBox_TextChanged(object sender, EventArgs e)
        {
            Patient.Psearch();
            label34.Text = main_textBox.Text;
        }
        private void textBox4_1_1_TextChanged(object sender, EventArgs e)
        {
            label4_1_1.Text = textBox4_1_1.Text;
        }

        private void textBox4_1_2_TextChanged(object sender, EventArgs e)
        {
            label4_1_2.Text = textBox4_1_2.Text;
        }
        private void textBox4_1_3_TextChanged(object sender, EventArgs e)
        {
            label4_1_3.Text = textBox4_1_3.Text;
        }
        private void textBox4_1_4_TextChanged(object sender, EventArgs e)
        {
            label4_1_4.Text = textBox4_1_4.Text;
        }
        private void textBox4_1_5_TextChanged(object sender, EventArgs e)
        {
            label4_1_5.Text = textBox4_1_5.Text;
        }
        private void textBox4_1_6_TextChanged(object sender, EventArgs e)
        {
            label4_1_6.Text = textBox4_1_6.Text;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label58.Text = textBox1.Text;
        }
        private void _Click(object sender, EventArgs e)
        {
            textBox1.Focus();
        }
        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            label60.Text = textBox10.Text;
        }
        private void label60_Click(object sender, EventArgs e)
        {
            textBox10.Focus();
        }
        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            label65.Text = textBox8.Text;
        }
        private void label65_Click(object sender, EventArgs e)
        {
            textBox8.Focus();
        }
        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            label66.Text = textBox9.Text;
        }
        private void label66_Click(object sender, EventArgs e)
        {
            textBox9.Focus();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            label45.Text = textBox2.Text;
        }
        private void label45_Click(object sender, EventArgs e)
        {
            textBox2.Focus();
        }
        private void label54_Click(object sender, EventArgs e)
        {
            panel106.Visible = true;
            panel107.Visible = false;
            label54.Font = new System.Drawing.Font(label54.Font, label54.Font.Style | System.Drawing.FontStyle.Underline);
            label53.Font = new System.Drawing.Font(label53.Font, label53.Font.Style & ~System.Drawing.FontStyle.Underline);
        }
        private void label53_Click(object sender, EventArgs e)
        {
            panel107.Visible = true;
            panel106.Visible = false;
            label53.Font = new System.Drawing.Font(label54.Font, label54.Font.Style | System.Drawing.FontStyle.Underline);
            label54.Font = new System.Drawing.Font(label53.Font, label53.Font.Style & ~System.Drawing.FontStyle.Underline);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                Patient.Pnew();
                textBox4.Text = null;
                textBox5.Text = null;
                textBox6.Text = null;
                textBox7.Text = null;
            }
            catch
            {
                label10.Visible = true;
            }

        }
        private void button5_Click(object sender, EventArgs e)
        {
            Patient.Psearch();

            if (panel1.Visible == true)
            {
                panel1.Visible = true;
                panel2.Visible = false;
                panel3.Visible = false;
                panel4.Visible = false;
                panel57.Visible = false;

            }
            else if (panel2.Visible == true)
            {
                panel1.Visible = false;
                panel2.Visible = true;
                panel3.Visible = false;
                panel4.Visible = false;
                panel57.Visible = false;

            }
            else if (panel3.Visible == true)
            {
                panel1.Visible = false;
                panel2.Visible = false;
                panel3.Visible = true;
                panel4.Visible = false;
                panel57.Visible = false;

            }
            else if (panel3.Visible == true)
            {
                panel1.Visible = false;
                panel2.Visible = false;
                panel3.Visible = false;
                panel4.Visible = true;
                panel57.Visible = false;
            }
            else
            {
                panel1.Visible = false;
                panel2.Visible = false;
                panel3.Visible = false;
                panel4.Visible = false;
                panel57.Visible = true;

            }
            panel8.Visible = true;
            //dataGridView 처음에 셀 선택 안 되어있게
            dataGridView2.ClearSelection();

        }
        private void button12_Click(object sender, EventArgs e)
        {
            if (button12.Text == "수정")
            {
                button12.Text = "완료";
                dataGridView2.EditMode = DataGridViewEditMode.EditOnEnter; // 편집 모드 설정
                dataGridView2.ReadOnly = false;
            }
            else
            {
                try
                {
                    DataGridViewRow selectedRow = dataGridView2.SelectedRows[0];
                    if (selectedRow == null)
                    {
                        MessageBox.Show("편집할 행을 선택하세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string originalName = selectedRow.Cells[0].Value.ToString(); // 원래 이름 저장
                    string name = selectedRow.Cells[0].Value.ToString(); // 현재 이름 가져오기
                    string birth = selectedRow.Cells[2].Value.ToString();
                    string phone = selectedRow.Cells[3].Value.ToString();
                    string city = selectedRow.Cells[4].Value.ToString();

                    DateTime birthDate = Convert.ToDateTime(selectedRow.Cells[2].Value); // 생년월일 가져오기
                    DateTime today = DateTime.Today; // 오늘 날짜 가져오기
                    int age = today.Year - birthDate.Year + 1; // 나이 계산

                    // 생일이 지나지 않은 경우 나이에서 1을 빼줌
                    if (birthDate > today.AddYears(-age))
                    {
                        age--;
                    }

                    string connectionString = $"SERVER = {server}; DATABASE = {database}; UID={uid}; PASSWORD={password};";
                    conn = new MySqlConnection(connectionString);

                    if (make_connection())
                    {
                        string FindQuery = $"SELECT * FROM patient WHERE name='{originalName}';";
                        MySqlCommand cmd = new MySqlCommand(FindQuery, conn);
                        MySqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            string existingAge = reader["age"].ToString();
                            string existingBirth = reader["birth"].ToString();
                            string existingPhone = reader["phone"].ToString();
                            string existingCity = reader["city"].ToString();
                            reader.Close(); // 데이터 리더 닫기

                            if (originalName != name || birth != existingBirth || phone != existingPhone || city != existingCity)
                            {
                                string updateQuery = $"UPDATE patient SET name='{name}', age='{age}', birth='{birth}', phone='{phone}', city='{city}' WHERE name = '{originalName}';";
                                MySqlCommand updateCommand = new MySqlCommand(updateQuery, conn);
                                updateCommand.ExecuteNonQuery(); // 데이터 업데이트
                                MessageBox.Show("데이터가 업데이트되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // 선택된 셀 해제
                                dataGridView2.ClearSelection();
                            }
                            else
                            {
                            }
                        }


                        conn.Close(); // 연결 종료
                    }

                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("이름은 변경 불가능 합니다.");
                }
            }
        }

        private void Form_main_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

     
        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void panel5_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void panel13_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void panel3_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void panel4_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView4_1_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tabControl2_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            dataGridView4_6.ClearSelection();
            dataGridView4_7.ClearSelection();
        }

        private void dataGridView4_2_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void panel6_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void chart1_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tabPage1_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tabPage2_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void label2_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        //
        //이윤서
        // 환자 예약 수정
        
        private void button21_Click(object sender, EventArgs e)
        {
            if (button21.Text == "수정하기")
            {
                button21.Text = "수정완료";
                dataGridView4.EditMode = DataGridViewEditMode.EditOnEnter; // 편집 모드 설정
                dataGridView4.ReadOnly = false;
                textBox2.ReadOnly = false;
            }
            else
            {
                try
                {
                    // DataGridView에서 선택된 행 가져오기
                    DataGridViewRow selectedRow = dataGridView4.SelectedRows[0];
                    if (selectedRow == null)
                    {
                        MessageBox.Show("편집할 행을 선택하세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    string reservationDay = textBox2.Text;
                    DateTime selectedDate = DateTime.Parse(reservationDay);

                    // 현재 날짜와 선택된 날짜 비교
                    if (selectedDate.Date <= DateTime.Today)
                    {
                        MessageBox.Show("이전 날짜는 변경할 수 없습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    string reservationTime = selectedRow.Cells[0].Value.ToString(); // 예약 시간
                    string name = selectedRow.Cells[1].Value.ToString(); // 이름
                    string dob = selectedRow.Cells[2].Value.ToString(); // 생년월일
                    string phoneNumber = selectedRow.Cells[3].Value.ToString(); // 전화번호
                    string address = selectedRow.Cells[4].Value.ToString(); // 주소


                    if (string.IsNullOrWhiteSpace(reservationTime) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(dob) || string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(address))
                    {
                        MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string connectionString = $"SERVER = {server}; DATABASE = {database}; UID={uid}; PASSWORD={password};";
                    conn = new MySqlConnection(connectionString);

                    if (make_connection())
                    {
                        string FindQuery = $"SELECT * FROM reservation WHERE name='{name}';";
                        MySqlCommand cmd = new MySqlCommand(FindQuery, conn);
                        MySqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            string existingReservationTime = reader["time"].ToString();
                            string existingReservationDay = reader["date"].ToString();
                            reader.Close();

                            if (reservationDay != existingReservationDay)
                            {
                                string updateQuery = $"UPDATE reservation SET time='{reservationTime}', date='{reservationDay}' WHERE name = '{name}';";
                                MySqlCommand updateCommand = new MySqlCommand(updateQuery, conn);
                                updateCommand.ExecuteNonQuery(); // 데이터 업데이트
                                MessageBox.Show("예약 변경이 완료 되었습니다.");
                            }
                            else if (reservationTime != existingReservationTime)
                            {
                                string updateQuery = $"UPDATE reservation SET time='{reservationTime}' WHERE name = '{name}';";
                                MySqlCommand updateCommand = new MySqlCommand(updateQuery, conn);
                                updateCommand.ExecuteNonQuery();
                                MessageBox.Show("예약 변경이 완료 되었습니다.");
                            }
                        }
                        else
                        {
                            reader.Close();
                            MessageBox.Show("해당 이름을 가진 예약이 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        conn.Close(); // DB 연결 종료

                        button21.Text = "수정하기";
                        dataGridView4.ReadOnly = true;
                        textBox2.ReadOnly = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        
        private void button4_1_1_Click(object sender, EventArgs e)
        {
            form_man_modify = new Form_man_modify();
            form_man_modify.ShowDialog();
        }


        private void label1_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 재고 이벤트 -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private static bool Isnumber(string input)
        {
            foreach (char c in input)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }
        private void textBox3_1_1_Click(object sender, EventArgs e)             // 코드 입력창
        {
            textBox3_1_1.Text = null;
        }
        private void textBox3_1_1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button3_1_1_Click(sender, e);
            }
        }
        private void textBox3_1_1_TextChanged(object sender, EventArgs e)
        {
            label3_1_3.Text = textBox3_1_1.Text;
            dataGridView3_2_1.Rows.Clear();
            dataGridView3_3_1.Rows.Clear();
            Inventory.search_inv();
        }
        private void textBox3_1_2_Click(object sender, EventArgs e)             // 이름 입력창
        {
            textBox3_1_2.Text = null;
        }
        private void textBox3_1_2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button3_1_2_Click(sender, e);
            }
        }
        private void textBox3_1_2_TextChanged(object sender, EventArgs e)
        {
            label3_1_5.Text = textBox3_1_2.Text;
            dataGridView3_2_1.Rows.Clear();
            dataGridView3_3_1.Rows.Clear();
            Inventory.search_inv();
        }
        private void textBox3_2_1_Click(object sender, EventArgs e)             // 수량 입력창
        {
            textBox3_2_1.Text = null;
        }
        private void textBox3_2_1_TextChanged(object sender, EventArgs e)
        {
            label3_2_2.Text = textBox3_2_1.Text;
        }
        private void textBox3_3_1_Click(object sender, EventArgs e)             // 장바구니 수량 입력창
        {
            textBox3_3_1.Text = null;
        }
        private void textBox3_3_1_TextChanged(object sender, EventArgs e)
        {
            label3_3_2.Text = textBox3_3_1.Text;
        }
        private void button3_1_1_Click(object sender, EventArgs e)          // 제품코드 검색 버튼
        {
            dataGridView3_2_1.Rows.Clear();
            dataGridView3_3_1.Rows.Clear();
            Inventory.search_inv();
            textBox3_1_1.Focus();
        }
        private void button3_1_2_Click(object sender, EventArgs e)          // 제품코드 검색 버튼
        {
            dataGridView3_2_1.Rows.Clear();
            dataGridView3_3_1.Rows.Clear();
            Inventory.search_inv();
            textBox3_1_2.Focus();
            textBox3_2_1.Visible = false;
        }
        private void button3_1_3_Click(object sender, EventArgs e)        // 제품 등록 버튼
        {
            form_inv_add = new Form_inv_add();
            form_inv_add.ShowDialog();
        }
        private void button3_1_3_KeyDown(object sender, KeyEventArgs e)     // 작동 안됨
        {
            if (e.KeyCode == Keys.Tab)
            {
                textBox3_1_1.Focus();
            }
        }
        private void button3_1_3_MouseMove(object sender, MouseEventArgs e)
        {
            button3_1_3.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }

        private void button3_1_3_MouseLeave(object sender, EventArgs e)
        {
            button3_1_3.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button3_2_1_Click(object sender, EventArgs e)        // 사용버튼
        {
            Inventory.use_inv();
            textBox3_2_1.Text = null;
            textBox3_2_1.Focus();
        }
        
        private void button3_2_1_MouseMove(object sender, MouseEventArgs e)
        {
            button3_2_1.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button3_2_1_MouseLeave(object sender, EventArgs e)
        {
            button3_2_1.Font = new Font("G마켓 산스 TTF Light", 16);

        }
        private void button3_2_2_Click(object sender, EventArgs e)          // 삭제 버튼
        {
            Inventory.del_inv();
        }
        private void button3_2_2_KeyDown(object sender, KeyEventArgs e)     
        {
            if (e.KeyCode == Keys.Tab)
            {
                textBox3_2_1.Focus();
            }
        }
        private void button3_2_2_MouseMove(object sender, MouseEventArgs e)
        {
            button3_2_2.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button3_2_2_MouseLeave(object sender, EventArgs e)
        {
            button3_2_2.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private async void button3_3_1_Click(object sender, EventArgs e)          // 주문 마이너스 버튼
        {
            if (textBox3_3_1.Text != "" && Isnumber(textBox3_3_1.Text))
            {
                for (int i = 0; i < dataGridView3_3_1.Rows.Count; i++)
                {
                    if ((Convert.ToBoolean(dataGridView3_3_1.Rows[i].Cells[2].Value) == true))
                    {
                        int x = int.Parse(dataGridView3_3_1.Rows[i].Cells[1].Value.ToString());
                        if (x >= Int32.Parse(textBox3_3_1.Text))
                        {
                            dataGridView3_3_1.Rows[i].Cells[1].Value = (x - Int32.Parse(textBox3_3_1.Text)).ToString();
                        }
                        else
                        {
                            textBox3_3_1.Text = "잘못된 값입니다.";
                            break;
                        }
                    }
                }
            }
            else
            {
                textBox3_3_1.Text = null;
                textBox3_3_1.Text = "잘못된 값입니다.";
            }
            await Task.Delay(1000);                     // 일정시간후 에러메시지 삭제
            textBox3_3_1.Text = null;
            textBox3_3_1.Focus();
        }

        private async void button3_3_2_Click(object sender, EventArgs e)          // 주문 플러스 버튼
        {
            if (textBox3_3_1.Text != "" && Isnumber(textBox3_3_1.Text))
            {
                for (int i = 0; i < dataGridView3_3_1.Rows.Count; i++)
                {
                    if (Convert.ToBoolean(dataGridView3_3_1.Rows[i].Cells[2].Value) == true)
                    {
                        int x = int.Parse(dataGridView3_3_1.Rows[i].Cells[1].Value.ToString());
                        dataGridView3_3_1.Rows[i].Cells[1].Value = (x + Int32.Parse(textBox3_3_1.Text));
                    }
                }
                textBox3_3_1.Text = null;
            }
            else
            {
                textBox3_3_1.Text = "잘못된 값입니다.";
            }
            await Task.Delay(1000);                     // 일정시간후 에러메시지 삭제
            textBox3_3_1.Text = null;
            textBox3_3_1.Focus();
        }
        private void button3_3_3_Click(object sender, EventArgs e)         // 결재버튼
        {
            Inventory.app_inv();
        }
        private void button3_3_3_MouseMove(object sender, MouseEventArgs e)
        {
            button3_3_3.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }

        private void button3_3_3_MouseLeave(object sender, EventArgs e)
        {
            button3_3_3.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void label3_1_3_Click(object sender, EventArgs e)
        {
            textBox3_1_1.Focus();
        }
        private void label3_1_5_Click(object sender, EventArgs e)
        {
            textBox3_1_2.Focus();
        }
        private void label3_2_2_Click(object sender, EventArgs e)
        {
            textBox3_2_1.Focus();
        }
        private void label3_3_2_Click(object sender, EventArgs e)
        {
            textBox3_3_1.Focus();
        }

        private void dataGridView3_2_1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int count = 0;

            if (e.RowIndex >= 0 && e.ColumnIndex == 3 && Convert.ToBoolean(dataGridView3_2_1.Rows[e.RowIndex].Cells[3].Value) == false)
            {
                dataGridView3_2_1.Rows[e.RowIndex].Cells[3].Value = true;       // 체크박스를 true
            }
            else if (e.RowIndex >= 0 && e.ColumnIndex == 3 && Convert.ToBoolean(dataGridView3_2_1.Rows[e.RowIndex].Cells[3].Value) == true)
            {
                dataGridView3_2_1.Rows[e.RowIndex].Cells[3].Value = false;
            }
            for (int i = 0; i < dataGridView3_2_1.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dataGridView3_2_1.Rows[i].Cells[3].Value) == true)        // 선택되어있는 체크박스가 하나라도 있는 경우
                {
                    button3_2_1.Visible = true;
                    button3_2_2.Visible = true;
                    textBox3_2_1.Visible = true;
                    label3_2_2.Visible = true;
                    panel10.Visible = true;
                    textBox3_2_1.Text = "1";          // 기본 1로
                    textBox3_2_1.Focus();
                    count++;
                }
                else if (count == 0)         // 선택되어있는 체크박스가 하나도 없는 경우
                {
                    button3_2_1.Visible = false;
                    button3_2_2.Visible = false;
                    textBox3_2_1.Visible = false;
                    label3_2_2.Visible = false;
                    panel10.Visible = false;
                }
            }
        }
        private bool allselected1 = false;
        private void dataGridView3_2_1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)      // 모두 선택
        {
            if (dataGridView3_2_1.RowCount > 0 && e.ColumnIndex == 3)
            {
                allselected1 = !allselected1;
                for (int i = 0; i < dataGridView3_2_1.RowCount; i++)
                {
                    dataGridView3_2_1.Rows[i].Cells[3].Value = allselected1;
                }
            }
        }
        private void dataGridView3_3_1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int count = 0;
            if (e.RowIndex >= 0 && e.ColumnIndex == 2 && Convert.ToBoolean(dataGridView3_3_1.Rows[e.RowIndex].Cells[2].Value) == false)
            {
                dataGridView3_3_1.Rows[e.RowIndex].Cells[2].Value = true;       // 체크박스를 true
            }
            else if (e.RowIndex >= 0 && e.ColumnIndex == 2 && Convert.ToBoolean(dataGridView3_3_1.Rows[e.RowIndex].Cells[2].Value) == true)
            {
                dataGridView3_3_1.Rows[e.RowIndex].Cells[2].Value = false;
            }
            for (int i = 0; i < dataGridView3_3_1.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dataGridView3_3_1.Rows[i].Cells[2].Value) == true)
                {
                    button3_3_1.Visible = true;
                    button3_3_2.Visible = true;
                    button3_3_3.Visible = true;
                    textBox3_3_1.Visible = true;
                    label3_3_2.Visible = true;
                    panel105.Visible = true;
                    textBox3_3_1.Text = "1";
                    textBox3_3_1.Focus();
                    count++;
                }
                else if (count == 0)         // 선택되어있는 체크박스가 하나도 없는 경우
                {
                    button3_3_1.Visible = false;
                    button3_3_2.Visible = false;
                    button3_3_3.Visible = false;
                    textBox3_3_1.Visible = false;
                    label3_3_2.Visible = false;
                    panel105.Visible = false;
                }
            }
        }
        private bool allselected2 = false;
        private void dataGridView3_3_1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView3_3_1.RowCount > 0 && e.ColumnIndex == 2)
            {
                allselected2 = !allselected2;
                for (int i = 0; i < dataGridView3_3_1.RowCount; i++)
                {
                    dataGridView3_3_1.Rows[i].Cells[2].Value = allselected2;
                }
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------재고 이벤트

        // 관리자 이벤트 -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            label52.Text = textBox3.Text;
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            label57.Text = textBox12.Text;
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            label59.Text = textBox13.Text;
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            label61.Text = textBox14.Text;
        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {
            label62.Text = textBox15.Text;
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            label64.Text = textBox16.Text;
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            label68.Text = textBox17.Text;
        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {
            label69.Text = textBox18.Text;
        }

        private void textBox19_TextChanged(object sender, EventArgs e)
        {
            label70.Text = textBox19.Text;
        }

        private void label52_Click(object sender, EventArgs e)
        {
            textBox3.Focus();
        }

        private void label57_Click(object sender, EventArgs e)
        {
            textBox12.Focus();
        }

        private void label59_Click(object sender, EventArgs e)
        {
            textBox13.Focus();
        }

        private void label61_Click(object sender, EventArgs e)
        {
            textBox14.Focus();
        }

        private void label62_Click(object sender, EventArgs e)
        {
            textBox15.Focus();
        }

        private void label64_Click(object sender, EventArgs e)
        {
            textBox16.Focus();
        }

        private void label68_Click(object sender, EventArgs e)
        {
            textBox17.Focus();
        }

        private void label69_Click(object sender, EventArgs e)
        {
            textBox18.Focus();
        }

        private void label70_Click(object sender, EventArgs e)
        {
            textBox19.Focus();
        }
        private void button4_1_Click(object sender, EventArgs e)        // 신규 버튼
        {
            form_man_add = new Form_man_add();
            form_man_add.ShowDialog();
        }
        private void button4_1_MouseMove(object sender, MouseEventArgs e)
        {
            button4_1.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }


        private void button4_1_MouseLeave(object sender, EventArgs e)
        {
            button4_1.Font = new Font("G마켓 산스 TTF Light", 16);
        }


        private void button4_3_Click(object sender, EventArgs e)        // 주문 버튼
        {
            int count = 0;
            for (int i = 0; i < dataGridView4_6.RowCount; i++)
            {
                if (Convert.ToBoolean(dataGridView4_6.Rows[i].Cells[3].Value))
                {
                    count++;
                }
            }
            if (count != 0)
            {
                Manage.pay_inv();
            }
        }
        private void button4_5_Click(object sender, EventArgs e)
        {
            int count = 0;
            for (int i = 0; i < dataGridView4_6.RowCount; i++)
            {
                if (Convert.ToBoolean(dataGridView4_6.Rows[i].Cells[3].Value))
                {
                    count++;
                }
            }
            if (count != 0)
            {
                Manage.cancel_inv();
            }
        }
        private void button4_6_Click(object sender, EventArgs e)
        {
            if (dataGridView4_1.SelectedRows.Count > 0)
            {
                int rowindex = Form_login.form_main.dataGridView4_1.SelectedRows[0].Index;
                Manage.del_man(rowindex);
            }
        }

        private void button4_6_MouseLeave(object sender, EventArgs e)
        {
            button4_6.Font = new Font("G마켓 산스 TTF Light", 16);
        }

        private void button4_6_MouseMove(object sender, MouseEventArgs e)
        {
            button4_6.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button4_4_Click(object sender, EventArgs e)
        {
            DataGridView Data_app = Form_login.form_main.dataGridView4_5;
            string connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};";
            conn = new MySqlConnection(connectionString);
            string Vac_REQ = $"SELECT * FROM VAC_REQ";
            string Vac_Ok = $"SELECT * FROM VAC_OK";
            string vac_count = $"SELECT * FROM INFO";
            if (make_connection())
            {
                for (int i = Data_app.RowCount - 1; i >= 0; i--)
                {
                    if (Convert.ToBoolean(Data_app.Rows[i].Cells[5].Value))
                    {
                        string name = Data_app.Rows[i].Cells[0].Value.ToString();
                        string birth = Data_app.Rows[i].Cells[1].Value.ToString();
                        string MonthDay = Data_app.Rows[i].Cells[2].Value.ToString();
                        string ETC = Data_app.Rows[i].Cells[3].Value.ToString();
                        string date = Convert.ToDateTime(Data_app.Rows[i].Cells[4].Value).ToString("yyyy-MM-dd");
                        MySqlCommand REQ_cmd = new MySqlCommand(Vac_REQ, conn);
                        MySqlDataReader reader = REQ_cmd.ExecuteReader();
                        Form_login.form_main.dataGridView8.Rows.Clear();
                        while (reader.Read())
                        {
                            for (int j = 0; j < Form_login.form_main.dataGridView6.Rows.Count; j++)
                                if (reader[1] == Form_login.form_main.dataGridView6.Rows[0].Cells[1] && reader[0] == Form_login.form_main.dataGridView6.Rows[0].Cells[0] && reader[4] == Form_login.form_main.dataGridView6.Rows[0].Cells[4])
                                {
                                    MessageBox.Show("이미 결재된 날짜입니다.");
                                }

                            reader.Close();
                        }
                        REQ_cmd.ExecuteNonQuery();  //SQL문 실행
                        string plus = $"INSERT INTO  VAC_OK VALUES('{name}','{birth}','{MonthDay}','{ETC}','{date}');";
                        MySqlCommand plus_cmd = new MySqlCommand(plus, conn);
                        MessageBox.Show("결재되었습니다.");

                        plus_cmd.ExecuteNonQuery();

                        string delete = $"DELETE FROM VAC_REQ WHERE name = '{name}' AND BIRTH = '{birth}' AND Date = '{date}'";
                        MySqlCommand delete_cmd = new MySqlCommand(delete, conn);
                        delete_cmd.ExecuteNonQuery();
                    }
                }
            }
        }
            private void dataGridView4_1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView4_2.Rows.Clear();
            if (e.RowIndex >= 0)
            {
                Manage.p_info(e.RowIndex);
                Manage.p_salary(e.RowIndex);
                Manage.Att_mang(e.RowIndex);
                button4_1_1.Visible = true;
                label52.Text = label4_1_1.Text;
                label57.Text = label4_1_2.Text;
                label59.Text = label4_1_4.Text;
                label61.Text = label4_1_6.Text;
                label64.Text = dataGridView4_1.Rows[e.RowIndex].Cells[1].Value.ToString();
                label68.Text = label4_1_3.Text;
                label4_1_1.Visible = true;
                label4_1_2.Visible = true;
                label4_1_3.Visible = true;
                label4_1_4.Visible = true;
                label4_1_5.Visible = true;
                label4_1_6.Visible = true;
                panel4_1_1.Visible = true;
            }
        }
        

        private void dataGridView4_2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 6 && dataGridView4_2.Rows[e.RowIndex].Cells[6].Value.ToString() == "N")
            {
                dataGridView4_2.Rows[e.RowIndex].Cells[6].Value = "Y";
            }
            else if (e.RowIndex >= 0 && e.ColumnIndex == 6 && dataGridView4_2.Rows[e.RowIndex].Cells[6].Value.ToString() == "Y")
            {
                dataGridView4_2.Rows[e.RowIndex].Cells[6].Value = "N";
            }
        }


        private void button15_Click(object sender, EventArgs e) //월차/반차 반려 버튼
        {
            DataGridView Data_app = Form_login.form_main.dataGridView4_5;
            string connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};";
            conn = new MySqlConnection(connectionString);
            string Vac_REQ = $"SELECT * FROM VAC_REQ";
            for (int i = Data_app.RowCount - 1; i >= 0; i--)
            {
                if (Convert.ToBoolean(Data_app.Rows[i].Cells[5].Value))
                {
                    if (make_connection())
                    {
                        string name = Data_app.Rows[i].Cells[0].Value.ToString();
                        string birth = Data_app.Rows[i].Cells[1].Value.ToString();
                        string date = Convert.ToDateTime(Data_app.Rows[i].Cells[4].Value).ToString("yyyy-MM-dd");
                        MySqlCommand cmd = new MySqlCommand(Vac_REQ, conn);
                        MySqlDataReader reader = cmd.ExecuteReader();
                        Form_login.form_main.dataGridView4_5.Rows.Clear();
                        string delete = $"DELETE FROM VAC_REQ WHERE name = '{name}' AND BIRTH = '{birth}' AND Date = '{date}'";
                        MySqlCommand delete_cmd = new MySqlCommand(delete, conn);
                        MessageBox.Show("반려되었습니다");
                        while (reader.Read())
                        {
                            if (reader[4].ToString() == clickedDateTime.ToString("yyyy-MM-dd"))
                            {
                                Form_login.form_main.dataGridView8.Rows.Add(reader[0], reader[1], reader[2], reader[3], reader[4]);
                            }

                        }
                        reader.Close();
                        delete_cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
            }
        }

        private bool allselected3 = false;
        

        private void dataGridView4_6_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView4_6.RowCount > 0 && e.ColumnIndex == 3)
            {
                allselected3 = !allselected3;
                for (int i = 0; i < dataGridView4_6.RowCount; i++)
                {
                    dataGridView4_6.Rows[i].Cells[3].Value = allselected3;
                }
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ 관리자 이벤트
    }
}



class Patient
{
    private static MySqlConnection conn;
    private static string server = "192.168.31.147";
    private static string database = "team4";
    private static string uid = "root";
    private static string password = "0000";



    public static List<string> Att_cal()
    {
        string schedule = "schedule.txt";
        string[] sch = File.ReadAllLines(schedule);
        List<string> schcel = sch.ToList();
        return schcel;
    }
    public static List<List<string>> Att()
    {
        List<string> AttData = new List<string>();
        List<List<string>> Att_list = new List<List<string>>();
        List<string> linez = Patient.Att_cal();
        for (int i = 0; i < linez.Count; i++)          // 파일 내용을 한 줄씩 읽어가며 처리
        {
            AttData = linez[i].Split('\t').ToList();
            Att_list.Add(AttData); // 각 환자 데이터 리스트를 전체 리스트에 추가
        }
        return Att_list;
    }


    public static void Pnew()
    {

        //text 입력
        string text_name = Form_login.form_main.textBox4.Text;
        string text_ssnum = Form_login.form_main.textBox5.Text;
        string text_phone = Form_login.form_main.textBox6.Text;
        string text_address = Form_login.form_main.textBox7.Text;

        // 하나라도 비어있으면 데이터를 파일에 추가하지 않음
        if (string.IsNullOrEmpty(text_name) || string.IsNullOrEmpty(text_ssnum) || string.IsNullOrEmpty(text_phone) || string.IsNullOrEmpty(text_address))
        {
            Form_login.form_main.label10.Visible = true;
            return;
        }

        // 전화번호 형식 변환
        if (text_phone.Length == 11) // 입력된 전화번호가 11자리인 경우에만 변환
        {
            string formatted_phone = $"{text_phone.Substring(0, 3)}-{text_phone.Substring(3, 4)}-{text_phone.Substring(7)}";
            text_phone = formatted_phone;
        }

        DateTime Pnew_age;

        // 6자리의 연도를 받아와서 yyyy-MM-dd 형태로 변경
        string yearString = text_ssnum.Substring(0, 2);
        int yearPrefix = int.Parse(yearString);
        int currentYear = DateTime.Now.Year % 100;
        string fullYearString = yearPrefix > currentYear ? "19" + yearString : "20" + yearString;

        // 나머지 날짜 정보를 가져와서 날짜로 변경
        string dateString = fullYearString + text_ssnum.Substring(2);
        if (!DateTime.TryParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out Pnew_age))
        {

            Form_login.form_main.label10.Visible = true;
            return;
        }
        string formatted_ssnum = Pnew_age.ToString("yyyy-MM-dd");

        DateTime today = DateTime.Now;

        int age = today.Year - Pnew_age.Year + 1;
        string text_age = age < 10 ? "0" + age.ToString() : age.ToString();



        string connectionString = $"SERVER = {server}; DATABASE = {database}; UID={uid}; PASSWORD={password};";
        conn = new MySqlConnection(connectionString); // MYSQL로 만든 생성자 함수 conn이라 부름.
        string createTableQuery = "CREATE TABLE IF NOT EXISTS Patient (name VARCHAR(5) NOT NULL, age VARCHAR(3) NOT NULL,birth VARCHAR(10) PRIMARY KEY NOT NULL, phone VARCHAR(15) NOT NULL, city VARCHAR(50) NOT NULL);";  // 넣으려는 SQL문

        string Pnew_info = $"INSERT INTO PATIENT VALUES('{text_name}','{text_age}','{formatted_ssnum}','{text_phone}','{text_address}');";


        if (make_connection())
        {
            MySqlCommand cmd = new MySqlCommand(createTableQuery, conn);
            cmd.ExecuteNonQuery();  //SQL문 실행

            MySqlCommand pnew_info = new MySqlCommand(Pnew_info, conn);
            pnew_info.ExecuteNonQuery();

            conn.Close();//conn이라는 DB연결 객체를 해제
        }
    }



    public static void Psearch()
    {
        try
        {

            string connectionString = $"SERVER = {server}; DATABASE = {database}; UID={uid}; PASSWORD={password};";
            conn = new MySqlConnection(connectionString);
            string reservation = "select * from patient ";
            if (Patient.make_connection())
            {
                MySqlCommand cmd = new MySqlCommand(reservation, conn);

                cmd.ExecuteNonQuery();  //SQL문 실행
                                        //   cmd1.ExecuteNonQuery();  //SQL문 실행
                MySqlDataReader reader = cmd.ExecuteReader();
                string readString = "";
                Form_login.form_main.dataGridView2.Rows.Clear();
                while (reader.Read())
                {
                    readString = string.Format("{0},{1},{2},{3},{4}", reader[0], reader[1], reader[2], reader[3], reader[4]);
                    Form_login.form_main.dataGridView2.Rows.Add(reader[0], reader[1], reader[2], reader[3], reader[4]);
                }

                conn.Close();//conn이라는 DB연결 객체를 해제
            }



        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
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







}


class Inventory
{
    private static MySqlConnection conn;
    private static string server = "192.168.31.147";
    private static string database = "team4";
    private static string uid = "root";
    private static string password = "0000";
    private static bool Isnumber(string input)     // 숫자인지 확인
    {
        foreach (char c in input)
        {
            if (!char.IsDigit(c))
            {
                return false;
            }
        }
        return true;
    }
    public static void search_inv()         // 제품 검색
    {
        string Code = Form_login.form_main.textBox3_1_1.Text;           // 코드 검색
        string Name = Form_login.form_main.textBox3_1_2.Text;           // 이름 검색
        DataGridView Data1 = Form_login.form_main.dataGridView3_2_1;    // 수량 조절 데이터그리드
        DataGridView Data2 = Form_login.form_main.dataGridView3_3_1;    // 주문 데이터 그리드
        string connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};";
        conn = new MySqlConnection(connectionString);
        string checkinv = "SELECT * FROM inventory ORDER BY I_code;";
        if (make_connection())
        {
            MySqlCommand cmd = new MySqlCommand(checkinv, conn);
            cmd.ExecuteNonQuery();      // SQL문 실행
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (Code == reader[0].ToString().Substring(0, Code.Length) && Name == reader[1].ToString().Substring(0, Name.Length) && Code != "" && Name != "")
                {
                    Data1.Rows.Add(reader[0], reader[1], reader[2]);
                    Data2.Rows.Add(reader[1], "0");
                }
                else if (Code == reader[0].ToString().Substring(0, Code.Length) && Code != "" && Name == "")
                {
                    Data1.Rows.Add(reader[0], reader[1], reader[2]);
                    Data2.Rows.Add(reader[1], "0");
                }
                else if (Name == reader[1].ToString().Substring(0, Name.Length) && Name != "" && Code == "")
                {
                    Data1.Rows.Add(reader[0], reader[1], reader[2]);
                    Data2.Rows.Add(reader[1], "0");
                }
                else if (Name == "" && Code == "")
                {
                    Data1.Rows.Add(reader[0], reader[1], reader[2]);
                    Data2.Rows.Add(reader[1], "0");
                }
            }
            reader.Close();
        }
        Data1.ClearSelection();
        Data2.ClearSelection();
    }


    public static async void use_inv()         // 사용버튼
    {
        DataGridView Data1 = Form_login.form_main.dataGridView3_2_1;                // 제품 데이터그리드
        DataGridView Data2 = Form_login.form_main.dataGridView3_3_1;
        System.Windows.Forms.TextBox Count = Form_login.form_main.textBox3_2_1;     // 수량입력창
        Label Error = Form_login.form_main.label3_2_2;
        string msg = "";
        if (Count.Text != "")
        {
            for (int i = 0; i < Data1.Rows.Count; i++)     // 체크박스가 선택되어있는 모든 행
            {
                if (Isnumber(Count.Text) && (Convert.ToBoolean(Data1.Rows[i].Cells[3].Value) == true) && Int32.Parse(Count.Text) <= Int32.Parse(Data1.Rows[i].Cells[2].Value.ToString()))         // 수량입력창에 숫자가 오고 체크박스에 선택이 되어있으면
                {
                    string connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};";
                    conn = new MySqlConnection(connectionString);
                    string code = Data1.Rows[i].Cells[0].Value.ToString();
                    string name = Data1.Rows[i].Cells[1].Value.ToString();
                    string use = $"UPDATE inventory SET I_count = I_count - '{Count.Text}' WHERE I_code = '{code}' OR I_name = '{name}';";
                    if (make_connection())
                    {
                        if (Count.Text != "")
                        {
                            MySqlCommand cmd = new MySqlCommand(use, conn);
                            cmd.ExecuteNonQuery();
                            msg += code + " " + name + " " + Count.Text + "개 \n";
                        }
                        conn.Close();               // conn이라는 DB연결 객체를 해제
                    }
                }
                else if (!Isnumber(Count.Text))       // 숫자가 입력되지 않은경우
                {
                    Error.Text = "잘못된 값입니다.";
                }
            }
            if (msg != "")
            {
                MessageBox.Show(msg + "사용했습니다.", "사용");
            }
        }
        else        // 수량입력창이 비어있을경우
        {
            Error.Text = "잘못된 값입니다.";
            await Task.Delay(1000);                     // 일정시간후 에러메시지 삭제
            Error.Text = null;
        }
        Count.Focus();
        Data1.Rows.Clear();
        Data2.Rows.Clear();
        Inventory.search_inv();
    }
    public static void del_inv()                // 삭제
    {
        DataGridView Data1 = Form_login.form_main.dataGridView3_2_1;
        DataGridView Data2 = Form_login.form_main.dataGridView3_3_1;
        string msg = "";
        for (int i = 0; i < Data1.RowCount; i++)
        {
            if (Convert.ToBoolean(Data1.Rows[i].Cells[3].Value))
            {
                msg += Data1.Rows[i].Cells[0].Value + "\t" + Data1.Rows[i].Cells[1].Value + "\t" + Data1.Rows[i].Cells[2].Value + "\n";
            }
        }
        DialogResult result = MessageBox.Show(msg + "삭제하시겠습니까?", "삭제", MessageBoxButtons.YesNo);
        if (result == DialogResult.Yes)
        {
            string connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};";
            conn = new MySqlConnection(connectionString);
            for (int i = 0; i < Data1.RowCount; i++)
            {
                if (Convert.ToBoolean(Data1.Rows[i].Cells[3].Value))
                {
                    string code = Data1.Rows[i].Cells[0].Value.ToString();
                    string delete = $"DELETE FROM inventory WHERE I_code = '{code}';";
                    if (make_connection())
                    {
                        MySqlCommand cmd = new MySqlCommand(delete, conn);
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();               // conn이라는 DB연결 객체를 해제
                    MessageBox.Show("삭제되었습니다.");
                }
            }
            Data1.Rows.Clear();
            Data2.Rows.Clear();
            Inventory.search_inv();
        }
    }
    public async static void app_inv()            // 결재버튼
    {
        string connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};";
        conn = new MySqlConnection(connectionString);
        DataGridView Data2 = Form_login.form_main.dataGridView3_3_1;
        string order = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string msg = "";
        for (int i = 0; i < Data2.RowCount; i++)        // 장바구니에 있는 제품수 만큼 반복
        {
            if (Convert.ToBoolean(Data2.Rows[i].Cells[2].Value) == true && Data2.Rows[i].Cells[1].Value.ToString() != "0")        // 선택되어있고 수량이 0이 아닌 제품만
            {
                msg += Data2.Rows[i].Cells[0].Value + "\t" + Data2.Rows[i].Cells[1].Value + "개\n";
                string name = Data2.Rows[i].Cells[0].Value.ToString();
                string count = Data2.Rows[i].Cells[1].Value.ToString();
                string orderTable = $"INSERT INTO approval VALUES(NULL,'{name}','{count}','{order}');";
                if (make_connection())
                {
                    MySqlCommand cmd = new MySqlCommand(orderTable, conn);
                    cmd.ExecuteNonQuery();      // SQL문 실행
                    conn.Close();               // conn이라는 DB연결 객체를 해제
                }
            }
            else if (Convert.ToBoolean(Data2.Rows[i].Cells[2].Value) == true && Data2.Rows[i].Cells[1].Value.ToString() == "0")
            {
                Form_login.form_main.label3_3_2.Text = "수량이 0인 값이 있습니다.";
                await Task.Delay(1000);                     // 일정시간후 에러메시지 삭제
                Form_login.form_main.label3_3_2.Text = null;
                Form_login.form_main.textBox3_3_1.Focus();
            }
        }
        if (msg != "")
        {
            MessageBox.Show(msg + "주문이 완료되었습니다", "주문");
            Form_login.form_main.button3_3_1.Visible = false;
            Form_login.form_main.button3_3_2.Visible = false;
            Form_login.form_main.button3_3_3.Visible = false;
            Form_login.form_main.textBox3_3_1.Visible = false;
            Form_login.form_main.label3_3_2.Visible = false;
            Form_login.form_main.panel105.Visible = false;
            Form_login.form_main.label3_3_2.Text = null;
            Form_login.form_main.textBox3_3_1.Text = null;
            Form_login.form_main.textBox3_3_1.Focus();
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
}
class Manage
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
    public static void Management()             //직원 메모장 불러오기
    {
        string connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};";
        conn = new MySqlConnection(connectionString);
        string info = "SELECT * FROM info;";
        if (make_connection())
        {
            MySqlCommand cmd = new MySqlCommand(info, conn);
            cmd.ExecuteNonQuery();      // SQL문 실행
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (reader[10].ToString() == "X")
                {
                    Form_login.form_main.dataGridView4_1.Rows.Add(reader[0], reader[12]);
                }
            }
            reader.Close();
            conn.Close();
        }
        Form_login.form_main.dataGridView4_1.ClearSelection();
    }
    public static void p_info(int RowIndex)             // 개인정보 함수
    {
        string connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};";
        conn = new MySqlConnection(connectionString);
        if (make_connection())
        {
            string name = Form_login.form_main.dataGridView4_1.Rows[RowIndex].Cells[0].Value.ToString();
            string select = $"SELECT * FROM info WHERE name = '{name}';";
            MySqlCommand cmd = new MySqlCommand(select, conn);
            cmd.ExecuteNonQuery();
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Form_login.form_main.label4_1_1.Text = reader[0].ToString();
                Form_login.form_main.label4_1_2.Text = reader[1].ToString();
                Form_login.form_main.label4_1_3.Text = reader[2].ToString();
                Form_login.form_main.label4_1_4.Text = reader[3].ToString();
                Form_login.form_main.label4_1_5.Text = reader[6].ToString();
                Form_login.form_main.label4_1_6.Text = reader[7].ToString();
                string debugFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, reader[13].ToString());    // 이미지 경로
                System.Drawing.Image backgroundImage = System.Drawing.Image.FromFile(debugFolderPath);        // 이미지 불러오기
                Form_login.form_main.panel4_1_1.BackgroundImage = backgroundImage;
                Form_login.form_main.panel4_1_1.BackgroundImageLayout = ImageLayout.Stretch;
            }
            reader.Close();
        }
        conn.Close();
    }
    public static void del_man(int RowIndex)                // 직원 삭제
    {
        DataGridView Data1 = Form_login.form_main.dataGridView4_1;
        string msg = "";
        msg += Data1.Rows[RowIndex].Cells[0].Value + "\t" + Data1.Rows[RowIndex].Cells[1].Value + "\n";
        DialogResult result = MessageBox.Show(msg + "삭제하시겠습니까?", "삭제", MessageBoxButtons.YesNo);
        if (result == DialogResult.Yes)
        {
            string connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};";
            conn = new MySqlConnection(connectionString);

            if (Data1.Rows[RowIndex] != null)
            {
                string name = Data1.Rows[RowIndex].Cells[0].Value.ToString();
                string delete = $"DELETE FROM info WHERE name = '{name}';";
                if (make_connection())
                {
                    MySqlCommand cmd = new MySqlCommand(delete, conn);
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
                MessageBox.Show("삭제되었습니다.");
            }
            else
            {
                MessageBox.Show("선택된 행이 없습니다.");
            }
            Data1.Rows.RemoveAt(RowIndex);
        }
    }
    public static void p_salary(int RowIndex)               // 월급내역, 2년전까지 확인가능
    {
        string connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};";
        conn = new MySqlConnection(connectionString);
        string name = Form_login.form_main.dataGridView4_1.Rows[RowIndex].Cells[0].Value.ToString();
        string info = $"SELECT * FROM info WHERE name = '{name}';";
        string sch = $"SELECT * FROM schedule WHERE name = {name}';";
        string salary = $"SELECT info.name,info.bankaddr,info.bankname,schedule.workcount,schedule.worktimecount, schedule.workdate FROM INFO JOIN SCHEDULE ON  schedule.name ='{name}' where schedule.workdate  ORDER BY schedule.workdate ASC LIMIT 1;";
        //string sort_salary = $"SELECT * FROM salary order by workdate asc limit1";
        if (make_connection())
        {
            MySqlCommand cmd = new MySqlCommand(salary, conn);
            cmd.ExecuteNonQuery();
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string month = DateTime.Now.ToString("yyyy-MM");
                Form_login.form_main.dataGridView4_2.Rows.Add(month, reader[3], reader[4], reader[2], reader[1], reader[0], "N");// 전부나옴
            }
        }
        Form_login.form_main.dataGridView4_2.ClearSelection();
    }
    public static void vac_check()
    {
        string connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};";
        conn = new MySqlConnection(connectionString);
        string Vac_REQ = $"SELECT * FROM VAC_REQ";
        if (make_connection())
        {
            MySqlCommand Vac_REQ_cmd = new MySqlCommand(Vac_REQ, conn);
            Vac_REQ_cmd.ExecuteNonQuery();
            MySqlDataReader reader = Vac_REQ_cmd.ExecuteReader();
            while (reader.Read())
            {
                Form_login.form_main.dataGridView4_5.Rows.Add(reader[0], reader[1], reader[2], reader[3], reader[4]);
            }
            reader.Close();
            conn.Close();
        }
    }
    
    public static void m_app_inv()            // 결재 탭
    {
        DataGridView Data_app = Form_login.form_main.dataGridView4_6;
        DataGridView Data_done = Form_login.form_main.dataGridView4_7;
        Data_app.Rows.Clear();
        string connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};";
        conn = new MySqlConnection(connectionString);
        string checkapp = "SELECT * FROM approval;";
        string checkdone = "SELECT * FROM approval_done;";
        if (make_connection())
        {
            MySqlCommand cmd = new MySqlCommand(checkapp, conn);
            MySqlCommand cmd1 = new MySqlCommand(checkdone, conn);
            cmd.ExecuteNonQuery();      // SQL문 실행
            cmd1.ExecuteNonQuery();
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Data_app.Rows.Add(reader[1], reader[2], reader[3]);
            }
            reader.Close();
            MySqlDataReader reader1 = cmd1.ExecuteReader();
            while (reader1.Read())
            {
                Data_done.Rows.Add(reader1[1], reader1[2], reader1[3]);
            }
            reader1.Close();
            conn.Close();
        }
    }
    public static void pay_inv()        // 주문 버튼
    {
        DataGridView Data_app = Form_login.form_main.dataGridView4_6;
        DataGridView Data_done = Form_login.form_main.dataGridView4_7;
        string connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};";
        conn = new MySqlConnection(connectionString);
        string order = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        if (make_connection())
        {
            for (int i = Data_app.RowCount - 1; i >= 0; i--)
            {
                if (Convert.ToBoolean(Data_app.Rows[i].Cells[3].Value))
                {
                    string name = Data_app.Rows[i].Cells[0].Value.ToString();
                    string count = Data_app.Rows[i].Cells[1].Value.ToString();
                    string time = Convert.ToDateTime(Data_app.Rows[i].Cells[2].Value).ToString("yyyy-MM-dd HH:mm:ss");
                    string plus = $"UPDATE inventory SET I_count = I_count + {count} WHERE I_name ='{name}';";
                    //string plus = "UPDATE inventory inv JOIN approval app ON inv.I_name = app.O_name SET inv.I_count = inv.I_count + app.O_count WHERE inv.I_name = app.O_name;";
                    string del_app = $"DELETE FROM approval WHERE O_name = '{name}' AND O_order = '{time}';";
                    string ins_done = $"INSERT INTO approval_done (Number, D_name, D_count, D_check) SELECT NULL, O_name, O_count, '{order}' FROM approval WHERE O_name = '{name}' AND O_order = '{time}';";
                    MySqlCommand cmd = new MySqlCommand(plus, conn);
                    MySqlCommand cmd1 = new MySqlCommand(ins_done, conn);
                    MySqlCommand cmd2 = new MySqlCommand(del_app, conn);
                    cmd.ExecuteNonQuery();      // SQL문 실행
                    cmd1.ExecuteNonQuery();
                    cmd2.ExecuteNonQuery();
                    Data_app.Rows.RemoveAt(i);
                    Data_done.Rows.Add(name, count, order);
                }
            }
        }
        conn.Close();
    }
    public static void cancel_inv()
    {
        DataGridView Data_app = Form_login.form_main.dataGridView4_6;
        string connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};";
        conn = new MySqlConnection(connectionString);
        if (make_connection())
        {
            for (int i = Data_app.RowCount - 1; i >= 0; i--)
            {
                if (Convert.ToBoolean(Data_app.Rows[i].Cells[3].Value))
                {
                    string name = Data_app.Rows[i].Cells[0].Value.ToString();
                    string count = Data_app.Rows[i].Cells[1].Value.ToString();
                    string time = Convert.ToDateTime(Data_app.Rows[i].Cells[2].Value).ToString("yyyy-MM-dd HH:mm:ss");
                    string cancel = $"DELETE FROM approval WHERE O_name = '{name}' AND O_count = '{count}' AND O_order = '{time}';";
                    MySqlCommand cmd = new MySqlCommand(cancel, conn);
                    cmd.ExecuteNonQuery();
                    Data_app.Rows.RemoveAt(i);
                }
            }
            MessageBox.Show("취소되었습니다.");
        }
        conn.Close();
    }
    public static void Att_mang(int RowIndex)
    {
        Form_login.form_main.dataGridView7.Rows.Clear();
        string schedule = "schedule.txt";
        List<string> lines = File.ReadAllLines(schedule).ToList();
        DateTime today = DateTime.Now;
        int date = 0, worktime = 0, workday = 0, name = 0, gowork_check = 0, outwork_check = 0;
        for (int i = 0; i < lines.Count; i++)
        {
            List<string> rows = lines[i].Split('\t').ToList();
            if (i == 0)
            {
                date = rows.IndexOf("근무날짜");
                gowork_check = rows.IndexOf("출근확인");
                outwork_check = rows.IndexOf("퇴근확인");
                workday = rows.IndexOf("근무일수");
                name = rows.IndexOf("이름");
                worktime = rows.IndexOf("근무시간");
            }
            else
            {
                string yearMonth = rows[date].Substring(0, 7); // 문자열의 처음부터 7번째 문자까지를 가져옴
                if (rows[name] == Form_login.form_main.dataGridView4_1.Rows[RowIndex].Cells[0].Value.ToString() && yearMonth == today.ToString("yyyy-MM"))
                {
                    Form_login.form_main.dataGridView7.Rows.Add(rows[date], rows[gowork_check], rows[outwork_check], rows[worktime], rows[workday]);
                }

            }
        }
    }
    public static void vac_check_mang()
    {
        Form_login.form_main.dataGridView8.Rows.Clear();
        string Vac_okay_check = "vac_okay.txt";
        List<string> lines = File.ReadAllLines(Vac_okay_check).ToList();
        int name = 0, check = 0, time = 0, identi = 0, etc = 0;
        for (int i = 0; i < lines.Count; i++)
        {
            List<string> rows = lines[i].Split('\t').ToList();
            if (i == 0)
            {
                time = rows.IndexOf("날짜");
                check = rows.IndexOf("월차/반차");
                name = rows.IndexOf("이름");
                identi = rows.IndexOf("주민번호");
                etc = rows.IndexOf("비고");
            }
            else
            {
                Form_login.form_main.dataGridView8.Rows.Add(rows[name], rows[identi], rows[check], rows[etc], rows[time]);
            }
        }
    }
}