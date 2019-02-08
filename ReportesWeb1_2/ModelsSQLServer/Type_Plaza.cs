namespace ReportesWeb1_2.ModelsSQLServer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Type_Plaza
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Type_Plaza()
        {
            Type_Carril = new HashSet<Type_Carril>();
            Type_Operadores = new HashSet<Type_Operadores>();
        }

        [Key]
        public int Id_Plaza { get; set; }

        [StringLength(10)]
        public string Num_Plaza { get; set; }

        [StringLength(100)]
        public string Nom_Plaza { get; set; }

        public int Delegacion_Id { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Type_Carril> Type_Carril { get; set; }

        public virtual Type_Delegacion Type_Delegacion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Type_Operadores> Type_Operadores { get; set; }
    }
}
