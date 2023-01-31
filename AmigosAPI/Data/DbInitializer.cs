namespace AmigosAPI.Data
{
    public class DbInitializer
    {
        public static void Initialize(BillManagerContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
