using System;
using IsKronosUpYet.API.Controllers;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using IsKronosUpYet.API.Models;

namespace IsKronosUpYet.API.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20160514182724_FirstMigration")]
    partial class FirstMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("IsKronosUpYet.API.Models.News", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<string>("Body");

                    b.Property<DateTimeOffset>("Timestamp");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("IsKronosUpYet.API.Models.Server", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("IP")
                        .IsRequired();

                    b.Property<string>("Name");

                    b.Property<int>("Port");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("IsKronosUpYet.API.Models.ServerStatus", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("ServerId");

                    b.Property<bool>("Status");

                    b.Property<DateTimeOffset>("Timestamp");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("IsKronosUpYet.API.Models.ServerStatus", b =>
                {
                    b.HasOne("IsKronosUpYet.API.Models.Server")
                        .WithMany()
                        .HasForeignKey("ServerId");
                });
        }
    }
}
