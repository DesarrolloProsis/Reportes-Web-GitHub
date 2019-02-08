namespace ReportesWeb1_2.ModelsSQLServer
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ProsisSQLServerModel : DbContext
    {
        public ProsisSQLServerModel()
            : base("name=DefaultConnection")
        {
        }

        public virtual DbSet<Type_Carril> Type_Carril { get; set; }
        public virtual DbSet<Type_Delegacion> Type_Delegacion { get; set; }
        public virtual DbSet<Type_Operadores> Type_Operadores { get; set; }
        public virtual DbSet<Type_Plaza> Type_Plaza { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Type_Carril>()
                .Property(e => e.Num_Gea)
                .IsUnicode(false);

            modelBuilder.Entity<Type_Carril>()
                .Property(e => e.Num_Capufe)
                .IsUnicode(false);

            modelBuilder.Entity<Type_Carril>()
                .Property(e => e.Num_Tramo)
                .IsUnicode(false);

            modelBuilder.Entity<Type_Delegacion>()
                .Property(e => e.Num_Delegacion)
                .IsUnicode(false);

            modelBuilder.Entity<Type_Delegacion>()
                .Property(e => e.Nom_Delegacion)
                .IsUnicode(false);

            modelBuilder.Entity<Type_Delegacion>()
                .HasMany(e => e.Type_Plaza)
                .WithRequired(e => e.Type_Delegacion)
                .HasForeignKey(e => e.Delegacion_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Type_Operadores>()
                .Property(e => e.Num_Capufe)
                .IsUnicode(false);

            modelBuilder.Entity<Type_Operadores>()
                .Property(e => e.Num_Gea)
                .IsUnicode(false);

            modelBuilder.Entity<Type_Operadores>()
                .Property(e => e.Nom_Operador)
                .IsUnicode(false);

            modelBuilder.Entity<Type_Plaza>()
                .Property(e => e.Num_Plaza)
                .IsUnicode(false);

            modelBuilder.Entity<Type_Plaza>()
                .Property(e => e.Nom_Plaza)
                .IsUnicode(false);

            modelBuilder.Entity<Type_Plaza>()
                .HasMany(e => e.Type_Carril)
                .WithRequired(e => e.Type_Plaza)
                .HasForeignKey(e => e.Plaza_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Type_Plaza>()
                .HasMany(e => e.Type_Operadores)
                .WithRequired(e => e.Type_Plaza)
                .HasForeignKey(e => e.Plaza_Id)
                .WillCascadeOnDelete(false);
        }
    }
}
