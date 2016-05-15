using System;

namespace IsKronosUpYet.Worker
{
    public class Server
    {
        public Guid Id { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"Id={Id},IP={IP},Port={Port},Name={Name}";
        }
    }
}