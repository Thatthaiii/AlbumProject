using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AlbumProject.Models;

[Table("File")]
public partial class File
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [StringLength(50)]
    public string? UpdatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    [InverseProperty("File")]
    public virtual ICollection<Album> Albums { get; set; } = new List<Album>();
}
