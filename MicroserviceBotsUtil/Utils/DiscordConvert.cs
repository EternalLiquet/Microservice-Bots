using Discord.WebSocket;
using MicroserviceBotsUtil.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroserviceBotsUtil.Utils
{
    public static class DiscordConvert
    {
        public static string SerializeObject(SocketMessage message)
        {
            var converted = new ConvertedMessage(message);
            return JsonConvert.SerializeObject(converted, Formatting.None);
        }

        public static ConvertedMessage DeSerializeObject(string jsonString)
        {
            return JsonConvert.DeserializeObject<ConvertedMessage>(jsonString);
        }
    }
}
