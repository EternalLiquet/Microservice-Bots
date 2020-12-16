# Microservice Bots
A library that turns Discord.NET messages into JSON serializable objects that can be sent to queues and converted back to Discord.NET messages.  

If you found this library particularly useful or helpful please consider supporting me:  
<a href='https://ko-fi.com/liquet' target='_blank'><img height='35' style='border:0px;height:46px;' src='https://az743702.vo.msecnd.net/cdn/kofi3.png?v=0' border='0' alt='Buy Me a Coffee at ko-fi.com' /></a>

### Usage Example

#### Gateway Application:  
Note: You would use Discord.NET here to log your bot in and listen to events, for more information on getting this set up, [please refer to the documentation here](https://discord.foxbot.me/docs/)
```cs
    [Name("Test Commands")]
    public class TestModule : ModuleBase
    {
        //I am only using SQS as an example here. You can use any queue you want, from Azure to AWS to Kafka. Depends on which one you want to set up. 
        private readonly AWSSQSService _AWSSQSService;

        public TestModule(AWSSQSService AWSSQSService)
        {
            _AWSSQSService = AWSSQSService;
        }

        [Command("ping")]
        public Task Ping()
        {
            await _AWSSQSService.PostMessageAsync(JsonConvert.SerializeObject(new DiscordMessage(context.Message, context.Guild.Id))); 
            //Only use this if you NEED to track the guild ID and it's for sure only going to be used in guilds. (I recommend creating a PreCondition that checks if the command is being used in a guild)
            
            await _AWSSQSService.PostMessageAsync(JsonConvert.SerializeObject(new DiscordMessage(context.Message))); 
            //Otherwise, this will work since to send messages, you technically only need the Channel ID
        }
    }
```  
  
#### In Your Microservice:  
Note: I am using AWS Lambda here but you can use any service you want that lets you host microservices  
```cs
    public class PingFunction
    {
        private static HttpClient httpClient = new HttpClient();
        
        public PingFunction()
        {

        }
        
        //The method that handles the events that trigger the lambda
        public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {
            foreach (var message in evnt.Records)
            {
                await ProcessMessageAsync(message, context);
            }
        }
        
        //Messages are processed here
        private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
        {
            var discordMessage = new DiscordMessage(message.Body); //message.Body comes in as a string, this will deserialize the string back into a JSON object.
            var messageContent = discordMessage.Content; 
            if(messageContent.Contains("!ping")) //This is not super necessary as Discord.NET already handles this check in your Gateway Application. You can have other checks here though. 
            {
                var reply = new DiscordReply(discordMessage, "pong!"); //We create a new reply
                //After this, you can choose any which way to send the data back to the Gateway Application. From, HTTP requests to additional queues. The choice is yours.
                //This example will assume you used an HTTP Post Request
                var content = new StringContent(reply.ToString(), Encoding.UTF8, "application/json");
                await client.PostAsync("examplegatewayapplication.io/api/Discord/reply", content); 
            }
        }
    }
```  
  
#### Back In Your Gateway Application:  
Note: This assumes that you've used an HTTP request to give the data back to the Gateway Application, but you can use any method you want to achieve this.  
```cs
    [Route("api/[controller]")]
    [ApiController]
    public class DiscordController : ControllerBase
    {
        private readonly DiscordShardedClient _discordClient;

        public DiscordController(DiscordShardedClient discordClient)
        {
            _discordClient = discordClient;
        }

        // POST: /api/Discord/Reply
        [HttpPost("Reply")]
        public IActionResult Reply([FromBody] DiscordReply message) // Get back data in the form of a DiscordReply object
        {
            try
            {
                var channel = _discordClient.GetChannel(message.ChannelId) as ITextChannel; //Use the discord client to get the channel provided by the DiscordReply object
                await textChannel.SendMessagesAsync(message.Reply); //Use this channel to send our reply. 
            }
            catch (Exception ex)
            {
                //Log your exception here
            }
            return Ok();
        }
    }
```

## Benefits of using a microservices architecture when it comes to Discord Bots  
You're probably looking at that example thinking that was a really long winded way to achieve what could be done with a simple 
```cs
        [Command("ping")]
        public Task Ping()
        {
            await ReplyAsync("pong!");
        }
```  
I guarantee you that there is some benefit to this though.  
  
### Reusability  
If you have 3-4 different custom bots that all share common commands (i.e an 8ball command or something) then they can all connect to that same microservice and you aren't copy pasting the code for an 8ball command over and over. This also reduces the amount of time it takes to develop complex services (i.e a leveling system or a Twitch Listener) as you can develop it once as a microservice and use it on any bot you have developed previously or will develop in the future with relatively small effort.  
  
### Programming Language Flexibility  
If you want to write your Gateway Application in C# or Java but want to write your microservice in Javascript or Python, you can do that! The purpose of this library is to turn the Discord.NET message events into something that translates into a basic string that can be passed around from application to application. You would just have to make sure the JSON/strings are in the correct format!  

### Avoid Gateway Blocking (Library Specific Issues)  
If you're familiar with how Discord.NET works then you know that it is ill-advised to have long running commands as that can block Gateway from recieving events and cause some issues with missed commands. This insures that you have a very fire-and-forget style of handling events and commands have very little chance of actually blocking a Gateway Event.  

### As a side note, if you would like to support the further development of this library:  
* Feel free to fork the repo and PR back any additions
* Contact me at aldmnatividad@gmail.com for any suggestions
* Donate to my [Ko-fi here](https://ko-fi.com/liquet) (I do this in my free time so any donation you can give helps me develop more things like this!)  
  
