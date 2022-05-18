using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    [Table("Localization")]
    public partial class Localization
    {
        public Localization()
        {
            Theaters = new HashSet<Theater>();
            Users = new HashSet<User>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("localization")]
        [StringLength(50)]
        public string name { get; set; } = null!;
        [Column("lat")]
        public double Lat { get; set; }
        [Column("longi")]
        public double Longi { get; set; }

        [InverseProperty("IdLocalizationNavigation")]
        public virtual ICollection<Theater> Theaters { get; set; }
        [InverseProperty("IdLocalizationNavigation")]
        public virtual ICollection<User> Users { get; set; }
    }
}
