namespace Domain.DatabaseSettings
{
    public interface ITgsSolverDatabaseSettings
    {
        // users
        string UsersCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
