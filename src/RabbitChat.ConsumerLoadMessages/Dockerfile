#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/RabbitChat.ConsumerLoadMessages/RabbitChat.ConsumerLoadMessages.csproj", "src/RabbitChat.ConsumerLoadMessages/"]
COPY ["src/RabbitChat.Shared/RabbitChat.Shared.csproj", "src/RabbitChat.Shared/"]
COPY ["src/RabbitChat.Application/RabbitChat.Application.csproj", "src/RabbitChat.Application/"]
RUN dotnet restore "src/RabbitChat.ConsumerLoadMessages/RabbitChat.ConsumerLoadMessages.csproj"
COPY . .
WORKDIR "/src/src/RabbitChat.ConsumerLoadMessages"
RUN dotnet build "RabbitChat.ConsumerLoadMessages.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RabbitChat.ConsumerLoadMessages.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RabbitChat.ConsumerLoadMessages.dll"]
