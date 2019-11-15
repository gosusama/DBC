namespace BTS.API.ENTITY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _21032018_addTableNvCongNo_HuyNQ_SecondTime : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TBNETERP.NVCONGNO",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MACHUNGTU = c.String(nullable: false, maxLength: 50),
                        LOAICHUNGTU = c.String(nullable: false, maxLength: 50),
                        MACHUNGTUPK = c.String(nullable: false, maxLength: 50),
                        NGAYCT = c.DateTime(),
                        MAKHACHHANG = c.String(maxLength: 50),
                        MANHACUNGCAP = c.String(maxLength: 50),
                        GHICHU = c.String(maxLength: 200),
                        THANHTIEN = c.Decimal(precision: 18, scale: 2),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
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
            DropTable("TBNETERP.NVCONGNO");
        }
    }
}
