namespace BTS.API.ENTITY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _04022018_A_U_THONGTIN_NGANHANG_ANHPT : DbMigration
    {
        public override void Up()
        {
            AddColumn("TBNETERP.DM_NHACUNGCAP", "THONGTIN_NGANHANG", c => c.String(maxLength: 500));
            AlterColumn("TBNETERP.DM_NHACUNGCAP", "TAIKHOAN_NGANHANG", c => c.String(maxLength: 30));
        }
        
        public override void Down()
        {
            AlterColumn("TBNETERP.DM_NHACUNGCAP", "TAIKHOAN_NGANHANG", c => c.String(maxLength: 500));
            DropColumn("TBNETERP.DM_NHACUNGCAP", "THONGTIN_NGANHANG");
        }
    }
}
