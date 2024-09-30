FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5281
EXPOSE 7285

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Core/Micro.Application/Micro.Application.csproj", "Core/Micro.Application/"]
COPY ["Core/Micro.Domain/Micro.Domain.csproj", "Core/Micro.Domain/"]
COPY ["Infrastructure/Micro.Persistence/Micro.Persistence.csproj", "Infrastructure/Micro.Persistence/"]
COPY ["Presentation/Order.Consumer/Order.Consumer.csproj", "Presentation/Order.Consumer/"]
RUN dotnet restore "Presentation/Order.Consumer/Order.Consumer.csproj"
COPY . .
WORKDIR "/src/Presentation/Order.Consumer"
RUN dotnet build "Order.Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Order.Consumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Order.Consumer.dll"]