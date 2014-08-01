using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Helpers
{
    public static class Globals
    {
        public static string ApplicationPath { get; set; }

        public static string ApplicationParameters { get; set; }

        public static void GetExamDiffPath()
        {
            ApplicationParameters = "\"{0}\" \"{1}\"";
            ApplicationPath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\PrestoSoft\ExamDiff", "ExePath", null);
        }

    }
}
