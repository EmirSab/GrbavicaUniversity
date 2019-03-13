namespace EmirApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class softdelete : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Course", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Course", "DeleteDate", c => c.DateTime());
            AddColumn("dbo.Department", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Department", "DeleteDate", c => c.DateTime());
            AddColumn("dbo.Person", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "DeleteDate", c => c.DateTime());
            AlterStoredProcedure(
                "dbo.Department_Insert",
                p => new
                    {
                        Name = p.String(maxLength: 50),
                        Budget = p.Decimal(precision: 19, scale: 4, storeType: "money"),
                        StartDate = p.DateTime(),
                        InstructorID = p.Int(),
                        IsDeleted = p.Boolean(),
                        DeleteDate = p.DateTime(),
                    },
                body:
                    @"INSERT [dbo].[Department]([Name], [Budget], [StartDate], [InstructorID], [IsDeleted], [DeleteDate])
                      VALUES (@Name, @Budget, @StartDate, @InstructorID, @IsDeleted, @DeleteDate)
                      
                      DECLARE @DepartmentID int
                      SELECT @DepartmentID = [DepartmentID]
                      FROM [dbo].[Department]
                      WHERE @@ROWCOUNT > 0 AND [DepartmentID] = scope_identity()
                      
                      SELECT t0.[DepartmentID], t0.[RowVersion]
                      FROM [dbo].[Department] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[DepartmentID] = @DepartmentID"
            );
            
            AlterStoredProcedure(
                "dbo.Department_Update",
                p => new
                    {
                        DepartmentID = p.Int(),
                        Name = p.String(maxLength: 50),
                        Budget = p.Decimal(precision: 19, scale: 4, storeType: "money"),
                        StartDate = p.DateTime(),
                        InstructorID = p.Int(),
                        RowVersion_Original = p.Binary(maxLength: 8, fixedLength: true, storeType: "rowversion"),
                        IsDeleted = p.Boolean(),
                        DeleteDate = p.DateTime(),
                    },
                body:
                    @"UPDATE [dbo].[Department]
                      SET [Name] = @Name, [Budget] = @Budget, [StartDate] = @StartDate, [InstructorID] = @InstructorID, [IsDeleted] = @IsDeleted, [DeleteDate] = @DeleteDate
                      WHERE (([DepartmentID] = @DepartmentID) AND (([RowVersion] = @RowVersion_Original) OR ([RowVersion] IS NULL AND @RowVersion_Original IS NULL)))
                      
                      SELECT t0.[RowVersion]
                      FROM [dbo].[Department] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[DepartmentID] = @DepartmentID"
            );
            
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "DeleteDate");
            DropColumn("dbo.Person", "IsDeleted");
            DropColumn("dbo.Department", "DeleteDate");
            DropColumn("dbo.Department", "IsDeleted");
            DropColumn("dbo.Course", "DeleteDate");
            DropColumn("dbo.Course", "IsDeleted");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
