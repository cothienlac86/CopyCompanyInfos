using CopyCompanyInfo.Common;
using CopyCompanyInfo.DataAccess;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
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
        static CompanyModel LastCompany = null;
        static readonly log4net.ILog CopyLogger =
                    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().Name);

        DataTable dtCity = new DataTable();
        DataTable dtDistrict = new DataTable();

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
                var lstCompany = new List<CompanyModel>();

                if (!copyInfoWorker.CancellationPending)
                {
                    var lstUrl = e.Argument as DataTable;
                    if (lstUrl != null)
                    {
                        var lastItem = lstUrl.Rows.Count - 1;
                        for (int i = 0; i < lstUrl.Rows.Count; i++)
                        {
                            Thread.Sleep(250);
                            var url = lstUrl.Rows[i]["AreaUrl"].ToString();
                            int cityId = int.Parse(lstUrl.Rows[i]["ParentId"].ToString());
                            int districtId = int.Parse(lstUrl.Rows[i]["AreaId"].ToString());
                            if (i == lastItem)
                                lstCompany = GetCompanyContent(url, cityId, districtId, true);
                            else
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
                var lstModel = e.Result as List<CompanyModel>;
                if (lstModel != null)
                {
                    lblLoading.Text = "Đã hoàn thành lọc dữ liệu...";
                    if (!exportWorker.IsBusy)
                    {
                        copyInfoWorker.Dispose();
                        exportWorker.RunWorkerAsync(lstModel);
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
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel Workbook | *.xlsx | Excel 97-2003 Workbook| *.xls";
                saveFileDialog.Title = "Lưu dữ liệu";
                saveFileDialog.ShowDialog();
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

            }
            catch (Exception ex)
            {
                CopyLogger.Error(string.Format("Trace Error:{0} \n Error Message:{1}",
                    ex.ToString(), ex.Message));
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void ExportData2Excel(List<CompanyModel> lstModel, string savePath)
        {

        }


        private void btnCopy_Click(object sender, EventArgs e)
        {
            string query = "SELECT * FROM tblArea";
            if (cboCity.SelectedIndex == -1)
            {
                MessageBox.Show("Xin hãy lựa chọn tỉnh thành để lọc tin !");
                return;
            }
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
                    copyInfoWorker.RunWorkerAsync(dt);
                }
            }
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

        List<CompanyModel> GetCompanyContent(string url, int cityId, int districtId, bool isFinished = false)
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
                    var copyPercent = (i * endPage) / 100;
                    copyInfoWorker.ReportProgress(copyPercent);
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
                                    (LastCompany.IssuedDate == company.IssuedDate))
                                {
                                    copyInfoWorker.CancelAsync();
                                    break;
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
        private void InsertItem2Db(CompanyModel model)
        {
            if (model != null)
            {
                var query = "INSERT INTO tblCompanyInfo (CompanyName, CompanyAddress, RepresentName, RepresentPhone,  IssuedDate, AcitivitiesDate, CityId, DistrictId) VALUES ";
                query += string.Format("('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', {6}, {7});",
                            model.CompanyName, model.CompanyAddress, model.RepresentName, model.RepresentPhone, model.IssuedDate, model.ActivitiesDate, model.CityId, model.DistrictId);
                CopyLogger.Debug("\n Insert Query:" + query);
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
                var query = string.Format("INSERT INTO tblLastCompany (CompanyName, IssuedDate) VALUES ('{0}', '{1}')",
                                                                        model.CompanyName, model.IssuedDate);
                DbHelper.ExecuteNoneQuery(query);
            }
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