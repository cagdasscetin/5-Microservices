FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5062
EXPOSE 7117

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Core/Micro.Application/Micro.Application.csproj", "Core/Micro.Application/"]
COPY ["Core/Micro.Domain/Micro.Domain.csproj", "Core/Micro.Domain/"]
COPY ["Infrastructure/Micro.Persistence/Micro.Persistence.csproj", "Infrastructure/Micro.Persistence/"]
COPY ["Presentation/OutboxWorker/OutboxWorker.csproj", "Presentation/OutboxWorker/"]
RUN dotnet restore "Presentation/OutboxWorker/OutboxWorker.csproj"
COPY . .
WORKDIR "/src/Presentation/OutboxWorker"
RUN dotnet build "OutboxWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OutboxWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OutboxWorker.dll"]