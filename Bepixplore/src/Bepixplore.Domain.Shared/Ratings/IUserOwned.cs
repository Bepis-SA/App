using System;

namespace Bepixplore.Ratings
{
    public interface IUserOwned
    {
        Guid UserId { get; set; }
    }
}
