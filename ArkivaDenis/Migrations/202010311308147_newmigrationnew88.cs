namespace ArkivaDenis.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newmigrationnew88 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Dokumentes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmerDok = c.String(),
                        Subjekte_Id = c.Int(nullable: false),
                        Inspektime_Id = c.Int(nullable: false),
                        Zyra = c.String(),
                        NrKutise = c.String(),
                        Rafti = c.String(),
                        Data = c.String(),
                        Llogaria_ID = c.Int(),
                        LlojDokument_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Llogarias", t => t.Llogaria_ID)
                .ForeignKey("dbo.LlojDokuments", t => t.LlojDokument_Id)
                .Index(t => t.Llogaria_ID)
                .Index(t => t.LlojDokument_Id);
            
            CreateTable(
                "dbo.Indeksimets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Emer = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Llogarias",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Emer = c.String(),
                        Mbiemer = c.String(),
                        Username = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.LlojDokuments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Emer = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Inspektimes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NrInspek = c.String(),
                        Subjekte_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Subjektes", t => t.Subjekte_Id)
                .Index(t => t.Subjekte_Id);
            
            CreateTable(
                "dbo.Subjektes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmerSubjekt = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IndeksimetDokumentes",
                c => new
                    {
                        Indeksimet_Id = c.Int(nullable: false),
                        Dokumente_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Indeksimet_Id, t.Dokumente_Id })
                .ForeignKey("dbo.Indeksimets", t => t.Indeksimet_Id, cascadeDelete: true)
                .ForeignKey("dbo.Dokumentes", t => t.Dokumente_Id, cascadeDelete: true)
                .Index(t => t.Indeksimet_Id)
                .Index(t => t.Dokumente_Id);
            
            CreateTable(
                "dbo.InspektimeLlojDokuments",
                c => new
                    {
                        Inspektime_Id = c.Int(nullable: false),
                        LlojDokument_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Inspektime_Id, t.LlojDokument_Id })
                .ForeignKey("dbo.Inspektimes", t => t.Inspektime_Id, cascadeDelete: true)
                .ForeignKey("dbo.LlojDokuments", t => t.LlojDokument_Id, cascadeDelete: true)
                .Index(t => t.Inspektime_Id)
                .Index(t => t.LlojDokument_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Inspektimes", "Subjekte_Id", "dbo.Subjektes");
            DropForeignKey("dbo.InspektimeLlojDokuments", "LlojDokument_Id", "dbo.LlojDokuments");
            DropForeignKey("dbo.InspektimeLlojDokuments", "Inspektime_Id", "dbo.Inspektimes");
            DropForeignKey("dbo.Dokumentes", "LlojDokument_Id", "dbo.LlojDokuments");
            DropForeignKey("dbo.Dokumentes", "Llogaria_ID", "dbo.Llogarias");
            DropForeignKey("dbo.IndeksimetDokumentes", "Dokumente_Id", "dbo.Dokumentes");
            DropForeignKey("dbo.IndeksimetDokumentes", "Indeksimet_Id", "dbo.Indeksimets");
            DropIndex("dbo.InspektimeLlojDokuments", new[] { "LlojDokument_Id" });
            DropIndex("dbo.InspektimeLlojDokuments", new[] { "Inspektime_Id" });
            DropIndex("dbo.IndeksimetDokumentes", new[] { "Dokumente_Id" });
            DropIndex("dbo.IndeksimetDokumentes", new[] { "Indeksimet_Id" });
            DropIndex("dbo.Inspektimes", new[] { "Subjekte_Id" });
            DropIndex("dbo.Dokumentes", new[] { "LlojDokument_Id" });
            DropIndex("dbo.Dokumentes", new[] { "Llogaria_ID" });
            DropTable("dbo.InspektimeLlojDokuments");
            DropTable("dbo.IndeksimetDokumentes");
            DropTable("dbo.Subjektes");
            DropTable("dbo.Inspektimes");
            DropTable("dbo.LlojDokuments");
            DropTable("dbo.Llogarias");
            DropTable("dbo.Indeksimets");
            DropTable("dbo.Dokumentes");
        }
    }
}
