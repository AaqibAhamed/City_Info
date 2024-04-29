using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;

namespace CityInfo.API.Profiles
{
    public class PointOfInterestProfile : Profile
    {
        public PointOfInterestProfile() 
        {
            CreateMap<PointOfInterest, PointOfInterestDto>(); // Get -Entity to Dto

            CreateMap<PointOfInterestCreationDto, PointOfInterest>(); // POST

            CreateMap<PointOfInterestUpdateDto, PointOfInterest>(); //PUT 

            CreateMap<PointOfInterest,PointOfInterestUpdateDto>(); // Fetch
        }
    }
}
