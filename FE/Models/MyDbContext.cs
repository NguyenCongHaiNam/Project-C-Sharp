using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace FE.Models
{
    public partial class MyDbContext : DbContext
    {
        public MyDbContext()
        {
        }

        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<LabelData> LabelData { get; set; }
        public virtual DbSet<Raw> Raw { get; set; }

        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<VisitCount> VisitCount { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                {
                    optionsBuilder.UseSqlServer("SERVER=H1N4M\\MSSQLSERVER01;DATABASE=NewsClassifier;UID=h1n4m;PWD=h1n4m;");
                }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LabelData>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("label_data");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnName("content");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Label)
                    .IsRequired()
                    .HasColumnName("label")
                    .HasMaxLength(20)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Raw>(entity =>
            {
                entity.ToTable("raw");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.Keywords)
                    .HasColumnName("keywords")
                    .HasMaxLength(500)
                    .IsFixedLength();

                entity.Property(e => e.PublishedDate)
                    .HasColumnName("published_date")
                    .HasMaxLength(100)
                    .IsFixedLength();

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(500)
                    .IsFixedLength();

                entity.Property(e => e.TopImg)
                    .HasColumnName("top_img")
                    .HasMaxLength(300)
                    .IsFixedLength();

                entity.Property(e => e.Url)
                    .HasColumnName("url")
                    .HasMaxLength(1000)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.IdUser)
                    .HasName("PK__Users__3717C98294906179");

                entity.Property(e => e.IdUser).HasColumnName("idUser");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });
            modelBuilder.Entity<Log>(entity =>
            {
                entity.ToTable("Log"); // Đặt tên cho bảng là "Log"

                entity.HasKey(e => e.logID).HasName("PK__Log__7839F62D023AD65F"); // Thiết lập khóa chính

                entity.Property(e => e.logID).HasColumnName("logID").ValueGeneratedOnAdd(); // Thiết lập cột logID là khóa tự động tăng

                entity.Property(e => e.idUser)
                    .IsRequired()
                    .HasColumnName("idUser"); // Thiết lập cột idUser và yêu cầu không được để trống

                entity.Property(e => e.logContent)
                    .HasColumnName("logContent"); // Thiết lập cột logContent

                entity.Property(e => e.dateTime)
                    .HasColumnName("dateTime")
                    .HasMaxLength(20)
                    .IsFixedLength(); // Thiết lập cột dateTime với độ dài tối đa là 20 và kiểu dữ liệu cố định
            });

            modelBuilder.Entity<VisitCount>(entity =>
            {
                entity.ToTable("VisitCount");

                entity.HasKey(e => e.Id); 
                entity.Property(e => e.Id)
                    .HasColumnName("Id") 
                    .ValueGeneratedOnAdd(); 

                entity.Property(e => e.Date)
                    .HasColumnName("Date") 
                    .IsRequired(); 

                entity.Property(e => e.Count)
                    .HasColumnName("Count")
                    .IsRequired(); 
            });



            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

    
}
