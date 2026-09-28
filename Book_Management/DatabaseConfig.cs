namespace Book_Management
{
    internal class DatabaseConfig
    {
        public const string ConnectionString =
            "Server=localhost;" +
            "Database=LibraryDB;" +
            "Integrated Security=True;" +
            "Encrypt=True;" +
            "TrustServerCertificate=True;" +
            "Connect Timeout=5;";
    }
}
