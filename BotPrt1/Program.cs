using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using SteamKit2;
using System.Threading;
using System.Json;
using System.Net.Http;

namespace BotPrt1
{
    class Program
    {
        private static bool isRunning;
        private static string user, pass;
        static string authCode, twoFactorAuth;
        static SteamClient client;
        static CallbackManager manager;
        static SteamUser credentials;
        static SteamFriends friends;
        static string[] ownedbadges = new string[1000];
        static bool confirm = false;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Welcome to Vietnam Trading Bot\nPress CTRL+C to shutdown bot at any time");
            Console.WriteLine("Input your credentials below to access as an Admin");
            Console.Write("Username:  "); user = Console.ReadLine();
            Console.Write("Password:  "); pass = Console.ReadLine();
            SteamConnect();
        }
        static void SteamConnect()
        {
            //Declare materials
            client = new SteamClient();

            manager = new CallbackManager(client);

            credentials = client.GetHandler<SteamUser>();

            friends = client.GetHandler<SteamFriends>();

            //Declare dependencies
            manager.Subscribe<SteamClient.ConnectedCallback>(onConnectingToSteam);
            manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedIn);
            manager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);
            manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);
            manager.Subscribe<SteamUser.AccountInfoCallback>(OnAccountInfo);
            manager.Subscribe<SteamUser.UpdateMachineAuthCallback>(OnMachineAuth);
            manager.Subscribe<SteamFriends.FriendMsgCallback>(OnChatMsg);
            manager.Subscribe<SteamFriends.ProfileInfoCallback>(OnRequestingCustomerInfo);

            //Connect Client to Steam
            client.Connect();

            Console.WriteLine("Connecting to Steam...");
            isRunning = true;

            //Await commands from customer
            while (isRunning)
            {
                manager.RunWaitCallbacks(TimeSpan.FromMilliseconds(500));
            }
            Console.ReadKey();

        }

        static void OnRequestingCustomerInfo(SteamFriends.ProfileInfoCallback callback)
        {

        }

        static string respondmsg = "Hello, Tôi đéo hiểu bạn nói gì";
        private static void OnChatMsg(SteamFriends.FriendMsgCallback callback)
        {
            string mate = callback.Message.ToLower();
            string command = "";
            string value = "";
            if (callback.EntryType == EChatEntryType.ChatMsg)
            {
                if (mate.Length > 1)
                {
                    if (mate.Remove(1) == "!")
                    {
                        command = mate.Substring(1, mate.Length - 1);
                        if (mate.Contains(" "))
                        {
                            value = command.Substring(command.IndexOf(' ') + 1, command.Length - command.IndexOf(' ') - 1);
                            command = command.Remove(command.IndexOf(' '));
                            Console.WriteLine(command);
                            Console.WriteLine(value);
                        }
                        CommandResolver(command, value, callback.Sender.ConvertToUInt64().ToString(), callback.Sender);
                    }
                }
                friends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, respondmsg);
                Console.WriteLine("[{2}] MSR - SteamID {0} :  {1}", callback.Sender, respondmsg, DateTime.Now.ToString());
                respondmsg = "Hello, Tôi đéo hiểu bạn nói gì";
            }
        }
        static int amount = 0;
        static int key = 0;
        static void CommandResolver(string command, string value, string sender, SteamID SteamID)
        {
            string filedirec = Directory.GetCurrentDirectory() + @"\DatabaseLibrary.json";
            StreamReader r = new StreamReader(@filedirec);
            string a = r.ReadToEnd().ToString();
            JsonValue js = JsonValue.Parse(a);
            string exchangecs = js["csgo"]["1key"];
            string exchangehy = js["csgo"]["hydra"];
            string exchangetf = js["tf"]["1key"];
            int i;
            int j;
            int k;
            switch (command)
            {
                case "buycs":
                    {
                        if (confirm) { confirm = false; }
                        try { i = Int32.Parse(value); }
                        catch (Exception) { respondmsg = "Số lượng của bạn nhập không đúng"; break; }
                        if (Int32.Parse(value) >= 80)
                        {
                            respondmsg = "Số lượng bạn nhập quá lớn, tầm đấy mua được hơn 1000 pack rồi, lấy đâu ra bán cho bạn :(";
                            break;
                        }
                        j = Int32.Parse(exchangecs);
                        k = i * j;
                        key = i;
                        amount = k;
                        respondmsg = "Đơn hàng của bạn bao gồm " + amount + " gói\nChúng tôi sẽ yêu cầu đổi lại " + key + " key(s) CSGO\nVui lòng gõ !cf để xác nhận";
                        confirm = true;
                        break;
                    }
                case "buycshydra":
                    {
                        if (confirm) { confirm = false; }
                        try { i = Int32.Parse(value); }
                        catch (Exception) { respondmsg = "Số lượng của bạn nhập không đúng"; break; }
                        if (Int32.Parse(value) >= 80)
                        {
                            respondmsg = "Số lượng bạn nhập quá lớn, tầm đấy mua được hơn 1000 pack rồi, lấy đâu ra bán cho bạn :(";
                            break;
                        }
                        j = Int32.Parse(exchangehy);
                        k = i * j;
                        key = i;
                        amount = k;
                        respondmsg = "Đơn hàng của bạn bao gồm " + amount + " gói\nChúng tôi sẽ yêu cầu đổi lại " + key + " key(s) Hydra\nVui lòng gõ !cf để xác nhận";
                        confirm = true;
                        break;
                    }
                case "buytf":
                    {
                        if (confirm) { confirm = false; }
                        try { i = Int32.Parse(value); }
                        catch (Exception) { respondmsg = "Số lượng của bạn nhập không đúng"; break; }
                        if (Int32.Parse(value) >= 80)
                        {
                            respondmsg = "Số lượng bạn nhập quá lớn, tầm đấy mua được hơn 1000 pack rồi, lấy đâu ra bán cho bạn :(";
                            break;
                        }
                        j = Int32.Parse(exchangetf);
                        k = i * j;
                        key = i;
                        amount = k;
                        respondmsg = "Đơn hàng của bạn bao gồm " + amount + " gói\nChúng tôi sẽ yêu cầu đổi lại " + key + " key(s) Team Fortress\nVui lòng gõ !cf để xác nhận";
                        confirm = true;
                        break;
                    }
                case "owner":
                    {
                        if (confirm) { confirm = false; }
                        respondmsg = "";//Thông tin người bán
                        break;
                    }
                case "price":
                    {
                        if (confirm) { confirm = false; }
                        respondmsg = "1 key TeamFortress tương đương " + exchangetf + " gói\n1 key CSGO tương đương " + exchangecs + " gói";
                        break;
                    }
                case "info":
                    {
                        if (confirm) { confirm = false; }
                        respondmsg = "BOT Mua bán thẻ Lv Steam được duy trì bởi ...";
                        break;
                    }
                case "help":
                    {
                        if (confirm) { confirm = false; }
                        respondmsg = "!level - Kiểm tra kinh nghiệm và gói cần để lên Level tiếp theo\n!buy <số lượng bộ thẻ> - Mua số bộ thẻ mong muốn\n!info - Thông tin BOT\nMọi thắc mắc vui lòng đọc trang cá nhân của BOT hoặc liên hệ 0914709818";
                        break;
                    }
                case "level":
                    {
                        int adinas;
                        try{ adinas =  int.Parse(value); }
                        catch (Exception) { respondmsg = "Vui lòng nhập đúng cú pháp !level <số>"; break; }
                        neededXP(value, SteamID).Wait();
                        break;
                    }
                case "cf":
                    {
                        startTrade(sender, amount, key);
                        amount = 0;
                        key = 0;
                        break;
                    }
            }
        }
        static async Task neededXP(string wanted, SteamID sender)
        {
            int i = 0;
            try { i = int.Parse(wanted); }
            catch (Exception) { respondmsg = "Nhập đúng số đi bạn eiiii"; }
            if (i > 5000)
            {
                respondmsg = "Thôi tôi đ tính cho bạn đâu, lên thế nào được level đấy :>";
                return;
            }
            HttpClient getuserinfo = new HttpClient();
            string webapi = "https://api.steampowered.com/IPlayerService/GetBadges/v1/?key=B3B34FA60F4E3A3C8C53CA40D510ADA4&steamid=" + sender.ConvertToUInt64().ToString();
            var responseString = await getuserinfo.GetStringAsync(webapi);
            JsonValue obj = JsonObject.Parse(responseString);
            int current = 0;
            int needed = 0;
            int multiple = 0;
            int userlevel;
            if (obj["response"].ContainsKey("player_xp"))
            {
                userlevel = int.Parse(obj["response"]["player_level"].ToString());
                current = int.Parse(obj["response"]["player_xp"].ToString());
                for (int a = 1; a<=int.Parse(wanted); a++)
                {
                    if (a % 10 == 1)
                    {
                        multiple += 100;
                    }
                    needed += multiple;
                }
            }
            needed -= current;
            respondmsg = "Bạn cần " + needed + " kinh nghiệm để lên cấp " + wanted +"\nTương đương với " + (needed/100).ToString() + " gói 100XP";
        }
        static void startTrade(string tradeID, int value, int key)
        {
            if (confirm)
            {
                //Thêm Trade Void
                respondmsg = "Đang gửi Trade Offer tới tài khoản của bạn. Vui lòng xác nhận trong thời gian sớm nhất";
                confirm = false;
            }
            else respondmsg = "Thế bạn muốn mua cái gì????????";
        }
        static void OnAccountInfo(SteamUser.AccountInfoCallback callback)
        {
            if (friends.GetPersonaName() != "VietNam Cards Trading Bot")
            {
                friends.SetPersonaName("VietNam Cards Trading Bot");
            }
            friends.SetPersonaState(EPersonaState.Online);
        }

        static void onConnectingToSteam(SteamClient.ConnectedCallback callback)
        {
            //After connected to Steam, Logon to decalred user
            Console.WriteLine("Connected to Steam, Logging in Account {0} ...", user);

            //Check for existing ownership proof
            byte[] sentryHash = null;
            if (File.Exists("sentry.bin"))
            {
                // if we have a saved sentry file, read and sha-1 hash it
                byte[] sentryFile = File.ReadAllBytes("sentry.bin");
                sentryHash = CryptoHelper.SHAHash(sentryFile);
            }

            //Pass Credentials to Logon
            credentials.LogOn(new SteamUser.LogOnDetails { Username = user, Password = pass, AuthCode = authCode, TwoFactorCode = twoFactorAuth, SentryFileHash = sentryHash });
        }

        static void OnLoggedOff(SteamUser.LoggedOffCallback callback)
        {
            Console.WriteLine("Logged off of Steam: {0}", callback.Result);
        }

        static void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            // after recieving an AccountLogonDenied, we'll be disconnected from steam
            // so after we read an authcode from the user, we need to reconnect to begin the logon flow again

            Console.WriteLine("Disconnected from Steam, reconnecting in 1...");

            Thread.Sleep(TimeSpan.FromSeconds(1));

            client.Connect();
        }

        static void OnLoggedIn(SteamUser.LoggedOnCallback callback)
        {
            bool isSteamGuard = callback.Result == EResult.AccountLogonDenied;
            bool is2FA = callback.Result == EResult.AccountLoginDeniedNeedTwoFactor;
            if (isSteamGuard || is2FA)
            {
                Console.WriteLine("This account is SteamGuard protected!");

                if (is2FA)
                {
                    Console.Write("Please enter your 2 factor auth code from your authenticator app: ");
                    twoFactorAuth = Console.ReadLine();
                }
                else
                {
                    Console.Write("Please enter the auth code sent to the email at {0}: ", callback.EmailDomain);
                    authCode = Console.ReadLine();
                }

                return;
            }
            if (callback.Result != EResult.OK)
            {
                Console.WriteLine("Unable to logon to Steam: {0} / {1}", callback.Result, callback.ExtendedResult);

                isRunning = false;
                return;
            }

            Console.WriteLine("Successfully logged on!");
        }

        static void OnMachineAuth(SteamUser.UpdateMachineAuthCallback callback)
        {
            Console.WriteLine("Updating sentryfile...");

            // write out our sentry file
            // ideally we'd want to write to the filename specified in the callback
            // but then this sample would require more code to find the correct sentry file to read during logon
            // for the sake of simplicity, we'll just use "sentry.bin"

            int fileSize;
            byte[] sentryHash;
            using (var fs = File.Open("sentry.bin", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.Seek(callback.Offset, SeekOrigin.Begin);
                fs.Write(callback.Data, 0, callback.BytesToWrite);
                fileSize = (int)fs.Length;

                fs.Seek(0, SeekOrigin.Begin);
                using (var sha = SHA1.Create())
                {
                    sentryHash = sha.ComputeHash(fs);
                }
            }

            // inform the steam servers that we're accepting this sentry file
            credentials.SendMachineAuthResponse(new SteamUser.MachineAuthDetails
            {
                JobID = callback.JobID,

                FileName = callback.FileName,

                BytesWritten = callback.BytesToWrite,
                FileSize = fileSize,
                Offset = callback.Offset,

                Result = EResult.OK,
                LastError = 0,

                OneTimePassword = callback.OneTimePassword,

                SentryFileHash = sentryHash,
            });

            Console.WriteLine("Done!");
        }
    }
}
