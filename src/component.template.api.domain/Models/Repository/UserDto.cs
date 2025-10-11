using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace component.template.api.domain.Models.Repository;

[Table("Users")]
public class UserDto
{
        [Key]
        public long UserId { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; }

        [Required, MaxLength(256)]
        public string PasswordHash { get; set; }

        [Required, MaxLength(100)]
        public string Email { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // public ICollection<UserProfileDto> UserProfiles { get; set; }
}
