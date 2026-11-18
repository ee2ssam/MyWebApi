using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using MyGame01.Data;
using MyGame01.Models;
using Npgsql;

namespace MyGame01.Services
{
    public interface IGameServiceRepository
    {
        Task<UserLoginResponse> LoginAsync(UserLoginRequest request);
        Task<UserAddResponse> RegisterAsync(UserAddRequest request);
        Task<UserInfoResponse> GetUserInfoAsync(UserInfoRequest request);
        Task<LevelupResponse> LevelupAsync(LevelupRequest request);
        Task<AddGoldResponse> AddGoldAsync(AddGoldRequest request);
    }

    public class GameServiceRepository : IGameServiceRepository
    {
        private readonly MyGameDbContext _context;

        public GameServiceRepository(MyGameDbContext context)
        {
            _context = context;
        }

        // 1. 로그인 (sp_user_login)
        public async Task<UserLoginResponse> LoginAsync(UserLoginRequest request)
        {
            var response = new UserLoginResponse
            {
                Protocol = -1101,
                UserId = request.UserId ?? string.Empty,
                Result = 2 // 기본 실패
            };

            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            await using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT result, userid FROM sp_user_login(@p_userid, @p_password);";

            var p1 = cmd.CreateParameter();
            p1.ParameterName = "@p_userid";
            p1.Value = request.UserId ?? string.Empty;
            cmd.Parameters.Add(p1);

            var p2 = cmd.CreateParameter();
            p2.ParameterName = "@p_password";
            p2.Value = request.Password ?? string.Empty;
            cmd.Parameters.Add(p2);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                response.Result = Convert.ToInt32(reader["result"]);
                response.UserId = reader["userid"]?.ToString()?.Trim() ?? request.UserId ?? string.Empty;
            }

            return response;
        }

        // 2. 신규 유저 등록 (sp_user_add)
        public async Task<UserAddResponse> RegisterAsync(UserAddRequest request)
        {
            var response = new UserAddResponse
            {
                Protocol = -1102,
                UserId = request.UserId ?? string.Empty,
                Result = 2
            };

            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            await using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT result, userid FROM sp_user_add(@p_userid, @p_name, @p_password);";

            var p1 = cmd.CreateParameter();
            p1.ParameterName = "@p_userid";
            p1.Value = request.UserId ?? string.Empty;
            cmd.Parameters.Add(p1);

            var p2 = cmd.CreateParameter();
            p2.ParameterName = "@p_name";
            p2.Value = request.Name ?? string.Empty;
            cmd.Parameters.Add(p2);

            var p3 = cmd.CreateParameter();
            p3.ParameterName = "@p_password";
            p3.Value = request.Password ?? string.Empty;
            cmd.Parameters.Add(p3);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                response.Result = Convert.ToInt32(reader["result"]);
                response.UserId = reader["userid"]?.ToString()?.Trim() ?? request.UserId ?? string.Empty;
            }

            return response;
        }

        // 3. 유저 정보 조회 (sp_user_info)
        public async Task<UserInfoResponse> GetUserInfoAsync(UserInfoRequest request)
        {
            var response = new UserInfoResponse
            {
                Protocol = -1103,
                UserId = request.UserId ?? string.Empty,
                Result = 1
            };

            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            await using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT result, userid, name, level, health, gold FROM sp_user_info(@p_userid);";

            var p1 = cmd.CreateParameter();
            p1.ParameterName = "@p_userid";
            p1.Value = request.UserId ?? string.Empty;
            cmd.Parameters.Add(p1);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                response.Result = Convert.ToInt32(reader["result"]);
                response.UserId = reader["userid"]?.ToString()?.Trim() ?? request.UserId ?? string.Empty;
                response.Name = reader["name"]?.ToString()?.Trim() ?? string.Empty;
                response.Level = reader["level"] != DBNull.Value ? Convert.ToInt32(reader["level"]) : 0;
                response.Health = reader["health"] != DBNull.Value ? Convert.ToInt32(reader["health"]) : 0;
                response.Gold = reader["gold"] != DBNull.Value ? Convert.ToInt32(reader["gold"]) : 0;
            }

            return response;
        }

        // 4. 레벨업 (sp_user_levelup)
        public async Task<LevelupResponse> LevelupAsync(LevelupRequest request)
        {
            var response = new LevelupResponse
            {
                Protocol = -1104,
                UserId = request.UserId ?? string.Empty,
                Result = 1
            };

            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            await using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT result, userid, level FROM sp_user_levelup(@p_userid);";

            var p1 = cmd.CreateParameter();
            p1.ParameterName = "@p_userid";
            p1.Value = request.UserId ?? string.Empty;
            cmd.Parameters.Add(p1);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                response.Result = Convert.ToInt32(reader["result"]);
                response.UserId = reader["userid"]?.ToString()?.Trim() ?? request.UserId ?? string.Empty;
                response.Level = reader["level"] != DBNull.Value ? Convert.ToInt32(reader["level"]) : 0;
            }

            return response;
        }

        // 5. 골드 추가 (sp_user_add_gold)
        public async Task<AddGoldResponse> AddGoldAsync(AddGoldRequest request)
        {
            var response = new AddGoldResponse
            {
                Protocol = -1105,
                UserId = request.UserId ?? string.Empty,
                Result = 1
            };

            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            await using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT result, userid, gold FROM sp_user_add_gold(@p_userid, @p_gold);";

            var p1 = cmd.CreateParameter();
            p1.ParameterName = "@p_userid";
            p1.Value = request.UserId ?? string.Empty;
            cmd.Parameters.Add(p1);

            var p2 = cmd.CreateParameter();
            p2.ParameterName = "@p_gold";
            p2.Value = request.Gold;
            cmd.Parameters.Add(p2);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                response.Result = Convert.ToInt32(reader["result"]);
                response.UserId = reader["userid"]?.ToString()?.Trim() ?? request.UserId ?? string.Empty;
                response.Gold = reader["gold"] != DBNull.Value ? Convert.ToInt32(reader["gold"]) : 0;
            }

            return response;
        }
    }
}
