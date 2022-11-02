using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace EvilChecker
{
    class Somemould
    {
        public static string Getsubstring(string strfull, string strhead, string strfoot)
        {
            int pos1, pos2;
            string result = "";
            pos1 = strfull.IndexOf(strhead, StringComparison.OrdinalIgnoreCase);
            if (pos1 > -1)
            {
                pos2 = strfull.IndexOf(strfoot, pos1 + strhead.Length, StringComparison.OrdinalIgnoreCase);
                if (pos2 > -1)
                {
                    result = strfull.Substring(pos1 + strhead.Length, pos2 - pos1 - strhead.Length);
                }
            }
            return (result);
        }
        public static string RemoveBadLines(string text)
        {
            string result = Regex.Replace(text, @"\r\n\r\n(\r\n)+", "\r\n").Trim();
            return result;
        }
        public static int Getlines(string text)
        {
            int textnum = text.Split('\r').Length;
            return textnum;
        }
        public static string Removeduplicate(string path)
        {
            string currentLine;
            StringBuilder strb = new StringBuilder();
            TextReader reader = File.OpenText(path);
            HashSet<string> previousLines = new HashSet<string>();
            while ((currentLine = reader.ReadLine()) != null)
            {
                if (previousLines.Add(currentLine))
                {
                    strb.AppendLine(currentLine);
                }
            }
            reader.Close();
            return strb.ToString();
        }
        public static string MD5Encode(string encryptString)
        {
            byte[] result = Encoding.Default.GetBytes(encryptString);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            string encryptResult = BitConverter.ToString(output).Replace("-", "");
            return encryptResult;

        }
        public static string UrlEncode(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = Encoding.UTF8.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }

            return (sb.ToString());
        }
        private static string GetCPUId()
        {
            string cpuInfo = String.Empty;
            string temp = String.Empty;
            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (cpuInfo == String.Empty)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                }
            }
            return cpuInfo;
        }
        private static string GetDiskID()
        {
            String HDid = string.Empty;
            ManagementClass cimobject = new ManagementClass("Win32_DiskDrive");
            ManagementObjectCollection moc = cimobject.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (HDid == string.Empty)
                {
                    HDid = (string)mo.Properties["Model"].Value;
                    break;
                }
            }
            return HDid;
        }
        public static string GetHWID()
        {
            return MD5Encode("By_Rhythm-" + GetCPUId() + "-" + GetDiskID() + "-" + Environment.UserName);
        }
    }
}
