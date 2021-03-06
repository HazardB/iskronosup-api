﻿using System;
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
        // Configuration variables
        private string _customAuthorisationHeader = "x-worker-auth";
        private string _customAuthorisationSecret = "!CHANGE-THIS!";

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
            var allStatuses = this._context.RetrieveAllStatuses();

            var formatted = allStatuses.GroupBy(ss => ss.Server.Id)
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

            var serialized = JsonConvert.SerializeObject(formatted);
            return this.Ok(serialized);
        }
        
        [HttpPost()]
        public IActionResult Post([FromBody] ServerStatusRequest payload)
        {
            // TODO: Implement more sophisticated API token authorization
            if (Request.Headers.ContainsKey(_customAuthorisationHeader))
            {
                if (Request.Headers[_customAuthorisationHeader] != _customAuthorisationSecret)
                {
                    return this.HttpUnauthorized();
                }
            }
            else
            {
                return this.HttpUnauthorized();
            }

            if (payload == null)
            {
                return this.HttpBadRequest();
            }
            
            var status = new ServerStatus
            {
                Id = Guid.NewGuid(),
                Server = new Server {  Id = payload.id },
                Timestamp = DateTimeOffset.UtcNow,
                Status = payload.status,
            };

            try
            {
                this._context.AddStatus(status);
                return this.Ok();
            }
            catch
            {
                return this.HttpNotFound();
            }
        }

        public class ServerStatusRequest
        {
            public Guid id { get; set; }
            public bool status { get; set; }
        }
    }
}