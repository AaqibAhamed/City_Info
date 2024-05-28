using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetAllCitiesAsync();

        Task<(IEnumerable<City>,PaginationMetaData)> GetCities(string? name, string? searchQuery,int pageNumber,  int pageSize);

        Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest);

        Task<bool> CityExistsAsync(int cityId);

        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);

        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId,
            int pointOfInterestId);

        Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);

        Task<bool> SaveChangesAsync();

        Task<bool> CityIdMatchesCityName(int cityId, string cityName);

        void DeletePointOfInterest(PointOfInterest pointOfInterest);
    }
}
