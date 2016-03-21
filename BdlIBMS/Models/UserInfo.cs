namespace BdlIBMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserInfo")]
    public partial class UserInfo
    {
        [Key]
        [StringLength(32)]
        public string UUID { get; set; }

        [Required]
        [StringLength(40)]
        public string RealName { get; set; }

        public bool? Sex { get; set; }

        [StringLength(200)]
        public string Company { get; set; }

        [StringLength(40)]
        public string Position { get; set; }

        [StringLength(11)]
        public string Phone { get; set; }

        [StringLength(60)]
        public string Email { get; set; }

        [StringLength(255)]
        public string Address { get; set; }
    }
}
