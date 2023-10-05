using Discord.Interactions;
using Discord.Rest;

namespace Discord.Net.DocfxDocs.Modules;

[DefaultMemberPermissions(GuildPermission.Administrator)]
[Group("docs", "docs")]
public class CommandsModule : RestInteractionModuleBase<RestInteractionContext>
{
    private readonly ILogger<CommandsModule> _logger;
    private readonly DiscordNetDocsTools _tools;

    private static CancellationTokenSource? _cts;

    public CommandsModule(ILogger<CommandsModule> logger, DiscordNetDocsTools tools)
    {
        _logger = logger;
        _tools = tools;
    }

    [SlashCommand("ping", "Get a pong")]
    public async Task PingAsync()
    {
        await RespondAsync("Pong!");
    }

    [SlashCommand("build", "Build & serve Discord.Net docs")]
    public async Task BuildCommandAsync()
    {
        _logger.LogInformation("Received build request");

        _cts = new();

        await RespondAsync(embed: new EmbedBuilder()
            .WithTitle("Docs rebuild")
            .WithColor(0x4CA7ED)
            .AddField("Status", "`Updating environment...`")
            .WithCurrentTimestamp()
            .Build());

        await Task.Run(async () =>
        {
            try
            {
                await _tools.InitEnvironmentAsync();

                await ModifyOriginalResponseAsync(x => x.Embed = new EmbedBuilder()
                    .WithTitle("Docs rebuild")
                    .WithColor(0xE8ED2C)
                    .AddField("Status", "`Building docs...`")
                    .WithCurrentTimestamp()
                    .Build());

                await _tools.BuildDocsAsync();

                await ModifyOriginalResponseAsync(x => x.Embed = new EmbedBuilder()
                    .WithTitle("Docs rebuild")
                    .WithColor(0xff00)
                    .AddField("Status", "`Site is online!`")
                    .WithCurrentTimestamp()
                    .Build());
            }
            catch (Exception ex)
            {
                await ModifyOriginalResponseAsync(x =>
                {
                    x.Content = "Something went wrong!";
                    x.Embed = new EmbedBuilder().WithDescription($"{ex.Message}\n{ex.StackTrace}").Build();
                });
            }
        }, _cts.Token).ContinueWith(async (t) =>
        {
            if (t.IsCanceled)
            {
                await ModifyOriginalResponseAsync(x =>
                {
                    x.Embed = new EmbedBuilder()
                        .WithTitle("Docs rebuild")
                        .WithColor(0xff0000)
                        .AddField("Status", "`Build cancelled`")
                        .WithCurrentTimestamp()
                        .Build();
                });
            }

            if (t.IsFaulted)
            {
                await ModifyOriginalResponseAsync(x =>
                {
                    x.Content = "Something went wrong!";

                    if (t.Exception is not null)
                        x.Embed = new EmbedBuilder().WithDescription($"{t.Exception.Message}\n{t.Exception.StackTrace}").Build();
                });
            }
        });
    }

    [SlashCommand("cancel", "Cancel pending docs build process")]
    public async Task CancelBuildAsync()
    {
        _cts?.Cancel();

        await RespondAsync("Build cancelled");
    }
}