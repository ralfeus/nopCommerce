namespace Nop.Plugin.Ralfeus.OrderItemsList.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVirtualProperties : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderItem2",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderItemId = c.Int(nullable: false),
                        OrderItemStatusId = c.Int(nullable: false),
                        PrivateComment = c.String(),
                        PublicComment = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.OrderItem2");
        }
    }
}
