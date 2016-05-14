using System;
using System.Linq;
using System.Runtime.CompilerServices;
using IsKronosUpYet.API.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Newtonsoft.Json;

namespace IsKronosUpYet.API.Controllers
{
    /// <summary>
    /// This controller is used for retrieving the status of the realms/ servers
    /// </summary>
    [Route("api/[controller]")]
    public class StatusController : Controller
    {
        private readonly DatabaseContext _context;

        public StatusController(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// This method is used to check the server status for aall tracked servers 
        /// </summary>
        /// <returns>JSON representation of the server status for every server</returns>
        [HttpGet]
        public IActionResult Get()
        {
            var statuses = _context.ServerStatus
                .Include(st => st.Server)
                .Where(s => s.Timestamp > (DateTimeOffset.UtcNow - TimeSpan.FromMinutes(15)))
                .ToList();

            var returnObject = statuses.GroupBy(ss => ss.Server.Id)
                .Select(g =>
                {
                    var latestStatus = g.Where(st => st.Server.Id == g.Key)
                        .OrderByDescending(st => st.Timestamp)
                        .FirstOrDefault();

                    if (latestStatus == null)
                    {
                        // TODO: Log this as a real error
                        return new
                        {
                            Id = g.Key,
                            Up = false,
                            LastUpdated = DateTimeOffset.MinValue,
                            Error = "No status report within the last 15 minutes for this server."
                        };
                    }

                    return new
                    {
                        Id = g.Key,
                        Up = latestStatus.Status,
                        LastUpdated = latestStatus.Timestamp,
                        Error = ""
                    };
                })
                .ToList();

         
            var serialized = JsonConvert.SerializeObject(returnObject);
            return this.Ok(serialized);
        }

        /// <summary>
        /// This method is used to check the server status for a server (by id)
        /// </summary>
        /// <param name="id">The id of the server to check</param>
        /// <returns>JSON representation of the server status</returns>
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                // Log bad request - no id provided
                return this.HttpBadRequest();
            }

            Guid serverId;
            if (!Guid.TryParse(id, out serverId))
            {
                // Log bad request - incorrectly formatted id
                return this.HttpBadRequest();
            }

            // attempt to lookup server with given id
            var server = _context.Servers.SingleOrDefault(s => s.Id == serverId);
            if (server == null)
            {
                // Log bad request - no server with that id
                return this.HttpBadRequest();
            }

            // retrieve the latest server status
            var latestServerStatus = _context.ServerStatus
                .Where(ss => ss.Id == serverId)
                .OrderBy(ss => ss.Timestamp)
                .Take(1)
                .SingleOrDefault();

            if (latestServerStatus == null)
            {
                // Log bad request - no latest status with that id
                return this.HttpBadRequest();
            }

            var returnObject = new
            {
                server.Id,
                Up = latestServerStatus.Status,
                LastUpdated = latestServerStatus.Timestamp,
            };

            var serialized = JsonConvert.SerializeObject(returnObject);
            return this.Ok(serialized);
        }
    }
}