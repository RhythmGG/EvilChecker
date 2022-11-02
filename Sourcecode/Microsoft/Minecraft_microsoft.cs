using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xNet;
using Console = Colorful.Console;
using System.Drawing;
using EvilChecker.Minecraft_General;
namespace EvilChecker.Microsoft
{
    class Minecraft_microsoft
    {
        public static void Minecraft_Checking()
        {
            Console.WriteLine("This is only a test. it will be release soon.");
            Console.Write("Microsoft Account Email: ");
            string email = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();

            var x = new XboxLive();
            var m = new MinecraftWithXbox();
            try
            {
                Proxy.Get_Proxy_By_List(out string proxy, out int proxymode);
                var preAuth = x.PreAuth(proxymode, proxy);

                var login = x.UserLogin(email, password, preAuth, proxymode, proxy);

                var xbl = x.XblAuthenticate(login, proxymode, proxy);

                var xsts = x.XSTSAuthenticate(xbl, proxymode, proxy);

                string accessToken = m.LoginWithXbox(xsts.UserHash, xsts.Token, proxymode, proxy);

                var profile = m.GetUserProfile(accessToken, proxymode, proxy);
                Console.WriteLine("Minecraft User Name: {0}", profile.UserName);
                Console.WriteLine("UUID: {0}", profile.UUID);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }
}
