using System;
using System.Collections.Generic;
using System.Text;

namespace MicroserviceBotsUtil.Entities
{
    public class NewMessage
    {
        public ulong ChannelId { get; set; }
        public string Content { get; set; }
    }
}
