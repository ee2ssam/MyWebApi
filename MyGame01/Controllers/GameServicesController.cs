using Microsoft.AspNetCore.Mvc;
using MyGame01.Models;
using MyGame01.Services;

namespace MyGame01.Controllers
{
    // 1. 로그인 (Protocol 1101 -> -1101)
    // URL: /api/UserLoginService
    [Route("api/[controller]")]
    [ApiController]
    public class UserLoginServiceController : ControllerBase
    {
        private readonly IGameServiceRepository _repository;

        public UserLoginServiceController(IGameServiceRepository repository1)
        {
            _repository = repository1;
        }

        [HttpPost]
        public async Task<ActionResult<UserLoginResponse>> Post([FromBody] UserLoginRequest request)
        {
            if (request == null)
            {
                return BadRequest(new UserLoginResponse { Protocol = -1101, Result = 2 });
            }

            var result = await _repository.LoginAsync(request);
            return Ok(result);
        }
    }

    // 2. 신규 유저 등록 (Protocol 1102 -> -1102)
    // URL: /api/UserAddService
    [Route("api/[controller]")]
    [ApiController]
    public class UserAddServiceController : ControllerBase
    {
        private readonly IGameServiceRepository _repository;

        public UserAddServiceController(IGameServiceRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<ActionResult<UserAddResponse>> Post([FromBody] UserAddRequest request)
        {
            if (request == null)
            {
                return BadRequest(new UserAddResponse { Protocol = -1102, Result = 2 });
            }

            var result = await _repository.RegisterAsync(request);
            return Ok(result);
        }
    }

    // 3. 플레이 유저 정보 (Protocol 1103 -> -1103)
    // URL: /api/UserInfoService
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoServiceController : ControllerBase
    {
        private readonly IGameServiceRepository _repository;

        public UserInfoServiceController(IGameServiceRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<ActionResult<UserInfoResponse>> Post([FromBody] UserInfoRequest request)
        {
            if (request == null)
            {
                return BadRequest(new UserInfoResponse { Protocol = -1103, Result = 1 });
            }

            var result = await _repository.GetUserInfoAsync(request);
            return Ok(result);
        }
    }

    // 4. 레벨업 (Protocol 1104 -> -1104)
    // URL: /api/LevelupService
    [Route("api/[controller]")]
    [ApiController]
    public class LevelupServiceController : ControllerBase
    {
        private readonly IGameServiceRepository _repository;

        public LevelupServiceController(IGameServiceRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<ActionResult<LevelupResponse>> Post([FromBody] LevelupRequest request)
        {
            if (request == null)
            {
                return BadRequest(new LevelupResponse { Protocol = -1104, Result = 1 });
            }

            var result = await _repository.LevelupAsync(request);
            return Ok(result);
        }
    }

    // 5. 골드 추가 (Protocol 1105 -> -1105)
    // URL: /api/AddGoldService
    [Route("api/[controller]")]
    [ApiController]
    public class AddGoldServiceController : ControllerBase
    {
        private readonly IGameServiceRepository _repository;

        public AddGoldServiceController(IGameServiceRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<ActionResult<AddGoldResponse>> Post([FromBody] AddGoldRequest request)
        {
            if (request == null)
            {
                return BadRequest(new AddGoldResponse { Protocol = -1105, Result = 1 });
            }

            var result = await _repository.AddGoldAsync(request);
            return Ok(result);
        }
    }
}
