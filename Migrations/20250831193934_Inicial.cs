using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace safealert_api.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Eventos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Tipo = table.Column<string>(type: "NCLOB", maxLength: 4000, nullable: true),
                    Descricao = table.Column<string>(type: "NCLOB", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Localidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Bairro = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Zona = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localidades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Senha = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Endereco = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Tipo_usuario = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Data_cadastro = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Postagens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Usuario_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Evento_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Localidade_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Titulo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Descricao = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Imagem_url = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Data_criacao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Postagens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Postagens_Eventos_Evento_id",
                        column: x => x.Evento_id,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Postagens_Localidades_Localidade_id",
                        column: x => x.Localidade_id,
                        principalTable: "Localidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Postagens_Usuarios_Usuario_id",
                        column: x => x.Usuario_id,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ocorrencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Postagem_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Status = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Data_ocorrencia = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ocorrencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ocorrencias_Postagens_Postagem_id",
                        column: x => x.Postagem_id,
                        principalTable: "Postagens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ocorrencias_Postagem_id",
                table: "Ocorrencias",
                column: "Postagem_id");

            migrationBuilder.CreateIndex(
                name: "IX_Postagens_Evento_id",
                table: "Postagens",
                column: "Evento_id");

            migrationBuilder.CreateIndex(
                name: "IX_Postagens_Localidade_id",
                table: "Postagens",
                column: "Localidade_id");

            migrationBuilder.CreateIndex(
                name: "IX_Postagens_Usuario_id",
                table: "Postagens",
                column: "Usuario_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ocorrencias");

            migrationBuilder.DropTable(
                name: "Postagens");

            migrationBuilder.DropTable(
                name: "Eventos");

            migrationBuilder.DropTable(
                name: "Localidades");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
