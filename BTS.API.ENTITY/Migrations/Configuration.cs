namespace BTS.API.ENTITY.Migrations
{
    using System.Data.Entity.Migrations;
    internal sealed class Configuration : DbMigrationsConfiguration<ERPContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ERPContext context)
        {
        }

    }
}
