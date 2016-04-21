namespace BdlIBMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LoginRecord")]
    public partial class LoginRecord
    {
        public int ID { get; set; }

        [StringLength(80)]
        public string UserName { get; set; }

        [StringLength(50)]
        public string IP { get; set; }

        public DateTime? DateTime { get; set; }

        [StringLength(100)]
        public string Result { get; set; }
    }
}
