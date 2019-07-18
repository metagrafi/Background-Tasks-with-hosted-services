using CacheExp.Controllers;
using ClassLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CacheExp
{
    public class MonsterJobs
    {
        private readonly IPInfoProvider _client;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger _logger;

        public MonsterJobs(IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, IPInfoProvider client)
        {
            _client = client;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = loggerFactory.CreateLogger<MonsterJobs>();
        }
        public async Task BatchRequest(string[] batch, string guid)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<DataContext>();
                
                foreach (var ip in batch)
                {
                    try
                    {                        
                        if (!db.IPProfiles.Any(p => p.IP == ip))
                        {
                            var pDetails = _client.GetDetails(ip);
                            var profile = new IPProfile(pDetails)
                            {
                                IP = ip
                            };

                            db.IPProfiles.Add(profile);
                            await db.SaveChangesAsync();
                            
                        }
                        db.Works.Where(w => w.Id == guid).First().CompletedActions++;
                        await db.SaveChangesAsync();
                        
                    }
                    catch (Exception ex)
                    {
                        db.Works.Where(w => w.Id == guid).First().FailedActions++;
                        await db.SaveChangesAsync();
                        _logger.LogError(ex, $" Guid: {guid}. An error occurred writing to the " + $"database. Error: {ex.Message}");
                    }
                }
            }
        }

    }
}
