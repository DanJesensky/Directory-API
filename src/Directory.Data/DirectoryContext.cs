using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Directory.Data {
    [ExcludeFromCodeCoverage]
    public partial class DirectoryContext : DbContext {
        public DirectoryContext() { }

        public DirectoryContext(DbContextOptions<DirectoryContext> options)
            : base(options) { }

        public virtual DbSet<Answer> Answer { get; set; }
        public virtual DbSet<Brother> Brother { get; set; }
        public virtual DbSet<BrotherExtracurricular> BrotherExtracurricular { get; set; }
        public virtual DbSet<BrotherMajor> BrotherMajor { get; set; }
        public virtual DbSet<BrotherMinor> BrotherMinor { get; set; }
        public virtual DbSet<BrotherPosition> BrotherPosition { get; set; }
        public virtual DbSet<Extracurricular> Extracurricular { get; set; }
        public virtual DbSet<InactiveBrother> InactiveBrother { get; set; }
        public virtual DbSet<Major> Major { get; set; }
        public virtual DbSet<Minor> Minor { get; set; }
        public virtual DbSet<Position> Position { get; set; }
        public virtual DbSet<Question> Question { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Answer>(entity => {
                entity.HasKey(e => new { e.QuestionId, e.BrotherId })
                    .HasName("PRIMARY");

                entity.ToTable("answer");

                entity.HasIndex(e => e.BrotherId)
                    .HasName("brotherID");

                entity.Property(e => e.QuestionId)
                    .HasColumnName("questionID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.BrotherId)
                    .HasColumnName("brotherID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AnswerText)
                    .IsRequired()
                    .HasColumnName("answerText")
                    .HasColumnType("varchar(2048)");

                entity.HasOne(d => d.Brother)
                    .WithMany(p => p.Answer)
                    .HasForeignKey(d => d.BrotherId)
                    .HasConstraintName("answer_ibfk_1");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.Answer)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("answer_ibfk_2");
            });

            modelBuilder.Entity<Brother>(entity => {
                entity.ToTable("brother");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.BigBrotherId)
                    .HasColumnName("bigBrotherID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ChapterDesignation)
                    .HasColumnName("chapterDesignation")
                    .HasColumnType("varchar(16)");

                entity.Property(e => e.DateInitiated)
                    .HasColumnName("dateInitiated")
                    .HasColumnType("date");

                entity.Property(e => e.DateJoined)
                    .HasColumnName("dateJoined")
                    .HasColumnType("date");

                entity.Property(e => e.ExpectedGraduation)
                    .HasColumnName("expectedGraduation")
                    .HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("firstName")
                    .HasColumnType("varchar(40)");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("lastName")
                    .HasColumnType("varchar(40)");

                entity.Property(e => e.Picture)
                    .HasColumnName("picture")
                    .HasColumnType("mediumblob");

                entity.Property(e => e.ZetaNumber)
                    .HasColumnName("zetaNumber")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<BrotherExtracurricular>(entity => {
                entity.HasKey(e => new { e.BrotherId, e.ExtracurricularId })
                    .HasName("PRIMARY");

                entity.ToTable("brother_extracurricular");

                entity.HasIndex(e => e.ExtracurricularId)
                    .HasName("extracurricularID");

                entity.Property(e => e.BrotherId)
                    .HasColumnName("brotherID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ExtracurricularId)
                    .HasColumnName("extracurricularID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Brother)
                    .WithMany(p => p.BrotherExtracurricular)
                    .HasForeignKey(d => d.BrotherId)
                    .HasConstraintName("brother_extracurricular_ibfk_1");

                entity.HasOne(d => d.Extracurricular)
                    .WithMany(p => p.BrotherExtracurricular)
                    .HasForeignKey(d => d.ExtracurricularId)
                    .HasConstraintName("brother_extracurricular_ibfk_2");
            });

            modelBuilder.Entity<BrotherMajor>(entity => {
                entity.HasKey(e => new { e.BrotherId, e.MajorId })
                    .HasName("PRIMARY");

                entity.ToTable("brother_major");

                entity.HasIndex(e => e.MajorId)
                    .HasName("majorID");

                entity.Property(e => e.BrotherId)
                    .HasColumnName("brotherID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.MajorId)
                    .HasColumnName("majorID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Brother)
                    .WithMany(p => p.BrotherMajor)
                    .HasForeignKey(d => d.BrotherId)
                    .HasConstraintName("brother_major_ibfk_1");

                entity.HasOne(d => d.Major)
                    .WithMany(p => p.BrotherMajor)
                    .HasForeignKey(d => d.MajorId)
                    .HasConstraintName("brother_major_ibfk_2");
            });

            modelBuilder.Entity<BrotherMinor>(entity => {
                entity.HasKey(e => new { e.BrotherId, e.MinorId })
                    .HasName("PRIMARY");

                entity.ToTable("brother_minor");

                entity.HasIndex(e => e.MinorId)
                    .HasName("minorID");

                entity.Property(e => e.BrotherId)
                    .HasColumnName("brotherID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.MinorId)
                    .HasColumnName("minorID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Brother)
                    .WithMany(p => p.BrotherMinor)
                    .HasForeignKey(d => d.BrotherId)
                    .HasConstraintName("brother_minor_ibfk_1");

                entity.HasOne(d => d.Minor)
                    .WithMany(p => p.BrotherMinor)
                    .HasForeignKey(d => d.MinorId)
                    .HasConstraintName("brother_minor_ibfk_2");
            });

            modelBuilder.Entity<BrotherPosition>(entity => {
                entity.HasKey(e => new { e.BrotherId, e.PositionId, e.Start, e.End })
                    .HasName("PRIMARY");

                entity.ToTable("brother_position");

                entity.HasIndex(e => e.PositionId)
                    .HasName("positionID");

                entity.Property(e => e.BrotherId)
                    .HasColumnName("brotherID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PositionId)
                    .HasColumnName("positionID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Start)
                    .HasColumnName("start")
                    .HasColumnType("date");

                entity.Property(e => e.End)
                    .HasColumnName("end")
                    .HasColumnType("date");

                entity.HasOne(d => d.Brother)
                    .WithMany(p => p.BrotherPosition)
                    .HasForeignKey(d => d.BrotherId)
                    .HasConstraintName("brother_position_ibfk_1");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.BrotherPosition)
                    .HasForeignKey(d => d.PositionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("brother_position_ibfk_2");
            });

            modelBuilder.Entity<Extracurricular>(entity => {
                entity.ToTable("extracurricular");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(200)");
            });

            modelBuilder.Entity<InactiveBrother>(entity => {
                entity.ToTable("inactive_brother");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Reason)
                    .IsRequired()
                    .HasColumnName("reason")
                    .HasColumnType("text");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.InactiveBrother)
                    .HasForeignKey<InactiveBrother>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("inactive_brother_ibfk_1");
            });

            modelBuilder.Entity<Major>(entity => {
                entity.ToTable("major");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(75)");
            });

            modelBuilder.Entity<Minor>(entity => {
                entity.ToTable("minor");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(75)");
            });

            modelBuilder.Entity<Position>(entity => {
                entity.ToTable("position");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("text");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(75)");
            });

            modelBuilder.Entity<Question>(entity => {
                entity.ToTable("question");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.QuestionText)
                    .IsRequired()
                    .HasColumnName("questionText")
                    .HasColumnType("varchar(512)");
            });
        }
    }
}
