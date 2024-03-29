﻿namespace SISGED.Server
{
    public class SysgedDatabaseSettings : ISysgedDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string StorageConnectionString { get; set; }
    }

    public interface ISysgedDatabaseSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string StorageConnectionString { get; set; }
    }
}
