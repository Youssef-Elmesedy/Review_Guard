using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Review_Guard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMediaAsset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MediaAssets_BranchId",
                table: "MediaAssets");

            migrationBuilder.DropIndex(
                name: "IX_MediaAssets_BusinessId",
                table: "MediaAssets");

            migrationBuilder.DropIndex(
                name: "IX_MediaAssets_Type",
                table: "MediaAssets");

            migrationBuilder.AddColumn<int>(
                name: "OwnerType",
                table: "MediaAssets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ProofId",
                table: "MediaAssets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "MediaAssets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "MediaAssets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_OwnerType_BranchId",
                table: "MediaAssets",
                columns: new[] { "OwnerType", "BranchId" });

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_OwnerType_BusinessId",
                table: "MediaAssets",
                columns: new[] { "OwnerType", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_OwnerType_ProofId",
                table: "MediaAssets",
                columns: new[] { "OwnerType", "ProofId" });

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_OwnerType_UserId",
                table: "MediaAssets",
                columns: new[] { "OwnerType", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_ProofId",
                table: "MediaAssets",
                column: "ProofId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_UserId_IsPrimary",
                table: "MediaAssets",
                columns: new[] { "UserId", "IsPrimary" },
                unique: true,
                filter: "[UserId] IS NOT NULL AND [IsPrimary] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_UserId1",
                table: "MediaAssets",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaAssets_Proofs_ProofId",
                table: "MediaAssets",
                column: "ProofId",
                principalTable: "Proofs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaAssets_Users_UserId",
                table: "MediaAssets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaAssets_Users_UserId1",
                table: "MediaAssets",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaAssets_Proofs_ProofId",
                table: "MediaAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaAssets_Users_UserId",
                table: "MediaAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaAssets_Users_UserId1",
                table: "MediaAssets");

            migrationBuilder.DropIndex(
                name: "IX_MediaAssets_OwnerType_BranchId",
                table: "MediaAssets");

            migrationBuilder.DropIndex(
                name: "IX_MediaAssets_OwnerType_BusinessId",
                table: "MediaAssets");

            migrationBuilder.DropIndex(
                name: "IX_MediaAssets_OwnerType_ProofId",
                table: "MediaAssets");

            migrationBuilder.DropIndex(
                name: "IX_MediaAssets_OwnerType_UserId",
                table: "MediaAssets");

            migrationBuilder.DropIndex(
                name: "IX_MediaAssets_ProofId",
                table: "MediaAssets");

            migrationBuilder.DropIndex(
                name: "IX_MediaAssets_UserId_IsPrimary",
                table: "MediaAssets");

            migrationBuilder.DropIndex(
                name: "IX_MediaAssets_UserId1",
                table: "MediaAssets");

            migrationBuilder.DropColumn(
                name: "OwnerType",
                table: "MediaAssets");

            migrationBuilder.DropColumn(
                name: "ProofId",
                table: "MediaAssets");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MediaAssets");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "MediaAssets");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_BranchId",
                table: "MediaAssets",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_BusinessId",
                table: "MediaAssets",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_Type",
                table: "MediaAssets",
                column: "Type");
        }
    }
}
