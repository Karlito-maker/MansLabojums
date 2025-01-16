/******************************************************
 *  MansLabojums/Helpers/ConfigHelper.cs
 ******************************************************/
using System;
using System.IO;

namespace MansLabojums.Helpers
{
    public static class ConfigHelper
    {
        public static string GetConnectionString()
        {
            try
            {
                // Izlasa connection string no C:\Temp\ConnS.txt
                string path = @"C:\Temp\ConnS.txt";
                return File.ReadAllText(path).Trim();
            }
            catch (Exception ex)
            {
                throw new Exception($"Neizdevās nolasīt connection string no {@"C:\Temp\ConnS.txt"}", ex);
            }
        }
    }
}
