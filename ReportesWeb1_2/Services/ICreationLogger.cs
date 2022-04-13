using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReportesWeb1_2.Services
{
    public interface ICreationLogger
    {
        void CreationFolders(string folderPath, string nameLog, int numberOfDays);
        void Information(string mensaje);
        void Warning(string mensaje);
        void Error(Exception ex, string mensaje);
        void Fatal(Exception ex, string mensaje);
    }
}