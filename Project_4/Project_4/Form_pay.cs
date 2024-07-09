using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Project_4
{
    public partial class Form_pay : Form
    {
        private MySqlConnection conn;
        private string server = "192.168.31.147"; // 어떤 서버에
        private string database = "team4"; // 어떤 DB에 -> team4 스키마  // 내거 market_db
                                           //member table 생성 
                                           //mem_id 열 생성 CHAR(5) 설정

        private string uid = "root"; // 어떤 권한으로
        private string password = "0000";// 비밀번호  // 내 비밀번호는 root0000 // 강사님 0000

        public Form_pay()
        {
            InitializeComponent();

            string connectionString = $"SERVER = {server}; DATABASE = {database}; UID={uid}; PASSWORD={password};";
            conn = new MySqlConnection(connectionString); // MYSQL로 만든 생성자 함수 conn이라 부름.

            if (make_connection())
            {
                string medical = $"SELECT * FROM medical";

                MySqlCommand cmd = new MySqlCommand(medical, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                dataGridView1.Rows.Clear();

                // 체크박스 열 추가
                DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();
                checkBoxColumn.HeaderText = "여부";
                dataGridView1.Columns.Add(checkBoxColumn);

                while (reader.Read())
                {
                    dataGridView1.Rows.Add(reader[0], reader[1], reader[4], reader[5]);
                }

                dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
                conn.Close(); // 연결 닫기
            }
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (make_connection())
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

                    // 체크박스 셀인지 확인
                    if (cell is DataGridViewCheckBoxCell)
                    {
                        bool isChecked = (bool)cell.Value;
                        string birth = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString(); // 이름이 저장된 열의 인덱스에 따라 수정
                        string pay = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();

                        string paydata = $"UPDATE medical SET ok = {Convert.ToInt32(isChecked)} WHERE birth = '{birth}' and pay = '{pay}'";
                        MySqlCommand cmd = new MySqlCommand(paydata, conn);
                        cmd.ExecuteNonQuery();
                    }
                }
                conn.Close(); // 연결 닫기
            }
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
                return false;
            }
        }

        private void Form_pay_Load(object sender, EventArgs e)
        {

        }
    }
}


