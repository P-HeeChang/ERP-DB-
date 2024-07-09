using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_4
{
    public partial class UserControldays : UserControl
    {
        int clickcount = 0;
        static List<UserControldays> redPanels = new List<UserControldays>();
        public string DayLabelText
        {
            get { return lbdays.Text; }
        }
        public static Form_main form_main;
        public UserControldays()
        {
            InitializeComponent();
        }
        private void UserControlDays_Load(object sender, EventArgs e)
        {

        }
        public void days(int numday)
        {
            lbdays.Text = numday + "";
            if (form_main != null)
            {
                form_main.SetDayLabel(lbdays.Text);
            }
        }

       
       

        private void lbdays_Click(object sender, EventArgs e)
        {
            clickcount++;

            // 현재 패널이 빨간색으로 표시된 상태인지 확인
            bool isRed = ForeColor == Color.Red;

            // 패널의 색상 변경
            ForeColor = isRed ? Color.Black : Color.Red;

            // 현재 패널이 빨간색으로 표시되었으면 리스트에 추가, 그렇지 않으면 리스트에서 제거
            if (isRed)
            {
                redPanels.Remove(this);
            }
            else
            {
                redPanels.Add(this);
            }

            // 이전에 빨간색으로 표시된 모든 패널의 색상을 검은색으로 변경
            foreach (var panel in redPanels)
            {
                if (panel != this)
                {
                    panel.ForeColor = Color.Black;
                }
            }

            OnPanelClick.Invoke(this, EventArgs.Empty);
        }

        public EventHandler OnPanelClick;

        
    }

}
