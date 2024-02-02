namespace Discord.Net.DocfxDocs.Services;

public class StartupBuildService : BackgroundService
{
    private readonly ILogger<StartupBuildService> _logger;
    private readonly DiscordNetDocsTools _tools;
    private readonly IConfiguration _configuration;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_configuration.GetValue<bool>("BuildOnStartup"))
            return;

		_logger.LogInformation("Starting startup build");

        try
        {
            await _tools.InitEnvironmentAsync();
			_logger.LogInformation("Env init complete");

            await _tools.BuildDocsAsync();
			_logger.LogInformation("Startup build complete");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during startup build");
        }
    }
}