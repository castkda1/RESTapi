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
    public class LolAccountsController : ControllerBase
    {
        private readonly LolAccountDbContext _context;
        private readonly UserDbContext _userContext;

        public LolAccountsController(LolAccountDbContext context, UserDbContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        // GET: api/LolAccounts
        [ProducesResponseType(typeof(IEnumerable<LolAccount>), StatusCodes.Status200OK)]
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<LolAccount>>> GetLolAccounts()
        {
            var result = await _context.lol_accounts.ToListAsync();
            if (result.Count == 0)
            {
                return NotFound(new { Title = "Nic se nenašlo", Status = "404" });
            }
            return Ok(result);
        }

        // GET: api/LolAccounts/5
        [ProducesResponseType(typeof(LolAccount), StatusCodes.Status200OK)]
        [HttpGet("GetById/{id:long}")]
        public async Task<ActionResult<LolAccount>> GetLolAccountId(ulong id)
        {
            var lolAccount = await _context.lol_accounts.FindAsync(id);

            if (lolAccount == null)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'id':" + id, Status = "404" });
            }

            return Ok(lolAccount);
        }

        [ProducesResponseType(typeof(IEnumerable<LolAccount>), StatusCodes.Status200OK)]
        [HttpGet("GetByUser/{id:int}")]
        public async Task<ActionResult<IEnumerable<LolAccount>>> GetLolAccountUser(int id)
        {
            var lolAccount = await _context.lol_accounts.Where(e => e.Id_User == id).ToListAsync();

            if (lolAccount.Count == 0)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'id_user':" + id, Status = "404" });
            }

            return Ok(lolAccount);
        }

        [ProducesResponseType(typeof(LolAccount), StatusCodes.Status200OK)]
        [HttpGet("GetBySumm/{summ}")]
        public async Task<ActionResult<LolAccount>> GetLolAccountSumm(string summ)
        {
            var lolAccount = await _context.lol_accounts.FirstOrDefaultAsync(e => e.Id_Summoner == summ);

            if (lolAccount == null)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'id_summoner':" + summ, Status = "404" });
            }

            return Ok(lolAccount);
        }

        [ProducesResponseType(typeof(LolAccount), StatusCodes.Status200OK)]
        [HttpGet("GetByAcc/{acc}")]
        public async Task<ActionResult<LolAccount>> GetLolAccountAcc(string acc)
        {
            var lolAccount = await _context.lol_accounts.FirstOrDefaultAsync(e => e.Id_Account == acc);

            if (lolAccount == null)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'id_account':" + acc, Status = "404" });
            }

            return Ok(lolAccount);
        }

        [ProducesResponseType(typeof(LolAccount), StatusCodes.Status200OK)]
        [HttpGet("GetByPuu/{puu}")]
        public async Task<ActionResult<LolAccount>> GetLolAccountPuu(string puu)
        {
            var lolAccount = await _context.lol_accounts.FirstOrDefaultAsync(e => e.Id_Puu == puu);

            if (lolAccount == null)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'id_puu':" + puu, Status = "404" });
            }

            return Ok(lolAccount);
        }

        [ProducesResponseType(typeof(BestRank), StatusCodes.Status200OK)]
        [HttpGet("GetBestRank/{id:int}")]
        public async Task<ActionResult<BestRank>> GetBestRank(int id)
        {
            //var lolAccount = await _context.lol_accounts.Where(e => e.Id_User == id).OrderBy(e => e.Mmr).FirstAsync();
            var lolAccountList = await _context.lol_accounts.Where(e => e.Id_User == id).ToListAsync();
            if (!lolAccountList.Any())
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'id_user':" + id, Status = "404" });
            }

            var lolAccount = lolAccountList.MinBy(e => e.Mmr);
            var checkTier = Tier(lolAccount.Tier);
            var rankStr = "";
            if (checkTier)
            {
                rankStr = lolAccount.Tier;
            }
            else
            {
                rankStr = lolAccount.Tier + " " + lolAccount.Rank;
            }
            
            BestRank bestRank = new BestRank();
            bestRank.Id = id;
            bestRank.Id_Puu = lolAccount.Id_Puu;
            bestRank.Summoner_Name = lolAccount.Summoner_Name;
            bestRank.Rank_Str = rankStr;
            bestRank.Mmr = lolAccount.Mmr; ;
            bestRank.Region = lolAccount.Region;

            return Ok(bestRank);
        }

        [ProducesResponseType(typeof(IEnumerable<LolAccount>), StatusCodes.Status200OK)]
        [HttpPost("GetByUserRegion")]
        public async Task<ActionResult<IEnumerable<LolAccount>>> GetLolAccountUserRegion(LolAccount lolAccount)
        {
            var lolAccountList = await _context.lol_accounts.Where(e => e.Id_User == lolAccount.Id_User && e.Region == lolAccount.Region).ToListAsync();

            if (!lolAccountList.Any())
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'id_user':" + lolAccount.Id_User + " a 'region':" + lolAccount.Region, Status = "404" });
            }

            return Ok(lolAccountList);
        }

        // POST: api/LolAccounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [ProducesResponseType(typeof(LolAccount), StatusCodes.Status201Created)]
        [HttpPost("New")]
        public async Task<ActionResult<LolAccount>> NewLolAccount(LolAccount newLolAccount)
        {
            var checkLol = LolExists(newLolAccount.Id_Puu);
            if (checkLol)
            {
                return BadRequest(new { Title = "Záznam již exituje v db, 'id_puu': " + newLolAccount.Id_Puu, Status = "400" });
            }

            _context.lol_accounts.Add(newLolAccount);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLolAccountId", new { id = newLolAccount.Id }, newLolAccount);
        }

        // PUT: api/LolAccounts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status201Created)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutLolAccount(int id, LolAccount lolAccount)
        {
            if (id != lolAccount.Id)
            {
                return BadRequest(new { Title = "Error", Status = "400" });
            }

            var lolAcc = await _context.lol_accounts.FindAsync(id);
            if (lolAcc == null)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'id':" + id, Status = "404" });
            }

            if (lolAccount.Id_User != null)
            {
                lolAcc.Id_User = lolAccount.Id_User;
            }

            if (lolAccount.Id_Summoner != null)
            {
                lolAcc.Id_Summoner = lolAccount.Id_Summoner;
            }

            if (lolAccount.Id_Account != null)
            {
                lolAcc.Id_Account = lolAccount.Id_Account;
            }

            if (lolAccount.Id_Puu != null)
            {
                lolAcc.Id_Puu = lolAccount.Id_Puu;
            }

            if (lolAccount.Tier != null)
            {
                lolAcc.Tier = lolAccount.Tier;
            }

            if (lolAccount.Rank != null)
            {
                lolAcc.Rank = lolAccount.Rank;
            }

            if (lolAccount.Summoner_Name != null)
            {
                lolAcc.Summoner_Name = lolAccount.Summoner_Name;
            }

            if (lolAccount.Mmr != null)
            {
                lolAcc.Mmr = lolAccount.Mmr;
            }

            if (lolAccount.Region != null)
            {
                lolAcc.Region = lolAccount.Region;
            }

            _context.Entry(lolAcc).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LolAccountExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            return Ok(new { Title = "Záznam byl upraven", Status = "200" });
        }

        // DELETE: api/LolAccounts/5
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status200OK)]
        [HttpDelete("Dell")]
        public async Task<IActionResult> DeleteLolAccount(DellLolAccount dell)
        {
            UsersController usersController = new(_userContext);
            var user = await usersController.GetUserDc(dell.Id_Dc);
            if (user == null)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'id_dc':" + dell.Id_Dc, Status = "404" });
            }
            var userId = user.Value?.Id;

            if (userId == null)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam", Status = "404" });
            }

            var lolAccount = await _context.lol_accounts.FirstOrDefaultAsync(e => e.Id_User == userId && e.Summoner_Name == dell.Name);
            if (lolAccount == null)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'id_user':" + userId + " a 'summoner_name':" + dell.Name, Status = "404" });
            }

            _context.lol_accounts.Remove(lolAccount);
            await _context.SaveChangesAsync();

            return Ok(new { Title = "Záznam byl smazán", Status = "200" });
        }

        private bool LolAccountExists(int id)
        {
            return (_context.lol_accounts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        private bool LolExists(string puu)
        {
            return (_context.lol_accounts?.Any(e => e.Id_Puu == puu)).GetValueOrDefault();
        }
        private bool Tier(string tier)
        {
            bool result = Enum.TryParse<TierEnum>(tier, out _);
            return result;
        }
        enum TierEnum
        {
            UNRANKED,
            MASTER,
            GRADNMASTER,
            CHALLENGER
        }
    }
}
