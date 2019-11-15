namespace BTS.API.ENTITY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _07032018_NullableTrangThaiCon_ANHPT : DbMigration
    {
        public override void Up()
        {
            AlterColumn("TBNETERP.DM_VATTU", "TRANGTHAICON", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("TBNETERP.DM_VATTU", "TRANGTHAICON", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
