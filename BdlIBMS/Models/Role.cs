namespace BdlIBMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Role")]
    public partial class Role
    {
        public int ID { get; set; }

        [Required]
        [StringLength(32)]
        public string UUID { get; set; }

        [Required]
        [StringLength(32)]
        public string ModuleID { get; set; }

        [Required]
        [StringLength(80)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public bool CanRead { get; set; }

        public bool CanWrite { get; set; }

        public bool? Status { get; set; }

        [StringLength(255)]
        public string Remark { get; set; }

        public virtual Module Module { get; set; }
    }
}
