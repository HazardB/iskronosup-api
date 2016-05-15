using System;

namespace IsKronosUpYet.Worker
{
    public class ServerStatusUpdate
    {
        public Guid id { get; set; }
        public bool status { get; set; }

        public override string ToString()
        {
            return $"id={id},status={status}";
        }
    }
}