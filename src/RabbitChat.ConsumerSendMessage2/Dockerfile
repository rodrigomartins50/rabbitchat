#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/RabbitChat.ConsumerSendMessage2/RabbitChat.ConsumerSendMessage2.csproj", "src/RabbitChat.ConsumerSendMessage2/"]
COPY ["src/RabbitChat.Domain/RabbitChat.Domain.csproj", "src/RabbitChat.Domain/"]
COPY ["src/RabbitChat.Infra.AmqpAdapters/RabbitChat.Infra.AmqpAdapters.csproj", "src/RabbitChat.Infra.AmqpAdapters/"]
COPY ["src/RabbitChat.Data/RabbitChat.Data.csproj", "src/RabbitChat.Data/"]
RUN dotnet restore "src/RabbitChat.ConsumerSendMessage2/RabbitChat.ConsumerSendMessage2.csproj"
COPY . .
WORKDIR "/src/src/RabbitChat.ConsumerSendMessage2"
RUN dotnet build "RabbitChat.ConsumerSendMessage2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RabbitChat.ConsumerSendMessage2.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RabbitChat.ConsumerSendMessage2.dll"]