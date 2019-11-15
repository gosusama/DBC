namespace BTS.API.ENTITY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _21092018_UDP_LAN2_DM_HANGKH_ANHPT : DbMigration
    {
        public override void Up()
        {
            AddColumn("TBNETERP.DM_HANGKHACHHANG", "HANG_KHOIDAU", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("TBNETERP.DM_HANGKHACHHANG", "HANG_KHOIDAU");
        }
    }
}
