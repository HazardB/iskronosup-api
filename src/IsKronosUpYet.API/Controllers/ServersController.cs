using System.Collections.Generic;
using System.Linq;
using IsKronosUpYet.API.Models;
using Microsoft.AspNet.Mvc;

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
        public IEnumerable<string> Get()
        {
            return _context.Servers.Select(s => s.Name);
        }
    }
}
