using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Bepixplore.Ratings
{
    public class Rating : AuditedEntity<Guid>, IUserOwned
    {
        public Guid UserId { get; set; }
        public Guid DestinationId { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; } = null;

        private Rating() { } // EF Core

        public Rating(
            Guid id,
            Guid userId,
            Guid destinationId,
            int score,
            string comment = null)
            : base(id)
        {
            if (userId == Guid.Empty)
                throw new BusinessException("Bepixplore:Rating:UserIdRequired")
                    .WithData("Field", nameof(userId));

            if (destinationId == Guid.Empty)
                throw new BusinessException("Bepixplore:Rating:DestinationIdRequired")
                    .WithData("Field", nameof(destinationId));

            if (score < 1 || score > 5)
                throw new BusinessException("Bepixplore:Rating:InvalidScore")
                    .WithData("Field", nameof(score))
                    .WithData("Value", score.ToString());

            Comment = Check.Length(comment, nameof(comment), maxLength: 1000);

            UserId = userId;
            DestinationId = destinationId;
            Score = score;
        }
    }
}
