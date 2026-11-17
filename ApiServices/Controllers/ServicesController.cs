using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiServices.Controllers
{
    /// <summary>
    /// 모델 클래스, db에 DTO테이블 속성과 일대일 매칭
    /// </summary>
    public class Dto
    {
        public int Id { get; set; }
        [MinLength(2)]
        public string Text { get; set; }
    }

    /// <summary>
    /// Wep Api 클래스
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        // GET: api/Services
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "또 만났네요", "다음에 봐요" };
        }

        // GET api/Services/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            //return $"또 넘어온 값: {id}";
            return Ok(new Dto {Id = id, Text = $"값: {id}" });
        }

        // POST api/Services
        [HttpPost]
        public IActionResult Post([FromBody] Dto value)
        {
            if(ModelState.IsValid == false)
            {
                //throw new InvalidOperationException("잘못되었습니다");
                return BadRequest(ModelState);
            }

            //value를 가지고 데이터 처리

            return Ok();
        }

        // PUT api/<ServicesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ServicesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
