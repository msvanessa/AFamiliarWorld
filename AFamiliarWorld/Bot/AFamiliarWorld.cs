using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace AFamiliarWorld.Bot;

public class AFamiliarWorld
{
    private DiscordSocketClient _client;
    private CommandHandler _commands;
    private CommandService _commandService;
    public AFamiliarWorld()
    {
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.All,
            LogLevel = LogSeverity.Debug,
        };
        
        this._commandService = new CommandService();
        _client = new DiscordSocketClient(config);
        this._commands = new CommandHandler(_client, _commandService);
    }

    public async Task RunAsync(string token)
    {
        await _commands.InstallCommandsAsync();
        

        _client.Log += Log;


        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await Task.Delay(-1);
    }
    public async Task Log(LogMessage arg)
    {
        Console.WriteLine(arg);
        await Task.CompletedTask;
    }
}