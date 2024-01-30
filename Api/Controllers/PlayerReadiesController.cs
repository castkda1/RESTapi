using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Divnolidi.Api.Data;
using Api.Divnolidi.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace Api.Divnolidi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "divnolidi")]
    [Authorize]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public class PlayerReadiesController : ControllerBase
    {
        private readonly PlayerReadyDbContext _context;

        public PlayerReadiesController(PlayerReadyDbContext context)
        {
            _context = context;
        }

        // GET: api/PlayerReadies
        [ProducesResponseType(typeof(IEnumerable<PlayerReady>), StatusCodes.Status200OK)]
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<PlayerReady>>> GetPlayerReadys()
        {
            var result = await _context.player_readys.ToListAsync();
            if (result.Count == 0)
            {
                return NotFound(new { Title = "Nic se nenašlo", Status = "404" });
            }
            return Ok(result);
        }

        // GET: api/PlayerReadies/5
        [ProducesResponseType(typeof(PlayerReady), StatusCodes.Status200OK)]
        [HttpGet("GetById/{id:int}")]
        public async Task<ActionResult<PlayerReady>> GetPlayerReady(int id)
        {
            var playerReady = await _context.player_readys.FindAsync(id);

            if (playerReady == null)
            {
                return NotFound(new { Title = "Nic se nenašlo", Status = "404" });
            }

            return Ok(playerReady);
        }

        [ProducesResponseType(typeof(IEnumerable<PlayerReady>), StatusCodes.Status200OK)]
        [HttpGet("GetByIdTournament/{id:int}")]
        public async Task<ActionResult<IEnumerable<PlayerReady>>> GetPlayerReadysByIdTournament(int id)
        {
            var result = await _context.player_readys.Where(e => e.Id_Tournament == id).ToListAsync();
            if (result.Count == 0)
            {
                return NotFound(new { Title = "Nic se nenašlo", Status = "404" });
            }
            return Ok(result);
        }

        [ProducesResponseType(typeof(PlayerReady), StatusCodes.Status200OK)]
        [HttpPost("CheckPlayer")]
        public async Task<ActionResult<PlayerReady>> GetPlayerReady(PlayerReady playerReady)
        {
            var result = await _context.player_readys.Where(e => e.Id_Tournament == playerReady.Id_Tournament).ToListAsync();
            var check = result.Where(e => e.Id_User == playerReady.Id_User).FirstOrDefault();

            if (check == null)
            {
                return NotFound(new { Title = "Nic se nenašlo", Status = "404" });
            }

            return Ok(check);
        }

        // PUT: api/PlayerReadies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status200OK)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutPlayerReady(int id, PlayerReady playerReady)
        {
            if (id != playerReady.Id)
            {
                return BadRequest(new { Title = id + " a " + playerReady.Id + " se neshodují", Status = "400" });
            }

            _context.Entry(playerReady).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerReadyExists(id))
                {
                    return NotFound(new { Title = "Nenašel se žadný záznam podle 'id': " + id, Status = "404" });
                }

                throw;
            }

            return Ok(new { Title = "Záznam byl upraven", Status = "200" });
        }

        // POST: api/PlayerReadies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        [HttpPost("New")]
        public async Task<ActionResult<PlayerReady>> PostPlayerReady(PlayerReady playerReady)
        {
            _context.player_readys.Add(playerReady);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlayerReady", new { id = playerReady.Id }, playerReady);
        }

        // DELETE: api/PlayerReadies/5
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status200OK)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePlayerReady(int id)
        {
            var playerReady = await _context.player_readys.FindAsync(id);
            if (playerReady == null)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'id': " + id, Status = "404" });
            }

            _context.player_readys.Remove(playerReady);
            await _context.SaveChangesAsync();

            return Ok(new { Title = "Záznam byl smazán", Status = "200" });
        }

        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status200OK)]
        [HttpDelete]
        public async Task<IActionResult> DeletePlayerReady(PlayerReady dell)
        {
            var list = await _context.player_readys.Where(e => e.Id_User == dell.Id_User).ToListAsync();

            if (!list.Any())
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'id_user': " + dell.Id_User, Status = "404" });
            }
            
            var playerReady = list.FirstOrDefault(e => e.Id_Tournament == dell.Id_Tournament);

            if (playerReady == null)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'id_tournament': " + dell.Id_Tournament, Status = "404" });
            }

            _context.player_readys.Remove(playerReady);
            await _context.SaveChangesAsync();

            return Ok(new { Title = "Záznam byl smazán", Status = "200" });
        }

        private bool PlayerReadyExists(int id)
        {
            return (_context.player_readys?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
