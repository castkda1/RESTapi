using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Token.Data;
using Api.Token.Models;
using Microsoft.AspNetCore.Authorization;

namespace Api.Token.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "token")]
    public class AuthController : ControllerBase
    {

        private readonly TokenDbContext _context;

        private readonly IConfiguration _configuration;

        public AuthController(TokenDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Applications
        [HttpGet()]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Models.Token>>> Get()
        {
            if (_context.tokens == null)
            {
                return NotFound();
            }
            return await _context.tokens.ToListAsync();
        }

        // GET: api/Applications/5
        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<ActionResult<Models.Token>> GetApplication(long id)
        {
            if (_context.tokens == null)
            {
              return NotFound();
            }
            var application = await _context.tokens.FindAsync(id);

            if (application == null)
            {
                return NotFound();
            }

            return application;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<AuthToken>> Login(LoginApplication app)
        {
            if (_context.tokens == null)
            {
                return NotFound("Error");
            }
            var application = await _context.tokens.FirstOrDefaultAsync(e => e.User_name == app.User_name);

            if (application == null)
            {
                return NotFound("Not found");
            }

            if(!BCrypt.Net.BCrypt.Verify(app.Pass_word, application.Pass_word))
            {
                return BadRequest("Wrong password");
            }


            JwlTokenManager jwlTokenManager = new JwlTokenManager(_configuration);

            AuthToken authToken = new AuthToken();
            authToken.Auth_token = jwlTokenManager.Authenticate(app);
            authToken.User_name = application.User_name;
            authToken.Id = application.Id;

            return authToken;
        }

        // POST: api/Applications
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("NewReg")]
        public async Task<ActionResult<Models.Token>> NewReg(NewApplication newApplication)
        {
            if(newApplication.Check_word != _configuration.GetSection("Pass:Key").Value!)
            {
                return Unauthorized("Wrong check password");
            }
            
            if (_context.tokens == null)
            {
                return BadRequest("Entity set 'ApplicationDbContext.auth_tokens'  is null.");
            }

            var checkName = await _context.tokens.FirstOrDefaultAsync(e => e.User_name == newApplication.User_name);
            if(checkName != null) 
            {
                return BadRequest("Name is used");
            }



            string paswordHash = BCrypt.Net.BCrypt.HashPassword(newApplication.Pass_word);

            Models.Token application = new Models.Token();
            application.User_name = newApplication.User_name;
            application.Pass_word = paswordHash;

            _context.tokens.Add(application);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetApplication", new { id = application.Id }, application);
        }

        // PUT: api/Applications/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //TODO heslo
        /*[HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutApplication(string heslo, long id, Application application)
        {
            if (heslo != "test")
            {
                return Unauthorized();
            }
            if (id != application.Id)
            {
                return BadRequest();
            }

            _context.Entry(application).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }*/

        private bool ApplicationExists(long id)
        {
            return (_context.tokens?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
