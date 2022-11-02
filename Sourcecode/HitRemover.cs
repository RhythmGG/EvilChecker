using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = Colorful.Console;
using System.Drawing;
using System.Threading;

namespace EvilChecker
{
    class HitRemover
    {
        public static void Hitremover()
        {
            string combo;
            try
            {
                combo = Somemould.RemoveBadLines(Somemould.Removeduplicate("accounts.txt"));
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
            Console.WriteLine("[*]Combo: Load " + Somemould.Getlines(combo) + " Lines", Color.YellowGreen);
            Console.WriteLine("Where is your hit file?Put it here",Color.Yellow);
            String path = Console.ReadLine();
            String hit;
            try
            {
                hit  = Somemould.RemoveBadLines(Somemould.Removeduplicate(path));

            }
            catch
            {
                Console.WriteLine("[Error]Invaild HitList", Color.Red);
                Console.ReadKey(true);
                Environment.Exit(0);
                return;
            }
            if(hit == "")
            {
                Console.WriteLine("[Error]Invaild HitList", Color.Red);
                Console.ReadKey(true);
                Environment.Exit(0);
                return;
            }
            string[] hits = hit.Split('\n');
            Console.WriteLine("[*]Hit: Load " + hits.Length.ToString() + " Lines", Color.YellowGreen);
            Console.WriteLine("Start Removing...", Color.Yellow);
            Thread.Sleep(2000);
            Console.Clear();
            for (int num = 0; num < hits.Length; num++)
            {
                if (combo.Contains(hits[num]))
                {
                    Console.WriteLine("[Removed]" + hits[num], Color.Aqua);
                    combo = combo.Replace(hits[num], "");
                }
                else
                {
                    Console.WriteLine("[Not Found]" + hits[num], Color.Red);
                }
            }
            while (true)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(MainChecker.directory + "\\Combo_HitsRemoved.txt", true))
                    {
                        sw.Write(Somemould.RemoveBadLines(combo));
                    }
                    break;
                }
                catch
                {

                }
            }
        }
    }
}
