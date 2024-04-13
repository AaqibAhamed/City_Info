using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : Controller
    {
        private readonly ILogger<PointsOfInterestController> _logger;

        private readonly IMailService _mailService;

        private readonly CitiesDataStore _citiesDataStore;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, 
            IMailService mailService, CitiesDataStore citiesDataStore)
        { 
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));

            _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
 
        }

        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
        {
            try
            {
                var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

                if (city == null)
                {
                    _logger.LogInformation($"City with id {cityId} was not found when accessing PointsOfInterest ");

                    return NotFound();
                }

                return Ok(city.PointsOfInterest);
            }
            catch (Exception ex)
            {

                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}.", ex);
               
                return StatusCode(500, "A problem happened while handling your request.");

            }

        }

        [HttpGet("{pointOfIntersetId}", Name = "GetPointOfInterst")]

        public ActionResult<PointOfInterestDto> GetPointOfInterst(int cityId, int pointOfIntersetId)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

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

            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            //Demo purpose -Improve later
            var maxPointOfInterest = _citiesDataStore.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

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
            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

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

            // return NoContent(); // not looks nice
            return Ok(pointOfIntersetIdFromStore); // when we want response back

        }

        [HttpPatch("{pointOfIntersetId}")]
        public ActionResult PartiallyUpdatePointOfInterst(int cityId, int pointOfIntersetId,
            JsonPatchDocument<PointOfInterestUpdateDto> patchDocument)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfIntersetIdFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfIntersetId);

            if (pointOfIntersetIdFromStore == null)
            {
                return NotFound();
            }

            var pointOfIntersetToPatch = new PointOfInterestUpdateDto()
            {
                Name = pointOfIntersetIdFromStore.Name,
                Description = pointOfIntersetIdFromStore.Description,
            };

            patchDocument.ApplyTo(pointOfIntersetToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pointOfIntersetToPatch))
            {
                return BadRequest(ModelState);
            }

            pointOfIntersetIdFromStore.Name = pointOfIntersetToPatch.Name;
            pointOfIntersetIdFromStore.Description = pointOfIntersetToPatch.Description;

            // return NoContent(); //not looks nice

            return Ok(pointOfIntersetIdFromStore);
        }

        [HttpDelete("{pointOfIntersetId}")]
        public ActionResult DeletePointOfInterst(int cityId, int pointOfIntersetId)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(p => p.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfIntersetIdFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfIntersetId);

            if (pointOfIntersetIdFromStore == null)
            {
                return NotFound();
            }

            city.PointsOfInterest.Remove(pointOfIntersetIdFromStore);

            _mailService.Send(
               "Point of interest deleted.",
               $"Point of interest {pointOfIntersetIdFromStore.Name} with id {pointOfIntersetIdFromStore.Id} was deleted.");

            return NoContent();

        }
    }
}
