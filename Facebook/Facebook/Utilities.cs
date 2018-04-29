﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using COMExcel = Microsoft.Office.Interop.Excel;

namespace Facebook
{
    class Utilities
    {

        public static bool CheckNumberEnterKey(string uid)
        {
            try
            {
                return int.TryParse(uid, out int n);
            }
            catch
            {
                return false;
            }
        }
    }

    public class Log
    {
        public string Message { get; set; }
        public string LogTime { get; set; }
        public string LogDate { get; set; }

        public Log(string message)
        {
            Message = message;
            LogDate = DateTime.Now.ToString("yyyy-MM-dd");
            LogTime = DateTime.Now.ToString("hh:mm:ss.fff tt");
        }
    }
    public class LogWriter
    {
        private static LogWriter instance;
        private static Queue<Log> logQueue;
        private static string logDir = "D";//<Path to your Log Dir or Config Setting>;
        private static string logFile = "log.txt";//<Your Log File Name or Config Setting>;
        private static int maxLogAge = int.Parse("5000");
        private static int queueSize = int.Parse("0");
        private static DateTime LastFlushed = DateTime.Now;

        /// <summary>
        /// Private constructor to prevent instance creation
        /// </summary>
        private LogWriter() { }

        /// <summary>
        /// An LogWriter instance that exposes a single instance
        /// </summary>
        public static LogWriter Instance {
            get {
                // If the instance is null then create one and init the Queue
                if (instance == null)
                {
                    instance = new LogWriter();
                    logQueue = new Queue<Log>();
                }
                return instance;
            }
        }


        /// <summary>
        /// The single instance method that writes to the log file
        /// </summary>
        /// <param name="message">The message to write to the log</param>
        public void WriteToLog(string message)
        {
            // Lock the queue while writing to prevent contention for the log file
            lock (logQueue)
            {
                // Create the entry and push to the Queue
                Log logEntry = new Log(message);
                logQueue.Enqueue(logEntry);

                // If we have reached the Queue Size then flush the Queue
                if (logQueue.Count >= queueSize || DoPeriodicFlush())
                {
                    FlushLog();
                }
            }
        }

        private bool DoPeriodicFlush()
        {
            TimeSpan logAge = DateTime.Now - LastFlushed;
            if (logAge.TotalSeconds >= maxLogAge)
            {
                LastFlushed = DateTime.Now;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Flushes the Queue to the physical log file
        /// </summary>
        private void FlushLog()
        {
            while (logQueue.Count > 0)
            {
                Log entry = logQueue.Dequeue();
                string logPath = logDir + entry.LogDate + "_" + logFile;

                // This could be optimised to prevent opening and closing the file for each write
                using (FileStream fs = File.Open(logPath, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter log = new StreamWriter(fs))
                    {
                        log.WriteLine(string.Format("{0}\t{1}", entry.LogTime, entry.Message));
                    }
                }
            }
        }
    }

    public static class TheardFacebookWriter
    {
        public static bool hasProcess = true; /*khai bao bien stop*/
        private static LogWriter writer;

        public static void getwebBrowser(DmGroupUID model, object arrControl)
        {
            ArrayList arr1 = (ArrayList)arrControl;
            Label lblMessage1 = (Label)arr1[0];
            Label lblPhanTram = (Label)arr1[1];
            Label lblSluongGoi = (Label)arr1[2];
            TextBox txtToken = (TextBox)arr1[3];
            CheckBox chkMe = (CheckBox)arr1[4];
            CheckBox chkBanBe = (CheckBox)arr1[5];
            CheckBox chkBaiViet = (CheckBox)arr1[6];
            ProgressBar progressBar1 = (ProgressBar)arr1[7];

            try
            {
                if (!TheardFacebookWriter.hasProcess) return;
                /*Lấy thông tin của UID*/
                if (chkMe.Checked) {
                    string requestUriString = model.IsLoai == 0 ? string.Format(@"https://graph.facebook.com/{0}/?fields={1}&access_token={2}", model.UID, Facebook.SelectMyUIDOfUser(), Facebook.Token()) : "";
                    string json= Facebook.GetHtmlFB(requestUriString);
                }

            }
            catch (Exception ex) { }
        }
    }

    


    public static class Helpers
    {
        private static LogWriter writer;
        public static int vitritim(List<string> array, string xx)
        {
            try
            {
                int i = 0;
                foreach (var bale in array)
                {
                    if (bale.Contains(xx))
                    {
                        return i;
                    }
                    i++;
                }
                return -1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        public static int vitritim(List<string> array, string xx, int vt)
        {
            try
            {
                if (vt == array.Count()) return -1;
                vt++;
                for (int i = vt; i < array.Count(); i++)
                {
                    if (array[i].Contains(xx))
                        return i;
                }
                return -1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }


        //<INPUT type=hidden value=011 name=code>
        public static string getValueHtml(string tabHTML)
        {
            //b1: cho tất cả vào mảng cách nhau bởi khoản trắng
            string[] arrGetHtmlDocument = tabHTML.Trim().Split(' ');
            //b2: tìm mảng nào có chứa value
            string giatri = Array.Find(arrGetHtmlDocument, n => n.Contains("value"));
            //b3: cắt giatri có trong string ra
            // Location of the letter c.
            int i_dau = giatri.IndexOf("'") + 1;
            int i_cuoi = giatri.LastIndexOf("'");
            string d = giatri.Substring(i_dau, i_cuoi - i_dau);
            return d;
        }


        /// <param name="chuoi1"></param>
        /// <param name="chuoi2"></param>
        /// <param name="and_or">and_or: and =true; or= false</param>
        /// <returns></returns>
        public static int vitritim(List<string> array, string chuoi1, string chuoi2, bool and_or)
        {
            try
            {
                int i = 0;
                foreach (var bale in array)
                {

                    if (and_or && (bale.Contains(chuoi1) && bale.Contains(chuoi2)))
                    {
                        return i;
                    }
                    if (!and_or && (bale.Contains(chuoi1) || bale.Contains(chuoi2)))
                    {
                        return i;
                    }
                    i++;
                }
                return -1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }



        public static List<string> getUrlHtml2(string html)
        {
            try
            {
                List<string> xx = new List<string>();
                while (html.IndexOf("href=\"") != -1)
                {
                    int vt1 = html.IndexOf("href=\"") + 6;
                    int vt2 = 0;
                    for (int i = vt1; i < html.Length; i++)
                    {
                        if (html[i].Equals('"'))
                        {
                            vt2 = i;
                            break;
                        }
                    }

                    xx.Add(html.Substring(vt1, (vt2 - vt1)));
                    html = html.Substring(vt2, html.Length - vt2);
                }
                return xx;
            }
            catch (Exception)
            {

                return null;
            }

        }

        public static List<string> getUrlHtml3(string html)
        {
            try
            {
                List<string> xx = new List<string>();
                while (html.IndexOf("href=\'") != -1)
                {
                    int vt1 = html.IndexOf("href=\'") + 6;
                    int vt2 = 0;
                    for (int i = vt1; i < html.Length; i++)
                    {
                        if (html[i].Equals('\''))
                        {
                            vt2 = i;
                            break;
                        }
                    }

                    xx.Add(html.Substring(vt1, (vt2 - vt1)));
                    html = html.Substring(vt2, html.Length - vt2);
                }
                return xx;
            }
            catch (Exception)
            {

                return null;
            }

        }

        public static string getUrlHtml(string tabHTML)
        {
            //b1: cho tất cả vào mảng cách nhau bởi khoản trắng
            string[] arrGetHtmlDocument = tabHTML.Trim().Split(' ');
            //b2: tìm mảng nào có chứa value
            string giatri = Array.Find(arrGetHtmlDocument, n => n.Contains("href"));
            //b3: cắt giatri có trong string ra
            // Location of the letter c.
            int i_dau = giatri.IndexOf("\"") + 1;
            int i_cuoi = giatri.LastIndexOf("\"");
            string d = giatri.Substring(i_dau, i_cuoi - i_dau);
            return d;
        }
        public static string getClassHtml(string tabHTML)
        {
            //b1: cho tất cả vào mảng cách nhau bởi khoản trắng
            string[] arrGetHtmlDocument = tabHTML.Trim().Split(' ');
            //b2: tìm mảng nào có chứa value
            string giatri = Array.Find(arrGetHtmlDocument, n => n.Contains("class"));
            //b3: cắt giatri có trong string ra
            // Location of the letter c.
            int i_dau = giatri.IndexOf("\"") + 1;
            int i_cuoi = giatri.LastIndexOf("\"");
            string d = giatri.Substring(i_dau, i_cuoi - i_dau);
            return d;
        }
        public static List<string> getDataHTML(string tabHTML)
        {
            List<string> arr = new List<string>();
            int vt1 = 0, vt2 = 0;
            try
            {

                for (int i = 0; i < tabHTML.Length - 1; i++)
                {
                    if (tabHTML[i].Equals('>'))
                    {
                        vt1 = i;
                        for (int j = i; j < tabHTML.Length; j++)
                        {
                            if (tabHTML[j].Equals('<'))
                            {
                                vt2 = j;
                                break;
                            }
                        }
                        if (vt2 == 0 && tabHTML.Length > 0)/*by luulong:16/06/2017*/
                            vt2 = tabHTML.Length;

                        if (!string.IsNullOrEmpty(tabHTML.Substring(vt1 + 1, vt2 - vt1 - 1)))
                            arr.Add(tabHTML.Substring(vt1 + 1, vt2 - vt1 - 1).Trim());
                    }
                }
                return arr;
            }
            catch
            {
                //writer = LogWriter.Instance;
                //writer.WriteToLog(string.Format("{0}", tabHTML));
                //MessageBox.Show("Lỗi cấu trúc không đúng Vui lòng nhấn enter để tiếp tục\n " + ex.Message, "getHTML");
                return arr;
            }
        }
        public static void LogMessage(string msg)
        {
            string sFilePath = Environment.CurrentDirectory + "Log_" + System.AppDomain.CurrentDomain.FriendlyName + ".txt";

            System.IO.StreamWriter sw = System.IO.File.AppendText(sFilePath);
            try
            {
                string logLine = System.String.Format(
                    "{0:G}: {1}.", System.DateTime.Now, msg);
                sw.WriteLine(logLine);
            }
            finally
            {
                sw.Close();
            }
        }
        public static List<string> FindAllPhone(string tabHTML)
        {
            try
            {
                List<string> danhsachdienthoai = new List<string>();
                string[] danhsachdauso = { "09", "01" };
                foreach (var item_dauso in danhsachdauso)
                {
                    int vt1 = 0;
                    while (vt1 <= tabHTML.Length)
                    {
                        vt1 = tabHTML.IndexOf(item_dauso, vt1);
                        if (vt1 == -1)
                            break;
                        string chuoitamlay = tabHTML.Substring(vt1, 20);
                        string dienthoai = "";
                        foreach (var item in chuoitamlay)
                        {
                            if (System.Text.RegularExpressions.Regex.IsMatch(item.ToString(), "[0-9]"))
                                dienthoai += item;
                        }

                        if (item_dauso == "09")
                        {
                            if (dienthoai.Length >= 10)
                            {
                                danhsachdienthoai.Add(dienthoai.Substring(0, 10));
                                vt1 = vt1 + 10;
                            }
                            else
                            {
                                vt1 = vt1 + dienthoai.Length;
                            }
                        }
                        if (item_dauso == "01")
                        {
                            if (dienthoai.Length >= 11)
                            {
                                danhsachdienthoai.Add(dienthoai.Substring(0, 11));
                                vt1 = vt1 + 11;
                            }
                            else
                            {
                                vt1 = vt1 + dienthoai.Length;
                            }
                        }
                    }
                }
                return danhsachdienthoai;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string FindAllEmail(string tabHTML)
        {
            int vt1 = 0;
            try
            {
                tabHTML = tabHTML.Replace(":&nbsp;", "").Replace("&nbsp;", "").Trim();
                List<string> danhsachemail = new List<string>();
                vt1 = tabHTML.IndexOf("@", vt1);
                int i_dau = tabHTML.LastIndexOf(" ", vt1);
                int i_cuoi = tabHTML.IndexOf(" ", vt1);
                if (i_dau == -1)
                    i_dau = 0;
                if (i_cuoi == -1)
                    i_cuoi = tabHTML.Length;
                string email = tabHTML.Substring(i_dau, i_cuoi - i_dau);
                return email;

            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static bool islink(string link)
        {
            try
            {
                if (link.Length < 8)
                    return false;
                if (link.StartsWith("http://") || link.StartsWith("https://"))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool IsNumber(string s)
        {
            return s.All(char.IsDigit);
        }
        public static string getDomain(string link)
        {
            try
            {
                int i = 0;
                for (i = link.IndexOf("//") + 2; i < link.Length; i++)
                {
                    if (link[i] == '/')
                        break;
                }

                return link.Substring(0, i);
            }
            catch
            {
                return "";
            }
        }

        public static string getTitleWeb(string html)
        {
            try
            {
                int vt1 = html.IndexOf("<title>");
                if (vt1 == -1)
                    return "";
                vt1 = vt1 + 7;
                int vt2 = html.IndexOf("</title>", vt1);
                return html.Substring(vt1, vt2 - vt1);
            }
            catch
            {
                return "";
            }
        }

        public static string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
    }

    public class ConvertType
    {
        public static int ToInt(object obj)
        {
            try
            {
                if (obj == null)
                    return 0;
                int rs = System.Convert.ToInt32(obj);
                if (rs < 0)
                    return 0;
                return rs;
            }
            catch
            {
                return 0;
            }
        }
        public static double ToDouble(object obj)
        {
            try
            {
                if (obj == null)
                    return 0;
                double rs = System.Convert.ToDouble(obj);
                if (rs < 0)
                    return 0;
                return rs;
            }
            catch
            {
                return 0;
            }
        }
        public static decimal ToDecimal(object obj)
        {
            try
            {
                if (obj == null)
                    return 0;
                decimal rs = System.Convert.ToDecimal(obj);
                if (rs < 0)
                    rs = 0;
                return rs;
            }
            catch { return 0; }
        }
        public static string ToString(object obj)
        {
            try
            {
                if (obj == null)
                    return "";
                return System.Convert.ToString(obj);
            }
            catch
            {
                return "";
            }
        }
        public static float ToFloat(object obj)
        {
            try
            {
                if (obj == null)
                    return 0;
                float rs = float.Parse(obj.ToString());
                if (rs < 0)
                    return 0;
                return rs;
            }
            catch
            {
                return 0;
            }
        }
        public static DateTime ToDateTime(object obj)
        {
            try
            {
                if (obj == null)
                    return DateTime.Now;
                DateTime dt = System.Convert.ToDateTime(obj, System.Globalization.CultureInfo.InvariantCulture);

                return dt;
            }
            catch
            {
                return DateTime.Now;
            }
        }
        public static Guid ToGuid(object obj)
        {
            try
            {
                if (obj == null)
                    return Guid.Empty;
                Guid dt = new Guid(obj.ToString());
                return dt;
            }
            catch
            {
                return Guid.Empty;
            }
        }

        public static string StripDiacritics(string accented)
        {
            return Regex.Replace(StripDiacriticsNormalize(accented), @"\s+", "-");
        }
        public static string StripDiacriticsNormalize(string accented)
        {
            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            string strFormD = accented.Normalize(System.Text.NormalizationForm.FormD);
            strFormD = regex.Replace(strFormD, String.Empty);
            strFormD = strFormD.Replace("Đ", "D").Replace("đ", "d");
            return Regex.Replace(strFormD, @"[^A-Za-z0-9 ]", "").Trim().ToLower();
        }
        const string HTML_TAG_PATTERN = "<.*?>";

        public static string StripHTML(object inputString, int charactor)
        {
            string str = Regex.Replace(ConvertType.ToString(inputString), HTML_TAG_PATTERN, string.Empty);
            if (str.Length > charactor)
                return str.Substring(0, charactor) + "...";
            return str;
        }
        public static string StripHTML(object inputString)
        {
            string str = Regex.Replace(ConvertType.ToString(inputString), HTML_TAG_PATTERN, string.Empty);
            return str;
        }
        public static string Encode(object str)
        {
            byte[] encbuff = System.Text.Encoding.UTF8.GetBytes(str.ToString());
            return Convert.ToBase64String(encbuff);
        }
        public static string Decode(object str)
        {
            byte[] decbuff = Convert.FromBase64String(str.ToString());
            return System.Text.Encoding.UTF8.GetString(decbuff);
        }
        public static string getTime120(System.DateTime dt)
        {
            int dd = dt.Day;
            int mm = dt.Month;
            int yy = dt.Year;
            int hh = dt.Hour;
            int min = dt.Minute;
            int ss = dt.Second;
            return string.Concat(new string[]
            {
        "convert(datetime,'",
        yy.ToString(),
        "-",
        mm.ToString(),
        "-",
        dd.ToString(),
        " ",
        hh.ToString(),
        ":",
        min.ToString(),
        ":",
        ss.ToString(),
        "',120)"
            });
        }
    }


    public class ExcelAdapter
    {
        protected string sFilePath;
        public string SFilePath {
            get { if (sFilePath == null) return ""; return sFilePath; }
            set { sFilePath = value; }
        }

        public ExcelAdapter(string filePath)
        {
            this.SFilePath = filePath;
        }

        public bool DeleteFile()
        {
            if (File.Exists(this.SFilePath))
            {
                File.Delete(this.SFilePath);
                return true;
            }
            else
                return false;
        }

        public bool IsExist()
        {
            return File.Exists(this.SFilePath);
        }

        public DataTable ReadFromFile(string commandText)
        {
            string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + this.sFilePath + ";Extended Properties=\"Excel 8.0;HDR=YES;\"";



            DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.OleDb");

            DbDataAdapter adapter = factory.CreateDataAdapter();

            DbCommand selectCommand = factory.CreateCommand();
            selectCommand.CommandText = commandText;

            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString = connectionString;

            selectCommand.Connection = connection;

            adapter.SelectCommand = selectCommand;

            DataSet cities = new DataSet();

            adapter.Fill(cities);

            connection.Close();
            adapter.Dispose();

            return cities.Tables[0];
        }

        protected void FormatDate(COMExcel.Worksheet sheet, int rstart, int cstart, int rend, int cend)
        {
            COMExcel.Range range = (COMExcel.Range)sheet.Range[sheet.Cells[rstart, cstart], sheet.Cells[rend, cend]];
            range.NumberFormat = "DD/MM/YYYY";
        }

        protected void FormatMoney(COMExcel.Worksheet sheet, int rstart, int cstart, int rend, int cend)
        {
            COMExcel.Range range = (COMExcel.Range)sheet.Range[sheet.Cells[rstart, cstart], sheet.Cells[rend, cend]];
            range.NumberFormat = "#,##0";
        }

        protected void Format(COMExcel.Worksheet sheet, int rstart, int cstart, int rend, int cend, string type)
        {
            COMExcel.Range range = (COMExcel.Range)sheet.Range[sheet.Cells[rstart, cstart], sheet.Cells[rend, cend]];
            range.NumberFormat = type;
        }

        public string CreateAndWrite(DataTable dt, string sheetName, int noSheet)
        {
            using (new ExcelUILanguageHelper())
            {
                COMExcel.Application exApp = new COMExcel.Application();
                COMExcel.Workbook exBook = exApp.Workbooks.Add(
                              COMExcel.XlWBATemplate.xlWBATWorksheet);
                try
                {
                    // Không hiển thị chương trình excel
                    exApp.Visible = false;

                    // Lấy sheet 1.
                    COMExcel.Worksheet exSheet = (COMExcel.Worksheet)exBook.Worksheets[noSheet];
                    exSheet.Name = sheetName;

                    //////////////////////
                    int rowCount = dt.Rows.Count;
                    int colCount = dt.Columns.Count;

                    // insert header name             
                    for (int j = 1; j <= colCount; j++)
                    {
                        exSheet.Cells[1, j] = dt.Columns[j - 1].Caption;
                    }

                    // format cho header
                    COMExcel.Range headr = (COMExcel.Range)exSheet.Range[exSheet.Cells[1, 1], exSheet.Cells[1, colCount]];
                    headr.Interior.Color = System.Drawing.Color.Gray.ToArgb();
                    headr.Font.Bold = true;
                    headr.Font.Name = "Arial";
                    headr.Font.Color = System.Drawing.Color.White.ToArgb();
                    headr.Cells.RowHeight = 30;
                    headr.Cells.ColumnWidth = 20;
                    headr.HorizontalAlignment = COMExcel.Constants.xlCenter;


                    //format cho cot ngay, tien, so
                    for (int i = 1; i <= colCount; i++)
                    {
                        if (dt.Columns[i - 1].DataType == Type.GetType("System.DateTime"))
                        {
                            FormatDate(exSheet, 2, i, rowCount + 1, i);
                        }
                        else if (dt.Columns[i - 1].DataType == Type.GetType("System.Decimal"))
                        {
                            Format(exSheet, 2, i, rowCount + 1, i, "##0.0");
                        }
                        else if (dt.Columns[i - 1].DataType == Type.GetType("System.Int64"))
                        {
                            FormatMoney(exSheet, 2, i, rowCount + 1, i);
                        }
                        else if (dt.Columns[i - 1].DataType == Type.GetType("System.Int32"))
                        {
                        }
                        else
                        {
                            Format(exSheet, 2, i, rowCount + 1, i, "@");
                        }
                    }
                    for (int i = 1; i <= rowCount; i++)
                    {
                        for (int j = 1; j <= colCount; j++)
                        {
                            exSheet.Cells[i + 1, j] = dt.Rows[i - 1][j - 1].ToString();
                        }
                    }

                    //format cho toan bo sheet
                    COMExcel.Range Sheet = (COMExcel.Range)exSheet.Range[exSheet.Cells[1, 1], exSheet.Cells[rowCount + 1, colCount]];
                    Sheet.Borders.Color = System.Drawing.Color.Black.ToArgb();
                    Sheet.WrapText = true;

                    // Save file
                    exBook.SaveAs(this.SFilePath, COMExcel.XlFileFormat.xlWorkbookNormal,
                                     null, null, false, false,
                                     COMExcel.XlSaveAsAccessMode.xlExclusive,
                                     false, false, false, false, false);


                    return "Export file excel thành công.\nĐường dẫn là: " + this.sFilePath;
                }
                catch (Exception ex)
                {
                    Thread.CurrentThread.CurrentCulture.DateTimeFormat = new System.Globalization.CultureInfo("en-US").DateTimeFormat;
                    return ex.ToString();
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture.DateTimeFormat = new System.Globalization.CultureInfo("en-US").DateTimeFormat;
                    // Đóng chương trình
                    exBook.Close(false, false, false);
                    exApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(exBook);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(exApp);
                }
            }
        }

        public string CreateAndWrite(DataTable[] dtList, string[] sheetNames)
        {
            using (new ExcelUILanguageHelper())
            {
                COMExcel.Application exApp = new COMExcel.Application();
                COMExcel.Workbook exBook = exApp.Workbooks.Add(
                              COMExcel.XlWBATemplate.xlWBATWorksheet);
                try
                {
                    // Không hiển thị chương trình excel
                    exApp.Visible = false;

                    //List<COMExcel.Worksheet> exSheetList = new List<Microsoft.Office.Interop.Excel.Worksheet>();
                    for (int i = 1; i < dtList.Length; i++)
                    {
                        //exSheetList.Add((COMExcel.Worksheet)exBook.Worksheets[i]);
                        exBook.Worksheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    }
                    int noSheet = 1;
                    foreach (DataTable dt in dtList)
                    {
                        COMExcel.Worksheet exSheet = (COMExcel.Worksheet)exBook.Worksheets[noSheet];
                        exSheet.Name = sheetNames[noSheet - 1];

                        //////////////////////
                        int rowCount = dt.Rows.Count;
                        int colCount = dt.Columns.Count;

                        // insert header name             
                        for (int j = 1; j <= colCount; j++)
                        {
                            exSheet.Cells[1, j] = dt.Columns[j - 1].Caption;
                        }

                        // format cho header
                        COMExcel.Range headr = (COMExcel.Range)exSheet.Range[exSheet.Cells[1, 1], exSheet.Cells[1, colCount]];
                        headr.Interior.Color = System.Drawing.Color.Gray.ToArgb();
                        headr.Font.Bold = true;
                        headr.Font.Name = "Arial";
                        headr.Font.Color = System.Drawing.Color.White.ToArgb();
                        headr.Cells.RowHeight = 30;
                        headr.Cells.ColumnWidth = 20;
                        headr.HorizontalAlignment = COMExcel.Constants.xlCenter;


                        //format cho cot ngay, tien, so
                        for (int i = 1; i <= colCount; i++)
                        {
                            if (dt.Columns[i - 1].DataType == Type.GetType("System.DateTime"))
                            {
                                FormatDate(exSheet, 2, i, rowCount + 1, i);
                            }
                            else if (dt.Columns[i - 1].DataType == Type.GetType("System.Decimal"))
                            {
                                Format(exSheet, 2, i, rowCount + 1, i, "##0.0");
                            }
                            else if (dt.Columns[i - 1].DataType == Type.GetType("System.Int64"))
                            {
                                FormatMoney(exSheet, 2, i, rowCount + 1, i);
                            }
                            else if (dt.Columns[i - 1].DataType == Type.GetType("System.Int32"))
                            {
                            }
                            else
                            {
                                Format(exSheet, 2, i, rowCount + 1, i, "@");
                            }
                        }

                        for (int i = 1; i <= rowCount; i++)
                        {
                            for (int j = 1; j <= colCount; j++)
                            {
                                exSheet.Cells[i + 1, j] = dt.Rows[i - 1][j - 1].ToString();
                            }
                        }

                        //format cho toan bo sheet
                        COMExcel.Range Sheet = (COMExcel.Range)exSheet.Range[exSheet.Cells[1, 1], exSheet.Cells[rowCount + 1, colCount]];
                        Sheet.Borders.Color = System.Drawing.Color.Black.ToArgb();
                        Sheet.WrapText = true;

                        noSheet++;
                    }
                    // Save file
                    exBook.SaveAs(this.SFilePath, COMExcel.XlFileFormat.xlWorkbookNormal,
                                    null, null, false, false,
                                    COMExcel.XlSaveAsAccessMode.xlExclusive,
                                    false, false, false, false, false);


                    return "Export file excel thành công.\nĐường dẫn là: " + this.sFilePath;
                }
                catch (Exception ex)
                {
                    Thread.CurrentThread.CurrentCulture.DateTimeFormat = new System.Globalization.CultureInfo("en-US").DateTimeFormat;
                    return ex.ToString();
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture.DateTimeFormat = new System.Globalization.CultureInfo("en-US").DateTimeFormat;
                    // Đóng chương trình
                    exBook.Close(false, false, false);
                    exApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(exBook);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(exApp);
                }
            }
        }

        public class ExcelUILanguageHelper : IDisposable
        {
            private System.Globalization.CultureInfo m_CurrentCulture;

            public ExcelUILanguageHelper()
            {
                // save current culture and set culture to en-US            
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                m_CurrentCulture = Thread.CurrentThread.CurrentCulture;
                m_CurrentCulture.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";
            }

            #region IDisposable Members

            public void Dispose()
            {
                // return to normal culture
                Thread.CurrentThread.CurrentCulture = m_CurrentCulture;
            }

            #endregion
        }

    }
}
