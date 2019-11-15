namespace BTS.API.ENTITY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _07_04_2018_AddField_VAT_VATTUCTU_ANHPT : DbMigration
    {
        public override void Up()
        {
            AddColumn("TBNETERP.VATTUCHUNGTUCHITIET", "VAT", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("TBNETERP.VATTUCHUNGTUCHITIET", "VAT");
        }
    }
}
