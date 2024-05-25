using System.Text.Json;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    [Authorize]
    //[Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        // private readonly CitiesDataStore _citiesDataStore;

        private readonly ICityInfoRepository _cityInfoRepository;

        private readonly IMapper _mapper;

        int maxCitiesPageSize = 30;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        /* [HttpGet]
          public JsonResult GetCities()
        {
            return new JsonResult(_citiesDataStore.Cities);          
        }*/

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<CityWithoutPointOfInterestDto>>> GetCities()
        //{     

        //    var cityEntites = await _cityInfoRepository.GetAllCitiesAsync();

        //    var results = new List<CityWithoutPointOfInterestDto>();

        //    foreach (var cityEntity in cityEntites)
        //    {
        //        results.Add(new CityWithoutPointOfInterestDto
        //        {
        //            Id = cityEntity.Id,
        //            Name = cityEntity.Name,
        //            Description = cityEntity.Description,
        //        });

        //    }
        //   // return Ok(results);

        //    return Ok(_mapper.Map<IEnumerable<CityWithoutPointOfInterestDto>>(cityEntites));

        //}

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointOfInterestDto>>> GetCities(
            [FromQuery(Name = "filterOnName")] string? name, string? searchQuery, //FromQuery added for redability - not mandatory
            int pageNumber = 1, int pageSize = 20)
        {
            if (pageSize > maxCitiesPageSize)
            {
                pageSize = maxCitiesPageSize;
            }

            var (cityEntites, paginationMetaData) = await _cityInfoRepository.GetCities(name, searchQuery, pageNumber, pageSize);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));

            return Ok(_mapper.Map<IEnumerable<CityWithoutPointOfInterestDto>>(cityEntites));

        }

        /* public JsonResult GetCity(int id)
         {
             return new JsonResult (_citiesDataStore.Cities.FirstOrDefault(x => x.Id == id));
         }*/

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCitiy(int id, bool includePointsOfInterest = false)
        {
            //var selectedCity = (_citiesDataStore.Cities.FirstOrDefault(c => c.Id == id));

            //if (selectedCity == null)
            //{
            //    return NotFound();
            //}

            //return Ok(selectedCity);

            var selectedCity = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);

            if (selectedCity == null)
            {
                return NotFound();
            }

            if (!includePointsOfInterest)
            {
                return Ok(_mapper.Map<CityWithoutPointOfInterestDto>(selectedCity));
            }

            return Ok(_mapper.Map<CityDto>(selectedCity));


        }
    }
}
