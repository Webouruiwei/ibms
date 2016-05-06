namespace BdlIBMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Building")]
    public partial class Building
    {
        public Building()
        {
            Areas = new HashSet<Area>();
        }

        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(150)]
        public string Description { get; set; }

        public int? FloorStart { get; set; }

        public int? FloorEnd { get; set; }

        public DateTime? CreateTime { get; set; }

        [StringLength(150)]
        public string Remark { get; set; }

        public virtual ICollection<Area> Areas { get; set; }
    }
}
