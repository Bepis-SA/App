using Bepixplore.Application.Contracts.Destinations;
using Bepixplore.Destinations; // Donde esté tu entidad Destination
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
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

        public async Task AddAsync(CreateUpdateDestinationDto input)
        {
            var userId = CurrentUser.GetId();

            // 1. Buscamos si el destino ya existe localmente por nombre y país
            var destination = await _destinationRepository.FirstOrDefaultAsync(x =>
                x.Name == input.Name && x.Country == input.Country);

            if (destination == null)
            {
                // 2. Aquí está la solución al error: Pasamos los 8 parámetros exactos
                destination = await _destinationRepository.InsertAsync(
                    new Destination(
                        GuidGenerator.Create(),      // 1: Guid id
                        input.Name,                  // 2: string name
                        input.Country,               // 3: string country
                        input.City,                  // 4: string city
                        input.Population,            // 5: uint population
                        input.Photo,                 // 6: string photo
                        input.UpdateDate,            // 7: DateTime updateDate
                        new Coordinates(             // 8: Coordinates coordinates
                            input.Coordinates.Latitude,
                            input.Coordinates.Longitude)
                    )
                );
            }

            // 3. Vinculamos el destino con el usuario en la tabla Favorites
            var existingFavorite = await _favoriteRepository.FirstOrDefaultAsync(x =>
                x.UserId == userId && x.DestinationId == destination.Id);

            if (existingFavorite == null)
            {
                await _favoriteRepository.InsertAsync(
                    new Favorite(GuidGenerator.Create(), userId, destination.Id)
                );
            }
        }

        // 6.2: Eliminar de favoritos
        public async Task RemoveAsync(Guid destinationId)
        {
            var userId = CurrentUser.GetId();
            var favorite = await _favoriteRepository.FirstOrDefaultAsync(x =>
                x.UserId == userId && x.DestinationId == destinationId);

            if (favorite != null)
            {
                await _favoriteRepository.DeleteAsync(favorite);
            }
        }

        // 6.3: Consultar lista personal de favoritos
        public async Task<List<DestinationDto>> GetListAsync()
        {
            var userId = CurrentUser.GetId();

            // Obtenemos todos los registros de favoritos del usuario
            var favorites = await _favoriteRepository.GetListAsync(x => x.UserId == userId);
            var destinationIds = favorites.Select(x => x.DestinationId).ToList();

            // Buscamos los destinos reales para devolver los nombres y datos
            var destinations = await _destinationRepository.GetListAsync(x =>
                destinationIds.Contains(x.Id));

            return ObjectMapper.Map<List<Destination>, List<DestinationDto>>(destinations);
        }
    }
}