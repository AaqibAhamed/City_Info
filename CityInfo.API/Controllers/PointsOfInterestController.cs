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

            if (city == null)
            {
                return NotFound();
            }

            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{pointOfIntersetId}", Name = "GetPointOfInterst")]

        public ActionResult<PointOfInterestDto> GetPointOfInterst(int cityId, int pointOfIntersetId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            //check Point Of Interst

            var pointOfInterst = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfIntersetId);

            if (pointOfInterst == null)
            {
                return NotFound();
            }

            return Ok(pointOfInterst);
        }

        [HttpPost]
        public ActionResult<PointOfInterestDto> CreatePointOfInterst(int cityId,
            PointOfInterestCreationDto pointOfInterestCreationDto)
        //  [FromBody] not neccesary since we have [ApiController] at top of the controller
        {
            //if(!ModelState.IsValid)
            //{
            //    return BadRequest();  //not neccesary since we have [ApiController] at top of the controller
            //}

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            //Demo purpose -Improve later
            var maxPointOfInterest = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterest,
                Name = pointOfInterestCreationDto.Name,
                Description = pointOfInterestCreationDto.Description,
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterst",
                new
                {
                    cityId, //cityId = cityId 
                    pointOfIntersetId = finalPointOfInterest.Id // both for GetPointOfInterst route arguments
                },
                finalPointOfInterest);

        }

        [HttpPut("{pointOfIntersetId}")]
        public ActionResult<PointOfInterestDto> UpdatePointOfInterst(int cityId, int pointOfIntersetId,
            PointOfInterestUpdateDto pointOfInterestUpdateDto)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            //Find pointOfInterset

            var pointOfIntersetIdFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfIntersetId);

            if (pointOfIntersetIdFromStore == null)
            {
                return NotFound();
            }

            pointOfIntersetIdFromStore.Name = pointOfInterestUpdateDto.Name;
            pointOfIntersetIdFromStore.Description = pointOfInterestUpdateDto.Description;

            // return NoContent(); - not looks nice
            return Ok(pointOfIntersetIdFromStore); // when we want response back


        }
    }
}
