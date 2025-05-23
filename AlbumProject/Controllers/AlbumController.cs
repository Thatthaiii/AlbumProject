using AlbumProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace AlbumProject.Controllers
{
    public class AlbumController : Controller
    {
        private readonly AlbumProjectContext _context;

        public AlbumController(AlbumProjectContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchString)
        {
            List<Album> albumsQuery =  _context.Albums
                .Include(a => a.File)
                .Include(a => a.Songs)
                .Where(a => a.IsDeleted != true)
                .ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                albumsQuery = albumsQuery.Where(a => a.Name.Contains(searchString) ||
                                                     a.Songs.Any(s => s.Name.Contains(searchString)) 
                                                     
                ).ToList();
            }

            List<Album> albums = albumsQuery.ToList();
            ViewData["CurrentFilter"] = searchString;
            return View(albums);
        }


        [HttpGet]
        public IActionResult Create()
        {
            Album album = new Album
            {
                Songs = new List<Song> { new Song() } // ใส่เปล่าก็ได้ เพื่อให้ EditorFor แสดง
            };

            return View(album);
        }

        // POST: Album/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Album album,IFormFile? CoverPhoto,string? CoverPhotoTempPath,string? AddSong,string? RemoveId)
        {
            album.Songs ??= new List<Song>();


            // กรณีกด "+ Add Song"
            if (!string.IsNullOrEmpty(AddSong))
            {
                album.Songs.Add(new Song());

                // ถ้าไม่ได้แนบ CoverPhoto ใหม่ แต่เคยมีค่าเดิม
                if (CoverPhoto == null && !string.IsNullOrEmpty(CoverPhotoTempPath))
                {
                    ViewBag.CoverPhotoTempPath = CoverPhotoTempPath;
                }

                ModelState.Clear(); // เพื่อให้ EditorFor ไม่ค้างค่าเก่า
                return View(album);
            }

            if (int.TryParse(RemoveId, out int songIdToRemove))
            {
                foreach (Song song in album.Songs)
                {
                    if (song.Id == songIdToRemove)
                    {
                        song.IsDeleted = true;
                        break;
                    }
                }

                ModelState.Clear();
                return View(album);
            }

            // บันทึกจริง
            if (ModelState.IsValid)
            {
                bool success = album.Create(_context, CoverPhoto);

                if (success)
                    return RedirectToAction("Index");
            }

            // ถ้า validate ไม่ผ่าน ให้แสดงรูป cover เดิม (temp)
            if (!string.IsNullOrEmpty(CoverPhotoTempPath))
            {
                ViewBag.CoverPhotoTempPath = CoverPhotoTempPath;
            }
            return View(album);
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            Album? album =  _context.Albums
                                .Include(a => a.Songs)
                                .Include(a => a.File)
                                .FirstOrDefault(a => a.Id == id && !a.IsDeleted);

            if (album == null) return NotFound();

            ViewBag.CoverPhotoTempPath = album.File?.FilePath;

            return View(album);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Album album, string? AddSong, string? RemoveId, string? OldCoverPhotoPath, List<int>? DeletedIds)
        {

            if (!string.IsNullOrEmpty(AddSong))
            {
                album.Songs.Add(new Song());

                if (album.File == null && !string.IsNullOrEmpty(OldCoverPhotoPath))
                {
                    album.File = new Models.File { FilePath = OldCoverPhotoPath };
                }

                ModelState.Clear();
                return View(album);
            }

            if (int.TryParse(RemoveId, out int songIdToRemove))
            {
                if (album.File == null && !string.IsNullOrEmpty(OldCoverPhotoPath))
                {
                    album.File = new Models.File { FilePath = OldCoverPhotoPath };
                }
                foreach (Song song in album.Songs)
                {
                    if (song.Id == songIdToRemove)
                    {
                        song.IsDeleted = true;
                        break;
                    }
                }

                ModelState.Clear();
                return View(album);
            }

            IFormFile newCoverFile = Request.Form.Files["CoverPhoto"];

            bool success = album.Update(_context, newCoverFile);

            if (!success)
                return NotFound();

            return RedirectToAction("Index");
        }

        // GET: Album/Delete/5
        public IActionResult Delete(int id)
        {
            Album? album =  _context.Albums
                .Include(a => a.File)
                .Include(a => a.Songs)
                .FirstOrDefault(a => a.Id == id && !a.IsDeleted);

            if (album == null)
                return NotFound();

            return View(album); // สร้าง View Delete.cshtml
        }

        // POST: Album/DeleteConfirmed/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Album? album =  _context.Albums.Find(id);
         
            if (album == null)
                return NotFound();
            
            album.IsDeleted = true;
            album.UpdatedDate = DateTime.Now;
            album.UpdatedBy = "Ohm"; // หรือดึงจากผู้ใช้ session

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


    }
}
