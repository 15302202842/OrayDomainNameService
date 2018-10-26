using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrayDomainNameService
{
    public partial class OrayDomainNameService : ServiceBase
    {
        public OrayDomainNameService()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            string path = HttpClientHelper.GetWindowsServiceInstallPath();
            string filePath = path + "\\Log.txt";
            using (FileStream stream = new FileStream(filePath, FileMode.Append))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine($"{DateTime.Now},服务启动！");
            }
            Thread thread = new Thread(BindDomainNameToIP);
            thread.IsBackground = true;
            if(args.Length == 0)
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Append))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine($"{DateTime.Now},请阅读readme.txt文件，按格式输入启动参数！");
                }
                OnStop();
            }
            string pram = args[0];
            thread.Start(pram);
        }

        public void BindDomainNameToIP(object obj)
        {
            try
            {
                string pram = obj as string;
                string[] data = pram.Split('|');
                string userName = data[0];//花生壳账号名
                string pwd = data[1];//花生壳账号密码
                string domainName = data[2];//需要绑定的域名
                string IP = data[3];//域名要绑定的IP地址
                string url = string.Format("http://{0}:{1}@ddns.oray.com/ph/update?hostname={2}&myip={3}", userName, pwd, domainName, IP);
                HttpWebResponse result;
                while (true)
                {
                    result = HttpClientHelper.SendGet(url, userName, pwd);
                    while (result == null || result.StatusCode != HttpStatusCode.OK)
                    {
                        Thread.Sleep(60000);//睡眠1分钟
                        result = HttpClientHelper.SendGet(url, userName, pwd);
                    }
                    Thread.Sleep(600000);//睡眠10分钟
                }
            }
            catch(Exception ex)
            {

            }
        }

        protected override void OnStop()
        {
            string path = HttpClientHelper.GetWindowsServiceInstallPath();
            string filePath = path + "\\Log.txt";
            using (FileStream stream = new FileStream(filePath, FileMode.Append))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine($"{DateTime.Now},服务停止！");
            }
        }
    }
}
