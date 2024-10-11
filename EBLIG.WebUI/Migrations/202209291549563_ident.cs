namespace EBLIG.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ident : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Cognome", c => c.String());
            AddColumn("dbo.AspNetUsers", "Nome", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Nome");
            DropColumn("dbo.AspNetUsers", "Cognome");
        }
    }
}
