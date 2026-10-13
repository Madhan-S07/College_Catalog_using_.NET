using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class AddNoStudents : Migration
    {
        /// <inheritdoc />
       protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AddColumn<int>(
        name: "NO_Students",
        table: "StudentsDetails",
        type: "INTEGER",
        nullable: false,
        defaultValue: 0);
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropColumn(
        name: "NO_Students",
        table: "StudentsDetails");
}
    }
}
