namespace BTS.API.ENTITY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _29_03_2018_CreateNvHetHanHangHoa_ANHPT : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TBNETERP.NVHETHAN_HANGHOA_CHITIET",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MAPHIEUPK = c.String(nullable: false, maxLength: 50),
                        MANHACUNGCAP = c.String(maxLength: 50),
                        TENNHACUNGCAP = c.String(maxLength: 500),
                        MAVATTU = c.String(maxLength: 50),
                        TENVATTU = c.String(maxLength: 500),
                        BARCODE = c.String(maxLength: 2000),
                        SOLUONG = c.Decimal(precision: 18, scale: 2),
                        NGAYSANXUAT = c.DateTime(),
                        NGAYHETHAN = c.DateTime(),
                        CONLAI_NGAYBAO = c.Decimal(precision: 18, scale: 2),
                        CONLAI_NGAYHETHAN = c.Decimal(precision: 18, scale: 2),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.NVHETHAN_HANGHOA",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MAPHIEU = c.String(nullable: false, maxLength: 50),
                        MAPHIEUPK = c.String(nullable: false, maxLength: 50),
                        NGAYBAODATE = c.DateTime(),
                        THOIGIAN = c.String(maxLength: 15),
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
            DropTable("TBNETERP.NVHETHAN_HANGHOA");
            DropTable("TBNETERP.NVHETHAN_HANGHOA_CHITIET");
        }
    }
}
