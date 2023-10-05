using Discord.Net.DocfxDocs;
using Discord.Net.DocfxDocs.Services;
using Discord.Rest;

using Microsoft.AspNetCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DiscordNetDocsTools>();


builder.Services.AddSingleton<DiscordRestClient>();
builder.Services.AddHostedService<DiscordBot>();

builder.Services.AddInteractionService(config => config.UseCompiledLambda = true);

var app = builder.Build();


var sitePath = Path.Combine(app.Environment.ContentRootPath, app.Configuration["Docs:site_root"]!);

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
        await context.Response.WriteAsync(
            $$"""
                  <html>
                    <head>
                        <title>Docs rebuild</title>
                        <style>
                            code {
                                width: 95%;
                                font-family: 'Source Code Pro', monospace;
                                color: #43892a;
                                background-color: #000;
                                text-align: left;
                                display: block;
                                align: center
                                 }
                        </style>
                    </head>
                    <body align="center">
                        <h2>Docs are being built, please wait...</h2>
                        <br><br><br><br><br>
                        <pre><code>
                        {{string.Join('\n', tools.ConsoleHistory)}}
                        </code></pre>
                    </body>
                  </html>
                  """);
    }
    else
    {
        await next.Invoke();
    }
});

app.MapInteractionService("/discord-interactions", app.Configuration["Discord:PublicKey"]!);

app.UseStaticFiles(new StaticFileOptions
{
    RequestPath = "",
    FileProvider = new PhysicalFileProvider(sitePath)
});

app.MapGet("/", () => Results.Redirect("/index.html"));

await app.RunAsync();
