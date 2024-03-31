using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : Controller
    {

        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if(city == null)
            {
                return NotFound();
            }
            
            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{pointOfIntersetId}")]

        public ActionResult<PointOfInterestDto> GetPointOfInterst(int cityId, int pointOfIntersetId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            //check Point Of Interst

            var pointOfInterst =  city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfIntersetId);

            if (pointOfInterst == null)
            {
                return NotFound();
            }

            return Ok(pointOfInterst);
        }
    }
}
