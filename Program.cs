﻿using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PassiveBOT.Discord;
using PassiveBOT.Handlers;
using PassiveBOT.Models;
using Raven.Client.Documents;

namespace PassiveBOT
{
    public class Program
    {
        public static DiscordSocketClient _client;
        private CommandHandler _handler;

        public static void Main(string[] args)
        {
            new Program().Start().GetAwaiter().GetResult();
        }

        public async Task Start()
        {
            Console.Title = "PassiveBOT";
            Console.WriteLine(
                "██████╗  █████╗ ███████╗███████╗██╗██╗   ██╗███████╗██████╗  ██████╗ ████████╗\n" +
                "██╔══██╗██╔══██╗██╔════╝██╔════╝██║██║   ██║██╔════╝██╔══██╗██╔═══██╗╚══██╔══╝\n" +
                "██████╔╝███████║███████╗███████╗██║██║   ██║█████╗  ██████╔╝██║   ██║   ██║   \n" +
                "██╔═══╝ ██╔══██║╚════██║╚════██║██║╚██╗ ██╔╝██╔══╝  ██╔══██╗██║   ██║   ██║   \n" +
                "██║     ██║  ██║███████║███████║██║ ╚████╔╝ ███████╗██████╔╝╚██████╔╝   ██║   \n" +
                "╚═╝     ╚═╝  ╚═╝╚══════╝╚══════╝╚═╝  ╚═══╝  ╚══════╝╚═════╝  ╚═════╝    ╚═╝   \n" +
                "/--------------------------------------------------------------------------\\ \n" +
                "| Designed by PassiveModding - PassiveNation.com  ||   Status: Connected   | \n" +
                "\\--------------------------------------------------------------------------/ \n");

            if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "setup/")))
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "setup/"));
            ConfigModel.CheckExistence();

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 50
            });

            try
            {
                await _client.LoginAsync(TokenType.Bot, ConfigModel.Load().Token);
                await _client.StartAsync();
            }
            catch (Exception e)
            {
                LogHandler.LogMessage($"Token was rejected by Discord (Invalid Token or Connection Error)\n{e}", LogSeverity.Critical);
            }

            var serviceProvider = ConfigureServices();
            _handler = new CommandHandler(serviceProvider);
            await _handler.ConfigureAsync();

            _client.Log += MLog;

            await Task.Delay(-1);
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(new DocumentStore
                {
                    Database = DatabaseHandler.DBName,
                    Urls = new[]
                    {
                        DatabaseHandler.ServerURL
                    }
                }.Initialize())
                .AddSingleton(new DatabaseHandler(new DocumentStore
                {
                    Urls = new[]
                    {
                        ConfigModel.Load().DBUrl
                    }
                }.Initialize()))
                .AddSingleton(new TimerService(_client))
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    CaseSensitiveCommands = false,
                    ThrowOnError = false,
                    DefaultRunMode = RunMode.Async
                }));
            return services.BuildServiceProvider();
        }

        public static Task MLog(LogMessage msg)
        {
            LogHandler.LogMessage(msg.Message, msg.Severity);
            return Task.CompletedTask;
        }
    }
}