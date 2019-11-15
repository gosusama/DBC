namespace BTS.API.ENTITY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _30012018_AddFielNvDatHang_HuyNQ : DbMigration
    {
        public override void Up()
        {
            AddColumn("TBNETERP.NVDATHANG", "SOPHIEUCON", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            DropColumn("TBNETERP.NVDATHANG", "SOPHIEUCON");
        }
    }
}
