using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    [Table("Purchase")]
    public partial class Purchase
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("id_show")]
        public int IdShow { get; set; }
        [Column("id_users")]
        public int IdUsers { get; set; }
        [Column("reference")]
        [StringLength(20)]
        public string Reference { get; set; } = null!;
        [Column("date_purchase", TypeName = "date")]
        public DateTime DatePurchase { get; set; }

        [ForeignKey("IdShow")]
        [InverseProperty("Purchases")]
        public virtual Show IdShowNavigation { get; set; } = null!;
        [ForeignKey("IdUsers")]
        [InverseProperty("Purchases")]
        public virtual User IdUsersNavigation { get; set; } = null!;
    }
}
