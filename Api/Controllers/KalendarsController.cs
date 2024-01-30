using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Api.Divnolidi.Api.Models;
using Api.Divnolidi.Api.Data;

namespace Api.Divnolidi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "divnolidi")]
    [Authorize]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public class KalendarsController : ControllerBase
    {
        private readonly KalendarJmenaDbContext _context;
        private readonly KalendarSvatekDbContext _contextSvatek;
        private readonly KalendarFaktDbContext _contextFakt;


        public KalendarsController(KalendarJmenaDbContext context, KalendarSvatekDbContext contextSvatek, KalendarFaktDbContext contextFakt)
        {
            _context = context;
            _contextSvatek = contextSvatek;
            _contextFakt = contextFakt;
        }

        [ProducesResponseType(typeof(IEnumerable<KalendarJmena>), StatusCodes.Status200OK)]
        [HttpGet("GetByName/{name}")]
        public async Task<ActionResult<IEnumerable<KalendarJmena>>> GetKalendarName(string name)
        {
            var userList = await _context.kalendar_jmena.Where(e => e.Jmeno == name).ToListAsync();

            if (!userList.Any())
            {
                return NotFound(new { Title = "Nic se nenašlo", Status = "404" });
            }

            return Ok(userList);
        }

        [ProducesResponseType(typeof(IEnumerable<KalendarJmena>), StatusCodes.Status200OK)]
        [HttpGet("GetByDate/{day:int},{month:int}")]
        public async Task<ActionResult<IEnumerable<KalendarJmena>>> GetKalendarNameDate(int day, int month)
        {
            var userList = await _context.kalendar_jmena.Where(e => e.Den == day && e.Mesic == month).ToListAsync();

            if (!userList.Any())
            {
                return NotFound(new { Title = "Nic se nenašlo", Status = "404" });
            }

            return Ok(userList);
        }

        [ProducesResponseType(typeof(IEnumerable<KalendarSvatek>), StatusCodes.Status200OK)]
        [HttpGet("SvatekGetByDate/{day:int},{month:int}")]
        public async Task<ActionResult<IEnumerable<KalendarSvatek>>> GetKalendarSvatekDate(int day, int month)
        {
            var userList = await _contextSvatek.kalendar_svatek.Where(e => e.Den == day && e.Mesic == month).ToListAsync();

            if (!userList.Any())
            {
                return NotFound(new { Title = "Nic se nenašlo", Status = "404" });
            }

            return Ok(userList);
        }

        [ProducesResponseType(typeof(IEnumerable<KalendarFakt>), StatusCodes.Status200OK)]
        [HttpGet("FaktGetByDate/{day:int},{month:int}")]
        public async Task<ActionResult<IEnumerable<KalendarFakt>>> GetKalendarFaktDate(int day, int month)
        {
            var userList = await _contextFakt.kalendar_fakt.Where(e => e.Den == day && e.Mesic == month).ToListAsync();

            if (!userList.Any())
            {
                return NotFound(new { Title = "Nic se nenašlo", Status = "404" });
            }

            return Ok(userList);
        }
    }
}
