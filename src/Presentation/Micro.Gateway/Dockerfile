FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5168
EXPOSE 7279

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Core/Micro.Application/Micro.Application.csproj", "Core/Micro.Application/"]
COPY ["Core/Micro.Domain/Micro.Domain.csproj", "Core/Micro.Domain/"]
COPY ["Infrastructure/Micro.Persistence/Micro.Persistence.csproj", "Infrastructure/Micro.Persistence/"]
COPY ["Presentation/Micro.Gateway/Micro.Gateway.csproj", "Presentation/Micro.Gateway/"]
RUN dotnet restore "Presentation/Micro.Gateway/Micro.Gateway.csproj"
COPY . .
WORKDIR "/src/Presentation/Micro.Gateway"
RUN dotnet build "Micro.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Micro.Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Micro.Gateway.dll"]
