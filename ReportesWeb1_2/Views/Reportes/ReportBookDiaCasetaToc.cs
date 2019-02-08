using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Reporting;

namespace ReportesWeb1_2.Views.Reportes
{
    public class ReportBookDiaCasetaToc : Telerik.Reporting.ReportBook
    {
        public ReportBookDiaCasetaToc()
        {
            AddTocTemplate();

            AddReports();
        }

        void AddTocTemplate()
        {
            var tocReportSource = new TypeReportSource() { TypeName = typeof(PreliquidacionDiaCaseta).AssemblyQualifiedName };
            TocReportSource = tocReportSource;
        }

        void AddReports()
        {
            this.ReportSources.Add(new TypeReportSource
            {
                TypeName = typeof(ComparativoDiaCasetaPart).AssemblyQualifiedName,
            });
        }

        class ComparativoDiaCasetaPart : ComparativoDiaCaseta
        {
            public ComparativoDiaCasetaPart()
            {

            }
        }

    }
}