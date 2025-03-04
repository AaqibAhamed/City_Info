﻿using Asp.Versioning;
using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/v{version:apiVersion}/cities/{cityId}/pointsofinterest")]
    [ApiController]
    [ApiVersion(2)]
    [Authorize (policy: "MustBeFromAntwerp")]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;

        private readonly IMailService _mailService;

        private readonly IMapper _mapper;

        private readonly ICityInfoRepository _cityInfoRepository;

        // private readonly CitiesDataStore _citiesDataStore;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger,
            IMailService mailService, IMapper mapper, ICityInfoRepository cityInfoRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));

            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));

            // _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            try
            {
                var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;

                //Add authorization policy control for GetPointsOfInterest for a city for specific user
                if (!await _cityInfoRepository.CityIdMatchesCityName(cityId, cityName))
                {
                    return Forbid();
                }


                if (!await _cityInfoRepository.CityExistsAsync(cityId))
                {
                    _logger.LogInformation($"City with id {cityId} was not found when accessing PointsOfInterest ");

                    return NotFound();
                }

                var pointsOfInterst = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);

                if (pointsOfInterst == null)
                {
                    return NotFound();
                }

                return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterst));

                //var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

                //if (city == null)
                //{
                //    _logger.LogInformation($"City with id {cityId} was not found when accessing PointsOfInterest ");

                //    return NotFound();
                //}

                //return Ok(city.PointsOfInterest);


            }
            catch (Exception ex)
            {

                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}.", ex);

                return StatusCode(500, "A problem happened while handling your request.");

            }

        }

        [HttpGet("{pointOfIntersetId}", Name = "GetPointOfInterst")]

        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterst(int cityId, int pointOfIntersetId)
        {
            
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {

                _logger.LogInformation($"City with id {cityId} was not found when accessing PointsOfInterest ");
                return NotFound();

            }

            var pointOfInterst = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfIntersetId);

            if (pointOfInterst == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterst));

            //var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

            //if (city == null)
            //{
            //    return NotFound();
            //}

            ////check Point Of Interst

            //var pointOfInterst = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfIntersetId);

            //if (pointOfInterst == null)
            //{
            //    return NotFound();
            //}

            //return Ok(pointOfInterst);

        }      

        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterst(int cityId,
            PointOfInterestCreationDto pointOfInterestCreationDto)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterstToAdd = _mapper.Map<PointOfInterest>(pointOfInterestCreationDto);

            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, pointOfInterstToAdd);

            await _cityInfoRepository.SaveChangesAsync();

            var createdpointOfInterstToReturn = _mapper.Map<PointOfInterestDto>(pointOfInterstToAdd);

            return CreatedAtRoute(nameof(GetPointOfInterst),
                new
                {
                    cityId = cityId,
                    pointOfIntersetId = createdpointOfInterstToReturn.Id

                },
                createdpointOfInterstToReturn);

        }      

        [HttpPut("{pointOfIntersetId}")]
        public async Task<ActionResult<PointOfInterestDto>> UpdatePointOfInterst(int cityId, int pointOfIntersetId,
            PointOfInterestUpdateDto pointOfInterestUpdateDto)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterstFromEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfIntersetId);

            if (pointOfInterstFromEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(pointOfInterestUpdateDto, pointOfInterstFromEntity);

            await _cityInfoRepository.SaveChangesAsync();

            // return NoContent();  // not looks good

            return Ok(pointOfInterestUpdateDto);
        }

        [HttpPatch("{pointOfIntersetId}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterst(int cityId, int pointOfIntersetId,
          JsonPatchDocument<PointOfInterestUpdateDto> patchDocument)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfIntersetFromEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfIntersetId);

            if (pointOfIntersetFromEntity == null)
            {
                return NotFound();
            }

            var pointOfIntersetToPatch = _mapper.Map<PointOfInterestUpdateDto>(pointOfIntersetFromEntity);

            patchDocument.ApplyTo(pointOfIntersetToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pointOfIntersetToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointOfIntersetToPatch, pointOfIntersetFromEntity);

            await _cityInfoRepository.SaveChangesAsync();

            // return NoContent(); // not looks good

            return Ok(pointOfIntersetFromEntity);

        }

        [HttpDelete("{pointOfIntersetId}")]
        public async Task<ActionResult> DeletePointOfInterst(int cityId, int pointOfIntersetId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestFromEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfIntersetId);

            if (pointOfInterestFromEntity == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestFromEntity);

            await _cityInfoRepository.SaveChangesAsync();

            _mailService.Send(
               "Point of interest deleted.",
               $"Point of interest {pointOfInterestFromEntity.Name} with id {pointOfInterestFromEntity.Id} was deleted.");

            return NoContent();

        }

        //[HttpDelete("{pointOfIntersetId}")]
        //public ActionResult DeletePointOfInterst(int cityId, int pointOfIntersetId)
        //{
        //    var city = _citiesDataStore.Cities.FirstOrDefault(p => p.Id == cityId);

        //    if (city == null)
        //    {
        //        return NotFound();
        //    }

        //    var pointOfIntersetIdFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfIntersetId);

        //    if (pointOfIntersetIdFromStore == null)
        //    {
        //        return NotFound();
        //    }

        //    city.PointsOfInterest.Remove(pointOfIntersetIdFromStore);

        //    _mailService.Send(
        //       "Point of interest deleted.",
        //       $"Point of interest {pointOfIntersetIdFromStore.Name} with id {pointOfIntersetIdFromStore.Id} was deleted.");

        //    return NoContent();

        //}

        //[HttpPut("{pointOfIntersetId}")]
        //public ActionResult<PointOfInterestDto> UpdatePointOfInterst(int cityId, int pointOfIntersetId,
        //    PointOfInterestUpdateDto pointOfInterestUpdateDto)
        //{
        //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

        //    if (city == null)
        //    {
        //        return NotFound();
        //    }

        //    //Find pointOfInterset

        //    var pointOfIntersetIdFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfIntersetId);

        //    if (pointOfIntersetIdFromStore == null)
        //    {
        //        return NotFound();
        //    }

        //    pointOfIntersetIdFromStore.Name = pointOfInterestUpdateDto.Name;
        //    pointOfIntersetIdFromStore.Description = pointOfInterestUpdateDto.Description;

        //    // return NoContent(); // not looks nice
        //    return Ok(pointOfIntersetIdFromStore); // when we want response back

        //}

        //[HttpPost]
        //public ActionResult<PointOfInterestDto> CreatePointOfInterst(int cityId,
        //    PointOfInterestCreationDto pointOfInterestCreationDto)
        ////  [FromBody] not neccesary since we have [ApiController] at top of the controller
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest();  //not neccesary since we have [ApiController] at top of the controller
        //    }

        //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city == null)
        //    {
        //        return NotFound();
        //    }

        //    //Demo purpose -Improve later
        //    var maxPointOfInterest = _citiesDataStore.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

        //    var finalPointOfInterest = new PointOfInterestDto()
        //    {
        //        Id = ++maxPointOfInterest,
        //        Name = pointOfInterestCreationDto.Name,
        //        Description = pointOfInterestCreationDto.Description,
        //    };

        //    city.PointsOfInterest.Add(finalPointOfInterest);

        //    return CreatedAtRoute("GetPointOfInterst",
        //        new
        //        {
        //            cityId, //cityId = cityId 
        //            pointOfIntersetId = finalPointOfInterest.Id // both for GetPointOfInterst route arguments
        //        },
        //        finalPointOfInterest);

        //}
    }
}
