using Bepixplore.Application.Contracts.Destinations;
using Bepixplore.Destinations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Bepixplore.Favorites
{
    public class FavoriteAppService : ApplicationService, IFavoriteAppService
    {
        private readonly IRepository<Favorite, Guid> _favoriteRepository;
        private readonly IRepository<Destination, Guid> _destinationRepository;

        public FavoriteAppService(
            IRepository<Favorite, Guid> favoriteRepository,
            IRepository<Destination, Guid> destinationRepository)
        {
            _favoriteRepository = favoriteRepository;
            _destinationRepository = destinationRepository;
        }

        [UnitOfWork(isTransactional: true)]
        public async Task<DestinationDto> AddAsync(CreateUpdateDestinationDto input)
        {
            var userId = CurrentUser.GetId();
            var destination = await _destinationRepository.FirstOrDefaultAsync(x =>
                x.Name == input.Name && x.Country == input.Country);

            if (destination == null)
            {
                destination = new Destination(
                    GuidGenerator.Create(),
                    input.Name,
                    input.Country,
                    input.City,
                    input.Population,
                    input.Photo,
                    input.UpdateDate,
                    new Coordinates(input.Coordinates.Latitude, input.Coordinates.Longitude)
                );
                await _destinationRepository.InsertAsync(destination, autoSave: true);
            }
            var existingFavorite = await _favoriteRepository.FirstOrDefaultAsync(x =>
                x.UserId == userId && x.DestinationId == destination.Id);

            if (existingFavorite == null)
            {
                await _favoriteRepository.InsertAsync(
                    new Favorite(GuidGenerator.Create(), userId, destination.Id),
                    autoSave: true
                );
            }
            return ObjectMapper.Map<Destination, DestinationDto>(destination);
        }

        public async Task DeleteAsync(Guid destinationId)
        {
            var userId = CurrentUser.GetId();
            var favorite = await _favoriteRepository.FirstOrDefaultAsync(x =>
                x.UserId == userId && x.DestinationId == destinationId);

            if (favorite != null)
            {
                await _favoriteRepository.DeleteAsync(favorite);
            }
        }
        public async Task<List<DestinationDto>> GetListAsync()
        {
            var userId = CurrentUser.GetId();
            var favorites = await _favoriteRepository.GetListAsync(x => x.UserId == userId);

            if (!favorites.Any()) return new List<DestinationDto>();

            var destinationIds = favorites.Select(x => x.DestinationId).ToList();
            var destinations = await _destinationRepository.GetListAsync(x => destinationIds.Contains(x.Id));

            return ObjectMapper.Map<List<Destination>, List<DestinationDto>>(destinations);
        }

        public async Task<bool> IsFavoriteAsync(string name, string country)
        {
            var userId = CurrentUser.GetId();

            var destination = await _destinationRepository.FirstOrDefaultAsync(x =>
                x.Name == name && x.Country == country);

            if (destination == null) return false;

            return await _favoriteRepository.AnyAsync(x =>
                x.UserId == userId && x.DestinationId == destination.Id);
        }
    }
}