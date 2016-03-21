namespace BdlIBMS.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class IbmsContext : DbContext
    {
        public IbmsContext()
            : base("name=IbmsContext")
        {
        }

        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserInfo> UserInfoes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Module>()
                .Property(e => e.UUID)
                .IsFixedLength();

            modelBuilder.Entity<Module>()
                .HasMany(e => e.Roles)
                .WithRequired(e => e.Module)
                .HasForeignKey(e => e.ModuleID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Role>()
                .Property(e => e.UUID)
                .IsFixedLength();

            modelBuilder.Entity<Role>()
                .Property(e => e.ModuleID)
                .IsFixedLength();

            modelBuilder.Entity<User>()
                .Property(e => e.UUID)
                .IsFixedLength();

            modelBuilder.Entity<User>()
                .Property(e => e.RoleID)
                .IsFixedLength();

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.UUID)
                .IsFixedLength();

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.Phone)
                .IsFixedLength();
        }
    }
}
