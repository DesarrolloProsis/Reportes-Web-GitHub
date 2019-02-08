namespace ReportesWeb1_2.ModelsSQLServer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Type_Operadores
    {
        [Key]
        public int Id_Operador { get; set; }

        [StringLength(20)]
        public string Num_Capufe { get; set; }

        [StringLength(20)]
        public string Num_Gea { get; set; }

        [StringLength(150)]
        public string Nom_Operador { get; set; }

        public int Plaza_Id { get; set; }

        public virtual Type_Plaza Type_Plaza { get; set; }
    }
}
