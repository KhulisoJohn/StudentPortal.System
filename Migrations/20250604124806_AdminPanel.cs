using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPortal.Migrations
{
    /// <inheritdoc />
    public partial class adminpanel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tutors_AspNetUsers_ApplicationUserId",
                table: "Tutors");

            migrationBuilder.DropForeignKey(
                name: "FK_Tutors_AspNetUsers_UserId",
                table: "Tutors");

            migrationBuilder.DropIndex(
                name: "IX_Tutors_ApplicationUserId",
                table: "Tutors");

            migrationBuilder.DropIndex(
                name: "IX_Tutors_UserId",
                table: "Tutors");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Tutors");

            migrationBuilder.DropColumn(
                name: "Grade",
                table: "Tutors");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Tutors",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Tutors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegisteredAt",
                table: "Tutors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Subjects",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "EnrollmentDate",
                table: "Students",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Students",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Tutors_UserId",
                table: "Tutors",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tutors_AspNetUsers_UserId",
                table: "Tutors",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tutors_AspNetUsers_UserId",
                table: "Tutors");

            migrationBuilder.DropIndex(
                name: "IX_Tutors_UserId",
                table: "Tutors");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Tutors");

            migrationBuilder.DropColumn(
                name: "RegisteredAt",
                table: "Tutors");

            migrationBuilder.DropColumn(
                name: "EnrollmentDate",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Students");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Tutors",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Tutors",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Grade",
                table: "Tutors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Subjects",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_Tutors_ApplicationUserId",
                table: "Tutors",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tutors_UserId",
                table: "Tutors",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Tutors_AspNetUsers_ApplicationUserId",
                table: "Tutors",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tutors_AspNetUsers_UserId",
                table: "Tutors",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
