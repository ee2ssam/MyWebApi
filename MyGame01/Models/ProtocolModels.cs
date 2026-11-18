namespace MyGame01.Models
{
    // 1. UserLoginService
    public class UserLoginRequest
    {
        public int Protocol { get; set; } = 1101;
        public string UserId { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UserLoginResponse
    {
        public int Protocol { get; set; } = -1101;
        public int Result { get; set; } // 0: 성공, 1: 유저 아이디 없음, 2: 로그인 실패
        public string UserId { get; set; } = string.Empty;
    }

    // 2. UserAddService
    public class UserAddRequest
    {
        public int Protocol { get; set; } = 1102;
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UserAddResponse
    {
        public int Protocol { get; set; } = -1102;
        public int Result { get; set; } // 0: 등록 성공, 1: 중복 유저 있음, 2: 등록 실패
        public string UserId { get; set; } = string.Empty;
    }

    // 3. UserInfoService
    public class UserInfoRequest
    {
        public int Protocol { get; set; } = 1103;
        public string UserId { get; set; } = string.Empty;
    }

    public class UserInfoResponse
    {
        public int Protocol { get; set; } = -1103;
        public int Result { get; set; } // 0: 성공, 1: 실패
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
        public int Health { get; set; }
        public int Gold { get; set; }
    }

    // 4. LevelupService
    public class LevelupRequest
    {
        public int Protocol { get; set; } = 1104;
        public string UserId { get; set; } = string.Empty;
    }

    public class LevelupResponse
    {
        public int Protocol { get; set; } = -1104;
        public int Result { get; set; } // 0: 성공, 1: 실패
        public string UserId { get; set; } = string.Empty;
        public int Level { get; set; }
    }

    // 5. AddGoldService
    public class AddGoldRequest
    {
        public int Protocol { get; set; } = 1105;
        public string UserId { get; set; } = string.Empty;
        public int Gold { get; set; }
    }

    public class AddGoldResponse
    {
        public int Protocol { get; set; } = -1105;
        public int Result { get; set; } // 0: 성공, 1: 실패
        public string UserId { get; set; } = string.Empty;
        public int Gold { get; set; }
    }
}
