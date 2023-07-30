namespace Domain.DatabaseSettings
{
    public class TgsSolverDatabaseSettings : ITgsSolverDatabaseSettings
    {
        // users
        public string UsersCollectionName { get; set; } = "users";

        public string ConnectionString { get; set; } = String.Empty;
        public string DatabaseName { get; set; } = String.Empty;
    }
}
