using System;
using Volo.Abp.Application.Dtos;

namespace Bepixplore.Users
{
    public class PublicUserProfileDto : EntityDto<Guid>
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePictureUrl { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
