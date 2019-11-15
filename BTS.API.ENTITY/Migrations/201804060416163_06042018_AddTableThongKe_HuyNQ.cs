namespace BTS.API.ENTITY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _06042018_AddTableThongKe_HuyNQ : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TBNETERP.THONGKE",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        LOAI = c.String(nullable: false, maxLength: 10),
                        MA = c.String(nullable: false, maxLength: 20),
                        Ten = c.String(maxLength: 100),
                        TUNGAY = c.DateTime(),
                        DENNGAY = c.DateTime(),
                        GIATRI = c.Decimal(nullable: false, precision: 18, scale: 2),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("TBNETERP.THONGKE");
        }
    }
}
