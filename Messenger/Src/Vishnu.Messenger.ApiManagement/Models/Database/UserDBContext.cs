namespace Vishnu.Messenger.ApiManagement.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class UserDBContext : DbContext
    {
        public UserDBContext()
            : base("name=UserDBContext")
        {
        }

        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(e => e.UserId)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.UserName)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.DisplayName)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.EmailId)
                .IsUnicode(false);
        }
    }
}
