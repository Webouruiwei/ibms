namespace BdlIBMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User")]
    public partial class User
    {
        [Key]
        [StringLength(32)]
        public string UUID { get; set; }

        [Required]
        [StringLength(32)]
        public string RoleID { get; set; }

        [Required]
        [StringLength(80)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        [StringLength(255)]
        public string Remark { get; set; }
    }
}
