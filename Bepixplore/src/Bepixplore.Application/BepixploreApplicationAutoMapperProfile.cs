using AutoMapper;
using Bepixplore.Application.Contracts.Destinations;
using Bepixplore.Destinations;
using Bepixplore.Experiences;
using Bepixplore.Notifications;
using Bepixplore.Ratings;

namespace Bepixplore;

public class BepixploreApplicationAutoMapperProfile : Profile
{
    public BepixploreApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<Destination, DestinationDto>();
        CreateMap<CreateUpdateDestinationDto, Destination>();
        CreateMap<Coordinates, CoordinatesDto>();
        CreateMap<CoordinatesDto, Coordinates>();
        CreateMap<TravelExperience, TravelExperienceDto>();
        CreateMap<CreateUpdateTravelExperienceDto, TravelExperience>();
        CreateMap<Notification, NotificationDto>();

        CreateMap<Rating, RatingDto>();
        CreateMap<CreateUpdateRatingDto, Rating>();
        CreateMap<TravelExperienceDto, TravelExperience>();
    }
}
