using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Auth.Entities
{
    public class User
    {
        [Required]
        public Guid UserID { get; set; }
        public string? GoogleID { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [EmailAddress,MaxLength(100)]
        public string Email { get; set; }
        [Required,Phone,MaxLength(10)]
        public long PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        [Url]
        public string? ProfilePictureURL { get; set; }
        public int RoleID { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        //public int RoleID { get; set; }

        //public ICollection<Leaves> Leaves { get; set; }
        //public ICollection<User_Sessions> Sessions { get; set; }
    }
}

