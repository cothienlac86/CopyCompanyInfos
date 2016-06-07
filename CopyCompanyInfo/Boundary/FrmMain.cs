﻿using CopyCompanyInfo.Common;
using CopyCompanyInfo.DataAccess;
using HtmlAgilityPack;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace CopyCompanyInfo.Boundary
{
    public partial class frmMain : Form
    {
        #region Private Fields

        private static readonly log4net.ILog CopyLogger =
                    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().Name);

        private static CompanyModel LastCompany = null;
        private DataTable dtCity = new DataTable();
        private DataTable dtDistrict = new DataTable();
        private DataTable dtSearchRes = new DataTable();

        #endregion Private Fields

        #region Public Constructors

        public frmMain()
        {
            InitializeComponent();
            // Build Data Worker
            buildCopyWorker.WorkerReportsProgress = true;
            buildCopyWorker.WorkerSupportsCancellation = true;
            buildCopyWorker.DoWork += new DoWorkEventHandler(buildCopyWorker_DoWork);
            buildCopyWorker.ProgressChanged += new ProgressChangedEventHandler(buildCopyWorker_ProgressChanged);
            buildCopyWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(buildCopyWorker_RunWorkerCompleted);
            // Copy Worker
            copyInfoWorker.WorkerReportsProgress = true;
            copyInfoWorker.WorkerSupportsCancellation = true;
            copyInfoWorker.DoWork += new DoWorkEventHandler(copyInfoWorker_DoWork);
            copyInfoWorker.ProgressChanged += new ProgressChangedEventHandler(copyInfoWorker_ProgressChanged);
            copyInfoWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(copyInfoWorker_RunWorkerCompleted);
            // Export 2 excel Worker
            exportWorker.WorkerReportsProgress = true;
            exportWorker.WorkerSupportsCancellation = true;
            exportWorker.DoWork += new DoWorkEventHandler(exportWorker_DoWork);
            exportWorker.ProgressChanged += new ProgressChangedEventHandler(exportWorker_ProgressChanged);
            exportWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(exportWorker_RunWorkerCompleted);
            if (LastCompany == null) LastCompany = GetLastItem();
        }

        #endregion Public Constructors

        #region Protected Methods

        protected void buildCopyWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                List<AreaModel> lstCities = new List<AreaModel>();
                if (!buildCopyWorker.CancellationPending)
                {
                    var url = e.Argument as string;
                    if (!string.IsNullOrEmpty(url))
                    {
                        var cities = GetCitiesUrl(url);
                        if (cities.Count > 0)
                        {
                            lstCities.AddRange(cities);
                            foreach (var item in cities)
                            {
                                GetCitiesUrl(item.AreaUrl, item.AreaId);
                            }
                        }
                    }
                }
                e.Result = "Đã hoàn thành lấy dữ liệu các tỉnh thành.";
            }
            catch (Exception ex)
            {
                CopyLogger.Error(string.Format("Trace Error:{0} \n Error Message:{1}",
                    ex, ex.Message));
            }
        }

        protected void buildCopyWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                var percent = e.ProgressPercentage;
                lblLoading.Text = string.Format("Đang tải dữ liệu tỉnh thành/quận huyện : {0}% ...", percent);
            }
            catch (Exception ex)
            {
                CopyLogger.Error(string.Format("Trace Error:{0} \n Error Message:{1}",
                    ex, ex.Message));
            }
        }

        protected void buildCopyWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                pnlLoading.Visible = false;
                pcbLoading.Visible = false;
                grbActions.Enabled = true;
                var result = e.Result as string;
                if (!string.IsNullOrEmpty(result))
                {
                    var ds = new AreaModel().GetListArea(0) as DataSet;
                    if (ds != null) dtCity = ds.Tables[0];
                    cboCity.DataSource = dtCity;
                    MessageBox.Show(result);
                    buildCopyWorker.Dispose();
                }
            }
            catch (Exception ex)
            {
                CopyLogger.Error(string.Format("Trace Error:{0} \n Error Message:{1}",
                    ex.ToString(), ex.Message));
            }
        }

        protected void copyInfoWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var lstUrl = e.Argument as DataTable;
                var lstCompany = new List<CompanyModel>();
                if (!copyInfoWorker.CancellationPending)
                {
                    if (lstUrl != null)
                    {
                        for (int i = 0; i < lstUrl.Rows.Count; i++)
                        {
                            Thread.Sleep(250);
                            var copyPercent = (i * lstUrl.Rows.Count - 1) / 100;
                            copyInfoWorker.ReportProgress(copyPercent);
                            var url = lstUrl.Rows[i]["AreaUrl"].ToString();
                            int cityId = int.Parse(lstUrl.Rows[i]["ParentId"].ToString());
                            int districtId = int.Parse(lstUrl.Rows[i]["AreaId"].ToString());
                            if (i == lstUrl.Rows.Count - 1)
                            {
                                lstCompany = GetCompanyContent(url, cityId, districtId, true);
                            }
                            lstCompany = GetCompanyContent(url, cityId, districtId);
                        }
                    }
                }
                e.Result = lstCompany as List<CompanyModel>;
            }
            catch (Exception ex)
            {
                CopyLogger.Error(string.Format("Trace Error:{0} \n Error Message:{1}",
                    ex.ToString(), ex.Message));
            }
        }

        protected void copyInfoWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                var percent = e.ProgressPercentage;
                var message = string.Format("Đang lọc dữ liệu: {0}% hoàn thành...", percent);
                lblLoading.Text = message;
            }
            catch (Exception ex)
            {
                CopyLogger.Error(string.Format("Trace Error:{0} \n Error Message:{1}",
                    ex.ToString(), ex.Message));
            }
        }

        protected void copyInfoWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                grbActions.Enabled = true;
                pcbLoading.Visible = false;
                pnlLoading.Visible = false;
                dtSearchRes.Clear();
                ExecuteSearch();
                var lstModel = e.Result as List<CompanyModel>;
                var dtTable = new DataTable();
                foreach (var item in lstModel)
                {
                    var row = dtTable.NewRow();
                    row["CompanyId"] = item.CityId;
                    row["CompanyName"] = item.CompanyName;
                    row["CompanyAddress"] = item.CompanyAddress;
                    row["RepresentName"] = item.RepresentName;
                    row["RepresentPhone"] = item.RepresentPhone;
                    row["IssuedDate"] = item.IssuedDate;
                    row["ActivitiesDate"] = item.ActivitiesDate;
                    row["CityId"] = item.CityId;
                    row["DistrictId"] = item.DistrictId;
                    dtTable.Rows.Add(row);
                }
                lblLoading.Text = "Đã hoàn thành lọc dữ liệu...";
                MessageBox.Show("Dữ liệu đã được lấy về. Ấn 'OK' để export dữ liệu..!");
                exportFileDialog.FileName = "CompanyInformation_" + DateTime.Now.ToShortDateString();
                if (dtSearchRes.Rows.Count > 0)
                {

                    exportFileDialog.Title = "Chọn nơi lưu trữ file";

                    if (exportFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (!exportWorker.IsBusy)
                        {
                            var fileName = exportFileDialog.FileName;
                            var newFile = new FileInfo(fileName);
                            if (newFile.Exists)
                            {
                                newFile.Delete();  // ensures we create a new workbook
                                newFile = new FileInfo(fileName);
                            }
                            object[] objParams = new object[2];
                            objParams[0] = dtTable as DataTable;
                            objParams[1] = newFile as FileInfo;
                            exportWorker.RunWorkerAsync(objParams);
                            copyInfoWorker.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CopyLogger.Error(string.Format("Trace Error:{0} \n Error Message:{1}",
                    ex.ToString(), ex.Message));
            }
        }

        protected void exportWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                string result = string.Empty;
                var lstParams = e.Argument as object[];
                if (lstParams != null)
                {
                    var dt = lstParams[0] as DataTable;
                    var fileName = lstParams[1] as FileInfo;
                    if (!exportWorker.CancellationPending)
                    {
                        ExportListCompany2File(dt, fileName);
                    }
                }
                e.Result = "Hoàn thành export dữ liệu.";
            }
            catch (Exception ex)
            {
                CopyLogger.Error(string.Format("Trace Error:{0} \n Error Message:{1}",
                    ex, ex.Message));
            }
        }

        protected void exportWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                var percent = e.ProgressPercentage;
                var message = string.Format("Đang export dữ liệu :{0}% hoàn thành...", percent);
                lblLoading.Text = message;
            }
            catch (Exception ex)
            {
                CopyLogger.Error(string.Format("Trace Error:{0} \n Error Message:{1}",
                    ex, ex.Message));
            }
        }

        protected void exportWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                pnlLoading.Visible = false;
                pcbLoading.Visible = false;
                grdSearchRes.Enabled = true;
                grbActions.Enabled = true;
                grpCopyCondition.Enabled = true;
                lblLoading.Text = string.Empty;

                DialogResult dialog = MessageBox.Show("Xuất dữ liệu ra excel đã hoàn thành.\n Bấm 'OK' để đóng thông báo?",
                                        "Xác nhận yêu cầu", MessageBoxButtons.OKCancel);
            }
            catch (Exception ex)
            {
                CopyLogger.Error(string.Format("Trace Error:{0} \n Error Message:{1}",
                    ex.ToString(), ex.Message));
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (cboCity.SelectedIndex == -1)
            {
                MessageBox.Show("Xin hãy lựa chọn tỉnh thành để lọc tin !");
                return;
            }
            string query = "SELECT * FROM tblArea";
            var parentId = cboCity.SelectedValue;
            query += " WHERE ParentId = " + parentId;
            if (cboDistrict.SelectedIndex != -1)
                query += " AND AreaId = " + cboDistrict.SelectedValue;
            var dt = new DataTable();
            var ds = DbHelper.ExecuteQuery(query);
            if (ds != null)
                dt = ds.Tables[0];
            if (!copyInfoWorker.IsBusy)
            {
                if (dt.Rows.Count > 0)
                {
                    grbActions.Enabled = false;
                    pcbLoading.Visible = true;
                    pnlLoading.Visible = true;
                    copyInfoWorker.RunWorkerAsync(dt);
                }
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            //exportFileDialog.FileName = "CompanyInformation_" + DateTime.Now.ToShortDateString();
            //if (exportFileDialog.ShowDialog() == DialogResult.OK)
            //{
            //}
            if (!exportWorker.IsBusy)
            {
                grdSearchRes.Enabled = false;
                pnlLoading.Visible = true;
                pcbLoading.Visible = true;
                grbActions.Enabled = false;
                grpCopyCondition.Enabled = false;
                lblLoading.Text = string.Empty;
                exportFileDialog.FileName = "CompanyInformation_" + DateTime.Now.ToShortDateString();
                if (exportFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (!exportWorker.CancellationPending)
                    {
                        var fileName = exportFileDialog.FileName;
                        var newFile = new FileInfo(fileName);
                        if (newFile.Exists)
                        {
                            newFile.Delete();  // ensures we create a new workbook
                            newFile = new FileInfo(fileName);
                        }
                        object[] objParams = new object[2];
                        objParams[0] = dtSearchRes;
                        objParams[1] = newFile;
                        exportWorker.RunWorkerAsync(objParams);
                    }
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ExecuteSearch();
            }
            catch (Exception ex)
            {
                CopyLogger.Error(string.Format("Trace Error:{0} \n Error Message:{1}",
                     ex.ToString(), ex.Message));
            }
        }

        private void ExecuteSearch()
        {
            string query = " SELECT * FROM tblCompanyInfo ";
            if (cboCity.SelectedIndex == -1)
            {
                MessageBox.Show("Xin hãy chọn lựa chọn tỉnh thành để tìm kiếm thông tin !",
                                    "Thông báo", MessageBoxButtons.OK);
                cboCity.Focus();
                return;
            }
            query += string.Format(" WHERE CityId = {0}", cboCity.SelectedValue);
            if (cboDistrict.SelectedIndex != -1)
            {
                query += string.Format(" AND DistrictId = {0}", cboDistrict.SelectedValue);
            }
            if (dtIssueEnd.Enabled && dtIssueStart.Enabled)
            {
                query += string.Format(" AND IssuedDate <= '{0}' AND IssuedDate >= '{1}'", dtIssueEnd.Value, dtIssueStart.Value);
            }
            query += " AND RepresentPhone != '' ";
            dtSearchRes.Clear();
            var ds = DbHelper.ExecuteQuery(query);
            if (ds != null)
                dtSearchRes = ds.Tables[0];
            grdSearchRes.DataSource = dtSearchRes;
        }

        private void cboCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = cboCity.SelectedIndex;
            if (idx == -1) return;
            int parentId = int.Parse(cboCity.SelectedValue.ToString());
            var ds = new AreaModel().GetListArea(parentId);
            if (ds != null)
                dtDistrict = ds.Tables[0];
            cboDistrict.DataSource = dtDistrict;
            cboDistrict.SelectedIndex = -1;
        }

        private void cboEnable_CheckedChanged(object sender, EventArgs e)
        {
            dtIssueStart.Enabled = true;
            dtIssueEnd.Enabled = true;
        }

        private void ExportListCompany2File(List<CompanyModel> lstModel, FileInfo filePath)
        {
            using (ExcelPackage package = new ExcelPackage(filePath))
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Company Informations");
                //Add the headers
                worksheet.Cells[2, 1].Value = "ID";
                worksheet.Cells[2, 2].Value = "CompanyName";
                worksheet.Cells[2, 3].Value = "CompanyAddress";
                worksheet.Cells[2, 4].Value = "RepresentName";
                worksheet.Cells[2, 5].Value = "RepresentPhone";
                worksheet.Cells[2, 6].Value = "IssuedDate";
                worksheet.Cells[2, 7].Value = "ActivitiesDate";
                int rowId = 3;
                for (int i = 0; i < lstModel.Count; i++)
                {
                    worksheet.Cells[rowId, 1].Value = lstModel[i].Id;
                    worksheet.Cells[rowId, 2].Value = lstModel[i].CompanyName;
                    worksheet.Cells[rowId, 3].Value = lstModel[i].CompanyAddress;
                    worksheet.Cells[rowId, 4].Value = lstModel[i].RepresentName;
                    worksheet.Cells[rowId, 5].Value = lstModel[i].RepresentPhone;
                    worksheet.Cells[rowId, 6].Value = lstModel[i].IssuedDate;
                    worksheet.Cells[rowId, 7].Value = lstModel[i].ActivitiesDate;
                    rowId++;
                    var copyPercent = (i * lstModel.Count - 1) / 100;
                    exportWorker.ReportProgress(copyPercent);
                }
                worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells
                package.Workbook.Properties.Title = "Company Informations";
                package.Workbook.Properties.Author = "Copyright VuThanh1986 Allright Reverses";
                // set some extended property values
                package.Workbook.Properties.Company = "VTH Inc.";

                // set some custom property values
                package.Workbook.Properties.SetCustomPropertyValue("Checked by", "VuThanh");
                package.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "EPPlus");
                package.Save();
            }
        }

        private void ExportListCompany2File(DataTable dtResult, FileInfo filePath)
        {
            using (ExcelPackage package = new ExcelPackage(filePath))
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Company Informations");
                //Add the headers
                worksheet.Cells[1, 1, 1, 6].Merge = true;
                worksheet.Cells[1, 1].Style.Font.Size = 12;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                worksheet.Cells[1, 1].Value = "Company Informations - " + DateTime.Now.ToShortDateString();
                worksheet.Cells[2, 1].Value = "STT";
                worksheet.Cells[2, 2].Value = "Tên c.ty";
                worksheet.Cells[2, 3].Value = "Địa chỉ c.ty";
                worksheet.Cells[2, 4].Value = "Tên người đại diện";
                worksheet.Cells[2, 5].Value = "Số Đt người đại diện";
                worksheet.Cells[2, 6].Value = "Ngày cấp phép";

                //worksheet.Cells[2, 7].Value = "ActivitiesDate";
                int rowId = 3;
                for (int i = 0; i < dtResult.Rows.Count; i++)
                {
                    worksheet.Cells[rowId, 1].Value = i + 1;
                    worksheet.Cells[rowId, 2].Value = (dtResult.Rows[i][1] == null) ? string.Empty : dtResult.Rows[i][1];
                    worksheet.Cells[rowId, 3].Value = (dtResult.Rows[i][2] == null) ? string.Empty : dtResult.Rows[i][2];
                    worksheet.Cells[rowId, 4].Value = (dtResult.Rows[i][3] == null) ? string.Empty : dtResult.Rows[i][3];
                    worksheet.Cells[rowId, 5].Value = (dtResult.Rows[i][4] == null) ? string.Empty : dtResult.Rows[i][4];
                    worksheet.Cells[rowId, 6].Value = (dtResult.Rows[i][5] == null) ? string.Empty : dtResult.Rows[i][5];
                    worksheet.Cells[rowId, 6].Style.Numberformat.Format = "yyyy-MM-dd";
                    rowId++;
                    var copyPercent = (i * dtResult.Rows.Count - 1) / 100;
                }

                worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells
                package.Workbook.Properties.Title = "Company Informations";
                package.Workbook.Properties.Author = "Copyright VuThanh1986 Allright Reverses";
                // set some extended property values
                package.Workbook.Properties.Company = "VTH Inc.";

                // set some custom property values
                package.Workbook.Properties.SetCustomPropertyValue("Checked by", "VuThanh");
                package.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "EPPlus");
                package.Save();
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            try
            {
                var query = string.Format("SELECT AreaId, AreaName, AreaUrl, ParentId FROM tblArea WHERE ParentId = {0}", 0);
                var ds = DbHelper.ExecuteQuery(query);
                if (ds != null)
                    dtCity = ds.Tables[0];
                cboCity.DataSource = dtCity;
                cboCity.SelectedIndex = -1;

                //var query = "SELECT COUNT(*) FROM tblArea";
                //var result = DbHelper.ExecuteScalar(query).ToString();
                //if (int.Parse(result) == 0)
                //{
                //    DialogResult dialog = MessageBox.Show("Hiện tại hệ thống dữ liệu tỉnh thành/quận huyên chưa có.\n Bấm 'OK' để lấy dữ liệu ?",
                //                            "Xác nhận yêu cầu", MessageBoxButtons.OKCancel);
                //    if (dialog == DialogResult.OK)
                //    {
                //        if (!buildCopyWorker.IsBusy)
                //        {
                //            pcbLoading.Visible = true;
                //            grbActions.Enabled = false;
                //            buildCopyWorker.RunWorkerAsync("http://www.thongtincongty.com/");
                //        }
                //    }
                //}
                //else
                //{
                //    query = string.Format("SELECT AreaId, AreaName, AreaUrl, ParentId FROM tblArea WHERE ParentId = {0}", 0);
                //    var ds = DbHelper.ExecuteQuery(query);
                //    if (ds != null)
                //        dtCity = ds.Tables[0];
                //    cboCity.DataSource = dtCity;
                //}
            }
            catch (Exception ex)
            {
                CopyLogger.Error(string.Format("Trace Error:{0} \n Error Message:{1}",
                    ex.ToString(), ex.Message));
            }
        }

        private List<AreaModel> GetCitiesUrl(string url, int parentId = 0)
        {
            //var result = String.Empty;
            List<AreaModel> lstCities = new List<AreaModel>();
            try
            {
                var resultHtml = GetContent(url, ".thongtincongty.com");
                //using Html Agility Pack
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(resultHtml);
                // Get city list url
                var cityXPath = "//*[@id='sidebar']/div[@class='list-group']/a";
                CopyLogger.Debug("\ncityXPath:" + cityXPath);
                // Get news by area categories
                int cityId = 1;
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes(cityXPath))
                {
                    int total = link.LinePosition;
                    int percent = (cityId / total) * 100;
                    buildCopyWorker.ReportProgress(percent);
                    if (link.InnerText.TrimStart().TrimEnd() == "Toàn Quốc") continue;
                    Thread.Sleep(250);
                    var model = new AreaModel();
                    model.AreaId = cityId;
                    var cityName = link.InnerText.TrimStart().TrimEnd();
                    var cityUrl = link.Attributes["href"].Value.ToString();
                    model.AreaName = cityName;
                    model.AreaUrl = cityUrl;
                    model.ParentId = parentId;
                    CopyLogger.Debug(model.ToString());
                    model.InsertAreaData(model);
                    lstCities.Add(model);
                    cityId++;
                }
            }
            catch (Exception ex)
            {
                CopyLogger.Error(string.Format("Trace Error:{0} \n Error Message:{1}",
                    ex.ToString(), ex.Message));
            }
            return lstCities;
        }

        private List<CompanyModel> GetCompanyContent(string url, int cityId, int districtId, bool isFinished = false)
        {
            List<CompanyModel> lstCompany = new List<CompanyModel>();
            try
            {
                var resultHtml = GetContent(url, ".thongtincongty.com");
                //using Html Agility Pack
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(resultHtml);
                // Get city sub url
                var subUrl = "//div[@class='search-results']/a";
                CopyLogger.Debug("\n subUrl:" + subUrl);
                var rangeXPath = "/html/body/div/div[1]/ul/li[6]/a";
                int startPage = 1;
                int endPage = 0;
                CopyLogger.Debug("\n rangeXPath:" + rangeXPath);
                var rangeNode = doc.DocumentNode.SelectNodes(rangeXPath);
                // Get page number
                if (rangeNode != null)
                {
                    foreach (var node in rangeNode)
                    {
                        var rangLink = node.Attributes["href"].Value.Replace("\r", "").Replace("\n", "").Replace("&nbsp;", "").TrimStart().TrimEnd();
                        CopyLogger.Debug("\n rangLink:" + rangLink);
                        int index = rangLink.IndexOf("=", StringComparison.Ordinal) + 1;
                        endPage = int.Parse(rangLink.Substring(index));
                        CopyLogger.Debug("\n pageEnd:" + endPage);
                    }
                }
                // Start loop from 1 to n page
                for (int i = startPage; i <= endPage; i++)
                {
                    // Start sub details to get information
                    foreach (HtmlNode link in doc.DocumentNode.SelectNodes(subUrl))
                    {
                        var company = new CompanyModel();
                        // Get details company link
                        var subLink = link.Attributes["href"].Value.TrimStart().TrimEnd();
                        if (!string.IsNullOrEmpty(subLink))
                        {
                            var subHtml = GetContent(subLink, ".thongtincongty.com");
                            var subDoc = new HtmlAgilityPack.HtmlDocument();
                            subDoc.LoadHtml(subHtml);
                            CopyLogger.Info("\n subLink:" + subLink);
                            // Get company name
                            var cpNameXPath = "/html/body/div/div[1]/div[3]/h4/a/span";
                            CopyLogger.Debug("\n cpNameXPath:" + cpNameXPath);
                            var cpNameNodes = subDoc.DocumentNode.SelectNodes(cpNameXPath);
                            if (cpNameNodes != null)
                            {
                                foreach (var node in cpNameNodes)
                                {
                                    var cpName = node.InnerText.Replace("\r", "").Replace("\n", "").
                                                                Replace("&nbsp;", "").TrimStart().TrimEnd();
                                    company.CompanyName = string.IsNullOrEmpty(cpName) ? string.Empty : cpName;
                                    CopyLogger.Info("\n Tên công ty:" + cpName);
                                }
                            }

                            // Get company issued date
                            var cpIssuedXPath = "/html/body/div/div[1]/div[3]/text()[contains(., 'Ngày cấp giấy phép')]";
                            CopyLogger.Debug("\n coIssuedXPath:" + cpIssuedXPath);
                            var cpIssuedNodes = subDoc.DocumentNode.SelectNodes(cpIssuedXPath);
                            if (cpIssuedNodes != null)
                            {
                                foreach (var node in cpIssuedNodes)
                                {
                                    var cpIssuedDt = node.InnerText.Replace("Ngày cấp giấy phép:", "").Replace("\r", "").
                                                                    Replace("\n", "").Replace("&nbsp;", "").TrimStart().TrimEnd();
                                    var issDate = new DateTime();
                                    if (DateTime.TryParse(cpIssuedDt, out issDate))
                                        company.IssuedDate = issDate;
                                    CopyLogger.Info("\n Ngày cấp giấy phép:" + issDate);
                                }
                            }

                            if (LastCompany != null)
                            {
                                if ((LastCompany.CompanyName == company.CompanyName) &&
                                    (LastCompany.CompanyAddress == company.CompanyAddress))
                                {
                                    copyInfoWorker.CancelAsync();
                                    return lstCompany;
                                }
                            }

                            // Get company phone. If does not constain phone number => continue
                            var phoneXPath = "/html/body/div/div[1]/div[3]/text()[contains(., 'Điện thoại:')]";
                            CopyLogger.Debug("\n phoneXPath:" + phoneXPath);
                            var phoneNodes = subDoc.DocumentNode.SelectNodes(phoneXPath);
                            if (phoneNodes != null)
                            {
                                foreach (var node in phoneNodes)
                                {
                                    var cpPhone = node.InnerText.Replace("Điện thoại:", "").Replace("\r", "").Replace("\n", "").
                                                                 Replace("&nbsp;", "").Replace(" ", "").TrimStart().TrimEnd();
                                    CopyLogger.Info("\n Điện thoại:" + cpPhone);
                                    if (cpPhone.StartsWith("04", StringComparison.Ordinal) ||
                                        cpPhone.StartsWith("08", StringComparison.Ordinal)) continue;
                                    var pattern = @"^09\d{8}$";
                                    var regex = new Regex(pattern);
                                    if (regex.IsMatch(cpPhone))
                                        company.RepresentPhone = cpPhone;
                                    else
                                        continue;
                                }
                            }
                            else
                            {
                                continue;
                            }
                            // Get company address
                            var addXPath = "/html/body/div/div[1]/div[3]/text()[contains(., 'Địa chỉ:')]";
                            CopyLogger.Debug("\n addXPath:" + addXPath);
                            var addNodes = subDoc.DocumentNode.SelectNodes(addXPath);
                            if (addNodes != null)
                            {
                                foreach (var node in addNodes)
                                {
                                    var cpAddress = node.InnerText.Replace("Địa chỉ:", "").Replace("\r", "").
                                                                   Replace("\n", "").Replace("&nbsp;", "").TrimStart().TrimEnd();
                                    CopyLogger.Info("\n Địa chỉ:" + cpAddress);
                                    company.CompanyAddress = string.IsNullOrEmpty(cpAddress) ? string.Empty : cpAddress;
                                }
                            }

                            // Get company represent name
                            var preNameXPath = "/html/body/div/div[1]/div[3]/text()[contains(., 'Đại diện pháp luật')]";
                            CopyLogger.Debug("\n preNameXPath:" + preNameXPath);
                            var preNameNodes = subDoc.DocumentNode.SelectNodes(preNameXPath);
                            if (preNameNodes != null)
                            {
                                foreach (var node in preNameNodes)
                                {
                                    var cpPreName = node.InnerText.Replace("Đại diện pháp luật:", "").Replace("\r", "").
                                                                   Replace("\n", "").Replace("&nbsp;", "").TrimStart().TrimEnd();
                                    CopyLogger.Info("\n Đại diện pháp luật:" + cpPreName);
                                    if (!string.IsNullOrEmpty(cpPreName))
                                        company.RepresentName = cpPreName;
                                    else
                                        company.RepresentName = string.Empty;
                                }
                            }
                            // Get company activites date
                            var cpActivityDt = "/html/body/div/div[1]/div[3]/text()[contains(., 'Ngày hoạt động')]";
                            CopyLogger.Debug("\n cpActivityDt:" + cpActivityDt);
                            var coActivityNodes = subDoc.DocumentNode.SelectNodes(cpActivityDt);
                            if (coActivityNodes != null)
                            {
                                foreach (var node in coActivityNodes)
                                {
                                    var activityDt = node.InnerText.Replace("Ngày hoạt động:", "").Replace("\r", "").Replace("\n", "").
                                                                    Replace("&nbsp;", "").Replace("(", "").TrimStart().TrimEnd();
                                    var actDate = new DateTime();
                                    if (DateTime.TryParse(activityDt, out actDate))
                                        company.ActivitiesDate = actDate;
                                    else
                                        company.ActivitiesDate = DateTime.Now;
                                    CopyLogger.Info("\n Ngày hoạt động::" + actDate);
                                }
                            }
                            company.CityId = cityId;
                            CopyLogger.Info("\n Thành phố Id:" + cityId);
                            company.DistrictId = districtId;
                            CopyLogger.Info("\n Quận huyện Id:" + districtId);
                            // Insert to DB
                            InsertItem2Db(company);
                            // Is last item ? Insert item to DB
                            if (isFinished)
                            {
                                InsertLastItem2Db(company);
                                lstCompany.Add(company);
                                break;
                            }
                            Thread.Sleep(250);
                            // Add to list
                            lstCompany.Add(company);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CopyLogger.Error(string.Format("Trace Error:{0} \n Error Message:{1}",
                    ex.ToString(), ex.Message));
            }
            return lstCompany;
        }

        /// <summary>
        ///   Get content of web site
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GetContent(string url, string domain)
        {
            string content = string.Empty;
            var requestUrl = (HttpWebRequest)WebRequest.Create(url);
            string domainValue = domain;
            //if (domain == null)
            //    domainValue = cboSourceUrls.Text;
            //else
            //    domainValue = domain;
            //--Start add proxy - Remove lines code and config from App.config - If you don't need
            //IWebProxy proxy = new WebProxy("fsoft-proxy", 8080);
            //proxy.Credentials = new NetworkCredential("hault1", "Ginta@123");
            //request.Proxy = proxy;
            //--End add proxy
            try
            {
                var request = TryAddCookie(requestUrl, new List<Cookie> {
                new Cookie("__asc", "c99bb1d41538995d02618de55b8") { Domain = domainValue },
                new Cookie("_gat", "1") { Domain = domainValue },
                new Cookie("_ga", "GA1.3.1787916081.1458302207") { Domain = domainValue },
                new Cookie("__auc", "c99bb1d41538995d02618de55b8") { Domain = domainValue },
                new Cookie("psortfilter", "1%24all%24VOE%2FWO8MpO1adIX%2BwMGNUA%3D%3D") { Domain = domainValue },
                new Cookie("raovatPosition", "3") { Domain = domainValue },
            });

                request.Timeout = 15 * 1000;
                request.Method = "GET";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.94 Safari/537.36";

                HttpStatusCode statusCode;

                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    var contentType = response.ContentType;
                    Encoding encoding = null;
                    if (contentType != null)
                    {
                        var match = Regex.Match(contentType, @"(?<=charset\=).*");
                        if (match.Success)
                            encoding = Encoding.GetEncoding(match.ToString());
                    }

                    encoding = encoding ?? Encoding.UTF8;
                    statusCode = ((HttpWebResponse)response).StatusCode;
                    var reader = new StreamReader(stream, encoding);
                    content = reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    content = reader.ReadToEnd();
                }
                CopyLogger.Error(ex.Message);
            }
            return content;
        }

        private CompanyModel GetLastItem()
        {
            var query = "SELECT Id, CompanyName, IssuedDate FROM tblLastCompany";
            var connection = DbHelper.GetConnection();
            var model = new CompanyModel();
            try
            {
                var command = new SQLiteCommand(query, connection);
                var reader = command.ExecuteReader();
                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        model.Id = reader.GetInt32(1);
                        model.CompanyName = reader.GetValue(2).ToString();
                        model.IssuedDate = reader.GetDateTime(3);
                    }
                }
            }
            catch (Exception ex)
            {
                CopyLogger.Error(string.Format("Trace Error:{0} \n Error Message:{1}",
                     ex.ToString(), ex.Message));
            }
            return model;
        }

        private void InsertItem2Db(CompanyModel model)
        {
            if (model != null)
            {
                var query = "INSERT OR REPLACE INTO tblCompanyInfo (CompanyName, CompanyAddress, RepresentName, RepresentPhone,  IssuedDate, AcitivitiesDate, CityId, DistrictId) VALUES ";
                query += string.Format("('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', {6}, {7}) ",
                            model.CompanyName, model.CompanyAddress, model.RepresentName, model.RepresentPhone, model.IssuedDate, model.ActivitiesDate, model.CityId, model.DistrictId);
                CopyLogger.Info("\n Insert Query:" + query);
                DbHelper.ExecuteNoneQuery(query);
            }
        }

        private void InsertLastItem2Db(CompanyModel model)
        {
            if (model != null)
            {
                // Reset old data
                var clearData = "DELETE FROM tblLastCompany";
                DbHelper.ExecuteNoneQuery(clearData);
                // Insert new data
                var query = string.Format("INSERT INTO tblLastCompany (CompanyName, RepresentName, IssuedDate) VALUES ('{0}', '{1}', '{2}')",
                                                                        model.CompanyName, model.RepresentName , model.IssuedDate);
                DbHelper.ExecuteNoneQuery(query);
            }
        }

        private HttpWebRequest TryAddCookie(WebRequest webRequest, List<Cookie> cookie)
        {
            HttpWebRequest httpRequest = webRequest as HttpWebRequest;
            if (httpRequest == null)
            {
                return httpRequest;
            }

            if (httpRequest.CookieContainer == null)
            {
                httpRequest.CookieContainer = new CookieContainer();
            }
            foreach (var item in cookie)
            {
                httpRequest.CookieContainer.Add(item);
            }
            return httpRequest;
        }

        #endregion Private Methods
    }
}