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
                // Nolasām connection string no C:\Temp\ConnS.txt
                // Piemērs faila saturam: Data Source=C:\Temp\myTestDB.db
                return File.ReadAllText(@"C:\Temp\ConnS.txt").Trim();
            }
            catch (Exception ex)
            {
                throw new Exception("Neizdevās nolasīt connection string no C:\\Temp\\ConnS.txt", ex);
            }
        }
    }
}
