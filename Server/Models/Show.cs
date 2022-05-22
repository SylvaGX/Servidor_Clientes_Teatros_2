using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    [Table("Show")]
    public partial class Show
    {
        public Show()
        {
            Sessions = new HashSet<Session>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; } = null!;
        [Column("sinopse")]
        [StringLength(150)]
        public string Sinopse { get; set; } = null!;
        [Column("id_theater")]
        public int IdTheater { get; set; }
        [Column("startDate", TypeName = "date")]
        public DateTime StartDate { get; set; }
        [Column("endDate", TypeName = "date")]
        public DateTime EndDate { get; set; }
        [Column("price", TypeName = "money")]
        public decimal Price { get; set; }

        [ForeignKey("IdTheater")]
        [InverseProperty("Shows")]
        public virtual Theater IdTheaterNavigation { get; set; } = null!;
        [InverseProperty("IdShowNavigation")]
        public virtual ICollection<Session> Sessions { get; set; }
    }
}
