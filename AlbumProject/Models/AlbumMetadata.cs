using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AlbumProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlbumProject.Models
{
    public class AlbumMetadata
    {

    }


    //[MetadataType(typeof(ItemMetadata))] อย่าไปเขียนงี้ .Net Core 9 แม่งกวนตีน
    [ModelMetadataType(typeof(AlbumMetadata))]
    public partial class Album
    {


        [NotMapped]
        [Required(ErrorMessage = "The Cover Photo field is required.")]
        [DataType(DataType.Upload)]
        public IFormFile CoverPhoto { get; set; } = null!;

        public List<Album> GetAll(AlbumProjectContext dbContext)
        {
            return dbContext.Albums.Where(q => q.IsDeleted != true).ToList();
        }





        public bool Create(AlbumProjectContext dbContext, IFormFile? ifile)
        {
            
            //  สร้างไฟล์ (หากมี)
            File? uploadedFile = null;
            if (ifile != null && ifile.Length > 0)
            {
                uploadedFile =  File.Create(dbContext, ifile);
                if (uploadedFile != null)
                    this.File = uploadedFile;

                //dbContext.Files.Add(uploadedFile);
            }

            // Validate: ต้องมีเพลงอย่างน้อย 1 เพลงที่ชื่อไม่ว่าง
            if (Songs == null || !Songs.Any(s => !string.IsNullOrWhiteSpace(s.Name)))
            {
                return false;
            }

            // เตรียมข้อมูลอัลบั้ม
            this.IsDeleted = false;
            this.CreatedBy = "Ohm";
            this.CreatedDate = DateTime.Now;
            this.UpdatedBy = "Ohm";
            this.UpdatedDate = DateTime.Now;





            foreach (Song song in this.Songs)
            {
                if (!string.IsNullOrWhiteSpace(song.Name))
                {

                    song.CreatedDate = DateTime.Now;
                    song.CreatedBy = "Ohm";

                }
            }

            dbContext.Albums.Add(this);

            dbContext.SaveChanges(); // บันทึกเพลง

            return true;
        }



        public bool Update(AlbumProjectContext dbContext,IFormFile? newCoverFile)
        {


            this.UpdatedDate = DateTime.Now;
            this.UpdatedBy = "Ohm";

            // หากมีการอัปโหลดไฟล์ใหม่
            if (newCoverFile != null && newCoverFile.Length > 0)
            {
                File? uploadedFile = File.Create(dbContext, newCoverFile);
                if (uploadedFile != null)
                {
                    this.FileId = uploadedFile.Id;
                }
            }




            foreach (Song song in this.Songs)
            {
                if (song.Id > 0 && !string.IsNullOrWhiteSpace(song.Name))
                {
                    song.UpdatedDate = DateTime.Now;
                    song.UpdatedBy = "Ohm";
                }

                if(song.Id == 0)
                {
                    song.CreatedDate = DateTime.Now;
                    song.CreatedBy = "Ohm";
                }

            }

            // ลบเพลงแบบ Soft Delete
            //if (deletedSongIds != null && deletedSongIds.Any())
            //{
            //    string idsString = string.Join(",", deletedSongIds);
            //    Song.Delete(dbContext, idsString); // <-- Soft delete ที่นี่
            //}
            dbContext.Albums.Update(this);

            dbContext.SaveChanges();
            return true;
        }

        public bool Delete(AlbumProjectContext dbContext)
        {
            IsDeleted = true;
            UpdatedBy = "John";
            UpdatedDate = DateTime.Now;
            dbContext.Albums.Update(this);
            dbContext.SaveChanges();

            return true;
        }
        
        public Album GetById(AlbumProjectContext dbContext, int id)
        {
            Album? album = dbContext.Albums
                                          .Include(i => i.Songs)
                                          .FirstOrDefault(q => q.IsDeleted != true && q.Id == id);
            return album;
        }

    }
}
