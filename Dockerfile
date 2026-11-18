# 1. Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 복원 캐싱을 위해 프로젝트 파일 먼저 복사
COPY ["FiveServices/FiveServices.csproj", "FiveServices/"]
RUN dotnet restore "FiveServices/FiveServices.csproj"

# 전체 소스 복사 및 배포 패키지 빌드
COPY . .
WORKDIR "/src/FiveServices"
RUN dotnet publish "FiveServices.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# 기본 포트 환경변수 (Render가 런타임에 PORT 환경변수를 주입하면 Program.cs에서 우선 반영됨)
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "FiveServices.dll"]
