using Microsoft.Data.SqlClient;

namespace DapperLunchAndLearn
{
    public static class Connection
    {
        private const string ConnectionString = "Server=ftepqfc44v.database.windows.net,1433;Database=DapperLunchAndLearn;User Id=DapperDemo;Password=4atpUig6arK#%xGA";

        public static SqlConnection Open() => new SqlConnection(ConnectionString);
    }
}
