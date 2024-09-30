FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5158
EXPOSE 7299

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Core/Micro.Application/Micro.Application.csproj", "Core/Micro.Application/"]
COPY ["Core/Micro.Domain/Micro.Domain.csproj", "Core/Micro.Domain/"]
COPY ["Infrastructure/Micro.Persistence/Micro.Persistence.csproj", "Infrastructure/Micro.Persistence/"]
COPY ["Presentation/Payment.Consumer/Payment.Consumer.csproj", "Presentation/Payment.Consumer/"]
RUN dotnet restore "Presentation/Payment.Consumer/Payment.Consumer.csproj"
COPY . .
WORKDIR "/src/Presentation/Payment.Consumer"
RUN dotnet build "Payment.Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Payment.Consumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Payment.Consumer.dll"]