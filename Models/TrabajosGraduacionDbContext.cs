using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CatalogoDeTrabajosDeGraduacion.Models
{
    public partial class TrabajosGraduacionDbContext : DbContext
    {
        public TrabajosGraduacionDbContext()
        {
        }

        public TrabajosGraduacionDbContext(DbContextOptions<TrabajosGraduacionDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Autor> Autores { get; set; }
        public virtual DbSet<Carrera> Carreras { get; set; }
        public virtual DbSet<Facultad> Facultades { get; set; }
        public virtual DbSet<TipoTrabajo> TiposTrabajos { get; set; }
        public virtual DbSet<Trabajo> Trabajos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Autor>(entity =>
            {
                entity.ToTable("AUTOR");

                entity.Property(e => e.AutorId).HasColumnName("AUTOR_ID");

                entity.Property(e => e.AutorApellido)
                    .IsRequired()
                    .HasColumnName("AUTOR_APELLIDO")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AutorNombre)
                    .IsRequired()
                    .HasColumnName("AUTOR_NOMBRE")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CarreId).HasColumnName("CARRE_ID");

                entity.Property(e => e.TrabaId).HasColumnName("TRABA_ID");

                entity.HasOne(d => d.Carre)
                    .WithMany(p => p.Autor)
                    .HasForeignKey(d => d.CarreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__AUTOR__CARRE_ID__6B24EA82");

                entity.HasOne(d => d.Traba)
                    .WithMany(p => p.Autores)
                    .HasForeignKey(d => d.TrabaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__AUTOR__CARRE_ID__6A30C649");
            });

            modelBuilder.Entity<Carrera>(entity =>
            {
                entity.HasKey(e => e.CarreId)
                    .HasName("PK__CARRERA__5877B85F0AC12B9B");

                entity.ToTable("CARRERA");

                entity.HasIndex(e => e.CarreNombre)
                    .HasName("UQ__CARRERA__A6FED8C532562FEF")
                    .IsUnique();

                entity.Property(e => e.CarreId).HasColumnName("CARRE_ID");

                entity.Property(e => e.CarreDescripcion)
                    .IsRequired()
                    .HasColumnName("CARRE_DESCRIPCION")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.CarreNombre)
                    .IsRequired()
                    .HasColumnName("CARRE_NOMBRE")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FaculId).HasColumnName("FACUL_ID");

                entity.HasOne(d => d.Facul)
                    .WithMany(p => p.Carrera)
                    .HasForeignKey(d => d.FaculId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CARRERA__FACUL_I__6754599E");
            });

            modelBuilder.Entity<Facultad>(entity =>
            {
                entity.HasKey(e => e.FaculId)
                    .HasName("PK__FACULTAD__0815BD56D6996C11");

                entity.ToTable("FACULTAD");

                entity.HasIndex(e => e.FaculNombre)
                    .HasName("UQ__FACULTAD__A15700B984B880B1")
                    .IsUnique();

                entity.Property(e => e.FaculId).HasColumnName("FACUL_ID");

                entity.Property(e => e.FaculDescripcion)
                    .IsRequired()
                    .HasColumnName("FACUL_DESCRIPCION")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.FaculNombre)
                    .IsRequired()
                    .HasColumnName("FACUL_NOMBRE")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoTrabajo>(entity =>
            {
                entity.HasKey(e => e.TpTrabaId)
                    .HasName("PK__TIPO_TRA__FF5EA4CC72BA3C22");

                entity.ToTable("TIPO_TRABAJO");

                entity.HasIndex(e => e.TpTrabaNombre)
                    .HasName("UQ__TIPO_TRA__7F7BD41A2730957E")
                    .IsUnique();

                entity.Property(e => e.TpTrabaId).HasColumnName("TP_TRABA_ID");

                entity.Property(e => e.TpTrabaDescripcion)
                    .IsRequired()
                    .HasColumnName("TP_TRABA_DESCRIPCION")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TpTrabaNombre)
                    .IsRequired()
                    .HasColumnName("TP_TRABA_NOMBRE")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Trabajo>(entity =>
            {
                entity.HasKey(e => e.TrabaId)
                    .HasName("PK__TRABAJO__8576A3323CC8C5AE");                

                entity.ToTable("TRABAJO");

                entity.HasIndex(e => e.TrabaTitulo)
                    .HasName("UQ__TRABAJO__C42F2FA6202F3FDD")
                    .IsUnique();

                entity.Property(e => e.TrabaFile)
                .HasColumnName("TRABA_FILE")
                .HasMaxLength(50)
                .IsUnicode(false);

                entity.Property(e => e.TrabaId).HasColumnName("TRABA_ID");

                entity.Property(e => e.TpTrabaId).HasColumnName("TP_TRABA_ID");

                entity.Property(e => e.TrabaFecha)
                    .HasColumnName("TRABA_FECHA")
                    .HasColumnType("date");

                entity.Property(e => e.TrabaTitulo)
                    .IsRequired()
                    .HasColumnName("TRABA_TITULO")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.TpTraba)
                    .WithMany(p => p.Trabajo)
                    .HasForeignKey(d => d.TpTrabaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TRABAJO__TP_TRAB__60A75C0F");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
