using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Reporting;

namespace ReportesWeb1_2.Views.Reportes
{
    public class ReportBookCajeroReceptorToc : Telerik.Reporting.ReportBook
    {
        public ReportBookCajeroReceptorToc()
        {
            AddTocTemplate();

            AddReports();
        }

        void AddTocTemplate()
        {
            var tocReportSource = new TypeReportSource() { TypeName = typeof(PreliquidacionCajeroReceptor).AssemblyQualifiedName };
            TocReportSource = tocReportSource;
        }

        void AddReports()
        {
            //this.ReportSources.Add(new TypeReportSource
            //{
            //    TypeName = typeof(PreliquidacionCajeroReceptorPart).AssemblyQualifiedName
            //});

            this.ReportSources.Add(new TypeReportSource
            {
                TypeName = typeof(ComparativoCajeroReceptorPart).AssemblyQualifiedName
            });
        }

        class ComparativoCajeroReceptorPart : ComparativoCajeroReceptor
        {
            public ComparativoCajeroReceptorPart()
            {
                //this.DocumentMapText = "Comparativo CR";
            }
        }
    }
}