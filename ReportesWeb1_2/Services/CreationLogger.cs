using Serilog;
using Serilog.Sinks.File.Archive;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace ReportesWeb1_2.Services
{
    public class CreationLogger : ICreationLogger
    {
        private int globalNumberOfDays;
        private string globalFolderPath;
        private string globalNameLog;
        private string globalFolderPathC;

        public void CreationFolders(string folderPath, string nameLog, int numberOfDays)
        {
            globalNumberOfDays = numberOfDays;
            globalFolderPath = folderPath;
            globalNameLog = nameLog;

            //Variable creation 
            string folderPathC = $@"{folderPath}\Logs\LogsComprimidos";

            globalFolderPathC = folderPathC;

            string year = DateTime.Now.ToString("yyyy");
            string month = DateTime.Now.ToString("MMMM").ToUpper();

            //Validation of the existence of the folders, if they do not exist, they are created
            if (!Directory.Exists($@"{folderPath}\Logs"))
            {
                Directory.CreateDirectory($@"{folderPath}\Logs");

                if (!Directory.Exists($@"{folderPath}\Logs\Log"))
                {
                    Directory.CreateDirectory($@"{folderPath}\Logs\Log");

                    if (!Directory.Exists(folderPathC))
                    {
                        Directory.CreateDirectory(folderPathC);

                        if (!Directory.Exists($@"{folderPathC}\{year}"))
                        {
                            Directory.CreateDirectory($@"{folderPathC}\{year}");

                            if (!Directory.Exists($@"{folderPathC}\{year}\{month}"))
                            {
                                Directory.CreateDirectory($@"{folderPathC}\{year}\{month}");

                                //The log is created, with the functions of rotating by day, automatically deleting every 7 days, compressing the log, all with the help of serilog
                                Log.Logger = new LoggerConfiguration()
                                               .MinimumLevel.Debug()
                                               //.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                                               //.Enrich.FromLogContext()
                                               .WriteTo.File($@"{globalFolderPath}\Logs\Log\{globalNameLog}.txt", hooks: new ArchiveHooks(CompressionLevel.Fastest, $@"{globalFolderPathC}\{year}\{month}"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: globalNumberOfDays)
                                               .CreateLogger();
                            }
                        }
                    }
                }
            }
            else if (!Directory.Exists($@"{folderPath}\Logs\Log"))
            {
                Directory.CreateDirectory($@"{folderPath}\Logs\Log");

                if (!Directory.Exists(folderPathC))
                {
                    Directory.CreateDirectory(folderPathC);

                    if (!Directory.Exists($@"{folderPathC}\{year}"))
                    {
                        Directory.CreateDirectory($@"{folderPathC}\{year}");

                        if (!Directory.Exists($@"{folderPathC}\{year}\{month}"))
                        {
                            Directory.CreateDirectory($@"{folderPathC}\{year}\{month}");

                            //The log is created, with the functions of rotating by day, automatically deleting every 7 days, compressing the log, all with the help of serilog
                            Log.Logger = new LoggerConfiguration()
                                           .MinimumLevel.Debug()
                                           //.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                                           //.Enrich.FromLogContext()
                                           .WriteTo.File($@"{globalFolderPath}\Logs\Log\{globalNameLog}.txt", hooks: new ArchiveHooks(CompressionLevel.Fastest, $@"{globalFolderPathC}\{year}\{month}"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: globalNumberOfDays)
                                           .CreateLogger();
                        }
                    }
                }
            }
            else if (!Directory.Exists(folderPathC))
            {
                Directory.CreateDirectory(folderPathC);

                if (!Directory.Exists($@"{folderPathC}\{year}"))
                {
                    Directory.CreateDirectory($@"{folderPathC}\{year}");

                    if (!Directory.Exists($@"{folderPathC}\{year}\{month}"))
                    {
                        Directory.CreateDirectory($@"{folderPathC}\{year}\{month}");

                        //The log is created, with the functions of rotating by day, automatically deleting every 7 days, compressing the log, all with the help of serilog
                        Log.Logger = new LoggerConfiguration()
                                       .MinimumLevel.Debug()
                                       //.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                                       //.Enrich.FromLogContext()
                                       .WriteTo.File($@"{globalFolderPath}\Logs\Log\{globalNameLog}.txt", hooks: new ArchiveHooks(CompressionLevel.Fastest, $@"{globalFolderPathC}\{year}\{month}"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: globalNumberOfDays)
                                       .CreateLogger();
                    }
                }
            }
            else if (!Directory.Exists($@"{folderPathC}\{year}"))
            {
                Directory.CreateDirectory($@"{folderPathC}\{year}");

                if (!Directory.Exists($@"{folderPathC}\{year}\{month}"))
                {
                    Directory.CreateDirectory($@"{folderPathC}\{year}\{month}");

                    //The log is created, with the functions of rotating by day, automatically deleting every 7 days, compressing the log, all with the help of serilog
                    Log.Logger = new LoggerConfiguration()
                                   .MinimumLevel.Debug()
                                   //.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                                   //.Enrich.FromLogContext()
                                   .WriteTo.File($@"{globalFolderPath}\Logs\Log\{globalNameLog}.txt", hooks: new ArchiveHooks(CompressionLevel.Fastest, $@"{globalFolderPathC}\{year}\{month}"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: globalNumberOfDays)
                                   .CreateLogger();
                }
            }
            else if (!Directory.Exists($@"{folderPathC}\{year}\{month}"))
            {
                Directory.CreateDirectory($@"{folderPathC}\{year}\{month}");

                //The log is created, with the functions of rotating by day, automatically deleting every 7 days, compressing the log, all with the help of serilog
                Log.Logger = new LoggerConfiguration()
                               .MinimumLevel.Debug()
                               //.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                               //.Enrich.FromLogContext()
                               .WriteTo.File($@"{globalFolderPath}\Logs\Log\{globalNameLog}.txt", hooks: new ArchiveHooks(CompressionLevel.Fastest, $@"{globalFolderPathC}\{year}\{month}"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: globalNumberOfDays)
                               .CreateLogger();
            }
            else
            {
                //The log is created, with the functions of rotating by day, automatically deleting every 7 days, compressing the log, all with the help of serilog
                Log.Logger = new LoggerConfiguration()
                               .MinimumLevel.Debug()
                               //.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                               //.Enrich.FromLogContext()
                               .WriteTo.File($@"{globalFolderPath}\Logs\Log\{globalNameLog}.txt", hooks: new ArchiveHooks(CompressionLevel.Fastest, $@"{globalFolderPathC}\{year}\{month}"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: globalNumberOfDays)
                               .CreateLogger();
            }
        }

        public void Information(string menssage)
        {
            EditLastLine(menssage, "Information");
        }
        public void Warning(string menssage)
        {
            EditLastLine(menssage, "Warning");
        }
        public void Error(Exception ex, string menssage)
        {
            EditLastLine(menssage, "Error", ex);
        }
        public void Fatal(Exception ex, string menssage)
        {
            EditLastLine(menssage, "Fatal", ex);
        }

        public void CallItAfterClosingIt()
        {
            string year = DateTime.Now.ToString("yyyy");
            string month = DateTime.Now.ToString("MMMM").ToUpper();
            //The log is created, with the functions of rotating by day, automatically deleting every 7 days, compressing the log, all with the help of serilog
            Log.Logger = new LoggerConfiguration()
                           .MinimumLevel.Debug()
                           //.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                           //.Enrich.FromLogContext()
                           .WriteTo.File($@"{globalFolderPath}\Logs\Log\{globalNameLog}.txt", hooks: new ArchiveHooks(CompressionLevel.Fastest, $@"{globalFolderPathC}\{year}\{month}"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: globalNumberOfDays)
                           .CreateLogger();
        }
        public string ErrorLine(Exception ex)
        {
            string exMessage = ex.ToString();
            int index = exMessage.IndexOf("line ");
            string line = exMessage.Substring(index + 5);
            return line;
        }

        public void EditLastLine(string menssage, string nameLevel, Exception ex = null)
        {
            //Variable creation 
            string day = DateTime.Now.ToString("dd");
            string month = DateTime.Now.ToString("MM");
            string year = DateTime.Now.ToString("yyyy");

            //The log is created, with the functions of rotating by day, automatically deleting every 7 days, compressing the log, all with the help of serilog
            Log.Logger = new LoggerConfiguration()
                           .MinimumLevel.Debug()
                           //.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                           //.Enrich.FromLogContext()
                           .WriteTo.File($@"{globalFolderPath}\Logs\Log\{globalNameLog}.txt", hooks: new ArchiveHooks(CompressionLevel.Fastest, $@"{globalFolderPathC}\{year}\{month}"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: globalNumberOfDays)
                           .CreateLogger();

            switch (nameLevel)
            {
                case "Information":

                    if (File.Exists($@"{globalFolderPath}\Logs\Log\{globalNameLog}{year}{month}{day}.txt"))
                    {
                        string[] lines2 = File.ReadAllLines($@"{globalFolderPath}\Logs\Log\{globalNameLog}{year}{month}{day}.txt");
                        Log.Information(menssage);
                        Log.CloseAndFlush();

                        string[] lines = File.ReadAllLines($@"{globalFolderPath}\Logs\Log\{globalNameLog}{year}{month}{day}.txt");
                        string subMenssage = lines[lines2.Length].Substring(11, 12);
                        List<string> line = new List<string>();
                        int j = 0;

                        for (int i = 0; i < lines.Length; i++)
                        {
                            if (lines[i].Contains(subMenssage))
                            {
                                line.Add(lines[i]);
                                j++;
                            }
                            else if (j != 0)
                            {
                                line.Add(lines[i]);
                                j++;
                            }
                        }

                        for (int i = 0; i < line.Count; i++)
                        {
                            lines[0] = $"{line[i]}.\n{lines[0]}";
                        }

                        StreamWriter temporary = new StreamWriter($@"{globalFolderPath}\Logs\Log\" + "temporary.txt", true);

                        for (int i = 0; i < lines.Length - line.Count; i++)
                        {
                            temporary.WriteLine(lines[i]);
                        }

                        temporary.Flush();
                        temporary.Close();

                        //Finally the old file is deleted and a new one is created
                        File.Delete($@"{globalFolderPath}\Logs\Log\{globalNameLog}{year}{month}{day}.txt");
                        File.Move($@"{globalFolderPath}\Logs\Log\" + "temporary.txt", $@"{globalFolderPath}\Logs\Log\{globalNameLog}{year}{month}{day}.txt");

                        CallItAfterClosingIt();
                    }
                    else
                    {
                        Log.Information(menssage);
                        Log.CloseAndFlush();
                    }

                    break;

                case "Warning":

                    if (File.Exists($@"{globalFolderPath}\Logs\Log\{globalNameLog}{year}{month}{day}.txt"))
                    {
                        string[] lines2 = File.ReadAllLines($@"{globalFolderPath}\Logs\Log\{globalNameLog}{year}{month}{day}.txt");
                        Log.Warning(menssage);
                        Log.CloseAndFlush();

                        string[] lines = File.ReadAllLines($@"{globalFolderPath}\Logs\Log\{globalNameLog}{year}{month}{day}.txt");
                        string subMenssage = lines[lines2.Length].Substring(11, 12);
                        List<string> line = new List<string>();
                        int j = 0;

                        for (int i = 0; i < lines.Length; i++)
                        {
                            if (lines[i].Contains(subMenssage))
                            {
                                line.Add(lines[i]);
                                j++;
                            }
                            else if (j != 0)
                            {
                                line.Add(lines[i]);
                                j++;
                            }
                        }

                        for (int i = 0; i < line.Count; i++)
                        {
                            lines[0] = $"{line[i]}.\n{lines[0]}";
                        }

                        StreamWriter temporary = new StreamWriter($@"{globalFolderPath}\Logs\Log\" + "temporary.txt", true);

                        for (int i = 0; i < lines.Length - line.Count; i++)
                        {
                            temporary.WriteLine(lines[i]);
                        }

                        temporary.Flush();
                        temporary.Close();

                        //Finally the old file is deleted and a new one is created
                        File.Delete($@"{globalFolderPath}\Logs\Log\{globalNameLog}{year}{month}{day}.txt");
                        File.Move($@"{globalFolderPath}\Logs\Log\" + "temporary.txt", $@"{globalFolderPath}\Logs\Log\{globalNameLog}{year}{month}{day}.txt");

                        CallItAfterClosingIt();
                    }
                    else
                    {
                        Log.Warning(menssage);
                        Log.CloseAndFlush();
                    }

                    break;

                case "Error":

                    if (File.Exists($@"{globalFolderPath}\Logs\Log\{globalNameLog}{year}{month}{day}.txt"))
                    {
                        string[] lines2 = File.ReadAllLines($@"{globalFolderPath}\Logs\Log\{globalNameLog}{year}{month}{day}.txt");
                        string lineError = ErrorLine(ex);
                        Log.Error($"Error on the line: {lineError} {menssage}");
                        Log.CloseAndFlush();

                        string[] lines = File.ReadAllLines($@"{globalFolderPath}\Logs\Log\{globalNameLog}{year}{month}{day}.txt");
                        string subMenssage = lines[lines2.Length].Substring(11, 12);
                        List<string> line = new List<string>();
                        int j = 0;

                        for (int i = 0; i < lines.Length; i++)
                        {
                            if (lines[i].Contains(subMenssage))
                            {
                                line.Add(lines[i]);
                                j++;
                            }
                            else if (j != 0)
                            {
                                line.Add(lines[i]);
                                j++;
                            }
                        }

                        for (int i = 0; i < line.Count; i++)
                        {
                            lines[0] = $"{line[i]}.\n{lines[0]}";
                        }

                        StreamWriter temporary = new StreamWriter($@"{globalFolderPath}\Logs\Log\" + "temporary.txt", true);

                        for (int i = 0; i < lines.Length - line.Count; i++)
                        {
                            temporary.WriteLine(lines[i]);
                        }

                        temporary.Flush();
                        temporary.Close();

                        //Finally the old file is deleted and a new one is created
                        File.Delete($@"{globalFolderPath}\Logs\Log\{globalNameLog}{year}{month}{day}.txt");
                        File.Move($@"{globalFolderPath}\Logs\Log\" + "temporary.txt", $@"{globalFolderPath}\Logs\Log\{globalNameLog}{year}{month}{day}.txt");

                        CallItAfterClosingIt();
                    }
                    else
                    {
                        string lineError = ErrorLine(ex);
                        Log.Error($"Error on the line: {lineError} {menssage}");
                        Log.CloseAndFlush();
                    }

                    break;

                case "Fatal":

                    if (File.Exists($@"{globalFolderPath}\Logs\Log\{globalNameLog}{year}{month}{day}.txt"))
                    {
                        string[] lines2 = File.ReadAllLines($@"{globalFolderPath}\Logs\Log\{globalNameLog}{year}{month}{day}.txt");
                        string lineError = ErrorLine(ex);
                        Log.Fatal($"Error fatal on the line: {lineError} {menssage}");
                        Log.CloseAndFlush();

                        string[] lines = File.ReadAllLines($@"{globalFolderPath}\Logs\Log\{globalNameLog}{year}{month}{day}.txt");
                        string subMenssage = lines[lines2.Length].Substring(11, 12);
                        List<string> line = new List<string>();
                        int j = 0;

                        for (int i = 0; i < lines.Length; i++)
                        {
                            if (lines[i].Contains(subMenssage))
                            {
                                line.Add(lines[i]);
                                j++;
                            }
                            else if (j != 0)
                            {
                                line.Add(lines[i]);
                                j++;
                            }
                        }

                        for (int i = 0; i < line.Count; i++)
                        {
                            lines[0] = $"{line[i]}.\n{lines[0]}";
                        }

                        StreamWriter temporary = new StreamWriter($@"{globalFolderPath}\Logs\Log\" + "temporary.txt", true);

                        for (int i = 0; i < lines.Length - line.Count; i++)
                        {
                            temporary.WriteLine(lines[i]);
                        }

                        temporary.Flush();
                        temporary.Close();

                        //Finally the old file is deleted and a new one is created
                        File.Delete($@"{globalFolderPath}\Logs\Log\{globalNameLog}{year}{month}{day}.txt");
                        File.Move($@"{globalFolderPath}\Logs\Log\" + "temporary.txt", $@"{globalFolderPath}\Logs\Log\{globalNameLog}{year}{month}{day}.txt");

                        CallItAfterClosingIt();
                    }
                    else
                    {
                        string lineError = ErrorLine(ex);
                        Log.Fatal($"Error fatal on the line: {lineError} {menssage}");
                        Log.CloseAndFlush();
                    }

                    break;
            }
        }
    }
}