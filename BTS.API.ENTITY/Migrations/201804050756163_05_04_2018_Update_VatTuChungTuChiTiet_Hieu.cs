namespace BTS.API.ENTITY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _05_04_2018_Update_VatTuChungTuChiTiet_Hieu : DbMigration
    {
        public override void Up()
        {
            AddColumn("TBNETERP.VATTUCHUNGTUCHITIET", "GIAMUACOVAT", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("TBNETERP.VATTUCHUNGTUCHITIET", "GIAMUACOVAT");
        }
    }
}
