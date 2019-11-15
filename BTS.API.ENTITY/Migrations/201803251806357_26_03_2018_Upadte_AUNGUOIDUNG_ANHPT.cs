namespace BTS.API.ENTITY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _26_03_2018_Upadte_AUNGUOIDUNG_ANHPT : DbMigration
    {
        public override void Up()
        {
            AlterColumn("TBNETERP.AU_NGUOIDUNG", "GIOITINH", c => c.Decimal(precision: 10, scale: 0));
            AlterColumn("TBNETERP.AU_NGUOIDUNG", "TRANGTHAI", c => c.Decimal(precision: 10, scale: 0));
        }
        
        public override void Down()
        {
            AlterColumn("TBNETERP.AU_NGUOIDUNG", "TRANGTHAI", c => c.Decimal(nullable: false, precision: 10, scale: 0));
            AlterColumn("TBNETERP.AU_NGUOIDUNG", "GIOITINH", c => c.Decimal(nullable: false, precision: 10, scale: 0));
        }
    }
}
