using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    public partial class User
    {
        public User()
        {
            Purchases = new HashSet<Purchase>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; } = null!;
        [Column("pass")]
        [StringLength(50)]
        public string Pass { get; set; } = null!;
        [Column("type")]
        [StringLength(1)]
        [Unicode(false)]
        public string Type { get; set; } = null!;
        [Column("mail")]
        [StringLength(50)]
        public string Mail { get; set; } = null!;
        [Column("id_localization")]
        public int IdLocalization { get; set; }
        [Column("fundos", TypeName = "money")]
        public decimal Fundos { get; set; }

        [ForeignKey("IdLocalization")]
        [InverseProperty("Users")]
        public virtual Localization IdLocalizationNavigation { get; set; } = null!;
        [InverseProperty("IdUsersNavigation")]
        public virtual ICollection<Purchase> Purchases { get; set; }
    }
}
