namespace BTS.API.ENTITY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _01042018_addColTienThanhToan_VatTuChungTu_HuyNQ : DbMigration
    {
        public override void Up()
        {
            AddColumn("TBNETERP.VATTUCHUNGTU", "TIENTHANHTOAN", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("TBNETERP.VATTUCHUNGTU", "TIENTHANHTOAN");
        }
    }
}
