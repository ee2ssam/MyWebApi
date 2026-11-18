using System.Collections.Concurrent;
using System.Diagnostics;

namespace MyGame01.Services
{
    public class PingLogEntry
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Url { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public long DurationMs { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsManual { get; set; }
    }

    public class PingStatusDto
    {
        public bool IsAutoPingEnabled { get; set; }
        public int IntervalMinutes { get; set; }
        public string? TargetUrl { get; set; }
        public DateTime ServerStartTime { get; set; }
        public TimeSpan ServerUptime { get; set; }
        public DateTime? LastPingTime { get; set; }
        public string? LastPingResult { get; set; }
        public long LastPingDurationMs { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<PingLogEntry> RecentLogs { get; set; } = new();
    }

    public class PingService : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PingService> _logger;
        private readonly IConfiguration _configuration;

        public DateTime ServerStartTime { get; } = DateTime.UtcNow;
        public bool IsAutoPingEnabled { get; private set; } = false; // 기본은 수동/사용자 제어
        public int IntervalMinutes { get; private set; } = 14; // Render 무료 티어 15분 절전 방지 기본 14분
        public string? TargetUrl { get; private set; }
        public DateTime? LastPingTime { get; private set; }
        public string? LastPingResult { get; private set; }
        public long LastPingDurationMs { get; private set; }
        public int SuccessCount { get; private set; }
        public int FailureCount { get; private set; }

        private readonly ConcurrentQueue<PingLogEntry> _logs = new();
        private const int MaxLogs = 20;

        public PingService(IHttpClientFactory httpClientFactory, ILogger<PingService> logger, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;

            // Render 환경변수(RENDER_EXTERNAL_URL) 또는 appsettings에서 타겟 URL 자동 감지
            var externalUrl = Environment.GetEnvironmentVariable("RENDER_EXTERNAL_URL") 
                           ?? _configuration["PingSettings:TargetUrl"];
            if (!string.IsNullOrEmpty(externalUrl))
            {
                TargetUrl = externalUrl.TrimEnd('/') + "/api/ping";
            }
        }

        public void Configure(bool? enabled, int? intervalMinutes, string? targetUrl)
        {
            if (enabled.HasValue)
                IsAutoPingEnabled = enabled.Value;

            if (intervalMinutes.HasValue && intervalMinutes.Value >= 1 && intervalMinutes.Value <= 60)
                IntervalMinutes = intervalMinutes.Value;

            if (!string.IsNullOrWhiteSpace(targetUrl))
                TargetUrl = targetUrl.Trim();
        }

        public async Task<PingLogEntry> SendPingAsync(string? overrideUrl = null, bool isManual = false)
        {
            var url = overrideUrl ?? TargetUrl;

            // 대상 URL이 지정되지 않았으면 자체 포트 기준 로컬 URL 사용
            if (string.IsNullOrEmpty(url))
            {
                var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
                url = $"http://localhost:{port}/api/ping";
            }

            var entry = new PingLogEntry
            {
                Timestamp = DateTime.UtcNow,
                Url = url,
                IsManual = isManual
            };

            var sw = Stopwatch.StartNew();
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(30);

                var response = await client.GetAsync(url);
                sw.Stop();

                entry.DurationMs = sw.ElapsedMilliseconds;
                entry.StatusCode = (int)response.StatusCode;
                entry.IsSuccess = response.IsSuccessStatusCode;
                entry.Message = response.IsSuccessStatusCode 
                    ? $"성공 (HTTP {entry.StatusCode}, {entry.DurationMs}ms)" 
                    : $"실패 (HTTP {entry.StatusCode})";

                if (entry.IsSuccess)
                    SuccessCount++;
                else
                    FailureCount++;
            }
            catch (Exception ex)
            {
                sw.Stop();
                entry.DurationMs = sw.ElapsedMilliseconds;
                entry.StatusCode = 0;
                entry.IsSuccess = false;
                entry.Message = $"오류: {ex.Message}";
                FailureCount++;
            }

            LastPingTime = entry.Timestamp;
            LastPingDurationMs = entry.DurationMs;
            LastPingResult = entry.Message;

            _logs.Enqueue(entry);
            while (_logs.Count > MaxLogs && _logs.TryDequeue(out _)) { }

            _logger.LogInformation("MyGame01 Ping executed: {Message} to {Url} (Manual: {IsManual})", entry.Message, url, isManual);
            return entry;
        }

        public PingStatusDto GetStatus()
        {
            return new PingStatusDto
            {
                IsAutoPingEnabled = IsAutoPingEnabled,
                IntervalMinutes = IntervalMinutes,
                TargetUrl = TargetUrl,
                ServerStartTime = ServerStartTime,
                ServerUptime = DateTime.UtcNow - ServerStartTime,
                LastPingTime = LastPingTime,
                LastPingResult = LastPingResult,
                LastPingDurationMs = LastPingDurationMs,
                SuccessCount = SuccessCount,
                FailureCount = FailureCount,
                RecentLogs = _logs.Reverse().ToList()
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MyGame01 PingBackgroundService started.");

            // 시작 직후 서버 초기화 완료 대기 (60초)
            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                if (IsAutoPingEnabled)
                {
                    try
                    {
                        await SendPingAsync(isManual: false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while running MyGame01 auto ping.");
                    }
                }

                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(IntervalMinutes), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }
    }
}
