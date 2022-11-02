using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = Colorful.Console;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace EvilChecker
{
    class ComboTool
    {
        public static void Convert_EmailToUser()
        {
            Console.WriteLine("[*]Editing,it may takes a little time", Color.YellowGreen);
            StringBuilder sb = new StringBuilder();
            foreach(string account in MainChecker.comboList)
            {
                string account1 = account.Trim();
                try
                {
                    sb.AppendLine(account1.Replace("@" + Somemould.Getsubstring(account1, "@", ":"), ""));
                }
                catch
                {

                }
            }
            File.WriteAllText(MainChecker.directory + "\\User_Pass.txt", sb.ToString());
            Console.WriteLine("[*]DONE,Press anything to quit", Color.Yellow);
            Console.ReadKey();
            Environment.Exit(0);
        }
        public static void Combo_Gaming()
        {
            string combo;
            Console.WriteLine("[*]Reading your combolist", Color.YellowGreen);
            try
            {
                combo = Somemould.Removeduplicate("accounts.txt");
                combo = Somemould.RemoveBadLines(combo);
            }
            catch
            {
                combo = "";
                Console.WriteLine("[!]Your Combolist is null", Color.Red);
                Console.ReadKey(true);
                Environment.Exit(0);
            }

            Console.WriteLine("[*]Editing,it may takes a little time", Color.YellowGreen);
            Console.WriteLine("[*]Remove lines whose password only has numbers", Color.YellowGreen);
            combo = Regex.Replace(combo, @".{1,}:\d*\r\n", "");
            Console.WriteLine("[*]Remove lines whose password is less than 6 characters", Color.YellowGreen);
            combo = Regex.Replace(combo, @".{1,}:.{0,5}\r\n", "");


            Console.WriteLine("[*]Add Special characters and Randomly shuffle the combolist", Color.YellowGreen);

            int num = 0;
            string[] combolist = combo.Split('\r');
            StringBuilder sb = new StringBuilder();
            string first_character_Upper;
            string first_character_lower;
            Random rd = new Random();
            foreach (string account in combolist)
            {
                bool unblock = true;
                string[] acc;
                string right;
                string left;
                try
                {
                    acc = account.Split(':');
                    right = acc[1];
                    left = acc[0];
                }
                catch
                {
                    continue;
                }
                foreach(string blockeds in MainChecker.blocked_character)
                {
                    if (right.Contains(blockeds))
                    {
                        sb.AppendLine(account.Trim());
                        num++;
                        unblock = false;
                        break;
                    }
                }
                if (unblock)
                {
                    try
                    {
                        string account1 = account.Trim();

                        string right1 = right.Substring(1, right.Length - 1);
                        string first = right.Substring(0, 1);
                        first_character_Upper = first.ToUpper();
                        first_character_lower = first.ToLower();

                        foreach (string sc in MainChecker.special_characters)
                        {
                            if (MainChecker.double_character)
                            {
                                if ("qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM".Contains(first_character_Upper))
                                {
                                    sb.AppendLine(left + ":" + first_character_Upper + right1 + sc);
                                    num++;
                                    sb.AppendLine(left + ":" + first_character_lower + right1 + sc);
                                    num++;
                                }
                                else
                                {
                                    sb.AppendLine(account1 + sc);
                                    num++;
                                }
                            }
                            else
                            {
                                sb.AppendLine(account1 + sc);
                                num++;
                            }
                        }
                        sb.AppendLine(account1);
                        num++;

                    }
                    catch
                    {
                    }
                }
                if (num >= 5000000)
                {
                    num = 0;
                    combo = sb.ToString().Trim();
                    sb.Clear();
                    combolist = combo.Split('\r');
                    combo = null;
                    for (int i = 0; i < combolist.Length; i++)
                    {
                        var index = rd.Next(i, combolist.Length);
                        var tmp = combolist[i].Trim();
                        var ran = combolist[index].Trim();
                        combolist[i] = ran;
                        combolist[index] = tmp;
                    }
                    File.AppendAllLines(MainChecker.directory + "\\ComboList_New.txt", combolist);
                    combolist = null;
                }
            }

            combo = sb.ToString();
            sb.Clear();
            combolist = combo.Split('\r');
            combo = null;
            for (int i = 0; i < combolist.Length; i++)
            {
                var index = rd.Next(i, combolist.Length);
                var tmp = combolist[i].Trim();
                var ran = combolist[index].Trim();
                combolist[i] = ran;
                combolist[index] = tmp;
            }
            File.AppendAllLines(MainChecker.directory + "\\ComboList_New.txt", combolist);
            combolist = null;
            Console.WriteLine("[*]Enjoy your new combolist,Press anything to quit", Color.Yellow);
            Console.ReadKey(true);
            Environment.Exit(0);
        }
    }
}
