using CoreMasterDetails.Models;
using CoreMasterDetails.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CoreMasterDetails.Controllers
{
    public class ArtistController : Controller
    {
        private readonly ArtistDbContext _db;
        private readonly IWebHostEnvironment _webHost;

        public ArtistController(ArtistDbContext db, IWebHostEnvironment webHost)
        {
            _db = db;
            _webHost = webHost;
        }
        public IActionResult Index()
        {
            var applicants = _db.Artists.Include(i => i.Movies).ToList();
            applicants = _db.Artists.Include(a => a.Role).ToList();
            return View(applicants);
        }
        public JsonResult GetCourses()
        {
            List<SelectListItem> cors = (from cor in _db.Roles
                                         select new SelectListItem
                                         {
                                             Value = cor.RoleId.ToString(),
                                             Text = cor.RoleName
                                         }).ToList();
            return Json(cors);
        }
        public IActionResult Create()
        {
            ArtistViewModel artist = new ArtistViewModel();
            artist.Roles = _db.Roles.ToList();
            artist.Movies.Add(new Movie() { MovieId = 1 });

            return View(artist);
        }
        [HttpPost]
        public IActionResult Create(ArtistViewModel avm)
        {
            string uniqueFileName = GetUploadedFileName(avm);
            avm.ImageUrl = uniqueFileName;
            Artist artist = new Artist
            {
                ArtistName = avm.ArtistName,
                RoleId = (int)avm.RoleId,
                Mobile = avm.MobileNo,
                IsAlive = avm.IsAlive,
                Dob = avm.Dob,
                ImageUrl = avm.ImageUrl
            };
            _db.Add(artist);
            _db.SaveChanges();

            var user = _db.Artists.FirstOrDefault(x => x.Mobile == avm.MobileNo);
            if (user != null && avm.Movies != null && avm.Movies.Count > 0)
            {
                foreach (var item in avm.Movies)
                {
                    if (!string.IsNullOrWhiteSpace(item.MovieName))
                    {
                        Movie mv = new Movie
                        {
                            ArtistId = user.ArtistId,
                            Duration = item.Duration,
                            MovieName = item.MovieName.Trim()
                        };
                        _db.Add(mv);
                    }
                }
            }
            _db.SaveChanges();
            return RedirectToAction("index");
        }


        public ActionResult Delete(int? id)
        {
            var app = _db.Artists.Find(id);
            var existsMovie = _db.Movies.Where(e => e.ArtistId == id).ToList();
            foreach (var exp in existsMovie)
            {
                _db.Movies.Remove(exp);
            }
            _db.Entry(app).State = EntityState.Deleted;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }



        private string GetUploadedFileName(ArtistViewModel avm)
        {
            string uniqueFileName = null;

            if (avm.ProfileFile != null)
            {
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + avm.ProfileFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    avm.ProfileFile.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            Artist artist = _db.Artists.Include(a => a.Movies).FirstOrDefault(x => x.ArtistId == id);

            if (artist != null)
            {
                ArtistViewModel avm = new ArtistViewModel()
                {
                    ArtistId = artist.ArtistId,
                    ArtistName = artist.ArtistName,
                    MobileNo = artist.Mobile,
                    Dob = artist.Dob,
                    ImageUrl = artist.ImageUrl,
                    RoleId = artist.RoleId,
                    IsAlive = artist.IsAlive,
                    Roles = _db.Roles.ToList(),
                    Movies = artist.Movies.ToList()
                };

                return View("Edit", avm);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(ArtistViewModel avm)
        {
            try
            {
                Artist existingArtist = _db.Artists
                    .Include(a => a.Movies)
                    .FirstOrDefault(x => x.ArtistId == avm.ArtistId);

                if (existingArtist != null)
                {
                    existingArtist.ArtistName = avm.ArtistName;
                    existingArtist.RoleId = (int)avm.RoleId;
                    existingArtist.Mobile = avm.MobileNo;
                    existingArtist.IsAlive = avm.IsAlive;
                    existingArtist.Dob = avm.Dob;

                    if (avm.ProfileFile != null)
                    {
                        string uniqueFileName = GetUploadedFileName(avm);
                        existingArtist.ImageUrl = uniqueFileName;
                    }


                    var existingMovie = existingArtist.Movies.Select(m => m.MovieId).ToList();
                    var newMovie = avm.Movies.Select(m => m.MovieId).ToList();


                    foreach (var movie in existingArtist.Movies.ToList())
                    {
                        if (!newMovie.Contains(movie.MovieId))
                        {
                            _db.Movies.RemoveRange(movie);
                        }
                    }


                    foreach (var item in avm.Movies)
                    {

                        Movie mv = new Movie()
                        {
                            Duration = item.Duration,
                            ArtistId = avm.ArtistId,
                            MovieName = item.MovieName,
                        };
                        _db.Movies.Add(mv);
                    }

                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return NotFound();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.FirstOrDefault();
                if (entry != null)
                {
                    var databaseValues = entry.GetDatabaseValues();
                    if (databaseValues != null)
                    {
                        var databaseArtist = (Artist)databaseValues.ToObject();

                        ModelState.AddModelError("", "The entity you are trying to edit has been modified by another user. Please refresh the page and try again.");

                        entry.OriginalValues.SetValues(databaseValues);

                        avm.Roles = _db.Roles.ToList();
                        avm.Movies = databaseArtist.Movies.ToList();

                        return View("Edit", avm);
                    }
                }

                ModelState.AddModelError("", "The entity you are trying to edit has been deleted by another user.");
            }

            return RedirectToAction("Index");
        }

    }
}
