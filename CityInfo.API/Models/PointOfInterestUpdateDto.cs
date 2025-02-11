using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models
{
    /// <summary>
    /// Point Of Interest Update Dto
    /// </summary>
    public class PointOfInterestUpdateDto
    {
        /// <summary>
        ///  The name of the Point Of Interest 
        /// </summary>
        [Required(ErrorMessage = "Please provide a name value")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        ///  The description of the Point Of Interest
        /// </summary>
        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
