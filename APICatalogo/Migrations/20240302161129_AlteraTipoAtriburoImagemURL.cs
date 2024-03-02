using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APICatalogo.Migrations
{
    /// <inheritdoc />
    public partial class AlteraTipoAtriburoImagemURL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImagemUrl",
                table: "Categorias",
                type: "varchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 300)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ImagemUrl",
                table: "Categorias",
                type: "int",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(300)",
                oldMaxLength: 300)
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
