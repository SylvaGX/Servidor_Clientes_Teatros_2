using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    [Table("Session")]
    public partial class Session
    {
        public Session()
        {
            Purchases = new HashSet<Purchase>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("id_show")]
        public int IdShow { get; set; }
        [Column("sessionDate", TypeName = "date")]
        public DateTime SessionDate { get; set; }
        [Column("startHour")]
        public TimeSpan StartHour { get; set; }
        [Column("endHour")]
        public TimeSpan EndHour { get; set; }
        [Column("avaiable_places")]
        public int AvaiablePlaces { get; set; }
        [Column("total_places")]
        public int TotalPlaces { get; set; }

        [ForeignKey("IdShow")]
        [InverseProperty("Sessions")]
        public virtual Show IdShowNavigation { get; set; } = null!;
        [InverseProperty("IdSessionNavigation")]
        public virtual ICollection<Purchase> Purchases { get; set; }
    }
}
