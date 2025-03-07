namespace Dapper.API.Models.AppSettings
{
    public class ConnectionStrings
    {
        public string MSSQLConnectionRemote { get; set; } = string.Empty;
        public string MSSQLConnectionLocal { get; set; } = string.Empty;
        public string PostgreSQL { get; set; } = string.Empty;
    }
}
