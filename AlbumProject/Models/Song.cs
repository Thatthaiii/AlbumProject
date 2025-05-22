using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AlbumProject.Models;

public partial class Song
{
    [Key]
    public int Id { get; set; }

    public int? AlbumId { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [StringLength(50)]
    public string? UpdatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("AlbumId")]
    [InverseProperty("Songs")]
    public virtual Album? Album { get; set; }
}
