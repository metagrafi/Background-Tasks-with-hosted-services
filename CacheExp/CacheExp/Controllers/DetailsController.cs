using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CacheExp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetailsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IPInfoProvider _client;
        private readonly IMemoryCache _cache;

        private readonly MonsterJobs _jobs;

        public IBackgroundTaskQueue Queue { get; }

        public DetailsController(DataContext context,
                                IPInfoProvider client, 
                                IMemoryCache cache,
                                IBackgroundTaskQueue queue,
                                MonsterJobs jobs)
        {
            _context = context;
            _client = client;
            _cache = cache;

            Queue = queue;
            _jobs = jobs;
 
        }
        private IPDetails GetDetails(string ip)
        {
            if (_context.IPProfiles.Any(p => p.IP == ip))
            {
                return _context.IPProfiles.Where(p => p.IP == ip).First();
            }
            else
            {
                var pDetails = _client.GetDetails(ip);
                var profile = new IPProfile(pDetails)
                {
                    IP = ip
                };

                _context.IPProfiles.Add(profile);
                _context.SaveChanges();
                return profile;
            }
        }
        [HttpGet("{ip}")]
        public async Task<IActionResult> Get(string ip)
        {
            try
            {
                var cacheEntry = _cache.GetOrCreate(ip, entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60);
                    return Task.Run(() => GetDetails(ip));
                });
                return Ok(await cacheEntry);
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    ErrorDescription = e.Message
                });
            }

        }
        
        [HttpPost]
        public IActionResult QueueWork([FromBody] List<string> ips)
        {
            try
            {
                var guid = Guid.NewGuid().ToString();
                var work = new Work
                {
                    Id = guid,
                    TotalActions = ips.Count,
                    CompletedActions = 0,
                    FailedActions = 0
                };
                _context.Works.Add(work);
                _context.SaveChanges();
                var count = 1;
                var list_chunk = new List<string>();
                foreach (var ip in ips)
                {
                    list_chunk.Add(ip);
                    if (count % 10 == 0 || ips.Count == count )
                    {
                        var batch = new string[10];
                        list_chunk.CopyTo(batch);
                        Queue.QueueBackgroundWorkItem( async token =>  await _jobs.BatchRequest(batch, guid));
                        list_chunk.Clear();
                    }

                    count++;
                }

                return Ok(work);
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    ErrorDescription = e.Message
                });
            }
        }

    }
}
