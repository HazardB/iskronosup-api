﻿using System;
using System.Linq;
using IsKronosUpYet.API.Models;
using Microsoft.AspNet.Mvc;
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