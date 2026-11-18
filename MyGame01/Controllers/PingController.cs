using Microsoft.AspNetCore.Mvc;
using MyGame01.Services;

namespace MyGame01.Controllers
{
    public class ConfigurePingRequest
    {
        public bool? Enabled { get; set; }
        public int? IntervalMinutes { get; set; }
        public string? TargetUrl { get; set; }
    }

    public class ManualPingRequest
    {
        public string? TargetUrl { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private readonly PingService _pingService;
        private readonly IHostEnvironment _env;

        public PingController(PingService pingService, IHostEnvironment env)
        {
            _pingService = pingService;
            _env = env;
        }

        // GET: /api/ping (콜드스타트 확인 및 기본 Ping 응답)
        [HttpGet]
        public IActionResult Get()
        {
            var uptime = DateTime.UtcNow - _pingService.ServerStartTime;
            return Ok(new
            {
                status = "Healthy",
                service = "MyGame01",
                message = "pong",
                serverTimeUtc = DateTime.UtcNow,
                serverStartTimeUtc = _pingService.ServerStartTime,
                uptime = $"{uptime.Days}일 {uptime.Hours}시간 {uptime.Minutes}분 {uptime.Seconds}초",
                environment = _env.EnvironmentName,
                isColdStart = uptime.TotalSeconds < 60
            });
        }

        // GET: /api/ping/status (Ping 배경 서비스 상태 및 최근 로그 조회)
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            var status = _pingService.GetStatus();
            return Ok(status);
        }

        // POST: /api/ping/send (수동 즉시 Ping 실행)
        [HttpPost("send")]
        public async Task<IActionResult> SendPing([FromBody] ManualPingRequest? request)
        {
            var targetUrl = request?.TargetUrl;
            if (string.IsNullOrEmpty(targetUrl) && string.IsNullOrEmpty(_pingService.TargetUrl))
            {
                targetUrl = $"{Request.Scheme}://{Request.Host}/api/ping";
            }
            var result = await _pingService.SendPingAsync(targetUrl, isManual: true);
            return Ok(result);
        }

        // POST: /api/ping/configure (자동 핑 활성/비활성 및 주기 설정)
        [HttpPost("configure")]
        public IActionResult Configure([FromBody] ConfigurePingRequest request)
        {
            var targetUrl = request.TargetUrl;
            if (string.IsNullOrEmpty(targetUrl) && string.IsNullOrEmpty(_pingService.TargetUrl))
            {
                targetUrl = $"{Request.Scheme}://{Request.Host}/api/ping";
            }
            _pingService.Configure(request.Enabled, request.IntervalMinutes, targetUrl);
            return Ok(new
            {
                message = "Ping 설정이 업데이트되었습니다.",
                status = _pingService.GetStatus()
            });
        }
    }
}
