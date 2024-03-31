using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    //[Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        [HttpGet]
        /*public JsonResult GetCities()
        {
            return new JsonResult(CitiesDataStore.Current.Cities);
           
        }*/

        public ActionResult<IEnumerable<CityDto>> GetCities()
        {
            return Ok(CitiesDataStore.Current.Cities);
        }


        [HttpGet("{id}")]

       /* public JsonResult GetCity(int id)
        {
            return new JsonResult (CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == id));
        }*/

        public ActionResult<CityDto> GetCitiy(int id)
        {
            var selectedCity = (CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id));

            if (selectedCity == null)
            {
                return NotFound();
            }

            return Ok(selectedCity);
        }
    }
}
