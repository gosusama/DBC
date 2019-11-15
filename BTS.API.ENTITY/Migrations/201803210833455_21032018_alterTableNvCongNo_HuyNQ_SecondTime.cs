namespace BTS.API.ENTITY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _21032018_alterTableNvCongNo_HuyNQ_SecondTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("TBNETERP.NVCONGNO", "THANHTIENCANTRA", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("TBNETERP.NVCONGNO", "THANHTIENCANTRA");
        }
    }
}
