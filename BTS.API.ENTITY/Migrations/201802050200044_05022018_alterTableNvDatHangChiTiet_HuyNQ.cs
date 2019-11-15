namespace BTS.API.ENTITY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _05022018_alterTableNvDatHangChiTiet_HuyNQ : DbMigration
    {
        public override void Up()
        {
            AddColumn("TBNETERP.NVDATHANGCHITIET", "SOLUONGTHUCTE", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("TBNETERP.NVDATHANGCHITIET", "SOLUONGTHUCTE");
        }
    }
}
