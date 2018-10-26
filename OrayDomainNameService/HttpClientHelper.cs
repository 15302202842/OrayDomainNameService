using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OrayDomainNameService
{
    public class HttpClientHelper
    {
        public static string GetWindowsServiceInstallPath()
        {
            string key = @"SYSTEM\CurrentControlSet\Services\OrayDomainNameService";
            string path = Registry.LocalMachine.OpenSubKey(key).GetValue("ImagePath").ToString();
            //替换掉双引号   
            path = path.Replace("\"", string.Empty);

            FileInfo fi = new FileInfo(path);
            return fi.Directory.ToString();

        }
        public static HttpWebResponse SendGet(string url,string userName,string pwd)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

                NetworkCredential d = new NetworkCredential(userName, pwd);//添加此代码

                req.Credentials = d;
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                return resp;
            }
            catch(Exception ex)
            {
                //string path = GetWindowsServiceInstallPath();
                //string filePath = path + "\\Log.txt";
                //using (FileStream stream = new FileStream(filePath, FileMode.Append))
                //using (StreamWriter writer = new StreamWriter(stream))
                //{
                //    writer.WriteLine($"{DateTime.Now},请求出错，请检查网络或启动参数是否有误,请阅读readme.txt文件！");
                //}
                return null;
            }
        }
    }
}
