using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using Console = Colorful.Console;
using System.Collections;
using System.Text.RegularExpressions;
using EvilChecker.Microsoft;
using EvilChecker.Mojang;
using xNet;
namespace EvilChecker
{
    class MainChecker
    {
        public static string license;
        public static int threads;
        public static int recheck;
        public static bool printall;
        //Normal Settings
        public static string proxymode;
        public static bool check_via_proxy;
        //Proxy Settings
        public static bool check_after_grabbed;
        public static int threads_for_grabber;
        //Proxy Grabber settings
        public static int timeout;
        public static bool check_after_proxy_checked;
        //Proxy checker settings

        public static bool check_hypixel_level;
        public static bool check_hypixel_rank;
        public static bool check_hypixel_GameStats;
        public static bool check_hypixel_Skyblock;

        public static bool check_optifine;
        public static bool check_minecon;
        public static bool check_ogname;
        public static bool check_ban;
        private static bool remove_duplicate_lines;
        public static bool double_character;
        public static int hypixel_API;
        public static int hypixel_level_stage1;
        public static int hypixel_level_stage2;
        public static int hypixel_uhcminlevel;
        public static int hypixel_bedwarsminlevel;
        public static int hypixel_skywarsminlevel;
        public static int hypixel_skyblock_mincoin;
        public static int hypixel_skyblock_minsouls;
        public static int hypixel_skyblock_minkills;
        public static string ognamelist;
        //Sever stat settings
        public static string[] comboList;
        public static string[] proxyList_HTTP;
        public static string[] proxyList_SOCKS4;
        public static string[] proxyList_SOCKS5;
        public static string[] urllist;
        public static string[] special_characters;
        public static string[] blocked_character;

        public static int combonum;
        public static int proxynum_HTTP = 0;
        public static int proxynum_SOCKS4 = 0;
        public static int proxynum_SOCKS5 = 0;
        public static int proxynum_all = 0;
        public static int urlnum;
        public static string hitSorter;
        public static string[] hitSorter_;
        public static string hit_email = "";
        //other settings
        public static int hit = 0;
        public static int checkeds = 0;
        public static int cpm = 0;
        public static int second = 0;
        public static int minute = 0;
        public static int hour = 0;
        public static int cnum = 0;
        public static int threadnum = 0;
        public static int cproxynum = 0;
        public static int cproxynum_banchecker = 0;
        public static int cproxytype = 2;
        public static ArrayList cpmlist = new ArrayList();
        public static string directory;
        //Checker stat
        public static readonly object locker1 = new object();
        public static readonly object locker2 = new object();
        public static readonly object getproxy_locker = new object();
        public static readonly object locker_Save_account = new object();
        private static string HttpLink;
        private static string Socks4Link;
        private static string Socks5Link;
        private static int load_delay;
        //lock
        static void Main(string[] args)
        {
            //Check dnSpy
            Console.Title = "EvilChecker By Rhythm - LastUpdate:2021.08.23 - Version:2.4.1 - Paid version - SuperPowerful Minecraft Checker";
            Console.WriteLine(@"
      ███████╗██╗   ██╗██╗██╗          ██████╗██╗  ██╗███████╗ ██████╗██╗  ██╗███████╗██████╗ 
      ██╔════╝██║   ██║██║██║         ██╔════╝██║  ██║██╔════╝██╔════╝██║ ██╔╝██╔════╝██╔══██╗
      █████╗  ██║   ██║██║██║         ██║     ███████║█████╗  ██║     █████╔╝ █████╗  ██████╔╝
      ██╔══╝  ╚██╗ ██╔╝██║██║         ██║     ██╔══██║██╔══╝  ██║     ██╔═██╗ ██╔══╝  ██╔══██╗
      ███████╗ ╚████╔╝ ██║███████╗    ╚██████╗██║  ██║███████╗╚██████╗██║  ██╗███████╗██║  ██║
      ╚══════╝  ╚═══╝  ╚═╝╚══════╝     ╚═════╝╚═╝  ╚═╝╚══════╝ ╚═════╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝
",Color.White);

            List<char> loading = new List<char>()
            {
              'L', 'o', 'a', 'd', 'i', 'n', 'g','.','.','.',
            };
            Console.WriteWithGradient(loading, Color.Blue, Color.Fuchsia, 16);
            Console.Write("\n");
            LoadConfig();
            //print loading
            Checklicense();
            //Check license key
            int mode = 0;
            do
            {
                Console.Clear();
                Console.WriteLine(@"
      ███████╗██╗   ██╗██╗██╗          ██████╗██╗  ██╗███████╗ ██████╗██╗  ██╗███████╗██████╗ 
      ██╔════╝██║   ██║██║██║         ██╔════╝██║  ██║██╔════╝██╔════╝██║ ██╔╝██╔════╝██╔══██╗
      █████╗  ██║   ██║██║██║         ██║     ███████║█████╗  ██║     █████╔╝ █████╗  ██████╔╝
      ██╔══╝  ╚██╗ ██╔╝██║██║         ██║     ██╔══██║██╔══╝  ██║     ██╔═██╗ ██╔══╝  ██╔══██╗
      ███████╗ ╚████╔╝ ██║███████╗    ╚██████╗██║  ██║███████╗╚██████╗██║  ██╗███████╗██║  ██║
      ╚══════╝  ╚═══╝  ╚═╝╚══════╝     ╚═════╝╚═╝  ╚═╝╚══════╝ ╚═════╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝
", Color.White);
                Console.WriteLine(@"[1]Mojang-Minecraft - Check for NFA，SFA，Hypixel，Mineplex，Cape, HypixelBan
[2]Microsoft-Minecraft - Check for NFA，SFA，Hypixel，Mineplex，Cape, HypixelBan
[3]ProxyChecker - Check your proxies to make it better for checking
[4]ProxyGrabber - Help you grab proxy form any websites
[5]HitRemover - Remove hits from your combolist
[6]ComboTool - A great tool to edit your combo
Which you want to use<<", Color.Aqua);//print modes

                try
                {
                    mode = Convert.ToInt32(Console.ReadLine());//Choose mode
                }
                catch
                {
                    Console.WriteLine("Invaild mode!", Color.Red);
                    Thread.Sleep(2000);
                    mode = 0;
                    continue;
                }
                switch (mode)
                {
                    case 1:
                        Console.Clear();
                        LoadCombo();
                        LoadProxy();
                        Minecraft_mojang.ReadOGnameList();
                        MainChecker.hitSorter_ = Somemould.RemoveBadLines(MainChecker.hitSorter).Split('\n');

                        if(proxymode != "2")
                        {
                            check_ban = false;
                            Console.WriteLine("[Warning]You don't choose a mode which uses proxy to check,banchecker will be shut down", Color.Red);
                            Thread.Sleep(2000);
                        }
                        Console.WriteLine("Start Checking...", Color.Yellow);
                        Thread.Sleep(2000);
                        Console.Clear();
                        Task.Factory.StartNew(() =>
                        {
                            for (int a = 0; a < threads; a++)
                            {
                                threadnum++;
                                new Thread(new ThreadStart(Minecraft_mojang.MinecraftChecker)).Start();
                                Thread.Sleep(10);
                            }
                        });
                        while(threadnum == 0)
                        {
                            Thread.Sleep(1000);
                        }
                        break;
                    case 2:
                        LoadProxy();
                        Minecraft_microsoft.Minecraft_Checking();
                        break;
                    //case 2:
                    //    Console.Clear();
                    //    LoadCombo();
                    //    LoadProxy();
                    //    Minecraft_mojang.ReadOGnameList();
                    //    MainChecker.hitSorter_ = Somemould.RemoveBadLines(MainChecker.hitSorter).Split('\n');
                    //    if (proxynum_HTTP == 0)
                    //    {
                    //        check_ban = false;
                    //        Console.WriteLine("[Warning]You don't have any HTTP proxy,banchecker will be shut down", Color.Red);
                    //        Thread.Sleep(2000);
                    //    }
                    //    if (proxymode != "2")
                    //    {
                    //        check_ban = false;
                    //    }
                    //    Console.WriteLine("Start Checking...", Color.Yellow);
                    //    Thread.Sleep(2000);
                    //    Console.Clear();
                    //    Task.Factory.StartNew(() =>
                    //    {
                    //        for (int a = 0; a < threads; a++)
                    //        {
                    //            threadnum++;
                    //            new Thread(new ThreadStart(Minecraft_microsoft.MinecraftChecker)).Start();
                    //            Thread.Sleep(10);
                    //        }
                    //    });
                    //    Thread.Sleep(1000);
                    //    break;
                    case 3:
                        Console.Clear();
                        proxymode = "2";
                        LoadProxy();
                        Console.WriteLine("Start Checking...", Color.Yellow);
                        Thread.Sleep(2000);
                        Console.Clear();
                        Task.Factory.StartNew(() =>
                        {
                            for (int a = 0; a < threads; a++)
                            {
                                threadnum++;
                                new Thread(new ThreadStart(Proxy.ProxyChecker)).Start();
                                Thread.Sleep(10);
                            }
                        });
                        Thread.Sleep(1000);
                        break;
                    case 4:
                        Console.Clear();
                        LoadURL();
                        Console.WriteLine("Start Grabbing...", Color.Yellow);
                        Thread.Sleep(2000);
                        Console.Clear();
                        if(threads > urlnum)
                        {
                            if(urlnum <= 50)
                            {
                                threads = 5;
                            }else
                            {
                                threads = 10;
                            }
                        }
                        Task.Factory.StartNew(() =>
                        {
                            for (int a = 0; a < threads; a++)
                            {
                                threadnum++;
                                new Thread(new ThreadStart(Proxy.ProxyGrabber)).Start();
                            }
                        });
                        Thread.Sleep(1000);
                        break;
                    case 5:
                        Console.Clear();
                        HitRemover.Hitremover();
                        break;
                    case 6:
                        ComboTool_Menu();
                        break;
                    default:
                        Console.WriteLine("Invaild mode!", Color.Red);
                        Thread.Sleep(2000);
                        mode = 0;
                        continue;
                }

            } while (mode == 0);
            int num = 0;
            while (true)
            {
                if (num < 60)
                {
                    cpmlist.Add(checkeds);
                    try
                    {
                        cpm = Convert.ToInt32(Convert.ToDouble(cpmlist[num]) / second * 60);
                    }
                    catch
                    {

                    }
                    num++;
                }
                else
                {
                    cpmlist.Add(checkeds);
                    cpmlist.RemoveAt(0);
                    cpm = Convert.ToInt32(cpmlist[59]) - Convert.ToInt32(cpmlist[0]);
                }
                second++;
                if (second == 60)
                {
                    second = 0;
                    minute++;
                }
                if (minute == 60)
                {
                    minute = 0;
                    hour++;
                }
                switch (mode)
                {
                    case 1:
                        Console.Title = "EvilChecker - Checked:" + checkeds + " - All: " + combonum + " - Hits:" + hit + " - Elapse:" + hour + ":" + minute + ":" + second + " - CPM:" + cpm + " - CurtainThreads:" + threadnum;
                        break;
                    case 2:
                        Console.Title = "EvilChecker - Checked:" + checkeds + " - All: " + combonum + " - Hits:" + hit + " - Elapse:" + hour + ":" + minute + ":" + second + " - CPM:" + cpm;
                        break;
                    case 3:
                        Console.Title = "EvilChecker - Checked:" + checkeds + " - All: " + proxynum_all + " - Working:" + hit + " - Elapse:" + hour + ":" + minute + ":" + second + " - CPM:" + cpm;
                        break;
                }
                if (threadnum == 0)
                {
                    break;
                }
                Thread.Sleep(1000);
            }
            if(mode == 4) //Remove duplicate proxies for proxyGrabber
            {
                Console.WriteLine("[ProxyGrabber]Removing duplicate lines", Color.Green);
                string proxy = Somemould.RemoveBadLines(Somemould.Removeduplicate(MainChecker.directory + "\\ProxiesGrabbed.txt"));
                Console.WriteLine("[ProxyGrabber]Grabbed " + Somemould.Getlines(proxy) + " lines", Color.YellowGreen);
            }
            Console.WriteLine("Checking over.Thanks for using my checker!",Color.Purple);
            Console.ReadKey(true);
            Environment.Exit(0);
            //Choose a mode
        }
        private static void ComboTool_Menu()
        {
            int mode = 0;
            do
            {
                Console.Clear();
                Console.WriteLine(@"
      ███████╗██╗   ██╗██╗██╗          ██████╗██╗  ██╗███████╗ ██████╗██╗  ██╗███████╗██████╗ 
      ██╔════╝██║   ██║██║██║         ██╔════╝██║  ██║██╔════╝██╔════╝██║ ██╔╝██╔════╝██╔══██╗
      █████╗  ██║   ██║██║██║         ██║     ███████║█████╗  ██║     █████╔╝ █████╗  ██████╔╝
      ██╔══╝  ╚██╗ ██╔╝██║██║         ██║     ██╔══██║██╔══╝  ██║     ██╔═██╗ ██╔══╝  ██╔══██╗
      ███████╗ ╚████╔╝ ██║███████╗    ╚██████╗██║  ██║███████╗╚██████╗██║  ██╗███████╗██║  ██║
      ╚══════╝  ╚═══╝  ╚═╝╚══════╝     ╚═════╝╚═╝  ╚═╝╚══════╝ ╚═════╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝
", Color.White);
                Console.WriteLine(@"[1]Convert Email:Pass To User:Pass
[2]Combo Editor - Make your combo better for minecraft
Which you want to use<<", Color.Aqua);//print modes
                try
                {
                    mode = Convert.ToInt32(Console.ReadLine());//Choose mode
                }
                catch
                {
                    Console.WriteLine("Invaild mode!", Color.Red);
                    Thread.Sleep(2000);
                    mode = 0;
                    continue;
                }
                switch (mode)
                {
                    case 1:
                        LoadCombo();
                        ComboTool.Convert_EmailToUser();
                        break;
                    case 2:
                        ComboTool.Combo_Gaming();
                        break;
                    default:
                        Console.WriteLine("Invaild mode!", Color.Red);
                        Thread.Sleep(2000);
                        mode = 0;
                        continue;
                }
            } while (mode == 0);
        }
        private static void Checklicense()
        {
            string hwid = Somemould.GetHWID();
            string result;
            try
            {
                HttpRequest http = new HttpRequest();
                http.IgnoreProtocolErrors = true;
                http.KeepAlive = true;
                http.AddHeader(HttpHeader.Accept, "*/*");
                http.AddHeader(HttpHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1)");
               
                if (File.ReadAllText(@"C:\Windows\System32\drivers\etc\hosts").ToLower().Contains("gitee.com"))
                {
                    Environment.Exit(0);
                }
                result = http.Get("http://42.159.86.85:81/shequ/license.txt").ToString();
            }
            catch
            {
                result = "";
            }
            if (result.Contains(hwid))
            {
                Console.WriteLine("[√]Welcome!Your are using the latest version of evilchecker", Color.Gold);
                Thread.Sleep(2000);
            }
            else
            {
                Console.WriteLine("[×]Your HWID is not vaild - " + hwid, Color.Red);
                Console.ReadKey(true);
                Environment.Exit(0);
            }
        }
        private static void LoadConfig()
        {
            string config;
            try
            {
                config = File.ReadAllText(Directory.GetCurrentDirectory() + "\\config\\config.yml");
            }
            catch
            {
                Console.WriteLine("Error! invaild config!", Color.Red);
                Console.ReadKey();
                return;
            }
            //Read Config
            license = Somemould.MD5Encode(Somemould.Getsubstring(config, "License:[", "]"));
            hitSorter = Somemould.Getsubstring(config, "HitSorter:[", "]");
            proxymode = Somemould.Getsubstring(config, "Proxy mode:[", "]");
            special_characters = Somemould.Getsubstring(config, "special_characters for Combo Editor:[", "]").Split(';');
            HttpLink = Somemould.Getsubstring(config, "HTTP Link:[", "]");
            Socks4Link = Somemould.Getsubstring(config, "SOCKS4 Link:[", "]");
            Socks5Link = Somemould.Getsubstring(config, "SOCKS5 Link:[", "]");
            blocked_character = Somemould.Getsubstring(config, "Blocked characters for Combo Editor:[", "]").Split(';');
            //String settings
            try
            {
                threads = Convert.ToInt32(Somemould.Getsubstring(config, "Threads:[", "]"));
                recheck = Convert.ToInt32(Somemould.Getsubstring(config, "Recheck:[", "]"));
                timeout = Convert.ToInt32(Somemould.Getsubstring(config, "Time out:[", "]")) * 1000;
                threads_for_grabber = Convert.ToInt32(Somemould.Getsubstring(config, "Threads for proxy grabber:[", "]"));
                hypixel_API = Convert.ToInt32(Somemould.Getsubstring(config, "Hypixel API:[", "]"));
                hypixel_level_stage1 = Convert.ToInt32(Somemould.Getsubstring(config, "Hypixel level stage_1:[", "]"));
                hypixel_level_stage2 = Convert.ToInt32(Somemould.Getsubstring(config, "Hypixel level stage_2:[", "]"));
                hypixel_skyblock_mincoin = Convert.ToInt32(Somemould.Getsubstring(config, "Hypixel skyblock coins:[", "]"));
                hypixel_skyblock_minsouls = Convert.ToInt32(Somemould.Getsubstring(config, "Hypixel skyblock fairy_souls:[", "]"));
                hypixel_skyblock_minkills = Convert.ToInt32(Somemould.Getsubstring(config, "Hypixel skyblock kills:[", "]"));
                hypixel_uhcminlevel = Convert.ToInt32(Somemould.Getsubstring(config, "Hypixel UHC level:[", "]"));
                hypixel_bedwarsminlevel = Convert.ToInt32(Somemould.Getsubstring(config, "Hypixel Bedwars level:[", "]"));
                hypixel_skywarsminlevel = Convert.ToInt32(Somemould.Getsubstring(config, "Hypixel Skywars level:[", "]"));
                load_delay = Convert.ToInt32(Somemould.Getsubstring(config, "Load delay:[", "]")) * 60 * 1000;
            }
            catch
            {
                Console.WriteLine("Error! invaild config!", Color.Red);
                Console.ReadKey();
                return;
            }
            //int settings
            try
            {
                printall = Convert.ToBoolean(Somemould.Getsubstring(config, "Printout Failed accounts:[", "]"));
                check_via_proxy = Convert.ToBoolean(Somemould.Getsubstring(config, "Check rank level cape via proxy:[", "]"));
                check_after_grabbed = Convert.ToBoolean(Somemould.Getsubstring(config, "Check proxies after grabbed:[", "]"));
                check_after_proxy_checked = Convert.ToBoolean(Somemould.Getsubstring(config, "Check accounts after check proxies:[", "]"));
                check_hypixel_level = Convert.ToBoolean(Somemould.Getsubstring(config, "Check for Level:[", "]"));
                check_hypixel_rank = Convert.ToBoolean(Somemould.Getsubstring(config, "Check for Ranks:[", "]"));
                check_hypixel_GameStats = Convert.ToBoolean(Somemould.Getsubstring(config, "Check for GameStats:[", "]"));
                check_hypixel_Skyblock = Convert.ToBoolean(Somemould.Getsubstring(config, "Check for Skyblock:[", "]"));
                
                check_minecon = Convert.ToBoolean(Somemould.Getsubstring(config, "Check for Minecon:[", "]"));
                check_ogname = Convert.ToBoolean(Somemould.Getsubstring(config, "Check for OGname:[", "]"));
                check_optifine = Convert.ToBoolean(Somemould.Getsubstring(config, "Check for Optifine:[", "]"));
                check_ban = Convert.ToBoolean(Somemould.Getsubstring(config, "EnableBanChecker:[", "]"));
                remove_duplicate_lines = Convert.ToBoolean(Somemould.Getsubstring(config, "Remove duplicate lines:[", "]"));
                double_character= Convert.ToBoolean(Somemould.Getsubstring(config, "Double edit first character:[", "]"));
            }
            catch
            {
                Console.WriteLine("Error! invaild config!", Color.Red);
                Console.ReadKey();
                return;
            }
            //bool settings
            directory = Directory.GetCurrentDirectory() + "\\Results\\" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + "\\";
            Directory.CreateDirectory(directory);
        }
        private static void LoadCombo()//Load combo
        {
            if (remove_duplicate_lines)
            {
                string combo;

                try
                {
                    combo = Somemould.Removeduplicate("accounts.txt");
                    combo = Somemould.RemoveBadLines(combo);
                }
                catch
                {
                    Console.WriteLine("[Error]Invaild Combolist!", Color.Red);
                    Console.ReadKey(true);
                    Environment.Exit(0);
                    return;
                }
                if (combo == "")
                {
                    Console.WriteLine("[Error]Invaild Combolist!", Color.Red);
                    Console.ReadKey(true);
                    Environment.Exit(0);
                    return;
                }
                else
                {
                    comboList = combo.Split('\n');
                    combonum = comboList.Length;
                    Console.WriteLine("[*]Combo: Load " + Convert.ToString(combonum) + " Lines", Color.YellowGreen);
                }
            }
            else
            {
                try
                {
                    comboList = File.ReadAllLines("accounts.txt");
                    combonum = comboList.Length;
                }
                catch
                {
                    Console.WriteLine("[Error]Invaild Combolist!", Color.Red);
                    Console.ReadKey(true);
                    Environment.Exit(0);
                    return;
                }
                Console.WriteLine("[*]Combo: Load " + Convert.ToString(combonum) + " Lines", Color.YellowGreen);
            }
        }
        private static void LoadProxy()//Load proxy
        {
            if(proxymode == "2")
            {
                string http_proxy;
                string socks4_proxy;
                string socks5_proxy;
                try
                {
                    http_proxy = Somemould.RemoveBadLines(Somemould.Removeduplicate(Directory.GetCurrentDirectory() + "\\proxy\\HTTP.txt"));
                    socks4_proxy = Somemould.RemoveBadLines(Somemould.Removeduplicate(Directory.GetCurrentDirectory() + "\\proxy\\SOCKS4.txt"));
                    socks5_proxy = Somemould.RemoveBadLines(Somemould.Removeduplicate(Directory.GetCurrentDirectory() + "\\proxy\\SOCKS5.txt"));
                }
                catch
                {
                    Console.WriteLine("[Error]Invaild Proxylist!", Color.Red);
                    Console.ReadKey(true);
                    Environment.Exit(0);
                    return;
                }
                if(http_proxy == "" & socks4_proxy == "" & socks5_proxy == "")
                {
                    Console.WriteLine("[Error]Invaild Proxylist!", Color.Red);
                    Console.ReadKey(true);
                    Environment.Exit(0);
                    return;
                }
                if(http_proxy != "")
                {
                    proxyList_HTTP = http_proxy.Split('\n');
                    proxynum_HTTP = proxyList_HTTP.Length;
                    Console.WriteLine("[*]HTTP_Proxy: Load " + Convert.ToString(proxynum_HTTP) + " Lines", Color.YellowGreen);
                }
                else
                {
                    proxynum_HTTP = 0;
                }
                if (socks4_proxy != "")
                {
                    proxyList_SOCKS4 = socks4_proxy.Split('\n');
                    proxynum_SOCKS4 = proxyList_SOCKS4.Length;
                    Console.WriteLine("[*]SOCKS4_Proxy: Load " + Convert.ToString(proxynum_SOCKS4) + " Lines", Color.YellowGreen);
                }
                else
                {
                    proxynum_SOCKS4 = 0;
                }
                if (socks5_proxy != "")
                {
                    proxyList_SOCKS5 = socks5_proxy.Split('\n');
                    proxynum_SOCKS5 = proxyList_SOCKS5.Length;
                    Console.WriteLine("[*]SOCKS5_Proxy: Load " + Convert.ToString(proxynum_SOCKS5) + " Lines", Color.YellowGreen);
                }
                else
                {
                    proxynum_SOCKS5 = 0;
                }
                proxynum_all = proxynum_HTTP + proxynum_SOCKS4 + proxynum_SOCKS5;
                if(HttpLink != "" || Socks4Link != "" || Socks5Link != "")
                {
                    new Thread(ProxyUpdate).Start();
                }
            }
            
        }
        private static void LoadURL()
        {
            string url;
            try
            {
                url = Somemould.RemoveBadLines(Somemould.Removeduplicate(Directory.GetCurrentDirectory() + @"\config\links.txt"));
            }
            catch
            {
                Console.WriteLine("[Error]Invaild Urllist!", Color.Red);
                Console.ReadKey(true);
                Environment.Exit(0);
                return;
            }
            if (url == "")
            {
                Console.WriteLine("[Error]Invaild Urllist!", Color.Red);
                Console.ReadKey(true);
                Environment.Exit(0);
                return;
            }
            else
            {
                urllist = url.Split('\n');
                urlnum = urllist.Length;
                url = "";
                Console.WriteLine("[*]Links: Load " + Convert.ToString(urlnum) + " Lines", Color.YellowGreen);
            }
        }
        private static void ProxyUpdate()
        {
            while (true)
            {
                Thread.Sleep(load_delay);
                lock (getproxy_locker)
                {
                    if (HttpLink != "")
                    {
                        using (HttpRequest http = new HttpRequest())
                        {
                        httpc:
                            try
                            {
                                http.IgnoreProtocolErrors = true;
                                http.KeepAlive = true;
                                string http_proxy = http.Get(HttpLink).ToString();
                                proxyList_HTTP = http_proxy.Split('\n');
                                proxynum_HTTP = proxyList_HTTP.Length;
                                if (http_proxy == "")
                                {
                                    goto httpc;
                                }
                            }
                            catch
                            {
                                goto httpc;
                            }
                        }
                    }
                    if (Socks4Link != "")
                    {
                        using (HttpRequest http = new HttpRequest())
                        {
                        socks4:
                            try
                            {
                                http.IgnoreProtocolErrors = true;
                                http.KeepAlive = true;
                                string socks4_proxy = http.Get(Socks4Link).ToString();
                                proxyList_SOCKS4 = socks4_proxy.Split('\n');
                                proxynum_SOCKS4 = proxyList_SOCKS4.Length;
                                if (socks4_proxy == "")
                                {
                                    goto socks4;
                                }
                            }
                            catch
                            {
                                goto socks4;
                            }
                        }
                    }
                    if (Socks5Link != "")
                    {
                        using (HttpRequest http = new HttpRequest())
                        {
                        socks5:
                            try
                            {
                                http.IgnoreProtocolErrors = true;
                                http.KeepAlive = true;
                                string socks5_proxy = http.Get(Socks5Link).ToString();
                                proxyList_SOCKS5 = socks5_proxy.Split('\n');
                                proxynum_SOCKS5 = proxyList_SOCKS5.Length;
                                if(socks5_proxy == "")
                                {
                                    goto socks5;
                                }
                            }
                            catch
                            {
                                goto socks5;
                            }
                        }
                    }
                    proxynum_all = proxynum_HTTP + proxynum_SOCKS4 + proxynum_SOCKS5;
                }
            }
        }

    }
}
