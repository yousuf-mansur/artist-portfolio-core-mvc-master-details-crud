using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreMasterDetails.Models;

public partial class Artist
{
    public int ArtistId { get; set; }

    public string ArtistName { get; set; } = null!;
    [Column(TypeName ="date")]

    public DateTime Dob { get; set; }

    public string Mobile { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;

    public bool IsAlive { get; set; }

    public int RoleId { get; set; }

    public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();

    public virtual Role Role { get; set; } = null!;
}
