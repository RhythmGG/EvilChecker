using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Console = Colorful.Console;
using System.Drawing;
using xNet;
using System.Reflection;
using ProxyLib.Proxy;
namespace EvilChecker.Minecraft_General
{
    public class BanChecker
    {
        private NetworkStream _stream;
        private List<byte> _buffer;
        private int _offset;
        private CryptoHandler.IAesStream s;
        private TcpClient client;
        public string BanCheckerMain(string name, string accessToken, string uuid)
        {
        check:
            _offset = 0;
            try
            {
                ProxyClientFactory factory = new ProxyClientFactory();
                Proxy.Get_Proxy_By_List(out string proxy, out int proxymode);
                ProxyLib.Proxy.ProxyType proxytype;
                switch (proxymode)
                {
                    case 1:
                        proxytype = ProxyLib.Proxy.ProxyType.None;
                        break;
                    case 2:
                        proxytype = ProxyLib.Proxy.ProxyType.Http;
                        break;
                    case 3:
                        proxytype = ProxyLib.Proxy.ProxyType.Socks4;
                        break;
                    case 4:
                        proxytype = ProxyLib.Proxy.ProxyType.Socks5;
                        break;
                    default:
                        proxytype = ProxyLib.Proxy.ProxyType.None;
                        break;
                }
                string[] proxy_ = proxy.Split(':');
                IProxyClient proxyClient;
                if (proxy_.Length == 4)
                {
                    proxyClient = factory.CreateProxyClient(proxytype, proxy_[0], Int32.Parse(proxy_[1]), proxy_[2], proxy_[3]);
                }
                else
                {
                    proxyClient = factory.CreateProxyClient(proxytype, proxy_[0], Int32.Parse(proxy_[1]));
                }
                //Setup timeouts
                proxyClient.ReceiveTimeout = (int)TimeSpan.FromSeconds(15).TotalMilliseconds;
                proxyClient.SendTimeout = (int)TimeSpan.FromSeconds(15).TotalMilliseconds;
                client = proxyClient.CreateConnection("mc.hypixel.net", 25565);
                Thread.Sleep(500);
                if (!client.Connected)
                {
                    if(_stream != null)
                    {
                        _stream.Dispose();
                    }
                    client.Dispose();
                    Thread.Sleep(100);
                    goto check;
                }
                _buffer = new List<byte>();
                _stream = client.GetStream();

                /*
                 * Send a "Handshake" packet
                 */
                WriteVarInt(47);//Protocol Version
                WriteString("mc.hypixel.net");//server IP
                WriteShort(25565);//server Port
                WriteVarInt(2);//Start Login
                Flush(0);//Send

                WriteString(name);//Player name
                Flush(0);//Send

                var buffer = new byte[Int16.MaxValue];
                _stream.Read(buffer, 0, buffer.Length);
                //Get stream

                var length = ReadVarInt(buffer);
                var packet_id = ReadVarInt(buffer);
                var server_id = ReadString(buffer, ReadVarInt(buffer));
                var serverKey = ReadByteArray(buffer);
                var verify_token = ReadByteArray(buffer);
                if (StartEncryption(uuid, accessToken, verify_token, server_id, serverKey, out string banreason)) 
                {
                    if (_stream != null)
                    {
                        _stream.Dispose();
                    }
                    client.Dispose();
                    return "Unban";
                }
                else
                {
                    if (_stream != null)
                    {
                        _stream.Dispose();
                    }
                    client.Dispose();
                    return Somemould.Getsubstring(banreason, "\"color\":\"white\",\"text\":\"", "\"");
                }//\"color\":\"white\",\"text\":\"
            }
            catch
            {
                if (_stream != null)
                {
                    _stream.Dispose();
                }
                if(client != null)
                {
                    client.Dispose();
                }
                Thread.Sleep(1000);
                goto check;
            }
        
        }
        internal byte ReadByte(byte[] buffer)
        {
            var b = buffer[_offset];
            _offset += 1;
            return b;
        }
        internal byte[] Read(byte[] buffer, int length)
        {
            var data = new byte[length];
            Array.Copy(buffer, _offset, data, 0, length);
            _offset += length;
            return data;
        }
        internal int ReadVarInt(byte[] buffer)
        {
            var value = 0;
            var size = 0;
            int b;
            while (((b = ReadByte(buffer)) & 0x80) == 0x80)
            {
                value |= (b & 0x7F) << (size++ * 7);
                if (size > 5)
                {
                    throw new IOException("This VarInt is an imposter!");
                }
            }
            return value | ((b & 0x7F) << (size * 7));
        }
        internal string ReadString(byte[] buffer, int length)
        {
            var data = Read(buffer, length);
            return Encoding.UTF8.GetString(data);
        }
        internal byte[] ReadByteArray(byte[] buffer)
        {
            var leng = ReadVarInt(buffer);
            return Read(buffer, leng);
            //leng.ToString + "s"
            //ReadString(buffer, leng)
        }
        internal void WriteVarInt(int value)
        {
            while ((value & 128) != 0)
            {
                _buffer.Add((byte)(value & 127 | 128));
                value = (int)((uint)value) >> 7;
            }
            _buffer.Add((byte)value);
        }
        internal void WriteShort(short value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }
        internal void WriteString(string data)
        {
            var buffer = Encoding.UTF8.GetBytes(data);
            WriteVarInt(buffer.Length);
            _buffer.AddRange(buffer);
        }
        internal void Flush(int id = -1)
        {
            var buffer = _buffer.ToArray();
            _buffer.Clear();

            var add = 0;
            var packetData = new[] { (byte)0x00 };
            if (id >= 0)
            {
                WriteVarInt(id);
                packetData = _buffer.ToArray();
                add = packetData.Length;
                _buffer.Clear();
            }

            WriteVarInt(buffer.Length + add);
            var bufferLength = _buffer.ToArray();
            _buffer.Clear();
            _stream.Write(bufferLength, 0, bufferLength.Length);
            _stream.Write(packetData, 0, packetData.Length);
            _stream.Write(buffer, 0, buffer.Length);
        }
        private bool StartEncryption(string uuid, string sessionID, byte[] token, string serverIDhash, byte[] serverKey,out string banreason)
        {
            banreason = "";
            RSACryptoServiceProvider rsacryptoServiceProvider = CryptoHandler.DecodeRSAPublicKey(serverKey);
            byte[] array = CryptoHandler.GenerateAESPrivateKey();
            if (serverIDhash != "-")
            {
                if (!SessionCheck(uuid, sessionID, CryptoHandler.getServerHash(serverIDhash, serverKey, array)))
                {
                    return false;
                }
            }
            byte[] array2 = GetArray(rsacryptoServiceProvider.Encrypt(array, false));
            byte[] array3 = GetArray(rsacryptoServiceProvider.Encrypt(token, false));
            SendPacket(1, ConcatBytes(new byte[][]
            {
                array2,
                array3
            }));
            s = CryptoHandler.getAesStream(_stream, array);



            //ReadNextPacket(ref num, queue);
            Queue<byte> packetData = new Queue<byte>();
            packetData.Clear();
            int length = ReadNextVarIntRAW();

            byte[] array_ = new byte[length];
            int start = 0;
            int offset = length;
            int i = 0;

            while (i < offset)
            {
                i += s.Read(array_, start + i, offset - i);
            }

            for (int i_ = 0; i_ < array_.Length; i_++)
            {
                packetData.Enqueue(array_[i_]);
            }

            var packet_id = ReadNextVarInt(packetData);
            
            int compression_treshold = ReadNextVarInt(packetData);
            if(compression_treshold > 1000)
            {
                throw new Exception();
            }

            try
            {
                banreason = ReadNextString(packetData);
            }
            catch
            {
                return true;
            }
                return false;
        }
        private byte[] GetArray(byte[] array)
        {
            return ConcatBytes(new byte[][]
            {
                GetVarInt(array.Length),
                array
            });
        }
        private byte[] GetVarInt(int paramInt)
        {
            List<byte> list = new List<byte>();
            while ((paramInt & -128) != 0)
            {
                list.Add((byte)((paramInt & 127) | 128));
                paramInt = (int)((uint)paramInt >> 7);
            }
            list.Add((byte)paramInt);
            return list.ToArray();
        }
        private byte[] ConcatBytes(params byte[][] bytes)
        {
            List<byte> list = new List<byte>();
            foreach (byte[] collection in bytes)
            {
                list.AddRange(collection);
            }
            return list.ToArray();
        }
        private int ReadNextVarInt(Queue<byte> cache)
        {
            string str = BitConverter.ToString(cache.ToArray());
            int num = 0;
            int num2 = 0;
            for (; ; )
            {
                int num3 = (int)ReadNextByte(cache);
                num |= (num3 & 127) << num2++ * 7;
                if (num2 > 5)
                {
                    break;
                }
                if ((num3 & 128) != 128)
                {
                    return num;
                }
            }
            throw new OverflowException("VarInt too big " + str);
        }
        private byte ReadNextByte(Queue<byte> cache)
        {
            return cache.Dequeue();
        }
        private string ReadNextString(Queue<byte> cache)
        {
            int num = ReadNextVarInt(cache);
            if (num > 0)
            {
                return Encoding.UTF8.GetString(ReadData(num, cache));
            }
            return "";
        }
        private byte[] ReadData(int offset, Queue<byte> cache)
        {
            byte[] array = new byte[offset];
            for (int i = 0; i < offset; i++)
            {
                array[i] = cache.Dequeue();
            }
            return array;
        }
        private int ReadNextVarIntRAW()
        {
            int num = 0;
            int num2 = 0;
            for (; ; )
            {
                byte[] array = new byte[1];
                int start = 0;
                int offset = 1;
                int i = 0;
                while (i < offset)
                {
                    i += s.Read(array, start + i, offset - i);
                }

                int num3 = array[0];
                num |= (num3 & 127) << num2++ * 7;
                if (num2 > 5)
                {
                    break;
                }
                if ((num3 & 128) != 128)
                {
                    return num;
                }
            }
            throw new OverflowException("VarInt too big");
        }
        private bool SessionCheck(string uuid, string accesstoken, string serverhash)
        {
            HttpResponse text;
            bool result;
            do
            {
                try
                {
                    string request = string.Concat(new string[]
                    {
                    "{\"accessToken\":\"",
                    accesstoken,
                    "\",\"selectedProfile\":\"",
                    uuid,
                    "\",\"serverId\":\"",
                    serverhash,
                    "\"}"
                    });
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
                                http.Proxy = xNet.HttpProxyClient.Parse(proxy);
                                break;
                            case 3:
                                http.Proxy = xNet.Socks4ProxyClient.Parse(proxy);
                                break;
                            case 4:
                                http.Proxy = xNet.Socks5ProxyClient.Parse(proxy);
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
                    text = http.Post("https://sessionserver.mojang.com/session/minecraft/join", request, "application/json");
                    break;
                }
                catch
                {
                }
            } while (true);
            int num = (int)text.StatusCode;
            result = (num >= 200 && num < 300);
            return result;
        }
        private void SendPacket(int packetID, IEnumerable<byte> packetData)
        {
            byte[] array = ConcatBytes(new byte[][]
            {
                GetVarInt(packetID),
                packetData.ToArray<byte>()
            });
            client.Client.Send(ConcatBytes(new byte[][]
            {
                GetVarInt(array.Length),
                array
            }));
        }
    }
}
