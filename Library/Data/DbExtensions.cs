namespace Library.Data
{
    public static class DbExtensions
    {
        public static void AddDb(this IServiceCollection services)
        {
            services.AddSingleton<IDb, MsDb>();
        }
    }
}
