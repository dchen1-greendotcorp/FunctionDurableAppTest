
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionDurableAppTest
{
    public static class AppConstants
    {
        
        public const string CancellAccount_Event = "CancelAccount";

        public const string TaskExpireEvent = "TaskExpire";

        public const string ResubmitAccount_Event = "ResubmitAccount";
        public static string[] RegisterdEvents = {  CancellAccount_Event, ResubmitAccount_Event };

        public static string FirstRetryInterval = "FirstRetryInterval";
        public static string MaxNumberOfAttempts = "MaxNumofAttempts";
        public static string BackoffCoefficient = "BackoffCoefficient";

        public const string ProcessSave = "SaveAccount";
        public const string ProcessArchive = "ArchiveAccount";
        public const string ProcessNotification = "NotifyAccount";
        public static List<string> AccountProcessList = new List<string>() { ProcessSave, ProcessArchive, ProcessNotification };
    }
}
