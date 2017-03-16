namespace DBSchemaComparator.Domain.Models.General
{
   public class DatabaseConnection
    {
        public string DbName { get; set; }
        public string IpAddress { get; set; }
        public string Username { get; set; }
        public string Pass { get; set; }
        public string DbType { get; set; }
        public uint Timeout { get; set; }
        public string Port { get; set; }
    }
}
