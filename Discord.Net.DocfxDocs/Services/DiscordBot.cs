using System.Reflection;
using Discord.Interactions;
using Discord.Rest;

namespace Discord.Net.DocfxDocs.Services;

public class DiscordBot : IHostedService
{
    private readonly ILogger<DiscordBot> _logger;
    private readonly IConfiguration _config;
    private readonly DiscordNetDocsTools _docsTools;
    private readonly DiscordRestClient _client;
    private readonly InteractionService _interactionService;

    private readonly IServiceProvider _serviceProvider;

    public DiscordBot(IConfiguration config, DiscordNetDocsTools docsTools, DiscordRestClient client, InteractionService interactionService, ILogger<DiscordBot> logger,
        IServiceProvider serviceProvider)
    {
        _config = config;
        _docsTools = docsTools;
        _client = client;
        _interactionService = interactionService;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _client.Log += Log;
        _interactionService.Log += Log;

        await _client.LoginAsync(TokenType.Bot, _config["DNet_Token"]);

        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
        await _interactionService.RegisterCommandsGloballyAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {

    }

    public Task Log(LogMessage msg)
    {
        var severity = msg.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Trace,
            LogSeverity.Debug => LogLevel.Debug,
            _ => LogLevel.Information
        };

        _logger.Log(severity, msg.Exception, msg.Message);

         return Task.CompletedTask;
    }
}