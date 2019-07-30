using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;

namespace BotPrt1
{
    class AdminLoginFormHost
    {
        private static string user, pass;
        static SteamClient client;
        static CallbackManager manager;
        static SteamUser credentials;
        public void login()
        {
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

            //Declare dependencies
            manager.Subscribe<SteamClient.ConnectedCallback>(onConnectingToSteam);

            //Connect Client to Steam
            client.Connect();

            Console.WriteLine("Connecting to Steam...");

        }
        static void onConnectingToSteam(SteamClient.ConnectedCallback callback)
        {
            Console.WriteLine("Connected to Steam, Logging in Account {0} ...", user);
            credentials.LogOn(new SteamUser.LogOnDetails { Username = user, Password = pass, });

        }
    }
}
