using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BlogPostWebApi.Models
{
    public partial class IWSTrainingContext : DbContext
    {
        public IWSTrainingContext()
        {
        }

        public IWSTrainingContext(DbContextOptions<IWSTrainingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PostComment> Comments { get; set; }
        public virtual DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("server=localhost;port=3306;user=root;password=root3306;database=iws_training", x => x.ServerVersion("8.0.19-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PostComment>(entity =>
            {
                entity.ToTable("comments");

                entity.HasIndex(e => e.PostId)
                    .HasName("post_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Comment)
                    .IsRequired()
                    .HasColumnName("comment")
                    .HasColumnType("varchar(255)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.PostId).HasColumnName("post_id");

                //entity.HasOne(d => d.Post)
                //    .WithMany(p => p.Comments)
                //    .HasForeignKey(d => d.PostId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("comments_ibfk_1");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("posts");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnName("content")
                    .HasColumnType("text")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.HasMany(e => e.Comments).WithOne()
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("comments_ibfk_1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
