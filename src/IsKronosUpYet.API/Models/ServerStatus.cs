using System;

namespace IsKronosUpYet.API.Models
{
    public class ServerStatus
    {
        public Guid Id { get; set; }

        public Server Server { get; set; }

        public bool Status { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }
}
