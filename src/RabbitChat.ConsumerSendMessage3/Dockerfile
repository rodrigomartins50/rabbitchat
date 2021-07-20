#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/RabbitChat.ConsumerSendMessage3/RabbitChat.ConsumerSendMessage3.csproj", "src/RabbitChat.ConsumerSendMessage3/"]
RUN dotnet restore "src/RabbitChat.ConsumerSendMessage3/RabbitChat.ConsumerSendMessage3.csproj"
COPY . .
WORKDIR "/src/src/RabbitChat.ConsumerSendMessage3"
RUN dotnet build "RabbitChat.ConsumerSendMessage3.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RabbitChat.ConsumerSendMessage3.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RabbitChat.ConsumerSendMessage3.dll"]