using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using xNet;
namespace EvilChecker.Microsoft
{
    class XboxLive
    {
        private readonly string authorize = "https://login.live.com/oauth20_authorize.srf?client_id=000000004C12AE6F&redirect_uri=https://login.live.com/oauth20_desktop.srf&scope=service::user.auth.xboxlive.com::MBI_SSL&display=touch&response_type=token&locale=en";
        private readonly string xbl = "https://user.auth.xboxlive.com/user/authenticate";
        private readonly string xsts = "https://xsts.auth.xboxlive.com/xsts/authorize";
        private readonly string userAgent = "Mozilla/5.0 (XboxReplay; XboxLiveAuth/3.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36";
        private Regex ppft = new Regex("sFTTag:'.*value=\"(.*)\"\\/>'");
        private Regex urlPost = new Regex("urlPost:'(.+?(?=\'))");
        private Regex confirm = new Regex("identity\\/confirm");
        public PreAuthResponse PreAuth(int proxymode, string proxy)
        {
            string html;
            CookieDictionary cookies = new CookieDictionary();
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
                http.AddHeader(HttpHeader.UserAgent, userAgent);
                http.Cookies = cookies;
                html = http.Get(authorize).ToString();
            }
            string PPFT = ppft.Match(html).Groups[1].Value;
            string urlPost = this.urlPost.Match(html).Groups[1].Value;
            if (string.IsNullOrEmpty(PPFT) || string.IsNullOrEmpty(urlPost))
            {
                throw new Exception("Fail to extract PPFT or urlPost");
            }
            return new PreAuthResponse()
            {
                UrlPost = urlPost,
                PPFT = PPFT,
                Cookie = cookies
            };
        }
        public UserLoginResponse UserLogin(string email, string password, PreAuthResponse preAuth, int proxymode, string proxy)
        {
            HttpResponse response;
            string postData = "login=" + Uri.EscapeDataString(email)
     + "&loginfmt=" + Uri.EscapeDataString(email)
     + "&passwd=" + Uri.EscapeDataString(password)
     + "&PPFT=" + Uri.EscapeDataString(preAuth.PPFT);

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
                http.AllowAutoRedirect = false;
                http.AddHeader(HttpHeader.Accept, "*/*");
                http.AddHeader(HttpHeader.UserAgent, userAgent);
                http.Cookies = preAuth.Cookie;
                response = http.Post(preAuth.UrlPost, postData, "application/x-www-form-urlencoded");
            }
            
            if ((int)response.StatusCode >= 300 && (int)response.StatusCode <= 399)
            {
                HttpResponse response2;
                string url = response.Location;
                string hash = url.Split('#')[1];
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
                    http.AllowAutoRedirect = false;
                    http.AddHeader(HttpHeader.Accept, "*/*");
                    http.AddHeader(HttpHeader.UserAgent, userAgent);
                    http.Cookies = preAuth.Cookie;
                    response2 = http.Get(url);
                }
                if ((int)response2.StatusCode != 200)
                {
                    throw new Exception("Authentication failed");
                }

                if (string.IsNullOrEmpty(hash))
                {
                    if (confirm.IsMatch(response2.ToString()))
                    {
                        throw new Exception("Activity confirmation required");
                    }
                    else throw new Exception("Invalid credentials or 2FA enabled");
                }
                var dict = Request.ParseQueryString(hash);
                response = null;
                response2 = null;

                return new UserLoginResponse()
                {
                    AccessToken = dict["access_token"],
                    RefreshToken = dict["refresh_token"],
                    ExpiresIn = int.Parse(dict["expires_in"])
                };
            }
            else
            {
                throw new Exception("Unexpected response. Check your credentials");
            }
        }
        public XblAuthenticateResponse XblAuthenticate(UserLoginResponse loginResponse, int proxymode, string proxy)
        {
            string payload = "{"
                + "\"Properties\": {"
                + "\"AuthMethod\": \"RPS\","
                + "\"SiteName\": \"user.auth.xboxlive.com\","
                + "\"RpsTicket\": \"" + loginResponse.AccessToken + "\""
                + "},"
                + "\"RelyingParty\": \"http://auth.xboxlive.com\","
                + "\"TokenType\": \"JWT\""
                + "}";

            HttpResponse response;

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
                    http.AddHeader(HttpHeader.Accept, "application/json");
                    http.AddHeader("accept-encoding", "gzip");
                    ////////////////////////
                    http.AddHeader(HttpHeader.UserAgent, userAgent);
                    http.AddHeader("x-xbl-contract-version", "0");
                    response = http.Post(xbl, payload, "application/json");
                }
                string jsonString = response.ToString();
                //Console.WriteLine(jsonString);

                Json.JSONData json = Json.ParseJson(jsonString);
                string token = json.Properties["Token"].StringValue;
                string userHash = json.Properties["DisplayClaims"].Properties["xui"].DataArray[0].Properties["uhs"].StringValue;
                return new XblAuthenticateResponse()
                {
                    Token = token,
                    UserHash = userHash
                };
            }
            catch
            {
                throw new Exception("XBL Authentication failed");
            }
        }
        public XSTSAuthenticateResponse XSTSAuthenticate(XblAuthenticateResponse xblResponse, int proxymode, string proxy)
        {
            HttpResponse response;
            string payload = "{"
                + "\"Properties\": {"
                + "\"SandboxId\": \"RETAIL\","
                + "\"UserTokens\": ["
                + "\"" + xblResponse.Token + "\""
                + "]"
                + "},"
                + "\"RelyingParty\": \"rp://api.minecraftservices.com/\","
                + "\"TokenType\": \"JWT\""
                + "}";
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
                    http.AddHeader(HttpHeader.Accept, "application/json");
                    http.AddHeader(HttpHeader.UserAgent, userAgent);
                    http.AddHeader("x-xbl-contract-version", "1");
                    response = http.Post(xsts, payload, "application/json");
                }
                string jsonString = response.ToString();
                Json.JSONData json = Json.ParseJson(jsonString);
                string token = json.Properties["Token"].StringValue;
                string userHash = json.Properties["DisplayClaims"].Properties["xui"].DataArray[0].Properties["uhs"].StringValue;
                return new XSTSAuthenticateResponse()
                {
                    Token = token,
                    UserHash = userHash
                };
            }
            catch (HttpException err)
            {
                if ((int)err.HttpStatusCode == 401)
                {
                    Json.JSONData json = Json.ParseJson(err.ToString());
                    if (json.Properties["XErr"].StringValue == "2148916233")
                    {
                        throw new Exception("The account doesn't have an Xbox account");
                    }
                    else if (json.Properties["XErr"].StringValue == "2148916238")
                    {
                        throw new Exception("The account is a child (under 18) and cannot proceed unless the account is added to a Family by an adult");
                    }
                    else throw new Exception("Unknown XSTS error code: " + json.Properties["XErr"].StringValue);
                }
                else
                {
                    throw new Exception("XSTS Authentication failed");
                }
            }
        }

        public struct PreAuthResponse
        {
            public string UrlPost;
            public string PPFT;
            public CookieDictionary Cookie;
        }

        public struct UserLoginResponse
        {
            public string AccessToken;
            public string RefreshToken;
            public int ExpiresIn;
        }

        public struct XblAuthenticateResponse
        {
            public string Token;
            public string UserHash;
        }

        public struct XSTSAuthenticateResponse
        {
            public string Token;
            public string UserHash;
        }
    }

    class MinecraftWithXbox
    {
        private readonly string loginWithXbox = "https://api.minecraftservices.com/authentication/login_with_xbox";
        private readonly string profile = "https://api.minecraftservices.com/minecraft/profile";

        /// <summary>
        /// Login to Minecraft using the XSTS token and user hash obtained before
        /// </summary>
        /// <param name="userHash"></param>
        /// <param name="xstsToken"></param>
        /// <returns></returns>
        public string LoginWithXbox(string userHash, string xstsToken, int proxymode, string proxy)
        {
            HttpResponse response;

            string payload = "{\"identityToken\": \"XBL3.0 x=" + userHash + ";" + xstsToken + "\"}";
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
                http.AddHeader(HttpHeader.Accept, "application/json");
                response = http.Post(loginWithXbox, payload, "application/json");
            }


            string jsonString = response.ToString();
            // See https://github.com/ORelio/Sharp-Tools/issues/1
            jsonString = jsonString.Replace("[ ]", "[]");
            Json.JSONData json = Json.ParseJson(jsonString);
            response = null;
            return json.Properties["access_token"].StringValue;
        }
        public UserProfile GetUserProfile(string accessToken, int proxymode, string proxy)
        {
            HttpResponse response;
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
                http.AddHeader("Authorization", string.Format("Bearer {0}", accessToken));
                response = http.Get(profile);
            }

            string jsonString = response.ToString();
            jsonString = jsonString.Replace("[ ]", "[]");
            Json.JSONData json = Json.ParseJson(jsonString);
            response = null;
            return new UserProfile()
            {
                UUID = json.Properties["id"].StringValue,
                UserName = json.Properties["name"].StringValue
            };
        }

        public struct UserProfile
        {
            public string UUID;
            public string UserName;
        }
    }

    /// <summary>
    /// Helper class
    /// </summary>
    static class Request
    {
        static public Dictionary<string, string> ParseQueryString(string query)
        {
            return query.Split('&')
                .ToDictionary(c => c.Split('=')[0],
                              c => Uri.UnescapeDataString(c.Split('=')[1]));
        }
    }
}
