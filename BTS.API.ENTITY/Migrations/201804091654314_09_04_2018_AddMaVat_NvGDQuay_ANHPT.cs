namespace BTS.API.ENTITY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _09_04_2018_AddMaVat_NvGDQuay_ANHPT : DbMigration
    {
        public override void Up()
        {
            AddColumn("TBNETERP.NVHANGGDQUAY_ASYNCCLIENT", "MAVAT", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("TBNETERP.NVHANGGDQUAY_ASYNCCLIENT", "MAVAT");
        }
    }
}
