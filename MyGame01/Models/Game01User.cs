using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyGame01.Models
{
    [Table("game01_users")]
    public class Game01User
    {
        [Key]
        [Column("userid", TypeName = "char(8)")]
        [StringLength(8)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Column("passward", TypeName = "char(20)")]
        [StringLength(20)]
        public string Passward { get; set; } = string.Empty;

        [Required]
        [Column("name", TypeName = "varchar(20)")]
        [StringLength(20)]
        public string Name { get; set; } = string.Empty;

        [Column("birthyear")]
        public short BirthYear { get; set; }

        [Required]
        [Column("addr", TypeName = "char(2)")]
        [StringLength(2)]
        public string Addr { get; set; } = string.Empty;

        [Column("mobile", TypeName = "char(11)")]
        [StringLength(11)]
        public string? Mobile { get; set; }

        [Column("level")]
        public short Level { get; set; }

        [Column("health")]
        public int Health { get; set; }

        [Column("gold")]
        public int Gold { get; set; }

        [Column("mdate", TypeName = "date")]
        public DateOnly? MDate { get; set; }
    }
}
