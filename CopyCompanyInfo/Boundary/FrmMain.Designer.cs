namespace CopyCompanyInfo.Boundary
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpCopyCondition = new System.Windows.Forms.GroupBox();
            this.dtIssueEnd = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dtIssueStart = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.chkPhoneExists = new System.Windows.Forms.CheckBox();
            this.cboDistrict = new System.Windows.Forms.ComboBox();
            this.lblDistrict = new System.Windows.Forms.Label();
            this.cboCity = new System.Windows.Forms.ComboBox();
            this.lblCity = new System.Windows.Forms.Label();
            this.grdSearchRes = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnSave2Db = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.pcbLoading = new System.Windows.Forms.PictureBox();
            this.buildCopyWorker = new System.ComponentModel.BackgroundWorker();
            this.copyInfoWorker = new System.ComponentModel.BackgroundWorker();
            this.grpCopyCondition.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdSearchRes)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pcbLoading)).BeginInit();
            this.SuspendLayout();
            // 
            // grpCopyCondition
            // 
            this.grpCopyCondition.Controls.Add(this.dtIssueEnd);
            this.grpCopyCondition.Controls.Add(this.label2);
            this.grpCopyCondition.Controls.Add(this.dtIssueStart);
            this.grpCopyCondition.Controls.Add(this.label1);
            this.grpCopyCondition.Controls.Add(this.chkPhoneExists);
            this.grpCopyCondition.Controls.Add(this.cboDistrict);
            this.grpCopyCondition.Controls.Add(this.lblDistrict);
            this.grpCopyCondition.Controls.Add(this.cboCity);
            this.grpCopyCondition.Controls.Add(this.lblCity);
            this.grpCopyCondition.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F);
            this.grpCopyCondition.Location = new System.Drawing.Point(14, 12);
            this.grpCopyCondition.Name = "grpCopyCondition";
            this.grpCopyCondition.Size = new System.Drawing.Size(568, 118);
            this.grpCopyCondition.TabIndex = 0;
            this.grpCopyCondition.TabStop = false;
            this.grpCopyCondition.Text = "Điều kiện lọc";
            // 
            // dtIssueEnd
            // 
            this.dtIssueEnd.CustomFormat = "    yyyy  /  MM  / dd";
            this.dtIssueEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtIssueEnd.Location = new System.Drawing.Point(379, 60);
            this.dtIssueEnd.Name = "dtIssueEnd";
            this.dtIssueEnd.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dtIssueEnd.Size = new System.Drawing.Size(183, 22);
            this.dtIssueEnd.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(321, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 16);
            this.label2.TabIndex = 7;
            this.label2.Text = "~";
            // 
            // dtIssueStart
            // 
            this.dtIssueStart.CustomFormat = "    yyyy  /  MM  / dd";
            this.dtIssueStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtIssueStart.Location = new System.Drawing.Point(102, 60);
            this.dtIssueStart.Name = "dtIssueStart";
            this.dtIssueStart.Size = new System.Drawing.Size(183, 22);
            this.dtIssueStart.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 16);
            this.label1.TabIndex = 5;
            this.label1.Text = "Ngày cấp:";
            // 
            // chkPhoneExists
            // 
            this.chkPhoneExists.AutoSize = true;
            this.chkPhoneExists.Location = new System.Drawing.Point(102, 92);
            this.chkPhoneExists.Name = "chkPhoneExists";
            this.chkPhoneExists.Size = new System.Drawing.Size(135, 20);
            this.chkPhoneExists.TabIndex = 4;
            this.chkPhoneExists.Text = "Lọc khi có số đtdd";
            this.chkPhoneExists.UseVisualStyleBackColor = true;
            // 
            // cboDistrict
            // 
            this.cboDistrict.DisplayMember = "AreaName";
            this.cboDistrict.FormattingEnabled = true;
            this.cboDistrict.Location = new System.Drawing.Point(379, 21);
            this.cboDistrict.Name = "cboDistrict";
            this.cboDistrict.Size = new System.Drawing.Size(183, 24);
            this.cboDistrict.TabIndex = 3;
            this.cboDistrict.ValueMember = "AreaId";
            // 
            // lblDistrict
            // 
            this.lblDistrict.AutoSize = true;
            this.lblDistrict.Location = new System.Drawing.Point(291, 24);
            this.lblDistrict.Name = "lblDistrict";
            this.lblDistrict.Size = new System.Drawing.Size(82, 16);
            this.lblDistrict.TabIndex = 2;
            this.lblDistrict.Text = "Quận huyện:";
            // 
            // cboCity
            // 
            this.cboCity.DisplayMember = "AreaName";
            this.cboCity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCity.FormattingEnabled = true;
            this.cboCity.Location = new System.Drawing.Point(102, 21);
            this.cboCity.Name = "cboCity";
            this.cboCity.Size = new System.Drawing.Size(183, 24);
            this.cboCity.TabIndex = 1;
            this.cboCity.ValueMember = "AreaId";
            this.cboCity.SelectedIndexChanged += new System.EventHandler(this.cboCity_SelectedIndexChanged);
            // 
            // lblCity
            // 
            this.lblCity.AutoSize = true;
            this.lblCity.Location = new System.Drawing.Point(17, 24);
            this.lblCity.Name = "lblCity";
            this.lblCity.Size = new System.Drawing.Size(72, 16);
            this.lblCity.TabIndex = 0;
            this.lblCity.Text = "Tỉnh thành:";
            // 
            // grdSearchRes
            // 
            this.grdSearchRes.BackgroundColor = System.Drawing.SystemColors.Window;
            this.grdSearchRes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdSearchRes.Location = new System.Drawing.Point(9, 136);
            this.grdSearchRes.Name = "grdSearchRes";
            this.grdSearchRes.Size = new System.Drawing.Size(828, 392);
            this.grdSearchRes.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnExport);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.btnSave2Db);
            this.groupBox1.Controls.Add(this.btnCopy);
            this.groupBox1.Location = new System.Drawing.Point(588, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(249, 118);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Thao tác";
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnExport.Location = new System.Drawing.Point(12, 70);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(110, 40);
            this.btnExport.TabIndex = 15;
            this.btnExport.Text = "Xuất file";
            this.btnExport.UseVisualStyleBackColor = false;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnSearch.Location = new System.Drawing.Point(12, 21);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(110, 40);
            this.btnSearch.TabIndex = 13;
            this.btnSearch.Text = "Tìm kiếm";
            this.btnSearch.UseVisualStyleBackColor = false;
            // 
            // btnSave2Db
            // 
            this.btnSave2Db.BackColor = System.Drawing.Color.PeachPuff;
            this.btnSave2Db.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnSave2Db.Location = new System.Drawing.Point(128, 70);
            this.btnSave2Db.Name = "btnSave2Db";
            this.btnSave2Db.Size = new System.Drawing.Size(110, 40);
            this.btnSave2Db.TabIndex = 14;
            this.btnSave2Db.Text = "Lưu tin lọc";
            this.btnSave2Db.UseVisualStyleBackColor = false;
            // 
            // btnCopy
            // 
            this.btnCopy.BackColor = System.Drawing.Color.PeachPuff;
            this.btnCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnCopy.Location = new System.Drawing.Point(128, 21);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(110, 40);
            this.btnCopy.TabIndex = 9;
            this.btnCopy.Text = "Bắt đầu lọc";
            this.btnCopy.UseVisualStyleBackColor = false;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // pcbLoading
            // 
            this.pcbLoading.BackColor = System.Drawing.SystemColors.Window;
            this.pcbLoading.Image = global::CopyCompanyInfo.CopyInfo.Loading;
            this.pcbLoading.Location = new System.Drawing.Point(338, 224);
            this.pcbLoading.Name = "pcbLoading";
            this.pcbLoading.Size = new System.Drawing.Size(123, 99);
            this.pcbLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pcbLoading.TabIndex = 3;
            this.pcbLoading.TabStop = false;
            this.pcbLoading.Visible = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(842, 536);
            this.Controls.Add(this.pcbLoading);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grdSearchRes);
            this.Controls.Add(this.grpCopyCondition);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quản lý thông tin công ty";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.grpCopyCondition.ResumeLayout(false);
            this.grpCopyCondition.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdSearchRes)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pcbLoading)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpCopyCondition;
        private System.Windows.Forms.Label lblCity;
        private System.Windows.Forms.ComboBox cboCity;
        private System.Windows.Forms.ComboBox cboDistrict;
        private System.Windows.Forms.Label lblDistrict;
        private System.Windows.Forms.CheckBox chkPhoneExists;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtIssueStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtIssueEnd;
        private System.Windows.Forms.DataGridView grdSearchRes;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnSave2Db;
        private System.Windows.Forms.Button btnExport;
        private System.ComponentModel.BackgroundWorker buildCopyWorker;
        private System.Windows.Forms.PictureBox pcbLoading;
        private System.ComponentModel.BackgroundWorker copyInfoWorker;
    }
}

