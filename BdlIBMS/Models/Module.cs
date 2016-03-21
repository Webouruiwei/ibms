namespace BdlIBMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Module")]
    public partial class Module
    {
        public Module()
        {
            Roles = new HashSet<Role>();
        }

        [Key]
        [StringLength(32)]
        public string UUID { get; set; }

        [Required]
        [StringLength(80)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public bool? Status { get; set; }

        [StringLength(255)]
        public string Remark { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}
