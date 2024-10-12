using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;

namespace CoreMasterDetails.Models.ViewModels
{
    public class ArtistViewModel
    {
        public int ArtistId { get; set; }

        [Required]
        [Display(Name = "Artist Name")]
        [StringLength(100, MinimumLength = 2)]
        public string ArtistName { get; set; } = null!;

        [Required, Display(Name = "Birth Date"), DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Dob { get; set; } = DateTime.Now;

        [Display(Name = "Mobile No.")]
        [Phone]
        public string? MobileNo { get; set; }

        public string? ImageUrl { get; set; }

        [Display(Name = "Is Alive?")]
        public bool IsAlive { get; set; }

        [Display(Name = "Profile Photo")]
        public IFormFile? ProfileFile { get; set; }

        [Display(Name = "Role")]
        public int? RoleId { get; set; }

        public List<Role>? Roles { get; set; }

        public virtual Role? Role { get; set; }

        public virtual IList<Movie> Movies { get; set; } = new List<Movie>();
    }
}