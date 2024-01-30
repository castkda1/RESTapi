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
    public class TournamentsController : ControllerBase
    {
        private readonly TournamentDbContext _context;

        public TournamentsController(TournamentDbContext context)
        {
            _context = context;
        }

        // GET: api/Tournaments
        [ProducesResponseType(typeof(IEnumerable<Tournament>), StatusCodes.Status200OK)]
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<Tournament>>> GetTournaments()
        { 
            var result = await _context.tournaments.ToListAsync();
            if (result.Count == 0)
            {
                return NotFound(new { Title = "Nic se nenašlo", Status = "404" });
            }
            return Ok(result);
        }

        [ProducesResponseType(typeof(Tournament), StatusCodes.Status200OK)]
        [HttpGet("Near")]
        public async Task<ActionResult<Tournament>> GetTournamentNear()
        {
            var date = DateTime.Now.Date;
            var tournaments = await _context.tournaments.Where(e => e.Start_Date_Time >= date).ToListAsync();

            if (tournaments.Count == 0)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'start_date_time':" + date, Status = "404" });
            }

            var tournamentOrder = tournaments.OrderBy(e => e.Start_Date_Time);
            var tournament = tournamentOrder.FirstOrDefault();
            if (tournament == null)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'start_date_time':" + date, Status = "404" });
            }
            return Ok(tournament);
        }

        // GET: api/Tournaments/5
        [ProducesResponseType(typeof(Tournament), StatusCodes.Status200OK)]
        [HttpGet("GetById/{id:int}")]
        public async Task<ActionResult<Tournament>> GetTournamentId(int id)
        {
            var tournament = await _context.tournaments.FindAsync(id);

            if (tournament == null)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'id':" + id, Status = "404" });
            }

            return Ok(tournament);
        }

        [ProducesResponseType(typeof(Tournament), StatusCodes.Status200OK)]
        [HttpPost("GetByDate")]
        public async Task<ActionResult<Tournament>> GetTournamentDate(TournamentsDate date)
        {
            var tournament = await _context.tournaments.Where(e => e.Start_Date_Time >= date.DateTime).FirstOrDefaultAsync();

            if (tournament == null)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'start_date_time':" + date, Status = "404" });
            }

            return Ok(tournament);
        }

        // POST: api/Tournaments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [ProducesResponseType(typeof(Tournament), StatusCodes.Status201Created)]
        [HttpPost("New")]
        public async Task<ActionResult<Tournament>> PostTournament(Tournament tournament)
        {
            var checkTournamentMonth = TournamentExistTodayMonth(tournament.Start_Date_Time);
            var checkTournamentDay = TournamentExistTodayDay(tournament.Start_Date_Time);
            if (checkTournamentMonth && checkTournamentDay)
            {
                return BadRequest(new { Title = "Záznam již exituje v db, 'start_date_time': " + tournament.Start_Date_Time, Status = "400" });
            }

            _context.tournaments.Add(tournament);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTournamentId", new { id = tournament.Id }, tournament);
        }


        // PUT: api/Tournaments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status200OK)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutTournament(int id, Tournament tournament)
        {
            if (id != tournament.Id)
            {
                return BadRequest(new { Title = id + " a " + tournament.Id + " se neshodují", Status = "400" });
            }

            var tour = await _context.tournaments.FindAsync(id);
            if (tour == null)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'id':" + id, Status = "404" });
            }

            if (tournament.Participant_Count != 0)
            {
                tour.Participant_Count = tournament.Participant_Count;
            }

            if (tournament.Start_Date_Time != DateTime.MinValue)
            {
                tour.Start_Date_Time = tournament.Start_Date_Time;
            }

            if (tournament.Name != null)
            {
                tour.Name = tournament.Name;
            }
            
            _context.Entry(tour).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TournamentExists(id))
                {
                    return NotFound(new { Title = "Nenašel se žadný záznam podle 'id':" + id, Status = "404" });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { Title = "Záznam byl upraven", Status = "200" });
        }

        // DELETE: api/Tournaments/5
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status200OK)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTournament(int id)
        {
            var tournament = await _context.tournaments.FindAsync(id);
            if (tournament == null)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'id': " + id, Status = "404" });
            }

            _context.tournaments.Remove(tournament);
            await _context.SaveChangesAsync();

            return Ok(new { Title = "Záznam byl smazán", Status = "200" });
        }

        private bool TournamentExists(int id)
        {
            return (_context.tournaments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        private bool TournamentExistTodayMonth(DateTime date)
        {
            return (_context.tournaments?.Any(e => e.Start_Date_Time.Month == date.Month)).GetValueOrDefault();
        }

        private bool TournamentExistTodayDay(DateTime date)
        {
            return (_context.tournaments?.Any(e => e.Start_Date_Time.Day == date.Day)).GetValueOrDefault();
        }
    }
}
