#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/RabbitChat.ConsumerSendMessage/RabbitChat.ConsumerSendMessage.csproj", "src/RabbitChat.ConsumerSendMessage/"]
RUN dotnet restore "src/RabbitChat.ConsumerSendMessage/RabbitChat.ConsumerSendMessage.csproj"
COPY . .
WORKDIR "/src/src/RabbitChat.ConsumerSendMessage"
RUN dotnet build "RabbitChat.ConsumerSendMessage.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RabbitChat.ConsumerSendMessage.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RabbitChat.ConsumerSendMessage.dll"]