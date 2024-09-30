using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace MauiTRU.Database
{

    public static class Constants
    {
        public const string DatabaseFilename = "TRUSQLite.db3";
        public const string RefreshRateKey = "refreshrate";
        public const string OfflineModeKey = "offlinemode";
        public const bool DefaultOfflineMode = false;

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;
        public const string PreferenceKeyForAPI = "APIRoute";
        public const string LocalHostDefault = "https://localhost:7288";
        public const string ProductionDefault = "https://localhost:7288";
        public const int DefaultRefreshRate = 3;
    }
}
