namespace BTS.API.ENTITY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _04022018_AddF_TK_NGANHANG_ANHPT : DbMigration
    {
        public override void Up()
        {
            AddColumn("TBNETERP.DM_NHACUNGCAP", "TAIKHOAN_NGANHANG", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            DropColumn("TBNETERP.DM_NHACUNGCAP", "TAIKHOAN_NGANHANG");
        }
    }
}
