using System;
using System.Collections.Generic;

namespace CoreMasterDetails.Models;

public partial class Movie
{
    public int MovieId { get; set; }

    public string MovieName { get; set; } = null!;

    public int Duration { get; set; }

    public int ArtistId { get; set; }

    public virtual Artist Artist { get; set; } = null!;
}
