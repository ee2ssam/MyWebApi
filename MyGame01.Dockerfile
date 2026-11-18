# 1. Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 복원 캐싱을 위해 프로젝트 파일 먼저 복사
COPY ["MyGame01/MyGame01.csproj", "MyGame01/"]
RUN dotnet restore "MyGame01/MyGame01.csproj"

# 전체 소스 복사 및 배포 패키지 빌드
COPY . .
WORKDIR "/src/MyGame01"
RUN dotnet publish "MyGame01.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "MyGame01.dll"]
