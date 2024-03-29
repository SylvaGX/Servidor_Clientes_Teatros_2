﻿using System;
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
        [Column("id_session")]
        public int IdSession { get; set; }
        [Column("id_users")]
        public int IdUsers { get; set; }
        [Column("reference")]
        public int Reference { get; set; }
        [Column("date_purchase", TypeName = "datetime")]
        public DateTime DatePurchase { get; set; }
        [Column("compra_lugares")]
        public int CompraLugares { get; set; }
        [Column("estado")]
        public int Estado { get; set; }

        [ForeignKey("IdSession")]
        [InverseProperty("Purchases")]
        public virtual Session IdSessionNavigation { get; set; } = null!;
        [ForeignKey("IdUsers")]
        [InverseProperty("Purchases")]
        public virtual User IdUsersNavigation { get; set; } = null!;
        [ForeignKey("Reference")]
        [InverseProperty("Purchases")]
        public virtual Reference ReferenceNavigation { get; set; } = null!;
    }
}
