FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Directory.Build.props .
COPY Directory.Packages.props .

COPY src/MiniTasksSystem.Domain/MiniTasksSystem.Domain.csproj src/MiniTasksSystem.Domain/
COPY src/MiniTasksSystem.Application/MiniTasksSystem.Application.csproj src/MiniTasksSystem.Application/
COPY src/MiniTasksSystem.Infrastructure/MiniTasksSystem.Infrastructure.csproj src/MiniTasksSystem.Infrastructure/
COPY src/MiniTasksSystem.Api/MiniTasksSystem.Api.csproj src/MiniTasksSystem.Api/

RUN dotnet restore src/MiniTasksSystem.Api/MiniTasksSystem.Api.csproj

COPY . .

RUN dotnet publish src/MiniTasksSystem.Api/MiniTasksSystem.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MiniTasksSystem.Api.dll"]
