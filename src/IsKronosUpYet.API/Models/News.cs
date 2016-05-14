using System;

namespace IsKronosUpYet.API.Models
{
    public class News
    {
        public Guid Id { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string Author { get; set; }

        public string Body { get; set; }
    }
}