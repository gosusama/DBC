namespace BTS.API.ENTITY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _04_02_2018_UpdateNgayHetHan_ANHPT : DbMigration
    {
        public override void Up()
        {
            AddColumn("TBNETERP.NVHETHAN_HANGHOA_CHITIET", "GHICHU", c => c.String(maxLength: 1000));
            AddColumn("TBNETERP.NVHETHAN_HANGHOA", "NOIDUNG", c => c.String(maxLength: 2000));
        }
        
        public override void Down()
        {
            DropColumn("TBNETERP.NVHETHAN_HANGHOA", "NOIDUNG");
            DropColumn("TBNETERP.NVHETHAN_HANGHOA_CHITIET", "GHICHU");
        }
    }
}
