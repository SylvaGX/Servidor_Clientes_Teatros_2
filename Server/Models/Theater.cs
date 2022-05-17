using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    [Table("Theater")]
    public partial class Theater
    {
        public Theater()
        {
            Shows = new HashSet<Show>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; } = null!;
        [Column("address")]
        [StringLength(50)]
        public string Address { get; set; } = null!;
        [Column("localization")]
        [StringLength(50)]
        public string Localization { get; set; } = null!;
        [Column("lat")]
        public double Lat { get; set; }
        [Column("longi")]
        public double Longi { get; set; }
        [Column("contact")]
        [StringLength(9)]
        public string Contact { get; set; } = null!;

        [InverseProperty("IdTheaterNavigation")]
        public virtual ICollection<Show> Shows { get; set; }
    }
}
