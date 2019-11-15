namespace BTS.API.ENTITY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _04062018_alterTableVtct_congno_HuyNQ : DbMigration
    {
        public override void Up()
        {
            AddColumn("TBNETERP.NVCONGNO", "THOIGIANDUYETPHIEU", c => c.Decimal(precision: 10, scale: 0));
            AddColumn("TBNETERP.VATTUCHUNGTU", "THOIGIANDUYETPHIEU", c => c.Decimal(precision: 10, scale: 0));
        }
        
        public override void Down()
        {
            DropColumn("TBNETERP.VATTUCHUNGTU", "THOIGIANDUYETPHIEU");
            DropColumn("TBNETERP.NVCONGNO", "THOIGIANDUYETPHIEU");
        }
    }
}
