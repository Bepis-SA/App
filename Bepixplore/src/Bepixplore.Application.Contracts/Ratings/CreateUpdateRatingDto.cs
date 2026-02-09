using System;
using System.ComponentModel.DataAnnotations;

namespace Bepixplore.Ratings
{
    public class CreateUpdateRatingDto
    {
        [Required(ErrorMessage = "Destination is required.")]
        public Guid DestinationId { get; set; }

        [Required(ErrorMessage = "Score is requiered.")]
        [Range(1, 5, ErrorMessage = "The score must be between 1 and 5.")]
        public int Score { get; set; }

        [StringLength(1000)]
        public string Comment { get; set; }
    }
}