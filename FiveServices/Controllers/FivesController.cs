using FiveServices.Data;
using FiveServices.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiveServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FivesController : ControllerBase
    {
        private readonly MyGameDbContext _context;

        public FivesController(MyGameDbContext context)
        {
            _context = context;
        }

        // GET: api/Fives
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Five>>> Get()
        {
            return await _context.Fives.OrderBy(f => f.Id).ToListAsync();
        }

        // GET: api/Fives/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Five>> Get(int id)
        {
            var five = await _context.Fives.FindAsync(id);

            if (five == null)
            {
                return NotFound(new { message = $"ID {id}에 해당하는 데이터를 찾을 수 없습니다." });
            }

            return Ok(five);
        }

        // POST: api/Fives
        [HttpPost]
        public async Task<ActionResult<Five>> Post([FromBody] Five five)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Fives.Add(five);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = five.Id }, five);
        }

        // PUT: api/Fives/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Five five)
        {
            if (five == null)
            {
                return BadRequest(new { message = "요청 본문이 비어있습니다." });
            }

            // 본문에 ID가 전달되지 않았거나 0인 경우 URL의 id를 매핑
            if (five.Id == 0)
            {
                five.Id = id;
            }
            else if (id != five.Id)
            {
                return BadRequest(new { message = $"URL의 ID({id})와 본문의 ID({five.Id})가 일치하지 않습니다." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingFive = await _context.Fives.FindAsync(id);
            if (existingFive == null)
            {
                return NotFound(new { message = $"ID {id}에 해당하는 데이터를 찾을 수 없습니다." });
            }

            existingFive.Note = five.Note;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FiveExists(id))
                {
                    return NotFound(new { message = $"ID {id}에 해당하는 데이터를 찾을 수 없습니다." });
                }
                else
                {
                    throw;
                }
            }

            return Ok(existingFive);
        }

        // DELETE: api/Fives/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var five = await _context.Fives.FindAsync(id);
            if (five == null)
            {
                return NotFound(new { message = $"ID {id}에 해당하는 데이터를 찾을 수 없습니다." });
            }

            _context.Fives.Remove(five);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"ID {id} 데이터가 성공적으로 삭제되었습니다.", deleted = five });
        }

        private bool FiveExists(int id)
        {
            return _context.Fives.Any(e => e.Id == id);
        }
    }
}
