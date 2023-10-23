# Discord.Net.DocfxDocs

Docs project for [Discord.Net](https://github.com/discord-net/Discord.Net)
- works on docfx 2.71.0
- rebuild docs with a command in discord
- clone repo, pull updates & serve docs with no user input
- *should* work fine in a docker container


## Running docker container
```bash
docker run -d -e DNet_Token='' \
-e DNet_PublicKey='' \
-p 5000:5000 \
--name dnet_docfx_docs \
discord.net.docfxdocs
```