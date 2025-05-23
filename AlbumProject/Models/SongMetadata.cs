using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;

namespace AlbumProject.Models
{

    public class SongMetadata
    {
        [Required]
        public string Name { get; set; }

        [BindNever]
        public Album? Album { get; set; }

    }

    [ModelMetadataType(typeof(SongMetadata))]
    public partial class Song
    {

        public void Create(AlbumProjectContext dbContext, int albumId)
        {
            this.AlbumId = albumId;
            this.CreatedDate = DateTime.Now;
            this.CreatedBy = "Ohm";
            dbContext.Songs.Add(this);
        }

        public static void Create(AlbumProjectContext dbContext, List<Song> songs, int albumId)
        {
            foreach (Song song in songs)
            {
                if (song.Id == 0 && !string.IsNullOrWhiteSpace(song.Name))
                {
                    song.AlbumId = albumId;
                    song.CreatedDate = DateTime.Now;
                    dbContext.Songs.Add(song);
                }
            }
        }

        public static void Update(AlbumProjectContext dbContext, List<Song> songs)
        {

            foreach (Song song in songs)
            {
                if (song.Id > 0)
                {
                    song.UpdatedDate = DateTime.Now;
                    song.UpdatedBy = "Ohm";
                    dbContext.Songs.Update(song);
                }
            }
        }

        public static void Delete(AlbumProjectContext dbContext, string deletedSongIds)
        {
            if (string.IsNullOrWhiteSpace(deletedSongIds))
                return;

            List<int> ids = deletedSongIds
                            .Split(',')
                            .Where(id => int.TryParse(id, out _))
                            .Select(int.Parse)
                            .ToList();

            if (ids.Count == 0)
                return;

            List<Song> songsToSoftDelete = dbContext.Songs.Where(s => ids.Contains(s.Id)).ToList();

            foreach (Song song in songsToSoftDelete)
            {
                song.IsDeleted = true; 
            }
        }
    }


}
