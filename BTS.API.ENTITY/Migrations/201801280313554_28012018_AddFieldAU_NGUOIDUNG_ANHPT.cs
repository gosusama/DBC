namespace BTS.API.ENTITY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _28012018_AddFieldAU_NGUOIDUNG_ANHPT : DbMigration
    {
        public override void Up()
        {
            AddColumn("TBNETERP.AU_NGUOIDUNG", "PARENT_UNITCODE", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("TBNETERP.AU_NGUOIDUNG", "PARENT_UNITCODE");
        }
    }
}
