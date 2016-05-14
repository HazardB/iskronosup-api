using System;
using System.Collections.Generic;
using Microsoft.Data.Entity;
using System.Linq;

namespace IsKronosUpYet.API.Models
{
    public class DatabaseContext : DbContext
    {
        private readonly string StatusCacheKey = "ServerStatus";
        private readonly string ServerCacheKey = "Server";

        public DbSet<Server> Servers { get; set; }
        public DbSet<ServerStatus> ServerStatus { get; set; }
        public DbSet<News> News { get; set; }

        public List<ServerStatus> RetrieveAllStatuses()
        {
            var cachedStatuses = InMemoryModelCache.Instance.Retrieve(StatusCacheKey) as List<ServerStatus>;
            if(cachedStatuses != null)
                return cachedStatuses;

            var statuses = this.ServerStatus
               .Include(st => st.Server)
               .Where(s => s.Timestamp > (DateTimeOffset.UtcNow - TimeSpan.FromMinutes(15)))
               .ToList();

            InMemoryModelCache.Instance.Save(StatusCacheKey, statuses);
            
            return statuses;
        }

        public List<Server> RetrieveAllServers()
        {
            var cachedServers = InMemoryModelCache.Instance.Retrieve(ServerCacheKey) as List<Server>;
            if (cachedServers != null)
                return cachedServers;

            var servers = this.Servers.ToList();
            InMemoryModelCache.Instance.Save(ServerCacheKey, servers);

            return servers;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // IP required
            modelBuilder.Entity<Server>()
                .Property(b => b.IP)
                .IsRequired();
        }
    }
}