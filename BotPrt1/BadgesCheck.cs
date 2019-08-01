using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Json;

namespace BotPrt1
{
    class BadgesCheck
    {
        public static string[] owned = new string[1000];
        private static readonly HttpClient client = new HttpClient();
        static async Task maintask()
        {
            int i = 0;
            var responseString = await client.GetStringAsync("https://api.steampowered.com/IPlayerService/GetBadges/v1/?key=B3B34FA60F4E3A3C8C53CA40D510ADA4&steamid=76561198141936333");
            JsonValue obj = JsonObject.Parse(responseString);
            foreach (JsonValue c in obj["response"]["badges"])
            {
                owned[i] = c["badgeid"].ToString();
                i++;
            }
            owned = owned.Where(c => c != null).ToArray();
        }
    }
}
