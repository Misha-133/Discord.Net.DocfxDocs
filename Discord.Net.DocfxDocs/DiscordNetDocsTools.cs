using Docfx;
using Docfx.Dotnet;

using LibGit2Sharp;

namespace Discord.Net.DocfxDocs;

public class DiscordNetDocsTools
{
    private readonly string _siteRoot;
    private readonly string _repoRoot;
    private readonly string _repo;
    private readonly string _branch;
    private readonly IConfiguration _config;
    private readonly ILogger<DiscordNetDocsTools> _logger;
    private readonly IWebHostEnvironment _environment;

    public Queue<string> ConsoleHistory = new();

    public bool DocsAvailable;

    public DiscordNetDocsTools(IConfiguration config, ILogger<DiscordNetDocsTools> logger, IWebHostEnvironment environment)
    {
        _config = config;

        _environment = environment;
        _repoRoot = Path.Combine(_environment.ContentRootPath, config["Docs:repo_root"]);
        _siteRoot = Path.Combine(_environment.ContentRootPath, config["Docs:site_root"]);
        _repo = config["Docs:repo"];
        _branch = config["Docs:branch"];
        _logger = logger;

        DocsAvailable = false;
    }

    public async Task InitEnvironmentAsync()
    {
        _logger.LogInformation("Initializing docs environment");

        if (!Directory.Exists(_repoRoot) || !Directory.EnumerateFiles(_repoRoot).Any())
        {
            await SetupEnvironmentAsync();
        }
        else
        {
            await UpdateEnvironmentAsync();
        }
    }

    public async Task SetupEnvironmentAsync()
    {
        _logger.LogInformation("Setting up docs environment");

        if (!Directory.Exists(_repoRoot))
            Directory.CreateDirectory(_repoRoot);

        _logger.LogInformation("Cloning repo...");

        var options = new CloneOptions
        {
            BranchName = _branch
        };
        Repository.Clone(_repo, _repoRoot, options);

        _logger.LogInformation("Repository clone complete");
    }

    public async Task UpdateEnvironmentAsync()
    {
        using var repo = new Repository(_repoRoot);

        PullOptions options = new PullOptions();

        var signature = new Signature(
            new Identity("dnet_bot", "amongus@among.us"), DateTimeOffset.Now);

        _logger.LogInformation("Pulling repo...");

        LibGit2Sharp.Commands.Pull(repo, signature, options);
        _logger.LogInformation("Repository pull complete");

    }

    public async Task BuildDocsAsync()
    {
        DocsAvailable = false;

        _logger.LogInformation("Building docs...");
        ConsoleHistory.Clear();

        await using (var capture = new ConsoleOutputCapture())
        {
            capture.OnWriteLine += (source, args) =>
            {
                if (ConsoleHistory.Count() >= 100)
                    ConsoleHistory.Dequeue();
                ConsoleHistory.Enqueue(args.Line);
            };

            try
            {
                await DotnetApiCatalog.GenerateManagedReferenceYamlFiles(Path.Combine(_repoRoot, "docs", "docfx.json"));

                await Docset.Build(Path.Combine(_repoRoot, "docs", "docfx.json"));

                DocsAvailable = true;

                _logger.LogInformation("Build complete. Copying to static root...");

                FileUtils.CopyFiles(Path.Combine(_repoRoot, "docs", "_site"), _siteRoot);

                _logger.LogInformation("Copy complete");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);




            }
            capture.OnWriteLine -= (source, args) =>
                {
                    if (ConsoleHistory.Count() >= 100)
                        ConsoleHistory.Dequeue();
                    ConsoleHistory.Enqueue(args.Line);
                };
        }
    }
}