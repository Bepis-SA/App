using Shouldly;
using System;
using Volo.Abp;
using Xunit;

namespace Bepixplore.Ratings
{
    public class RatingDomain_Test
    {
        [Fact]
        public void Should_Create_Rating_With_Valid_Data()
        {
            // Arrange & Act
            var rating = new Rating(
                id: Guid.NewGuid(),
                userId: Guid.NewGuid(),
                destinationId: Guid.NewGuid(),
                score: 4,
                comment: "Muy bueno"
            );

            // Assert
            rating.Score.ShouldBe(4);
            rating.Comment.ShouldBe("Muy bueno");
        }

        [Fact]
        public void Should_Allow_Optional_Comment()
        {
            // Arrange & Act
            var rating = new Rating(
                id: Guid.NewGuid(),
                userId: Guid.NewGuid(),
                destinationId: Guid.NewGuid(),
                score: 5
            );

            // Assert
            rating.Comment.ShouldBeNull();
        }

        [Fact]
        public void Should_Throw_BusinessException_When_Score_Is_Invalid()
        {
            // Arrange & Act
            var exception = Assert.Throws<BusinessException>(() =>
            {
                new Rating(
                    id: Guid.NewGuid(),
                    userId: Guid.NewGuid(),
                    Guid.NewGuid(),
                    score: 6);
            });

            // Assert
            exception.Code.ShouldBe("Bepixplore:Rating:InvalidScore");
            exception.Data["Value"].ShouldBe("6");
        }

        [Fact]
        public void Should_Throw_When_UserId_Is_Empty()
        {
            // Arrange & Act
            var exception = Assert.Throws<BusinessException>(() =>
            {
                new Rating(
                    id: Guid.NewGuid(),
                    userId: Guid.Empty,
                    destinationId: Guid.NewGuid(),
                    score: 5);
            });

            // Assert
            exception.Code.ShouldBe("Bepixplore:Rating:UserIdRequired");
        }

        [Fact]
        public void Should_Throw_When_DestinationId_Is_Empty()
        {
            // Arrange & Act
            var exception = Assert.Throws<BusinessException>(() =>
            {
                new Rating(
                    id: Guid.NewGuid(),
                    userId: Guid.NewGuid(),
                    destinationId: Guid.Empty,
                    score: 5);
            });

            // Assert
            exception.Code.ShouldBe("Bepixplore:Rating:DestinationIdRequired");
        }
    }
}
