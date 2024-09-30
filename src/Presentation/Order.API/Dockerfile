FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5036
EXPOSE 7060

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Core/Micro.Application/Micro.Application.csproj", "Core/Micro.Application/"]
COPY ["Core/Micro.Domain/Micro.Domain.csproj", "Core/Micro.Domain/"]
COPY ["Infrastructure/Micro.Persistence/Micro.Persistence.csproj", "Infrastructure/Micro.Persistence/"]
COPY ["Presentation/Order.API/Order.API.csproj", "Presentation/Order.API/"]
RUN dotnet restore "Presentation/Order.API/Order.API.csproj"
COPY . .
WORKDIR "/src/Presentation/Order.API"
RUN dotnet build "Order.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Order.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Order.API.dll"]