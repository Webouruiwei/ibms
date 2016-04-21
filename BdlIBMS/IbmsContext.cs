namespace BdlIBMS
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class IbmsContext : DbContext
    {
        public IbmsContext()
            : base("name=IbmsContext1")
        {
        }

        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<LoginRecord> LoginRecords { get; set; }
        public virtual DbSet<OperationRecord> OperationRecords { get; set; }
        public virtual DbSet<Point> Points { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Point>()
                .Property(e => e.ModuleID)
                .IsFixedLength();
        }
    }
}
