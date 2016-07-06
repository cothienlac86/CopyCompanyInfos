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
            this.cboEnable = new System.Windows.Forms.CheckBox();
            this.dtIssueEnd = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dtIssueStart = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.cboDistrict = new System.Windows.Forms.ComboBox();
            this.lblDistrict = new System.Windows.Forms.Label();
            this.cboCity = new System.Windows.Forms.ComboBox();
            this.lblCity = new System.Windows.Forms.Label();
            this.grdSearchRes = new System.Windows.Forms.DataGridView();
            this.grbActions = new System.Windows.Forms.GroupBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.buildCopyWorker = new System.ComponentModel.BackgroundWorker();
            this.copyInfoWorker = new System.ComponentModel.BackgroundWorker();
            this.lblLoading = new System.Windows.Forms.Label();
            this.pcbLoading = new System.Windows.Forms.PictureBox();
            this.pnlLoading = new System.Windows.Forms.Panel();
            this.exportWorker = new System.ComponentModel.BackgroundWorker();
            this.exportFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.grpCopyCondition.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdSearchRes)).BeginInit();
            this.grbActions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pcbLoading)).BeginInit();
            this.pnlLoading.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpCopyCondition
            // 
            this.grpCopyCondition.Controls.Add(this.cboEnable);
            this.grpCopyCondition.Controls.Add(this.dtIssueEnd);
            this.grpCopyCondition.Controls.Add(this.label2);
            this.grpCopyCondition.Controls.Add(this.dtIssueStart);
            this.grpCopyCondition.Controls.Add(this.label1);
            this.grpCopyCondition.Controls.Add(this.cboDistrict);
            this.grpCopyCondition.Controls.Add(this.lblDistrict);
            this.grpCopyCondition.Controls.Add(this.cboCity);
            this.grpCopyCondition.Controls.Add(this.lblCity);
            this.grpCopyCondition.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F);
            this.grpCopyCondition.Location = new System.Drawing.Point(14, 12);
            this.grpCopyCondition.Name = "grpCopyCondition";
            this.grpCopyCondition.Size = new System.Drawing.Size(592, 118);
            this.grpCopyCondition.TabIndex = 0;
            this.grpCopyCondition.TabStop = false;
            this.grpCopyCondition.Text = "Điều kiện lọc";
            // 
            // cboEnable
            // 
            this.cboEnable.AutoSize = true;
            this.cboEnable.Location = new System.Drawing.Point(102, 92);
            this.cboEnable.Name = "cboEnable";
            this.cboEnable.Size = new System.Drawing.Size(145, 20);
            this.cboEnable.TabIndex = 9;
            this.cboEnable.Text = "Enable Issued Date";
            this.cboEnable.UseVisualStyleBackColor = true;
            this.cboEnable.CheckedChanged += new System.EventHandler(this.cboEnable_CheckedChanged);
            // 
            // dtIssueEnd
            // 
            this.dtIssueEnd.CustomFormat = "    yyyy  /  MM  / dd";
            this.dtIssueEnd.Enabled = false;
            this.dtIssueEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtIssueEnd.Location = new System.Drawing.Point(315, 60);
            this.dtIssueEnd.Name = "dtIssueEnd";
            this.dtIssueEnd.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dtIssueEnd.Size = new System.Drawing.Size(183, 22);
            this.dtIssueEnd.TabIndex = 8;
            this.dtIssueEnd.Value = new System.DateTime(2016, 6, 7, 0, 0, 0, 0);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(291, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 16);
            this.label2.TabIndex = 7;
            this.label2.Text = "~";
            // 
            // dtIssueStart
            // 
            this.dtIssueStart.AllowDrop = true;
            this.dtIssueStart.CustomFormat = "    yyyy  /  MM  / dd";
            this.dtIssueStart.Enabled = false;
            this.dtIssueStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtIssueStart.Location = new System.Drawing.Point(102, 60);
            this.dtIssueStart.Name = "dtIssueStart";
            this.dtIssueStart.Size = new System.Drawing.Size(183, 22);
            this.dtIssueStart.TabIndex = 6;
            this.dtIssueStart.Value = new System.DateTime(2016, 5, 7, 0, 0, 0, 0);
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
            // cboDistrict
            // 
            this.cboDistrict.DisplayMember = "AreaName";
            this.cboDistrict.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
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
            this.grdSearchRes.Size = new System.Drawing.Size(852, 392);
            this.grdSearchRes.TabIndex = 1;
            // 
            // grbActions
            // 
            this.grbActions.Controls.Add(this.btnExport);
            this.grbActions.Controls.Add(this.btnSearch);
            this.grbActions.Controls.Add(this.btnCopy);
            this.grbActions.Location = new System.Drawing.Point(612, 12);
            this.grbActions.Name = "grbActions";
            this.grbActions.Size = new System.Drawing.Size(249, 118);
            this.grbActions.TabIndex = 2;
            this.grbActions.TabStop = false;
            this.grbActions.Text = "Thao tác";
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnExport.Location = new System.Drawing.Point(6, 65);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(110, 40);
            this.btnExport.TabIndex = 15;
            this.btnExport.Text = "Xuất file";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnSearch.Location = new System.Drawing.Point(6, 21);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(110, 40);
            this.btnSearch.TabIndex = 13;
            this.btnSearch.Text = "Tìm kiếm";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.BackColor = System.Drawing.Color.PeachPuff;
            this.btnCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnCopy.Location = new System.Drawing.Point(122, 19);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(121, 86);
            this.btnCopy.TabIndex = 9;
            this.btnCopy.Text = "Bắt đầu lọc";
            this.btnCopy.UseVisualStyleBackColor = false;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // lblLoading
            // 
            this.lblLoading.AutoSize = true;
            this.lblLoading.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblLoading.ForeColor = System.Drawing.Color.Crimson;
            this.lblLoading.Location = new System.Drawing.Point(41, 101);
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(0, 16);
            this.lblLoading.TabIndex = 0;
            this.lblLoading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pcbLoading
            // 
            this.pcbLoading.BackColor = System.Drawing.SystemColors.Window;
            this.pcbLoading.Image = global::CopyCompanyInfo.CopyInfo.Loading;
            this.pcbLoading.Location = new System.Drawing.Point(128, 15);
            this.pcbLoading.Name = "pcbLoading";
            this.pcbLoading.Size = new System.Drawing.Size(74, 74);
            this.pcbLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pcbLoading.TabIndex = 3;
            this.pcbLoading.TabStop = false;
            this.pcbLoading.Visible = false;
            // 
            // pnlLoading
            // 
            this.pnlLoading.BackColor = System.Drawing.SystemColors.Window;
            this.pnlLoading.Controls.Add(this.pcbLoading);
            this.pnlLoading.Controls.Add(this.lblLoading);
            this.pnlLoading.Location = new System.Drawing.Point(255, 156);
            this.pnlLoading.Name = "pnlLoading";
            this.pnlLoading.Size = new System.Drawing.Size(321, 131);
            this.pnlLoading.TabIndex = 4;
            this.pnlLoading.Visible = false;
            // 
            // exportFileDialog
            // 
            this.exportFileDialog.DefaultExt = "xslx";
            this.exportFileDialog.Filter = "Excel Workbook | *.xlsx | Excel 97-2003 Workbook| *.xls";
            this.exportFileDialog.OverwritePrompt = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 536);
            this.Controls.Add(this.pnlLoading);
            this.Controls.Add(this.grbActions);
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
            this.grbActions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pcbLoading)).EndInit();
            this.pnlLoading.ResumeLayout(false);
            this.pnlLoading.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpCopyCondition;
        private System.Windows.Forms.Label lblCity;
        private System.Windows.Forms.ComboBox cboCity;
        private System.Windows.Forms.ComboBox cboDistrict;
        private System.Windows.Forms.Label lblDistrict;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtIssueStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtIssueEnd;
        private System.Windows.Forms.DataGridView grdSearchRes;
        private System.Windows.Forms.GroupBox grbActions;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnExport;
        private System.ComponentModel.BackgroundWorker buildCopyWorker;
        private System.ComponentModel.BackgroundWorker copyInfoWorker;
        private System.Windows.Forms.Label lblLoading;
        private System.Windows.Forms.PictureBox pcbLoading;
        private System.Windows.Forms.Panel pnlLoading;
        private System.ComponentModel.BackgroundWorker exportWorker;
        private System.Windows.Forms.SaveFileDialog exportFileDialog;
        private System.Windows.Forms.CheckBox cboEnable;
    }
}

