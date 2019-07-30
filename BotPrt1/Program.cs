using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using SteamKit2;
using System.Threading;

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

        static void Main(string[] args)
        {
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
            manager.Subscribe<SteamFriends.ChatMsgCallback>(OnChatMsg);
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

        private static void OnChatMsg(SteamFriends.ChatMsgCallback callback)
        {

        }

        static void OnAccountInfo(SteamUser.AccountInfoCallback callback)
        {
            if(friends.GetPersonaName() != "VietNam Cards Trading Bot")
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
