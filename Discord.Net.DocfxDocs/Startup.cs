using Discord.Net.DocfxDocs;
using Discord.Net.DocfxDocs.Services;
using Discord.Rest;

using Microsoft.AspNetCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables(prefix: "DNet_");

builder.Services.AddSingleton<DiscordNetDocsTools>();

builder.Services.AddSingleton<DiscordRestClient>();
builder.Services.AddHostedService<DiscordBot>();

builder.Services.AddInteractionService(config => config.UseCompiledLambda = true);

var app = builder.Build();


var sitePath = Path.Combine(app.Environment.ContentRootPath, app.Configuration["Docs:site_root"]!);
var pagesPath = Path.Combine(app.Environment.ContentRootPath, "Pages");

var docsBeingBuiltPage = File.ReadAllText(Path.Combine(pagesPath, "docs_being_built.html"));

if (!Directory.Exists(sitePath))
    Directory.CreateDirectory(sitePath);

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments(app.Configuration["Discord:InteractionsPath"]))
    {
        await next.Invoke();
        return;
    }

    var tools = context.RequestServices.GetRequiredService<DiscordNetDocsTools>();

    if (!tools.DocsAvailable)
    {
        await context.Response.WriteAsync(docsBeingBuiltPage.Replace("@console_logs", "<li>" + string.Join("</li>\n<li>", $"{tools.ConsoleHistory}") + "</li>"));
    }
    else
    {
        await next.Invoke();
    }
});

app.MapInteractionService("/discord-interactions", app.Configuration["DNet_PublicKey"]!);

app.UseStaticFiles(new StaticFileOptions
{
    RequestPath = "",
    FileProvider = new PhysicalFileProvider(sitePath)
});

app.MapGet("/", () => Results.Redirect("/index.html"));

await app.RunAsync();
