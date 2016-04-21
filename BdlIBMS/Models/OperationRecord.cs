namespace BdlIBMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OperationRecord")]
    public partial class OperationRecord
    {
        public int ID { get; set; }

        [StringLength(80)]
        public string UserName { get; set; }

        [StringLength(50)]
        public string IP { get; set; }

        [StringLength(150)]
        public string Content { get; set; }

        public DateTime? DateTime { get; set; }

        [StringLength(100)]
        public string Result { get; set; }
    }
}
