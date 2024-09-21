using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblFaculties",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FacultyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFaculties", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tblFields",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FieldName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFields", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tblRoles",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblRoles", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tblSpecializations",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FacultyID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblSpecializations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tblSpecializations_tblFaculties_FacultyID",
                        column: x => x.FacultyID,
                        principalTable: "tblFaculties",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblAccount",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblAccount", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tblAccount_tblRoles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "tblRoles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblPermissions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    PermissionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CanManageAccounts = table.Column<bool>(type: "bit", nullable: false),
                    CanManageRoles = table.Column<bool>(type: "bit", nullable: false),
                    CanManagePermissions = table.Column<bool>(type: "bit", nullable: false),
                    CanUpdateProgress = table.Column<bool>(type: "bit", nullable: false),
                    CanRegisterTopics = table.Column<bool>(type: "bit", nullable: false),
                    CanUpdateAccounts = table.Column<bool>(type: "bit", nullable: false),
                    CanRequestTopicChanges = table.Column<bool>(type: "bit", nullable: false),
                    CanConfirmThesis = table.Column<bool>(type: "bit", nullable: false),
                    CanManageTheses = table.Column<bool>(type: "bit", nullable: false),
                    CanRequestGuidance = table.Column<bool>(type: "bit", nullable: false),
                    CanConfirmGuidance = table.Column<bool>(type: "bit", nullable: false),
                    CanApproveTopicChanges = table.Column<bool>(type: "bit", nullable: false),
                    CanUpdatePersonalInfo = table.Column<bool>(type: "bit", nullable: false),
                    CanManageDepartments = table.Column<bool>(type: "bit", nullable: false),
                    CanManageStudents = table.Column<bool>(type: "bit", nullable: false),
                    CanManageTeachers = table.Column<bool>(type: "bit", nullable: false),
                    CanUpdateTopics = table.Column<bool>(type: "bit", nullable: false),
                    CanAssignGuidance = table.Column<bool>(type: "bit", nullable: false),
                    CanManageSpecializations = table.Column<bool>(type: "bit", nullable: false),
                    CanAssignCouncils = table.Column<bool>(type: "bit", nullable: false),
                    CanManageClasses = table.Column<bool>(type: "bit", nullable: false),
                    CanManageProgress = table.Column<bool>(type: "bit", nullable: false),
                    CanManageFields = table.Column<bool>(type: "bit", nullable: false),
                    CanManageFaculties = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblPermissions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tblPermissions_tblRoles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "tblRoles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblClasses",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClassName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpecializationID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblClasses", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tblClasses_tblSpecializations_SpecializationID",
                        column: x => x.SpecializationID,
                        principalTable: "tblSpecializations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblTeacher",
                columns: table => new
                {
                    TeacherID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountID = table.Column<int>(type: "int", nullable: false),
                    FacultyID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Dob = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sex = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTeacher", x => x.TeacherID);
                    table.ForeignKey(
                        name: "FK_tblTeacher_tblAccount_AccountID",
                        column: x => x.AccountID,
                        principalTable: "tblAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblTeacher_tblFaculties_FacultyID",
                        column: x => x.FacultyID,
                        principalTable: "tblFaculties",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblStudent",
                columns: table => new
                {
                    StudentID = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    AccountID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Dob = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sex = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false),
                    Status = table.Column<int>(type: "int", maxLength: 300, nullable: false),
                    ClassID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblStudent", x => x.StudentID);
                    table.ForeignKey(
                        name: "FK_tblStudent_tblAccount_AccountID",
                        column: x => x.AccountID,
                        principalTable: "tblAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblStudent_tblClasses_ClassID",
                        column: x => x.ClassID,
                        principalTable: "tblClasses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblRegistrationRequest",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<string>(type: "nvarchar(14)", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblRegistrationRequest", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_tblRegistrationRequest_tblStudent_StudentId",
                        column: x => x.StudentId,
                        principalTable: "tblStudent",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblRegistrationRequest_tblTeacher_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "tblTeacher",
                        principalColumn: "TeacherID");
                });

            migrationBuilder.CreateTable(
                name: "tblTopics",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StudentID = table.Column<string>(type: "nvarchar(14)", nullable: false),
                    TeacherID = table.Column<int>(type: "int", nullable: false),
                    FieldID = table.Column<int>(type: "int", nullable: false),
                    RegistrationDate = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTopics", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tblTopics_tblFields_FieldID",
                        column: x => x.FieldID,
                        principalTable: "tblFields",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblTopics_tblStudent_StudentID",
                        column: x => x.StudentID,
                        principalTable: "tblStudent",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblTopics_tblTeacher_TeacherID",
                        column: x => x.TeacherID,
                        principalTable: "tblTeacher",
                        principalColumn: "TeacherID"
                      );
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblAccount_RoleID",
                table: "tblAccount",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_tblClasses_SpecializationID",
                table: "tblClasses",
                column: "SpecializationID");

            migrationBuilder.CreateIndex(
                name: "IX_tblPermissions_RoleID",
                table: "tblPermissions",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_tblRegistrationRequest_StudentId",
                table: "tblRegistrationRequest",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_tblRegistrationRequest_TeacherId",
                table: "tblRegistrationRequest",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_tblSpecializations_FacultyID",
                table: "tblSpecializations",
                column: "FacultyID");

            migrationBuilder.CreateIndex(
                name: "IX_tblStudent_AccountID",
                table: "tblStudent",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_tblStudent_ClassID",
                table: "tblStudent",
                column: "ClassID");

            migrationBuilder.CreateIndex(
                name: "IX_tblTeacher_AccountID",
                table: "tblTeacher",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_tblTeacher_FacultyID",
                table: "tblTeacher",
                column: "FacultyID");

            migrationBuilder.CreateIndex(
                name: "IX_tblTopics_FieldID",
                table: "tblTopics",
                column: "FieldID");

            migrationBuilder.CreateIndex(
                name: "IX_tblTopics_StudentID",
                table: "tblTopics",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_tblTopics_TeacherID",
                table: "tblTopics",
                column: "TeacherID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblPermissions");

            migrationBuilder.DropTable(
                name: "tblRegistrationRequest");

            migrationBuilder.DropTable(
                name: "tblTopics");

            migrationBuilder.DropTable(
                name: "tblFields");

            migrationBuilder.DropTable(
                name: "tblStudent");

            migrationBuilder.DropTable(
                name: "tblTeacher");

            migrationBuilder.DropTable(
                name: "tblClasses");

            migrationBuilder.DropTable(
                name: "tblAccount");

            migrationBuilder.DropTable(
                name: "tblSpecializations");

            migrationBuilder.DropTable(
                name: "tblRoles");

            migrationBuilder.DropTable(
                name: "tblFaculties");
        }
    }
}
