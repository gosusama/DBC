namespace BTS.API.ENTITY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20180617_addColQuyen_nhomquyen_Anhpt : DbMigration
    {
        public override void Up()
        {
            AddColumn("TBNETERP.AU_NGUOIDUNG_QUYEN", "GIAMUA", c => c.Decimal(nullable: false, precision: 1, scale: 0));
            AddColumn("TBNETERP.AU_NGUOIDUNG_QUYEN", "GIABAN", c => c.Decimal(nullable: false, precision: 1, scale: 0));
            AddColumn("TBNETERP.AU_NGUOIDUNG_QUYEN", "GIAVON", c => c.Decimal(nullable: false, precision: 1, scale: 0));
            AddColumn("TBNETERP.AU_NGUOIDUNG_QUYEN", "TYLELAI", c => c.Decimal(nullable: false, precision: 1, scale: 0));
            AddColumn("TBNETERP.AU_NGUOIDUNG_QUYEN", "BANCHIETKHAU", c => c.Decimal(nullable: false, precision: 1, scale: 0));
            AddColumn("TBNETERP.AU_NGUOIDUNG_QUYEN", "BANBUON", c => c.Decimal(nullable: false, precision: 1, scale: 0));
            AddColumn("TBNETERP.AU_NGUOIDUNG_QUYEN", "BANTRALAI", c => c.Decimal(nullable: false, precision: 1, scale: 0));
            AddColumn("TBNETERP.AU_NHOMQUYEN_CHUCNANG", "GIAMUA", c => c.Decimal(nullable: false, precision: 1, scale: 0));
            AddColumn("TBNETERP.AU_NHOMQUYEN_CHUCNANG", "GIABAN", c => c.Decimal(nullable: false, precision: 1, scale: 0));
            AddColumn("TBNETERP.AU_NHOMQUYEN_CHUCNANG", "GIAVON", c => c.Decimal(nullable: false, precision: 1, scale: 0));
            AddColumn("TBNETERP.AU_NHOMQUYEN_CHUCNANG", "TYLELAI", c => c.Decimal(nullable: false, precision: 1, scale: 0));
            AddColumn("TBNETERP.AU_NHOMQUYEN_CHUCNANG", "BANCHIETKHAU", c => c.Decimal(nullable: false, precision: 1, scale: 0));
            AddColumn("TBNETERP.AU_NHOMQUYEN_CHUCNANG", "BANBUON", c => c.Decimal(nullable: false, precision: 1, scale: 0));
            AddColumn("TBNETERP.AU_NHOMQUYEN_CHUCNANG", "BANTRALAI", c => c.Decimal(nullable: false, precision: 1, scale: 0));
        }
        
        public override void Down()
        {
            DropColumn("TBNETERP.AU_NHOMQUYEN_CHUCNANG", "BANTRALAI");
            DropColumn("TBNETERP.AU_NHOMQUYEN_CHUCNANG", "BANBUON");
            DropColumn("TBNETERP.AU_NHOMQUYEN_CHUCNANG", "BANCHIETKHAU");
            DropColumn("TBNETERP.AU_NHOMQUYEN_CHUCNANG", "TYLELAI");
            DropColumn("TBNETERP.AU_NHOMQUYEN_CHUCNANG", "GIAVON");
            DropColumn("TBNETERP.AU_NHOMQUYEN_CHUCNANG", "GIABAN");
            DropColumn("TBNETERP.AU_NHOMQUYEN_CHUCNANG", "GIAMUA");
            DropColumn("TBNETERP.AU_NGUOIDUNG_QUYEN", "BANTRALAI");
            DropColumn("TBNETERP.AU_NGUOIDUNG_QUYEN", "BANBUON");
            DropColumn("TBNETERP.AU_NGUOIDUNG_QUYEN", "BANCHIETKHAU");
            DropColumn("TBNETERP.AU_NGUOIDUNG_QUYEN", "TYLELAI");
            DropColumn("TBNETERP.AU_NGUOIDUNG_QUYEN", "GIAVON");
            DropColumn("TBNETERP.AU_NGUOIDUNG_QUYEN", "GIABAN");
            DropColumn("TBNETERP.AU_NGUOIDUNG_QUYEN", "GIAMUA");
        }
    }
}
