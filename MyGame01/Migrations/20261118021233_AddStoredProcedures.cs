using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGame01.Migrations
{
    /// <inheritdoc />
    public partial class AddStoredProcedures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. 로그인 프로시저
            migrationBuilder.Sql(@"
CREATE OR REPLACE FUNCTION sp_user_login(
    p_userid VARCHAR,
    p_password VARCHAR
)
RETURNS TABLE (
    result INT,
    userid VARCHAR
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_db_pw VARCHAR;
BEGIN
    SELECT trim(passward) INTO v_db_pw
    FROM game01_users
    WHERE trim(game01_users.userid) = trim(p_userid);

    IF NOT FOUND THEN
        RETURN QUERY SELECT 1, p_userid; -- 유저 아이디가 없음
        RETURN;
    END IF;

    IF v_db_pw = trim(p_password) THEN
        RETURN QUERY SELECT 0, p_userid; -- 로그인 성공
    ELSE
        RETURN QUERY SELECT 2, p_userid; -- 로그인 실패 (비밀번호 불일치)
    END IF;
END;
$$;
");

            // 2. 유저 등록 프로시저
            migrationBuilder.Sql(@"
CREATE OR REPLACE FUNCTION sp_user_add(
    p_userid VARCHAR,
    p_name VARCHAR,
    p_password VARCHAR
)
RETURNS TABLE (
    result INT,
    userid VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF EXISTS (SELECT 1 FROM game01_users WHERE trim(game01_users.userid) = trim(p_userid)) THEN
        RETURN QUERY SELECT 1, p_userid; -- 중복 유저 있음
        RETURN;
    END IF;

    BEGIN
        INSERT INTO game01_users (userid, passward, name, birthyear, addr, level, health, gold, mdate)
        VALUES (p_userid, p_password, p_name, 2000, '00', 1, 100, 0, CURRENT_DATE);

        RETURN QUERY SELECT 0, p_userid; -- 등록 성공
    EXCEPTION WHEN OTHERS THEN
        RETURN QUERY SELECT 2, p_userid; -- 등록 실패
    END;
END;
$$;
");

            // 3. 플레이 유저 정보 프로시저
            migrationBuilder.Sql(@"
CREATE OR REPLACE FUNCTION sp_user_info(
    p_userid VARCHAR
)
RETURNS TABLE (
    result INT,
    userid VARCHAR,
    name VARCHAR,
    level INT,
    health INT,
    gold INT
)
LANGUAGE plpgsql
AS $$
DECLARE
    r_user RECORD;
BEGIN
    SELECT trim(u.userid) as uid, trim(u.name) as uname, u.level::INT as ulevel, u.health as uhealth, u.gold as ugold
    INTO r_user
    FROM game01_users u
    WHERE trim(u.userid) = trim(p_userid);

    IF NOT FOUND THEN
        RETURN QUERY SELECT 1, p_userid, ''::VARCHAR, 0, 0, 0; -- 정보 가져오기 실패
        RETURN;
    END IF;

    RETURN QUERY SELECT 0, r_user.uid::VARCHAR, r_user.uname::VARCHAR, r_user.ulevel, r_user.uhealth, r_user.ugold;
END;
$$;
");

            // 4. 레벨업 프로시저 (컬럼 모호성 해결: u.level)
            migrationBuilder.Sql(@"
CREATE OR REPLACE FUNCTION sp_user_levelup(
    p_userid VARCHAR
)
RETURNS TABLE (
    result INT,
    userid VARCHAR,
    level INT
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_new_level INT;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM game01_users u WHERE trim(u.userid) = trim(p_userid)) THEN
        RETURN QUERY SELECT 1, p_userid, 0; -- 레벨업 실패 (유저 없음)
        RETURN;
    END IF;

    UPDATE game01_users u
    SET level = (u.level + 1)::SMALLINT
    WHERE trim(u.userid) = trim(p_userid)
    RETURNING u.level::INT INTO v_new_level;

    RETURN QUERY SELECT 0, p_userid, v_new_level; -- 레벨업 성공
END;
$$;
");

            // 5. 골드 추가 프로시저 (컬럼 모호성 해결: u.gold)
            migrationBuilder.Sql(@"
CREATE OR REPLACE FUNCTION sp_user_add_gold(
    p_userid VARCHAR,
    p_gold INT
)
RETURNS TABLE (
    result INT,
    userid VARCHAR,
    gold INT
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_total_gold INT;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM game01_users u WHERE trim(u.userid) = trim(p_userid)) THEN
        RETURN QUERY SELECT 1, p_userid, 0; -- 추가 실패 (유저 없음)
        RETURN;
    END IF;

    UPDATE game01_users u
    SET gold = u.gold + p_gold
    WHERE trim(u.userid) = trim(p_userid)
    RETURNING u.gold INTO v_total_gold;

    RETURN QUERY SELECT 0, p_userid, v_total_gold; -- 추가 성공
END;
$$;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS sp_user_login(VARCHAR, VARCHAR);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS sp_user_add(VARCHAR, VARCHAR, VARCHAR);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS sp_user_info(VARCHAR);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS sp_user_levelup(VARCHAR);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS sp_user_add_gold(VARCHAR, INT);");
        }
    }
}
