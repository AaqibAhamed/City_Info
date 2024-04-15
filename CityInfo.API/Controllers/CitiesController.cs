using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    //[Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        // private readonly CitiesDataStore _citiesDataStore;

        private readonly ICityInfoRepository _cityInfoRepository;

        public CitiesController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
        }


        /* [HttpGet]
          public JsonResult GetCities()
        {
            return new JsonResult(_citiesDataStore.Cities);          
        }*/

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointOfInterestDto>>> GetCities()  
        {
            //return Ok(_citiesDataStore.Cities);

            var cityEntites = await _cityInfoRepository.GetAllCitiesAsync();

            var results = new List<CityWithoutPointOfInterestDto>();

            foreach (var cityEntity in cityEntites)
            {
                results.Add(new CityWithoutPointOfInterestDto
                {
                    Id = cityEntity.Id,
                    Name = cityEntity.Name,
                    Description = cityEntity.Description,
                });

            }
            return Ok(results);
        }

        /* public JsonResult GetCity(int id)
         {
             return new JsonResult (_citiesDataStore.Cities.FirstOrDefault(x => x.Id == id));
         }*/

        //[HttpGet("{id}")]
        //public ActionResult<CityDto> GetCitiy(int id)
        //{
        //    var selectedCity = (_citiesDataStore.Cities.FirstOrDefault(c => c.Id == id));

        //    if (selectedCity == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(selectedCity);
        //}
    }
}
