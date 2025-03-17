﻿// <auto-generated />
using Indexer.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Indexer.Migrations
{
    [DbContext(typeof(DoLsDbContext))]
    partial class DoLsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Indexer.DbContext.Email", b =>
                {
                    b.Property<int>("emailid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("emailid"));

                    b.Property<string>("emailcontent")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("emailname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("emailid");

                    b.ToTable("emails", (string)null);
                });

            modelBuilder.Entity("Indexer.DbContext.Occurrence", b =>
                {
                    b.Property<int>("occurrenceid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("occurrenceid"));

                    b.Property<int>("count")
                        .HasColumnType("integer");

                    b.Property<int>("emailid")
                        .HasColumnType("integer");

                    b.Property<int>("wordid")
                        .HasColumnType("integer");

                    b.HasKey("occurrenceid");

                    b.HasIndex("emailid");

                    b.HasIndex("wordid");

                    b.ToTable("occurrences", (string)null);
                });

            modelBuilder.Entity("Indexer.DbContext.Word", b =>
                {
                    b.Property<int>("wordid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("wordid"));

                    b.Property<string>("wordvalue")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("wordid");

                    b.ToTable("words", (string)null);
                });

            modelBuilder.Entity("Indexer.DbContext.Occurrence", b =>
                {
                    b.HasOne("Indexer.DbContext.Email", "email")
                        .WithMany("Occurrences")
                        .HasForeignKey("emailid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Indexer.DbContext.Word", "word")
                        .WithMany("Occurrences")
                        .HasForeignKey("wordid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("email");

                    b.Navigation("word");
                });

            modelBuilder.Entity("Indexer.DbContext.Email", b =>
                {
                    b.Navigation("Occurrences");
                });

            modelBuilder.Entity("Indexer.DbContext.Word", b =>
                {
                    b.Navigation("Occurrences");
                });
#pragma warning restore 612, 618
        }
    }
}
