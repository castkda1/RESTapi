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
    public class UsersController : ControllerBase
    {
        private readonly UserDbContext _context;

        public UsersController(UserDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var result = await _context.users.ToListAsync();
            if (result.Count == 0)
            {
                return NotFound(new { Title = "Nic se nenašlo", Status = "404" });
            }
            return Ok(result);
        }

        [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
        [HttpGet("AllString")]
        public async Task<ActionResult<IEnumerable<SUser>>> GetUsersString()
        {
            var result = await _context.users.ToListAsync();

            if (result.Count == 0)
            {
                return NotFound(new { Title = "Nic se nenašlo", Status = "404" });
            }

            var values = result.Select(item => new SUser { Id = item.Id ,Id_Dc = item.Id_Dc.ToString(), First_Name = item.First_Name ,Nation = item.Nation, Naroz_Den = item.Naroz_Den, Naroz_Mesic = item.Naroz_Mesic}).ToList();

            return Ok(values);
        }

        // GET: api/Users/5
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [HttpGet("GetById/{id:int}")]
        public async Task<ActionResult<User>> GetUserId(int id)
        {
            var user = await _context.users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'id': " + id, Status = "404" });
            }

            return Ok(user);
        }

        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [HttpGet("GetByDc/{id:long}")]
        public async Task<ActionResult<User>> GetUserDc(ulong id)
        {
            var user = await _context.users.FirstOrDefaultAsync(e => e.Id_Dc == id);

            if (user == null)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'id_dc': " + id, Status = "404" });
            }

            return Ok(user);
        }

        [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
        [HttpGet("GetByName/{name}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUserName(string name)
        {
            var user = await _context.users.Where(e => e.First_Name == name).ToListAsync();

            if (user.Count == 0)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'first_name': " + name, Status = "404" });
            }

            return Ok(user);
        }

        [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
        [HttpGet("GetByDate/{day:int},{month:int}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUserDate(int day, int month)
        {
            var userList = await _context.users.Where(e => e.Naroz_Den == day && e.Naroz_Mesic == month).ToListAsync();

            if (userList.Count == 0)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'naroz_den': " + day + " a 'naroz_mesic': " + month, Status = "404" });
            }

            return Ok(userList);
        }

        [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
        [HttpGet("GetByNameCount/{name}")]
        public async Task<ActionResult<long>> GetUserNameCount(string name)
        {
            var userList = await _context.users.Where(e => e.First_Name == name).ToListAsync();

            if (userList.Count == 0)
            {
                return 0;
            }

            var countUser = userList.Count;

            return Ok(countUser);
        }

        [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
        [HttpGet("GetByDateCount/{day:int},{month:int}")]
        public async Task<ActionResult<long>> GetUserDateCount(int day, int month)
        {
            var userList = await _context.users.Where(e => e.Naroz_Den == day && e.Naroz_Mesic == month).ToListAsync();

            if (userList.Count == 0)
            {
                return 0;
            }

            var countUser = userList.Count;

            return Ok(countUser);
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        [HttpPost("New")]
        public async Task<ActionResult<User>> NewUser(User newUser)
        {
            var checkDc = DcExists(newUser.Id_Dc);
            if (checkDc)
            {
                return BadRequest(new { Title = "Záznam již exituje v db, 'id_dc': " + newUser.Id_Dc, Status = "400" });
            }

            _context.users.Add(newUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserId), new { id = newUser.Id }, newUser);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status200OK)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutUser(int id, User updateUser)
        {
            if (id != updateUser.Id)
            {
                return BadRequest(new { Title = id + " a " + updateUser.Id + " se neshodují", Status = "400" });
            }

            var user = await _context.users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'id': " + id, Status = "404" });
            }

            if (updateUser.Id_Dc != 0)
            {
                user.Id_Dc = updateUser.Id_Dc;
            }

            if (updateUser.First_Name != null)
            {
                user.First_Name = updateUser.First_Name;
            }

            if (updateUser.Nation != null)
            {
                user.Nation = updateUser.Nation;
            }

            if (updateUser.Naroz_Den != null)
            {
                user.Naroz_Den = updateUser.Naroz_Den;
            }

            if (updateUser.Naroz_Mesic != null)
            {
                user.Naroz_Mesic = updateUser.Naroz_Mesic;
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound(new { Title = "Nenašel se žadný záznam podle 'id': " + id, Status = "404" });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { Title = "Záznam byl upraven", Status = "200" });
        }

        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status200OK)]
        [HttpPut("logcode")]
        public async Task<IActionResult> PutLogCode(LogCode updateUser)
        {
            var user = await _context.users.Where(e => e.Id_Dc == updateUser.Id_Dc).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'id': " + updateUser.Id_Dc, Status = "404" });
            }

            user.Id_Dc = updateUser.Id_Dc;
            user.Dc_Name = updateUser.Dc_Name; 
            user.Log_Code = updateUser.Log_Code;
    
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DcExists(user.Id_Dc))
                {
                    return NotFound(new { Title = "Nenašel se žadný záznam podle 'id': " + user.Id_Dc, Status = "404" });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { Title = "Záznam byl upraven", Status = "200" });
        }

        // DELETE: api/Users/5
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status200OK)]
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var user = await _context.users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'id': " + id, Status = "404" });
            }

            _context.users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { Title = "Záznam byl smazán", Status = "200" });
        }

        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status200OK)]
        [HttpDelete("IdUser/{id:long}")]
        public async Task<IActionResult> DeleteUserByIdUser(ulong id)
        {
            var user = await _context.users.Where(e => e.Id_Dc == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound(new { Title = "Nenašel se žadný záznam podle 'id': " + id, Status = "404" });
            }

            _context.users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { Title = "Záznam byl smazán", Status = "200" });
        }

        private bool UserExists(int id)
        {
            return (_context.users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool DcExists(ulong id)
        {
            return (_context.users?.Any(e => e.Id_Dc == id)).GetValueOrDefault();
        }
    }
}
