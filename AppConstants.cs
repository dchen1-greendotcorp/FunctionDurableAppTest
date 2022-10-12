using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionDurableAppTest
{
    public static class AppConstants
    {
        public const string UserAction_Event = "UserAction";
        public const string CancellAccount_Event = "CancelAccount";

        public const string TaskExpireEvent = "TaskExpire";

        public const string ResubmitAccount_Event = "ResubmitAccount";
        public static string[] RegisterdEvents = { UserAction_Event, CancellAccount_Event, ResubmitAccount_Event };

        public static string FirstRetryInterval = "FirstRetryInterval";

        public static string MaxNumberOfAttempts = "MaxNumberOfAttempts";
        public static string BackoffCoefficient = "BackoffCoefficient";

        public const string ProcessSave = "Save";
        public const string ProcessArchive = "Archive";
        public const string ProcessNotification = "Notification";
        public static List<string> AccountProcessList = new List<string>() { ProcessSave, ProcessArchive, ProcessNotification };
    }
}
