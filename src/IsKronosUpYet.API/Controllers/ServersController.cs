using System.Linq;
using IsKronosUpYet.API.Models;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;

namespace IsKronosUpYet.API.Controllers
{
    /// <summary>
    /// This controller is used as an api for retrieving all of the servers monitored.
    /// </summary>
    [Route("api/[controller]")]
    public class ServersController : Controller
    {
        private readonly DatabaseContext _context;
        
        public ServersController(DatabaseContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// This method returns all of the servers with details.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            var servers = _context.Servers.ToList();
            var serialized = JsonConvert.SerializeObject(servers);
            return this.Ok(serialized);
        }
    }
}
