namespace BTS.API.ENTITY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _21092018_UDP_DM_HANGKH_ANHPT : DbMigration
    {
        public override void Up()
        {
            AddColumn("TBNETERP.DM_HANGKHACHHANG", "QUYDOITIEN_THANH_DIEM", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("TBNETERP.DM_HANGKHACHHANG", "QUYDOIDIEM_THANH_TIEN", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("TBNETERP.DM_HANGKHACHHANG", "QUYDOIDIEM_THANH_TIEN");
            DropColumn("TBNETERP.DM_HANGKHACHHANG", "QUYDOITIEN_THANH_DIEM");
        }
    }
}
