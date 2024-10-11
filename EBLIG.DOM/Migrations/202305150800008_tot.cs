namespace EBLIG.DOM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tot : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Liquidazione", "MailDaInviareTotale", c => c.Int());
            DropColumn("dbo.Liquidazione", "MailInviata");
            DropColumn("dbo.Liquidazione", "MailInviataErrors");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Liquidazione", "MailInviataErrors", c => c.String());
            AddColumn("dbo.Liquidazione", "MailInviata", c => c.Boolean());
            DropColumn("dbo.Liquidazione", "MailDaInviareTotale");
        }
    }
}
