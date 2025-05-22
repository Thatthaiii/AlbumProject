using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;


namespace AlbumProject.Models
{

    public class FileMetadata
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string FileName { get; set; } = null!;

        public string FilePath { get; set; } = null!;

        [StringLength(50)]
        public DateTime CreatedDate { get; set; }
    }

    [ModelMetadataType(typeof(FileMetadata))]
    public partial class File
    {
        public static File? Create(AlbumProjectContext dbcontext, IFormFile? ifile)
        {
            if (ifile == null || ifile.Length == 0)
                return null;

            string fileName = Guid.NewGuid() + Path.GetExtension(ifile.FileName);
            string uploadDir = Path.Combine("wwwroot/uploads");
            Directory.CreateDirectory(uploadDir);

            string filePath = Path.Combine(uploadDir, fileName);
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                ifile.CopyTo(stream);
            }

            File newFile = new File
            {
                FileName = fileName,
                FilePath = "/uploads/" + fileName,
                CreatedDate = DateTime.Now
            };

            dbcontext.Files.Add(newFile);
            dbcontext.SaveChanges();

            return newFile;
        }

        public static File? MoveTempFile(string tempPath)
        {
            if (string.IsNullOrEmpty(tempPath))
                return null;

            string fileName = Path.GetFileName(tempPath);
            string tempFilePath = Path.Combine("wwwroot/uploads/tmp", fileName);
            string permFilePath = Path.Combine("wwwroot/uploads", fileName);

            if (System.IO.File.Exists(tempFilePath))
            {
                System.IO.File.Move(tempFilePath, permFilePath);

                return new File
                {
                    FileName = fileName,
                    FilePath = "/uploads/" + fileName,
                    CreatedDate = DateTime.Now
                };
            }

            return null;
        }
    }


}
