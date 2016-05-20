namespace BdlIBMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Alarm")]
    public partial class Alarm
    {
        public int ID { get; set; }

        public int PointID { get; set; }

        [StringLength(20)]
        public string Priority { get; set; }

        [StringLength(150)]
        public string Reason { get; set; }

        public DateTime? OccurTime { get; set; }

        public bool? IsProcess { get; set; }

        [StringLength(20)]
        public string Principal { get; set; }

        [StringLength(150)]
        public string ProcessContent { get; set; }

        public bool? IsSuccess { get; set; }

        public DateTime? ProcessTime { get; set; }

        [StringLength(150)]
        public string Remark { get; set; }

        public virtual Point Point { get; set; }
    }
}
