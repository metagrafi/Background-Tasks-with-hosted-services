using AutoMapper;
using ClassLibrary.IPStack;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class IPStackClient : IPInfoProvider
    {
        private readonly HttpClient client = new HttpClient();

        private readonly MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddProfile<IPStackProfile>());
        

        private string GetEndPoint(string ip)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".iNikitz ClassLibrary");
            return string.Format(Properties.Resources.IPStackEndpoint, ip, Properties.Resources.IPStackAccessKey);
        }
        private async Task<string> GetIPStackDetailsAsync(string ip)
        {
            try
            {
                return await client.GetStringAsync(GetEndPoint(ip));
            }
            catch(HttpRequestException h)
            {
                throw h;
            }
        }

        private string GetIPStackDetails(string ip)
        {
            try
            {
                return client.GetStringAsync(GetEndPoint(ip)).Result;
            }
            catch (HttpRequestException h)
            {
                throw h;
            }
        }

        public async Task<IPDetails> GetDetailsAsync(string ip)
        {
            try
            {
                return config.CreateMapper().Map<IPDetails>(IPStackDetails.FromJson(await GetIPStackDetailsAsync(ip)));
            }
            catch(Exception e)
            {
                throw new IPServiceNotAvailableException(e.Message);
            }
        }

        public IPDetails GetDetails(string ip)
        {
            try
            {
                return config.CreateMapper().Map<IPDetails>(IPStackDetails.FromJson(GetIPStackDetails(ip)));
            }
            catch (Exception e)
            {
                throw new IPServiceNotAvailableException(e.Message);
            }
        }
    }
}
