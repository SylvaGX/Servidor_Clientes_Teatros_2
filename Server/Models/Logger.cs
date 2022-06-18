using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    [Table("Logger")]
    public partial class Logger
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("level_log")]
        public int LevelLog { get; set; }
        [Column("msg", TypeName = "text")]
        public string Msg { get; set; } = null!;
        [Column("dataTime", TypeName = "datetime")]
        public DateTime DataTime { get; set; }
    }
}
