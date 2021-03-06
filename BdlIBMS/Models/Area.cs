namespace BdlIBMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Area")]
    public partial class Area
    {
        public Area()
        {
            Area1 = new HashSet<Area>();
            Points = new HashSet<Point>();
        }

        public int ID { get; set; }

        public int BuildingID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(150)]
        public string Description { get; set; }

        public int? ParentID { get; set; }

        public DateTime? CreateTime { get; set; }

        [StringLength(150)]
        public string Remark { get; set; }

        public virtual ICollection<Area> Area1 { get; set; }

        public virtual Area Area2 { get; set; }

        public virtual Building Building { get; set; }

        public virtual ICollection<Point> Points { get; set; }
    }
}
