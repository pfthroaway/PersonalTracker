using System;
using System.IO;

namespace PersonalTracker
{
    public static class AppData
    {
        internal static string Location = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PF Software", "PersonalTracker");
    }
}