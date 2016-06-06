using CopyCompanyInfo.Common;
using CopyCompanyInfo.DataAccess;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        private static readonly log4net.ILog CopyLogger =
                    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().Name);
        private DataTable dtCity = new DataTable();
        private DataTable dtDistrict = new DataTable();
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
        }

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
                    ex.ToString(), ex.Message));
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
                    ex.ToString(), ex.Message));
            }
        }

        protected void buildCopyWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                pcbLoading.Visible = false;
                var result = e.Result as string;
                if (!string.IsNullOrEmpty(result))
                {
                    var ds = new AreaModel().GetListArea(0) as DataSet;
                    if (ds != null) dtCity = ds.Tables[0];
                    cboCity.DataSource = dtCity;
                    MessageBox.Show(result);
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
                //List<AreaModel> lstCities = new List<AreaModel>();

                if (!copyInfoWorker.CancellationPending)
                {
                    var lstUrl = e.Argument as DataTable;
                    if (lstUrl != null)
                    {
                        for (int i = 0; i < lstUrl.Rows.Count; i++)
                        {
                            Thread.Sleep(250);
                            var url = lstUrl.Rows[i]["AreaUrl"].ToString();
                            GetCompanyContent(url);
                        }
                    }
                }
                e.Result = "Đã hoàn thành lấy dữ liệu các tỉnh thành.";
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
                //pcbLoading.Visible = false;
                //var result = e.Result as string;
                //if (!string.IsNullOrEmpty(result))
                //{
                //    var ds = new AreaModel().GetListArea(0) as DataSet;
                //    if (ds != null) dtCity = ds.Tables[0];
                //    cboCity.DataSource = dtCity;
                //    MessageBox.Show(result);
                //}
            }
            catch (Exception ex)
            {
                CopyLogger.Error(string.Format("Trace Error:{0} \n Error Message:{1}",
                    ex.ToString(), ex.Message));
            }
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
                new Cookie("login_name", "vuthanh86") { Domain = domainValue },
                new Cookie("login_name", "vuthanh86") { Domain = ".vatgia.com" },
                new Cookie("eb_area", "3") { Domain = ".enbac.com" },
                new Cookie("_dc_gtm_UA-35679026-6", "1") { Domain = ".id.vatgia.com" },
                new Cookie("_ga", "GA1.3.17308481.1464728906") { Domain = ".id.vatgia.com" },
                new Cookie("_gsn", "ARCLHFJJ") { Domain = ".id.vatgia.com" },
                new Cookie("auth", "eyJfc2lkIjoiSFYzM2kzR0kybDlBNHdzZjNhQm1rWCJ9|1464730241|b3d2d7fa86a44afd5842f9582273c6540861dba4") { Domain = "id.vatgia.com" },
                new Cookie("__vgiu", "0ed21b6c9fe18a09f5eba54e30fb0707503127efa951ad.3a7b1f") { Domain = "id.vatgia.com" },
                new Cookie("province_id", "29") { Domain = ".enbac.com" },
                new Cookie("bds_prid", "29") { Domain = ".enbac.com" },
                new Cookie("rd_session_id", "39b608a5-278a-11e6-a88d-4d5fa6d086e8") { Domain = ".enbac.com" },
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

        private void GetCompanyContent(string url)
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
                if (rangeNode != null)
                {
                    foreach (var node in rangeNode)
                    {
                        var rangLink = node.Attributes["href"].Value.Replace("\r", "").Replace("\n", "").Replace("&nbsp;", "").TrimStart().TrimEnd();
                        CopyLogger.Debug("\n rangLink:" + rangLink);
                        int index = rangLink.IndexOf("=") + 1;
                        endPage = int.Parse(rangLink.Substring(index));
                        CopyLogger.Debug("\n pageEnd:" + endPage);
                    }
                }
                for (int i = startPage; i <= endPage; i++)
                {
                    foreach (HtmlNode link in doc.DocumentNode.SelectNodes(subUrl))
                    {
                        var company = new CompanyModel();
                        var subLink = link.Attributes["href"].Value.TrimStart().TrimEnd();
                        CopyLogger.Debug("\n subLink:" + subLink);

                        if (!string.IsNullOrEmpty(subLink))
                        {
                            var subHtml = GetContent(subLink, ".thongtincongty.com");
                            var subDoc = new HtmlAgilityPack.HtmlDocument();
                            subDoc.LoadHtml(subHtml);
                            var cpNameXPath = "//div[@class='jumbotron']/h4/a/span";
                            CopyLogger.Debug("\n cpNameXPath:" + cpNameXPath);
                            var cpContentXPath = "//div[@class='jumbotron']/text()";
                            CopyLogger.Debug("\n cpContentXPath:" + cpContentXPath);
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
    }
}
