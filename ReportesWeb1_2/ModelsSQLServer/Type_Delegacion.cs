namespace ReportesWeb1_2.ModelsSQLServer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Type_Delegacion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Type_Delegacion()
        {
            Type_Plaza = new HashSet<Type_Plaza>();
        }

        [Key]
        public int Id_Delegacion { get; set; }

        [StringLength(10)]
        public string Num_Delegacion { get; set; }

        [StringLength(100)]
        public string Nom_Delegacion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Type_Plaza> Type_Plaza { get; set; }
    }
}
