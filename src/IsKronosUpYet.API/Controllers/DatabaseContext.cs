using System;
using System.Collections.Generic;
using System.Linq;
using IsKronosUpYet.API.Caching;
using IsKronosUpYet.API.Models;
using Microsoft.Data.Entity;

namespace IsKronosUpYet.API.Controllers
{
    public class DatabaseContext : DbContext
    {
        private readonly string StatusCacheKey = "ServerStatus";
        private readonly string ServerCacheKey = "Server";

        public DbSet<Server> Servers { get; set; }
        public DbSet<ServerStatus> ServerStatus { get; set; }
        public DbSet<News> News { get; set; }

        public List<ServerStatus> RetrieveAllStatuses(bool ignoreCache = false)
        {
            if (!ignoreCache)
            {
                var cachedStatuses = InMemoryModelCache.Instance.Retrieve(StatusCacheKey) as List<ServerStatus>;
                if (cachedStatuses != null)
                    return cachedStatuses;
            }

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

        public void AddStatus(ServerStatus status)
        {
            this.ServerStatus.Add(status, GraphBehavior.SingleObject);
            this.SaveChanges();

            // Remove (and thus invalidate) the status cache, forcing re-retrieval at next request.
            InMemoryModelCache.Instance.Remove(StatusCacheKey);
        }
    }
}