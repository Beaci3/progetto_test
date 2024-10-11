namespace EBLIG.DOM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class liql : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LiquidazionePraticheRegionaliMailInviatiEsito", "Inviata", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LiquidazionePraticheRegionaliMailInviatiEsito", "Inviata");
        }
    }
}
