FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RabbitMQ-v10/RabbitMQ-v10.csproj", "RabbitMQ-v10/"]
RUN dotnet restore "RabbitMQ-v10/RabbitMQ-v10.csproj"
COPY . .
WORKDIR "/src/RabbitMQ-v10"
RUN dotnet build "RabbitMQ-v10.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RabbitMQ-v10.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RabbitMQ-v10.dll"]
