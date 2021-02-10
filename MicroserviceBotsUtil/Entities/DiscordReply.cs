using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroserviceBotsUtil.Entities
{
    public class DiscordReply
    {
        /// <summary> 
        /// The Id / Snowflake of the author of this message. 
        /// </summary>
        public ulong AuthorId { get; set; }
        /// <summary> 
        /// The Id / Snowflake of the channel this message was posted in.
        /// </summary>
        public ulong ChannelId { get; set; }
        /// <summary> 
        /// The Id / Snowflake of the guild this message was posted in.
        /// </summary>
        public ulong GuildId { get; set; }
        /// <summary> 
        /// The content of the message. IE. the message as seen in Discord
        /// </summary>
        public string Reply { get; set; }

        public DiscordReply() { }
        public DiscordReply(DiscordMessage message, string reply)
        {
            AuthorId = message.AuthorId;
            ChannelId = message.ChannelId;
            if (!message.GuildId.Equals(null))
                GuildId = message.GuildId;
            Reply = reply;
        }

        public DiscordReply(string message)
        {
            var jsonMessage = JsonConvert.DeserializeObject<DiscordReply>(message);
            AuthorId = jsonMessage.AuthorId;
            ChannelId = jsonMessage.ChannelId;
            if (!jsonMessage.GuildId.Equals(null))
                GuildId = jsonMessage.GuildId;
            Reply = jsonMessage.Reply;
        }

        /// <summary> 
        /// Returns the Message as a JSON formatted string
        /// </summary>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }
    }
}
