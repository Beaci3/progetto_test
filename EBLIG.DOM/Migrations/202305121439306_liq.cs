namespace EBLIG.DOM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class liq : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LiquidazionePraticheRegionaliMailInviatiEsito",
                c => new
                    {
                        LiquidazionePraticheRegionaliMailInviatiEsitoId = c.Int(nullable: false, identity: true),
                        Email = c.String(maxLength: 75),
                        Esito = c.String(maxLength: 1000),
                        LiquidazioneId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LiquidazionePraticheRegionaliMailInviatiEsitoId)
                .ForeignKey("dbo.Liquidazione", t => t.LiquidazioneId, cascadeDelete: true)
                .Index(t => t.LiquidazioneId);
            
            AlterColumn("dbo.Liquidazione", "MailInviata", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LiquidazionePraticheRegionaliMailInviatiEsito", "LiquidazioneId", "dbo.Liquidazione");
            DropIndex("dbo.LiquidazionePraticheRegionaliMailInviatiEsito", new[] { "LiquidazioneId" });
            AlterColumn("dbo.Liquidazione", "MailInviata", c => c.Boolean(nullable: false));
            DropTable("dbo.LiquidazionePraticheRegionaliMailInviatiEsito");
        }
    }
}
