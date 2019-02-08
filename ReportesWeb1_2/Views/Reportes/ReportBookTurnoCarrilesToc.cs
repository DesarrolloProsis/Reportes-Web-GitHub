using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Reporting;

namespace ReportesWeb1_2.Views.Reportes
{
    public class ReportBookTurnoCarrilesToc : Telerik.Reporting.ReportBook
    {
        public ReportBookTurnoCarrilesToc()
        {
            AddTocTemplate();

            AddReports();
        }

        void AddTocTemplate()
        {
            var tocReportSource = new TypeReportSource() { TypeName = typeof(PreliquidacionTurnoCarriles).AssemblyQualifiedName };
            TocReportSource = tocReportSource;
        }

        void AddReports()
        {
            this.ReportSources.Add(new TypeReportSource
            {
                TypeName = typeof(ComparativoTurnoCarrilesPart).AssemblyQualifiedName,
            });
        }

        class ComparativoTurnoCarrilesPart : ComparativoTurnoCarriles
        {
            public ComparativoTurnoCarrilesPart()
            {

            }
        }

    }
}