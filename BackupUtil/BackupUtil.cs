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
            string primaryFolder = @"C:\Users\vradoyko\Desktop\Test\Source";
            string backupFolder = @"C:\Users\vradoyko\Desktop\Test";
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
                            //msg
                        }
                        catch (Exception)
                        {
                            //msg
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
                        //msg for sucess copy
                    }
                    catch (Exception e)
                    {
                        //err msg
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
        
    }
}
