using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace IdentityServer.Models
{
    public partial class CISDI_TEST20180829Context : DbContext
    {
        public IConfiguration Configuration { get; set; }
        public CISDI_TEST20180829Context()
        {
        }

        public CISDI_TEST20180829Context(DbContextOptions<CISDI_TEST20180829Context> options, IConfiguration configuration)
            : base(options)
        {
            Configuration = configuration;
        }

        public virtual DbSet<Popedom> Popedom { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(
                    Configuration["ConnectionStrings"]
                    //"Data Source=.\\SQLEXPRESS;Initial Catalog=CISDI_TEST20180829;User ID=sa;Password=dsfd;"
                    );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<Popedom>(entity =>
            {
                entity.HasKey(e => e.Userid);

                entity.ToTable("popedom");

                entity.HasIndex(e => e.LoginName)
                    .HasName("IX_popedom")
                    .IsUnique();

                entity.Property(e => e.Userid)
                    .HasColumnName("USERID")
                    .HasMaxLength(30)
                    .ValueGeneratedNever();

                entity.Property(e => e.LoginName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Macaddress)
                    .HasColumnName("MACAddress")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pm)
                    .HasColumnName("PM")
                    .HasColumnType("ntext");

                entity.Property(e => e.Popedom1)
                    .HasColumnName("POPEDOM")
                    .HasColumnType("ntext");

                entity.Property(e => e.Pws)
                    .IsRequired()
                    .HasColumnName("PWS")
                    .HasMaxLength(1000);

                entity.Property(e => e.RoleId)
                    .HasColumnName("RoleID")
                    .HasMaxLength(50);

                entity.Property(e => e.Rolename)
                    .HasColumnName("ROLENAME")
                    .HasMaxLength(50);

                entity.Property(e => e.Usergrade)
                    .HasColumnName("USERGRADE")
                    .HasMaxLength(2);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("USERNAME")
                    .HasMaxLength(20);
            });
        }
    }
}
