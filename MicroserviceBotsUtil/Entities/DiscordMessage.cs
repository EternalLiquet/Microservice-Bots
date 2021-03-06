﻿using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroserviceBotsUtil.Entities
{
    public class DiscordMessage
    {
        /// <summary> 
        /// The Id / Snowflake of this message. 
        /// </summary>
        public ulong MessageId { get; set; }
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
        /// The source of the message - System, User, Bot or Webhook.
        /// </summary>
        public MessageSource Source { get; set; }
        /// <summary> 
        /// The content of the message. IE. the message as seen in Discord
        /// </summary>
        public string Content { get; set; }
        /// <summary> 
        /// The creation date of the message 
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }
        /// <summary> 
        /// A flag that determines if the message has been pinned
        /// </summary>
        public bool IsPinned { get; set; }

        /// <summary> 
        /// A collection of Ids / Snowflakes of channels mentioned within the message
        /// </summary>
        public ICollection<ulong> MentionedChannelIDs { get; set; }
        /// <summary> 
        /// A collection of Ids / Snowflakes of roles mentioned within the message
        /// </summary>
        public ICollection<ulong> MentionedRoleIDs { get; set; }
        /// <summary> 
        /// A collection of Ids / Snowflakes of users mentioned within the message
        /// </summary>
        public ICollection<ulong> MentionedUserIDs { get; set; }
        /// <summary> 
        /// A collection of Ids / Snowflakes of attachments associated within the message
        /// </summary>
        public ICollection<ulong> AttachmentIDs { get; set; }

        public DiscordMessage() { }

        public DiscordMessage(SocketMessage message)
        {
            MessageId = message.Id;
            AuthorId = message.Author.Id;
            ChannelId = message.Channel.Id;
            Source = message.Source;
            Content = message.Content;
            CreatedAt = message.CreatedAt;
            IsPinned = message.IsPinned;

            MentionedChannelIDs = new List<ulong>();
            foreach (var channel in message.MentionedChannels)
                MentionedChannelIDs.Add(channel.Id);

            MentionedRoleIDs = new List<ulong>();
            foreach (var role in message.MentionedRoles)
                MentionedRoleIDs.Add(role.Id);

            MentionedUserIDs = new List<ulong>();
            foreach (var user in message.MentionedUsers)
                MentionedUserIDs.Add(user.Id);

            AttachmentIDs = new List<ulong>();
            foreach (var attachment in message.Attachments)
                AttachmentIDs.Add(attachment.Id);
        }

        public DiscordMessage(SocketMessage message, ulong guildId)
        {
            MessageId = message.Id;
            AuthorId = message.Author.Id;
            ChannelId = message.Channel.Id;
            GuildId = guildId;
            Source = message.Source;
            Content = message.Content;
            CreatedAt = message.CreatedAt;
            IsPinned = message.IsPinned;

            MentionedChannelIDs = new List<ulong>();
            foreach (var channel in message.MentionedChannels)
                MentionedChannelIDs.Add(channel.Id);

            MentionedRoleIDs = new List<ulong>();
            foreach (var role in message.MentionedRoles)
                MentionedRoleIDs.Add(role.Id);

            MentionedUserIDs = new List<ulong>();
            foreach (var user in message.MentionedUsers)
                MentionedUserIDs.Add(user.Id);

            AttachmentIDs = new List<ulong>();
            foreach (var attachment in message.Attachments)
                AttachmentIDs.Add(attachment.Id);
        }

        public DiscordMessage(RestMessage message)
        {
            MessageId = message.Id;
            AuthorId = message.Author.Id;
            ChannelId = message.Channel.Id;
            Source = message.Source;
            Content = message.Content;
            CreatedAt = message.CreatedAt;
            IsPinned = message.IsPinned;

            MentionedChannelIDs = new List<ulong>();
            foreach (var channel in message.MentionedChannelIds)
                MentionedChannelIDs.Add(channel);

            MentionedRoleIDs = new List<ulong>();
            foreach (var role in message.MentionedRoleIds)
                MentionedRoleIDs.Add(role);

            MentionedUserIDs = new List<ulong>();
            foreach (var user in message.MentionedUsers)
                MentionedUserIDs.Add(user.Id);

            AttachmentIDs = new List<ulong>();
            foreach (var attachment in message.Attachments)
                AttachmentIDs.Add(attachment.Id);
        }

        public DiscordMessage(RestMessage message, ulong guildId)
        {
            MessageId = message.Id;
            AuthorId = message.Author.Id;
            ChannelId = message.Channel.Id;
            GuildId = guildId;
            Source = message.Source;
            Content = message.Content;
            CreatedAt = message.CreatedAt;
            IsPinned = message.IsPinned;

            MentionedChannelIDs = new List<ulong>();
            foreach (var channel in message.MentionedChannelIds)
                MentionedChannelIDs.Add(channel);

            MentionedRoleIDs = new List<ulong>();
            foreach (var role in message.MentionedRoleIds)
                MentionedRoleIDs.Add(role);

            MentionedUserIDs = new List<ulong>();
            foreach (var user in message.MentionedUsers)
                MentionedUserIDs.Add(user.Id);

            AttachmentIDs = new List<ulong>();
            foreach (var attachment in message.Attachments)
                AttachmentIDs.Add(attachment.Id);
        }

        public DiscordMessage(string message)
        {
            var jsonMessage = JsonConvert.DeserializeObject<DiscordMessage>(message);
            MessageId = jsonMessage.MessageId;
            AuthorId = jsonMessage.AuthorId;
            ChannelId = jsonMessage.ChannelId;
            if (!jsonMessage.GuildId.Equals(null))
                GuildId = jsonMessage.GuildId;
            Source = jsonMessage.Source;
            Content = jsonMessage.Content;
            CreatedAt = jsonMessage.CreatedAt;
            IsPinned = jsonMessage.IsPinned;

            MentionedChannelIDs = new List<ulong>();
            foreach (var channel in jsonMessage.MentionedChannelIDs)
                MentionedChannelIDs.Add(channel);

            MentionedRoleIDs = new List<ulong>();
            foreach (var role in jsonMessage.MentionedRoleIDs)
                MentionedRoleIDs.Add(role);

            MentionedUserIDs = new List<ulong>();
            foreach (var user in jsonMessage.MentionedUserIDs)
                MentionedUserIDs.Add(user);

            AttachmentIDs = new List<ulong>();
            foreach (var attachment in jsonMessage.AttachmentIDs)
                AttachmentIDs.Add(attachment);
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
