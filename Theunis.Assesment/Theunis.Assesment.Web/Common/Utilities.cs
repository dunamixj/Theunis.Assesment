namespace Theunis.Assesment.Web.Common
{
    public class Utilities
    {
        public static string logFilePath = Directory.GetCurrentDirectory() + "\\AuditLogs\\";
        public static string ErrorlogFilePath = Directory.GetCurrentDirectory() + "\\ErrorLogs\\";

        #region Logs
        public static void CreateLog(string sErrMsg)
        {
            //sLogFormat used to create log files format :
            // dd/mm/yyyy hh:mm:ss AM/PM ==> Log Message
            var sLogFormat = DateTime.Now.ToString("dd/MM/yyyy").ToString() + " " + DateTime.Now.ToLongTimeString() + " ==> ";

            //this variable used to create log filename format "
            //for example filename : ErrorLogYYYYMMDD
            string sPathName;
            string sYear = DateTime.Now.Year.ToString();
            string sMonth = DateTime.Now.Month.ToString();
            string sDay = DateTime.Now.Day.ToString();
            var sErrorTime = sYear + sMonth + sDay;

            sPathName = (logFilePath + sErrorTime + ".txt");
            //File exists or not
            DirectoryInfo info = new DirectoryInfo(logFilePath);
            if (!info.Exists)
            {
                info.Create();
            }
            StreamWriter sw;
            if (!File.Exists(sPathName))
            {
                sw = File.CreateText(sPathName);
            }
            else
            {
                sw = File.AppendText(sPathName);

            }
            sw.WriteLine(sLogFormat + sErrMsg);
            sw.Flush();
            sw.Close();
        }
        #endregion
    }
}
