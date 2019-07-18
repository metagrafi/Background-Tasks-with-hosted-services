using System;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public interface IPInfoProvider
    {
        Task<IPDetails> GetDetailsAsync(string ip);
        IPDetails GetDetails(string ip);
    }
    public interface IPDetails
    {
        string City { get; set; }
        string Country { get; set; }
        string Continent { get; set; }
        double Latitude { get; set; }
        double Longitude { get; set; }
    }
}
