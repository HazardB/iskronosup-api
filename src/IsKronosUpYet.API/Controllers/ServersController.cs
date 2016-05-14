using System.Linq;
using IsKronosUpYet.API.Models;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;

namespace IsKronosUpYet.API.Controllers
{
    [Route("api/[controller]")]
    public class ServersController : Controller
    {
        private readonly DatabaseContext _context;
        
        public ServersController(DatabaseContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            var servers = _context.Servers.ToList();
            var serialized = JsonConvert.SerializeObject(servers);
            return this.Ok(serialized);
        }
    }
}
