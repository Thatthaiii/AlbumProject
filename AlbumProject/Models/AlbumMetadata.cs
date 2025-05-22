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

            // เก็บเพลงแยกไว้ก่อน insert
            List<Song> tempSongs = this.Songs.ToList();

            // ล้าง Songs เพื่อไม่ให้ EF พยายาม Insert ซ้ำ
            this.Songs = null;

            dbContext.Albums.Add(this);
            dbContext.SaveChanges();

            // เพิ่มเพลง
            foreach (Song song in tempSongs)
            {
                if (!string.IsNullOrWhiteSpace(song.Name))
                {
                    song.Create(dbContext, this.Id);
                }
            }

            dbContext.SaveChanges(); // บันทึกเพลง

            return true;
        }



        public bool Update(AlbumProjectContext dbContext,IFormFile? newCoverFile,List<Song> newSongs,List<int> deletedSongIds)
        {
            // ดึงข้อมูลจากฐานข้อมูล
            Album? existingAlbum = dbContext.Albums
                .Include(a => a.Songs)
                .Include(a => a.File)
                .FirstOrDefault(a => a.Id == this.Id && !a.IsDeleted);

            if (existingAlbum == null)
                return false;

            // อัปเดตข้อมูลอัลบั้ม
            existingAlbum.Name = this.Name;
            existingAlbum.Description = this.Description;
            existingAlbum.UpdatedDate = DateTime.Now;
            existingAlbum.UpdatedBy = "Ohm";

            // หากมีการอัปโหลดไฟล์ใหม่
            if (newCoverFile != null && newCoverFile.Length > 0)
            {
                File? uploadedFile = File.Create(dbContext, newCoverFile);
                if (uploadedFile != null)
                {
                    existingAlbum.FileId = uploadedFile.Id;
                }
            }

            // เพิ่มเพลงใหม่
            Song.Create(dbContext, newSongs, existingAlbum.Id);

            // ลบเพลงแบบ Soft Delete
            if (deletedSongIds != null && deletedSongIds.Any())
            {
                string idsString = string.Join(",", deletedSongIds);
                Song.Delete(dbContext, idsString); // <-- Soft delete ที่นี่
            }

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
