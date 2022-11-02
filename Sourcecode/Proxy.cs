using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using xNet;
using Console = Colorful.Console;
using System.Drawing;

namespace EvilChecker
{
    class Proxy
    {
        public static void ProxyChecker()
        {
            while (true)
            {
                String responString;
                string proxy;
                int proxymode;
                Proxy.Get_Proxy_By_List(out proxy, out proxymode);
                lock (MainChecker.locker2)
                {
                    MainChecker.cnum++;
                    if (MainChecker.cnum > MainChecker.proxynum_all)
                    {
                        MainChecker.threadnum--;
                        break;
                    }
                }
                try
                {
                    HttpRequest http = new HttpRequest();
                    http.IgnoreProtocolErrors = true;
                    http.KeepAlive = true;
                    switch (proxymode)
                    {
                        case 1:
                            break;
                        case 2:
                            http.Proxy = HttpProxyClient.Parse(proxy);
                            break;
                        case 3:
                            http.Proxy = Socks4ProxyClient.Parse(proxy);
                            break;
                        case 4:
                            http.Proxy = Socks5ProxyClient.Parse(proxy);
                            break;
                        default:
                            break;
                    }
                    http.ConnectTimeout = MainChecker.timeout;
                    http.KeepAliveTimeout = MainChecker.timeout;
                    http.ReadWriteTimeout = MainChecker.timeout;
                    http.AddHeader(HttpHeader.Accept, "*/*");
                    http.AddHeader(HttpHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1)");
                    responString = http.Get("https://www.google.net").ToString();
                }
                catch
                {
                    responString = "";
                }
                if (responString != "") //Working proxy
                {
                    MainChecker.hit++;
                    MainChecker.checkeds++;
                    Console.Title = "EvilChecker - Checked:" + MainChecker.checkeds + " - All: " + MainChecker.proxynum_all + " - Working:" + MainChecker.hit + " - Elapse:" + MainChecker.hour + ":" + MainChecker.minute + ":" + MainChecker.second + " - CPM:" + MainChecker.cpm;
                    switch (proxymode)
                    {
                        case 2:
                            Console.WriteLine("[Working HTTP]" + proxy, Color.Yellow);
                            while (true)
                            {
                                try
                                {
                                    File.AppendAllText(MainChecker.directory + "\\HTTP.txt", proxy + "\n");
                                    break;
                                }
                                catch
                                {

                                }
                            }
                            break;
                        case 3:
                            Console.WriteLine("[Working SOCKS4]" + proxy, Color.Yellow);
                            while (true)
                            {
                                try
                                {
                                    File.AppendAllText(MainChecker.directory + "\\SOCKS4.txt", proxy + "\n");
                                    break;
                                }
                                catch
                                {

                                }
                            }
                            break;
                        case 4:
                            Console.WriteLine("[Working SOCKS5]" + proxy, Color.Yellow);
                            while (true)
                            {
                                try
                                {
                                    File.AppendAllText(MainChecker.directory + "\\SOCKS5.txt", proxy + "\n");
                                    break;
                                }
                                catch
                                {

                                }
                            }
                            break;
                    }
                }
                else
                {
                    switch (proxymode)
                    {
                        case 2:
                            Console.WriteLine("[Bad HTTP]" + proxy, Color.Red);
                            break;
                        case 3:
                            Console.WriteLine("[Bad SOCKS4]" + proxy, Color.Red);
                            break;
                        case 4:
                            Console.WriteLine("[Bad SOCKS5]" + proxy, Color.Red);
                            break;
                    }
                    MainChecker.checkeds++;
                    Console.Title = "EvilChecker - Checked:" + MainChecker.checkeds + " - All: " + MainChecker.proxynum_all + " - Working:" + MainChecker.hit + " - Elapse:" + MainChecker.hour + ":" + MainChecker.minute + ":" + MainChecker.second + " - CPM:" + MainChecker.cpm;
                }
            }
            
        }
        public static void ProxyGrabber()
        {
            while (true)
            {
                String responString;
                string url;
                try
                {
                    lock (MainChecker.locker1)
                    {
                        url = MainChecker.urllist[MainChecker.cnum].Replace("\r", "");
                        MainChecker.cnum++;
                    }
                }
                catch
                {
                    MainChecker.threadnum--;
                    break;
                }
                try
                {
                    HttpRequest http = new HttpRequest();
                    http.IgnoreProtocolErrors = true;
                    http.KeepAlive = true;
                    http.ConnectTimeout = MainChecker.timeout;
                    http.KeepAliveTimeout = MainChecker.timeout;
                    http.ReadWriteTimeout = MainChecker.timeout;
                    http.AddHeader(HttpHeader.Accept, "*/*");
                    http.AddHeader(HttpHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1)");
                    responString = http.Get(url).ToString();
                }
                catch
                {
                    responString = "";
                }
                MainChecker.checkeds++;
                if (responString != "")
                {
                    string proxy = GetResult(responString).TrimEnd();
                    if (proxy != "")
                    {
                        Console.WriteLine("[ProxyGrabber]add " + proxy.Split('\n').Length + " proxies - " + url, Color.Aqua);
                    }
                    else
                    {
                        Console.WriteLine("[No Proxy found]" + url, Color.Red);
                    }
                    while (true)
                    {
                        try
                        {
                            using (StreamWriter sw = new StreamWriter(MainChecker.directory + "\\ProxiesGrabbed.txt", true))
                            {
                                sw.WriteLine(proxy);
                            }
                            break;
                        }
                        catch
                        {

                        }
                    }
                }
                else
                {
                    Console.WriteLine("[BadURL]" + url, Color.Red);
                }
            }
        }
        private static string GetResult(string response)
        {
            string text = "";
            MatchCollection mc = Regex.Matches(response, "(\\d{1,3}\\.){3}\\d{1,3}\\:\\d{1,5}");
            foreach (Match m in mc)
            {
                text = text + m + "\n";
            }
            return Somemould.RemoveBadLines(text) + "\n";
        }
        public static void Get_Proxy_By_List(out string proxy,out int proxymode)
        {
            if (MainChecker.proxymode == "2")
            {
                proxy = "";
                proxymode = 0;
                lock (MainChecker.getproxy_locker)
                {
                getproxy:
                    switch (MainChecker.cproxytype)
                    {
                        case 2:
                            proxymode = 2;
                            if (MainChecker.cproxynum == (MainChecker.proxynum_HTTP))
                            {
                                MainChecker.cproxynum = 0;
                                MainChecker.cproxytype++;
                                Check_if_proxy_prepared();
                                goto getproxy;
                            }
                            proxy = MainChecker.proxyList_HTTP[MainChecker.cproxynum].TrimEnd();
                            break;
                        case 3:
                            proxymode = 3;
                            if (MainChecker.cproxynum == (MainChecker.proxynum_SOCKS4))
                            {
                                MainChecker.cproxynum = 0;
                                MainChecker.cproxytype++;
                                Check_if_proxy_prepared();
                                goto getproxy;
                            }
                            proxy = MainChecker.proxyList_SOCKS4[MainChecker.cproxynum].TrimEnd();
                            break;
                        case 4:
                            proxymode = 4;
                            if (MainChecker.cproxynum == (MainChecker.proxynum_SOCKS5))
                            {
                                MainChecker.cproxynum = 0;
                                MainChecker.cproxytype = 2;
                                Check_if_proxy_prepared();
                                goto getproxy;
                            }
                            proxy = MainChecker.proxyList_SOCKS5[MainChecker.cproxynum].TrimEnd();
                            break;
                    }
                    MainChecker.cproxynum++;
                }
            }
            else
            {
                proxy = "";
                proxymode = 1;
            }

        }
        private static void Check_if_proxy_prepared()
        {
            switch (MainChecker.cproxytype)
            {
                case 2:
                    if (MainChecker.proxynum_HTTP == 0)
                    {
                        MainChecker.cproxytype++;
                    }
                    if (MainChecker.cproxytype == 3 & MainChecker.proxynum_SOCKS4 == 0)
                    {
                        MainChecker.cproxytype++;
                    }
                    break;
                case 3:
                    if (MainChecker.proxynum_SOCKS4 == 0)
                    {
                        MainChecker.cproxytype++;
                    }
                    if (MainChecker.cproxytype == 4 & MainChecker.proxynum_SOCKS5 == 0)
                    {
                        MainChecker.cproxytype = 2;
                    }
                    break;
                case 4:
                    if (MainChecker.proxynum_SOCKS5 == 0)
                    {
                        MainChecker.cproxytype = 2;
                    }
                    if (MainChecker.cproxytype == 2 & MainChecker.proxynum_HTTP == 0)
                    {
                        MainChecker.cproxytype++;
                    }
                    break;
            }
        }
    }
}
