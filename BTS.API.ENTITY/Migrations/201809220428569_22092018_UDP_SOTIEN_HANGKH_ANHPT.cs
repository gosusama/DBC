namespace BTS.API.ENTITY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _22092018_UDP_SOTIEN_HANGKH_ANHPT : DbMigration
    {
        public override void Up()
        {
            AddColumn("TBNETERP.DM_HANGKHACHHANG", "SOTIEN", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("TBNETERP.DM_HANGKHACHHANG", "SODIEM");
        }
        
        public override void Down()
        {
            AddColumn("TBNETERP.DM_HANGKHACHHANG", "SODIEM", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("TBNETERP.DM_HANGKHACHHANG", "SOTIEN");
        }
    }
}
