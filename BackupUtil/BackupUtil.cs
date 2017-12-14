using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BackupUtil
{
    class BackupUtil
    {
        static void Main(string[] args)
        {
            //"C:\Users\vradoyko\Desktop\Test\Source1" "C:\Users\vradoyko\Desktop\Test"

            var validator = new Validator(args);
            if (validator.IsDataValid)
            {
                if ((int)DateTime.Now.DayOfWeek == validator.WeeklyTriggerDay)
                {
                    CreateWeeklyBackup(validator.PrimaryFolder, validator.BackupFolder);
                }

                if (DateTime.Now.Day == validator.MonthlyTriggerDay)
                {
                    RemoveOldWeeklyBackups(validator.BackupFolder);
                }
            }
            else
            {
                Console.WriteLine(validator.ErrMessage);
            }
        }

        private static void RemoveOldWeeklyBackups(string backupFolder)
        {
            var filesInBackupFodler = GetFileNamesAndCreationDates(backupFolder);
            for (int month = 1; month <= 12; month++)
            {
                var filesToRemove = filesInBackupFodler.Where(f => f.Value.Month == month).OrderByDescending(f => f.Value).Skip(1).Select(f => f.Key);
                if (filesToRemove.Count() != 0)
                {
                    foreach (var file in filesToRemove)
                    {
                        try
                        {
                            File.Delete(Path.Combine(backupFolder, file));
                            WriteToOutput($"{Path.GetFileName(file)} sucessfuly deleted.");
                        }
                        catch (Exception ex)
                        {
                            WriteToOutput("Error. Cannot delete file: " + Path.GetFileName(file) + Environment.NewLine + ex.Message);
                        }
                    }
                }
            }
        }

        private static void CreateWeeklyBackup(string primaryFolder, string backupFolder)
        {
            var filesInPrimaryFodler = GetFileNamesAndCreationDates(primaryFolder);
            string todayBackupFile = filesInPrimaryFodler.OrderByDescending(f => f.Key).FirstOrDefault(f => f.Value.Date == DateTime.Now.Date).Key;

            if (todayBackupFile != null)
            {
                if (!File.Exists(Path.Combine(backupFolder, todayBackupFile)))
                {
                    try
                    {
                        File.Copy(Path.Combine(primaryFolder, todayBackupFile), Path.Combine(backupFolder, todayBackupFile));
                        WriteToOutput($"{Path.GetFileName(todayBackupFile)} sucessfuly copied.");
                    }
                    catch (Exception ex)
                    {
                        WriteToOutput("Error. Cannot copy file: " + Path.GetFileName(todayBackupFile) + Environment.NewLine + ex.Message);
                    }
                }
            }
        }

        private static Dictionary<string, DateTime> GetFileNamesAndCreationDates(string folder)
        {
            Dictionary<string, DateTime> files = new Dictionary<string, DateTime>();
            if (!Directory.Exists(folder))
            {
                //msg;
                return files;
            }

            string pattern = @"^CtrlBackup\s(\d{4}-\d{2}-\d{2}\s\d{2}\.\d{2}\.\d{2})\.zip$";
            string timeStampPattern = @"yyyy-MM-dd HH.mm.ss";

            string[] filesInFolder = Directory.GetFiles(folder);
            foreach (var file in filesInFolder)
            {
                var fileName = Path.GetFileName(file);
                var regex = new Regex(pattern);
                Match match = regex.Match(fileName);

                if (match.Success)
                {
                    var timeStamp = DateTime.ParseExact(match.Groups[1].Value, timeStampPattern, CultureInfo.InvariantCulture);
                    files.Add(fileName, timeStamp);
                }
            }
            return files;
        }

        private static void WriteToOutput(string message)
        {
            Console.WriteLine(DateTime.Now + " : " + message);
        }

    }
}
