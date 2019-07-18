using AutoMapper;
using ClassLibrary.IPStack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{

    /// <summary>
    // usage: var config = new MapperConfiguration(cfg =>  cfg.AddProfile<IPStackProfile>());
    /// </summary>
    public class IPStackProfile : Profile
    {
        public IPStackProfile()
        {
            CreateMap<IPStackDetails, IPDetails>()
                .ForMember(d => d.Continent, a => a.MapFrom(src => src.ContinentName))
                .ForMember(d => d.Country, a => a.MapFrom(src => src.CountryName));
        }
    }
}
