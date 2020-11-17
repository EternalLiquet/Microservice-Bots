using BeanBot.Helpers;
using Discord;
using Discord.WebSocket;
using MicroserviceBotsUtil.Entities;
using MicroserviceBotsUtil.Utils;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BeanBot.Services
{
    public class DiscordSocketService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IConfiguration _config;

        private static DiscordShardedClient _discordClient;
        private static string _botToken;

        private CloudStorageAccount _storageAccount;
        private CloudQueueClient _queueClient;
        private CloudQueue _pingInboundQueue;

        private string _serviceBusConnectionString;
        const string _queueName = "pingqueue";
        private static IQueueClient _servicebusClient;

        public DiscordSocketService(ILogger<DiscordSocketService> logger, IHostApplicationLifetime appLifetime, IConfiguration config)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _config = config;
            _botToken = _config["BEAN_BOT_TOKEN"];
            _serviceBusConnectionString = _config["AzureWebJobsServiceBus"];
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);

            return Task.CompletedTask;
        }

        private void OnStopped()
        {
            _logger.LogInformation("Calling OnStopped()");
        }

        private void OnStopping()
        {
            _logger.LogInformation("Calling OnStopping()");
        }

        private void OnStarted()
        {
            _logger.LogInformation("Calling Onstarted()");

            LogSeverity logLevel;

            switch (_config["LOGLEVEL"])
            {
                case "DEBUG":
                    logLevel = LogSeverity.Debug;
                    break;
                case "WARNING":
                    logLevel = LogSeverity.Warning;
                    break;
                case "ERROR":
                    logLevel = LogSeverity.Error;
                    break;
                default:
                    logLevel = LogSeverity.Info;
                    break;
            }

            // Setup the Discord Client Configuration
            _discordClient = new DiscordShardedClient(new DiscordSocketConfig
            {
                LogLevel = logLevel
            });

            ConfigureEventHandlers();
            ConfigureStorageQueue();
            ConfigureServiceBus();

            _discordClient.LoginAsync(TokenType.Bot, _botToken).Wait();
            _discordClient.StartAsync().Wait();
        }

        private void ConfigureEventHandlers()
        {
            _discordClient.MessageReceived += async m => await RecieveMessage(m);
        }

        private void ConfigureStorageQueue()
        {
            // Try and load the queue storage account
            try
            {
                _storageAccount = CloudStorageAccount.Parse(_config["StorageQueueConnectionString"]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return;
            }

            _queueClient = _storageAccount.CreateCloudQueueClient();
            _pingInboundQueue = _queueClient.GetQueueReference("ping-inbound-queue");
            _pingInboundQueue.CreateIfNotExistsAsync();
        }

        private void ConfigureServiceBus()
        {
            _servicebusClient = new QueueClient(_serviceBusConnectionString, _queueName);
            var handlerOptions = new MessageHandlerOptions(SBException)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            _servicebusClient.RegisterMessageHandler(ProcessMessage, handlerOptions);
        }

        private Task SBException(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            _logger.LogError("ServiceBus Error | Endpoint: "
                + exceptionReceivedEventArgs.ExceptionReceivedContext.Endpoint + " | "
                + exceptionReceivedEventArgs.Exception.Message);

            return Task.CompletedTask;
        }

        private Task RecieveMessage(SocketMessage message)
        {
            CloudQueueMessage jsonMessage = new CloudQueueMessage(DiscordConvert.SerializeObject(message));
            _pingInboundQueue.AddMessage(jsonMessage);
            return Task.CompletedTask;
        }

        private async Task ProcessMessage(Message message, CancellationToken token)
        {
            var bodyString = Encoding.UTF8.GetString(message.Body);
            Formatter.GenerateLog(_logger, LogSeverity.Info, "Self", "Sending message - Sequence: " + message.SystemProperties.SequenceNumber + " -- Message: " + bodyString);

            try
            {
                NewMessage response = JsonConvert.DeserializeObject(bodyString, typeof(NewMessage)) as NewMessage;
                var channel = _discordClient.GetChannel(response.ChannelId);

                ITextChannel textChannel = channel as ITextChannel;
                if (textChannel != null)
                {
                    await textChannel.SendMessageAsync(response.Content);
                }
                else
                {
                    Formatter.GenerateLog(_logger, LogSeverity.Error, "Self", "Error sending message: Channel is not a text channel");
                }
            }
            catch (Exception ex)
            {
                Formatter.GenerateLog(_logger, LogSeverity.Error, "Self", "Error sending message: " + ex.Message);
            }

            await _servicebusClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _discordClient.LogoutAsync();

            return Task.CompletedTask;
        }
    }
}
