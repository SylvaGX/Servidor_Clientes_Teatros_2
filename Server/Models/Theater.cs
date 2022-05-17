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
        [Column("id_localization")]
        public int IdLocalization { get; set; }
        [Column("contact")]
        [StringLength(9)]
        public string Contact { get; set; } = null!;

        [ForeignKey("IdLocalization")]
        [InverseProperty("Theaters")]
        public virtual Localization IdLocalizationNavigation { get; set; } = null!;
        [InverseProperty("IdTheaterNavigation")]
        public virtual ICollection<Show> Shows { get; set; }
    }
}
