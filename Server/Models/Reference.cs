using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    [Table("Reference")]
    public partial class Reference
    {
        public Reference()
        {
            Purchases = new HashSet<Purchase>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("ref")]
        [StringLength(10)]
        public string Ref { get; set; } = null!;

        [InverseProperty("ReferenceNavigation")]
        public virtual ICollection<Purchase> Purchases { get; set; }
    }
}
