# Discord.Net.DocfxDocs

Docs project for [Discord.Net](https://github.com/discord-net/Discord.Net)
- works on docfx 2.71.0
- rebuild docs with a command in discord
- clone repo, pull updates & serve docs with no user input
- *should* work fine in a docker container




## Building docker image
```bash
git clone https://github.com/Misha-133/Discord.Net.DocfxDocs
cd Discord.Net.DocfxDocs
docker build -t discord.net.docfxdocs -f Discord.Net.DocfxDocs/Dockerfile .
```

## Running docker container from local image
```bash
docker run -d -e DNet_Token='' \
-e DNet_PublicKey='' \
-p 5000:5000 \
--name dnet_docfx_docs \
discord.net.docfxdocs
```

## Running docker container from github packages
```bash
docker run -d -e DNet_Token='' \
-e DNet_PublicKey='' \
-p 5000:5000 \
--name dnet_docfx_docs \
ghcr.io/misha-133/discord.net.docfxdocs:latest
```