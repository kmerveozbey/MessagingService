using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessagingService.DAL.Migrations
{
    public partial class initializeDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserName);
                });

            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    ActivityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginUserName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActivityDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.ActivityID);
                    table.ForeignKey(
                        name: "FK_ActivityLogs_Users_LoginUserName",
                        column: x => x.LoginUserName,
                        principalTable: "Users",
                        principalColumn: "UserName");
                });

            migrationBuilder.CreateTable(
                name: "BlockLists",
                columns: table => new
                {
                    BlockID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HinderingUserName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    BlockedUserName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockLists", x => x.BlockID);
                    table.ForeignKey(
                        name: "FK_BlockLists_Users_BlockedUserName",
                        column: x => x.BlockedUserName,
                        principalTable: "Users",
                        principalColumn: "UserName");
                    table.ForeignKey(
                        name: "FK_BlockLists_Users_HinderingUserName",
                        column: x => x.HinderingUserName,
                        principalTable: "Users",
                        principalColumn: "UserName");
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderUserName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReceiverUserName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SendDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageID);
                    table.ForeignKey(
                        name: "FK_Messages_Users_ReceiverUserName",
                        column: x => x.ReceiverUserName,
                        principalTable: "Users",
                        principalColumn: "UserName");
                    table.ForeignKey(
                        name: "FK_Messages_Users_SenderUserName",
                        column: x => x.SenderUserName,
                        principalTable: "Users",
                        principalColumn: "UserName");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_LoginUserName",
                table: "ActivityLogs",
                column: "LoginUserName");

            migrationBuilder.CreateIndex(
                name: "IX_BlockLists_BlockedUserName",
                table: "BlockLists",
                column: "BlockedUserName");

            migrationBuilder.CreateIndex(
                name: "IX_BlockLists_HinderingUserName",
                table: "BlockLists",
                column: "HinderingUserName");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverUserName",
                table: "Messages",
                column: "ReceiverUserName");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderUserName",
                table: "Messages",
                column: "SenderUserName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLogs");

            migrationBuilder.DropTable(
                name: "BlockLists");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
