using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    //[Route("api/cities")]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        [HttpGet]
        public JsonResult GetCities()
        {
            return new JsonResult(CitiesDataStore.Current.Cities);
           
        }


        [HttpGet("{id}")]
        public JsonResult GetCity(int id)
        {
            return new JsonResult (CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == id));
        }
    }
}
