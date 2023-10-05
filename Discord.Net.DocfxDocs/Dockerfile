#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.


FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5000

RUN apt-get update && \
    apt-get upgrade -y && \
    apt-get install -y wget git

RUN wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN rm packages-microsoft-prod.deb

RUN apt-get update && \
    apt-get upgrade -y && \
    apt-get install -y dotnet-sdk-7.0

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Discord.Net.DocfxDocs/Discord.Net.DocfxDocs.csproj", "Discord.Net.DocfxDocs/"]
RUN dotnet restore "Discord.Net.DocfxDocs/Discord.Net.DocfxDocs.csproj"
COPY . .
WORKDIR "/src/Discord.Net.DocfxDocs"
RUN dotnet build "Discord.Net.DocfxDocs.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "Discord.Net.DocfxDocs.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Discord.Net.DocfxDocs.dll"]