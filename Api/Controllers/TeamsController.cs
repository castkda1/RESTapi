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
    public class TeamsController : ControllerBase
    {
        private readonly TeamDbContext _context;
        private readonly LolAccountDbContext _lolAccount;
        private readonly PlayerReadyDbContext _playerReady;
        private readonly TournamentDbContext _tournament;


        public TeamsController(TeamDbContext context, LolAccountDbContext lolAccountDbContext, PlayerReadyDbContext playerReadyDbContext, TournamentDbContext tournamentDbContext)
        {
            _context = context;
            _lolAccount = lolAccountDbContext;
            _playerReady = playerReadyDbContext;
            _tournament = tournamentDbContext;
        }

        // GET: api/Teams
        [ProducesResponseType(typeof(IEnumerable<Team>), StatusCodes.Status200OK)]
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            var result = await _context.teams.ToListAsync();
            if (result.Count == 0)
            {
                return NotFound(new { Title = "Nic se nenašlo", Status = "404" });
            }

            return Ok(result);
        }

        // GET: api/Teams/5
        [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
        [HttpGet("GetById/{id:long}")]
        public async Task<ActionResult<Team>> GetTeamId(ulong id)
        {
            var team = await _context.teams.FindAsync(id);

            if (team == null)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'id': " + id, Status = "404" });
            }

            return Ok(team);
        }

        [ProducesResponseType(typeof(IEnumerable<IEnumerable<LolPlayer>>), StatusCodes.Status200OK)]
        [HttpGet("CreateTeam/{id:int},{size:int}")]
        public async Task<ActionResult<IEnumerable<IEnumerable<LolPlayer>>>>CreateTeam(int id, int size)
        {
            try
            {
                var tournament = await _tournament.tournaments.FindAsync(id);

                // Check if the tournament exists and is not locked
                if (tournament == null || tournament.Lock == true)
               {
                return NotFound(new { Title = "Not Found", Status = "404", Detail = "Tournament not found or is locked" });
               }

                // Set the tournament as locked to prevent concurrent access
                tournament.Lock = true;
                await _tournament.SaveChangesAsync();

                var ready = await _playerReady.player_readys.Where(x => x.Id_Tournament == id).ToListAsync();
                if (ready.Count == 0)
                {
                    return NotFound(new { Title = "Not Found", Status = "404", Detail = "No participants registered for the tournament" });
                }

                List<LolPlayer> players = new List<LolPlayer>();

                foreach (var participant in ready)
                {
                    var lolAccountList = await _lolAccount.lol_accounts.Where(e => e.Id_User == participant.Id_User).ToListAsync();
                    LolAccount? lolAccount = lolAccountList.OrderBy(e => e.Mmr).FirstOrDefault();
                    players.Add(new LolPlayer { UserId = lolAccount.Id_User.GetValueOrDefault(), Mmr = lolAccount.Mmr.GetValueOrDefault() });
                }

                GenerateTeam generateTeam = new GenerateTeam();
                var result = generateTeam.CreateTeams(players, size);

                if (result == null)
                {
                    return NotFound(new { Title = "Not Found", Status = "404", Detail = "Insufficient players for team creation" });
                }

                var names = generateTeam.createName(result.Count());
                var countName = 0;
                foreach (var item in result)
                {
                    var team = new Team { Id_Tournament = id };

                    for (int i = 0; i < Math.Min(item.Count(), 5); i++)
                    {
                        var propertyName = $"Player{i + 1}";
                        var userId = item[i].UserId;
                        typeof(Team).GetProperty(propertyName)?.SetValue(team, userId);
                    }
                  
                    team.Name = names[countName];

                    _context.teams.Add(team);
                    await _context.SaveChangesAsync();
                    countName++;
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Title = "Internal Server Error", Status = "500", Detail = "An error occurred while saving teams" });
            }
        }

        // POST: api/Teams
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
        [HttpPost("New")]
        public async Task<ActionResult<Team>> PostTeam(Team team)
        {
            _context.teams.Add(team);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTeamId", new { id = team.Id }, team);
        }

        // PUT: api/Teams/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status200OK)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutTeam(int id, Team team)
        {
            if (id != team.Id)
            {
                return BadRequest(new { Title = id + " a " + team.Id + " se neshodují", Status = "400" });
            }

            _context.Entry(team).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamExists(id))
                {
                    return NotFound(new { Title = "Nenašel se žadný záznam podle 'id': " + id, Status = "404" });
                }

                throw;
            }

            return Ok(new { Title = "Záznam byl upraven", Status = "200" });
        }

        // DELETE: api/Teams/5
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status200OK)]
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteTeam(ulong id)
        {
            var team = await _context.teams.FindAsync(id);
            if (team == null)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'id': " + id, Status = "404" });
            }

            _context.teams.Remove(team);
            await _context.SaveChangesAsync();

            return Ok(new { Title = "Záznam byl smazán", Status = "200" });
        }

        private bool TeamExists(int id)
        {
            return (_context.teams?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
