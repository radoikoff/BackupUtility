using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupUtil
{
    class Validator
    {
        public Validator(string[] cmdArgs)
        {
            Validate(cmdArgs);
        }

        private StringBuilder sb = new StringBuilder();
        private string primaryFolder;
        private string backupFolder;
        private int weeklyTrigerDay = 7;
        private int monthlyTriggerDay = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
        private bool isDataValid = true;
        private string errMessage;

        public string ErrMessage
        {
            get { return errMessage; }
        }

        public bool IsDataValid
        {
            get { return isDataValid; }
        }

        public int MonthlyTriggerDay
        {
            get { return monthlyTriggerDay; }
        }

        public int WeeklyTriggerDay
        {
            get { return weeklyTrigerDay; }
        }

        public string BackupFolder
        {
            get { return backupFolder; }
        }

        public string PrimaryFolder
        {
            get { return primaryFolder; }
        }

        private void Validate(string[] cmdArgs)
        {

            if (cmdArgs.Count() == 1 && cmdArgs[0] == "/?")
            {
                sb.AppendLine("Hints section");
                this.isDataValid = false;
            }

            if (cmdArgs.Count() >= 2 && cmdArgs.Count() <= 4)
            {
                ValidateFolderPath(cmdArgs[0], "Weekly");
                ValidateFolderPath(cmdArgs[1], "Monthly");

                if (cmdArgs.Count() >= 3)
                {
                    ValidateTrigerDay(cmdArgs[2], "Weekly");
                }

                if (cmdArgs.Count() == 4)
                {
                    ValidateTrigerDay(cmdArgs[3], "Monthly");
                }
            }
            else
            {
                this.isDataValid = false;
                sb.AppendLine("Argument number is not correct");
            }

            this.errMessage = sb.ToString();
        }

        private void ValidateTrigerDay(string dayAsString, string triggerType)
        {
            int day = 0;
            bool parseResult = int.TryParse(dayAsString, out day);

            switch (triggerType)
            {
                case "Weekly":
                    if (parseResult && day >= 1 && day <= 7)
                    {
                        this.weeklyTrigerDay = day;
                    }
                    else
                    {
                        sb.AppendLine($"Provided weekly trigger day is not valid");
                        this.isDataValid = false;
                    }
                    break;
                case "Monthly":
                    if (parseResult && day >= 1 && day <= 31)
                    {
                        this.monthlyTriggerDay = day;
                    }
                    else
                    {
                        sb.AppendLine($"Provided monthly trigger day is not valid");
                        this.isDataValid = false;
                    }
                    break;
                default:
                    sb.AppendLine($"Provided monthly trigger day is not valid");
                    this.isDataValid = false;
                    break;
            }

        }

        private void ValidateFolderPath(string folderName, string folderType)
        {
            if (Directory.Exists(folderName))
            {
                if (folderType == "Weekly")
                {
                    this.primaryFolder = folderName;
                }
                else if (folderType == "Monthly")
                {
                    this.backupFolder = folderName;
                }
                else
                {
                    this.isDataValid = false;
                }
            }
            else
            {
                sb.AppendLine($"{folderType} backup folder is either not exisit or the path is not valid");
                this.isDataValid = false;
            }
        }


    }
}
