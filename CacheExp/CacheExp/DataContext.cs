using ClassLibrary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CacheExp
{
    [Table("Works")]
    public class Work
    {
        [Key]
        public string Id { get; set; }
        public int TotalActions { get; set; }
        public int CompletedActions { get; set; }
        public int FailedActions { get; set; }
    }
    [Table("IPProfiles")]
    public class IPProfile :IPDetails
    {
        public IPProfile() { }
        public IPProfile(IPDetails details)
        {
            city = details.City;
            country = details.Country;
            continent = details.Continent;
            latitude = details.Latitude;
            longitude = details.Longitude;

        }
        [Key]
        public string IP { get; set; }

        private string city;
        public string City
        {
            get { return city; }
            set { city = value; }
        }
        private string country;
        public string Country
        {
            get { return country; }
            set { country = value; }
        }
        private string continent;
        public string Continent
        {
            get { return continent; }
            set { continent = value; }
        }
        private double latitude;
        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }
        private double longitude;
        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }
    }

    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<IPProfile> IPProfiles { get; set; }
        public DbSet<Work> Works { get; set; }

    }
}
