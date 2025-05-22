using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AlbumProject.Models;

public partial class Album
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    public int? FileId { get; set; }

    public string Description { get; set; } = null!;

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("FileId")]
    [InverseProperty("Albums")]
    public virtual File? File { get; set; }

    [InverseProperty("Album")]
    public virtual ICollection<Song> Songs { get; set; } = new List<Song>();


    [NotMapped]
    public List<int> DeletedIds { get; set; } = new List<int>();
}
