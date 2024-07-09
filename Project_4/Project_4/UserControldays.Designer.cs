namespace Project_4
{
    partial class UserControldays
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbdays = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbdays
            // 
            this.lbdays.Font = new System.Drawing.Font("굴림", 12F);
            this.lbdays.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbdays.Location = new System.Drawing.Point(3, 0);
            this.lbdays.Name = "lbdays";
            this.lbdays.Size = new System.Drawing.Size(42, 30);
            this.lbdays.TabIndex = 1;
            this.lbdays.Text = "00";
            this.lbdays.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbdays.Click += new System.EventHandler(this.lbdays_Click);
            // 
            // UserControldays
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbdays);
            this.Name = "UserControldays";
            this.Size = new System.Drawing.Size(70, 40);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label lbdays;
    }
}
