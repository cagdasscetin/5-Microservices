FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5033
EXPOSE 7040

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Core/Micro.Application/Micro.Application.csproj", "Core/Micro.Application/"]
COPY ["Core/Micro.Domain/Micro.Domain.csproj", "Core/Micro.Domain/"]
COPY ["Infrastructure/Micro.Persistence/Micro.Persistence.csproj", "Infrastructure/Micro.Persistence/"]
COPY ["Presentation/Stock.Consumer/Stock.Consumer.csproj", "Presentation/Stock.Consumer/"]
RUN dotnet restore "Presentation/Stock.Consumer/Stock.Consumer.csproj"
COPY . .
WORKDIR "/src/Presentation/Stock.Consumer"
RUN dotnet build "Stock.Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Stock.Consumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Stock.Consumer.dll"]
