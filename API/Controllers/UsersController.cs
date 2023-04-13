

using API.Data;
using API.Entity;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [ApiController]
     [Route("[Controller]")]// /api/users
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<AppUser>> GetUsers(){
            var users = _context.Users.ToList();
            return users;
        }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppUser>> GetUser(int id){
        return await _context.Users.FindAsync(id);
    }
}
}