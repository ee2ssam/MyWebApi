using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace ApiHelloWorld.Components
{
    public class FiveComponent
    {
        //Empty
    }

    /// <summary>
    /// 리포지토리 인터페이스 - 기능 정의
    /// </summary>
    public interface IFiveRepository
    {
        Five Add(Five five);        //매개변수로 들어온 데이터를 테이블에 추가
        List<Five> GetAll();        //테이블에 있는 데이터를 모두 반환
        Five GetById(int id);       //매개변수로 들어온 Id의 데이터를 반환
        Five Update(int id);        //매개변수로 들어온 Id의 데이터를 수정
        void Remove(int id);        //매개변수로 들어온 Id의 데이터를 삭제
    }

    /// <summary>
    /// 리포지토리 클래스 - 기능 구현
    /// </summary>
    public class FiveRepository : IFiveRepository
    {
        private IDbConnection db;
        private IConfiguration _config;

        //생성자 - DB 접근
        public FiveRepository(IConfiguration config)
        {
            _config = config;
            db = new SqlConnection(
                _config.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
        }

        //매개변수로 들어온 데이터를 테이블에 추가
        public Five Add(Five five)
        {
            //Five Table 데이터 추가
            Five model = new Five() { Note = five.Note } ;
            string sql = @"
                insert into Five (Note) values (@Note);
            ";
            db.Execute(sql, new { Note = five.Note });
            return model;
        }

        public List<Five> GetAll()
        {
            throw new NotImplementedException();
        }

        public Five GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Remove(int id)
        {
            throw new NotImplementedException();
        }

        public Five Update(int id)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Five Data를 관리하는 Web Api 클래스
    /// </summary>
    [Route("api/[controller]")]
    public class FiveServiceController : ControllerBase
    {
        private IFiveRepository _repository;

        //생성자
        public FiveServiceController(IFiveRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult Post([FromBody]Five five)
        {
            try
            {
                //예외처리
                if (five.Note == null || five.Note.Length < 1)
                {
                    ModelState.AddModelError("Note", "Note를 입력해야 합니다");
                }
                //모델유효성 검사
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);  //400
                }

                var model = _repository.Add(five);
                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

    }

    /// <summary>
    /// 모델 클래스 - DB Table 일대일 매핑
    /// </summary>
    public class Five
    {
        public int Id { get; set; }
        [Required]  //반드시 입력하라, 특성추가
        public string Note { get; set; }
    }
}
