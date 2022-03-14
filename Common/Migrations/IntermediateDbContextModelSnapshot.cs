﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Common.IntermediateDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Common.Migrations
{
    [DbContext(typeof(IntermediateDbContext))]
    partial class IntermediateDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("exportready")
                .HasAnnotation("ProductVersion", "6.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Common.IntermediateDb.Dotace", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<List<string>>("Chyba")
                        .HasColumnType("jsonb")
                        .HasColumnName("chyba");

                    b.Property<DateTime?>("DatumAktualizace")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("datumaktualizace");

                    b.Property<DateTime?>("DatumPodpisu")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("datumpodpisu");

                    b.Property<string>("Duplicita")
                        .HasColumnType("text")
                        .HasColumnName("duplicita");

                    b.Property<string>("IdDotace")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("iddotace");

                    b.Property<string>("KodProjektu")
                        .HasColumnType("text")
                        .HasColumnName("kodprojektu");

                    b.Property<string>("NazevProjektu")
                        .HasColumnType("text")
                        .HasColumnName("nazevprojektu");

                    b.Property<string>("PrijemceHlidacJmeno")
                        .HasColumnType("text")
                        .HasColumnName("prijemcehlidacjmeno");

                    b.Property<string>("PrijemceIco")
                        .HasColumnType("text")
                        .HasColumnName("prijemceico");

                    b.Property<string>("PrijemceJmeno")
                        .HasColumnType("text")
                        .HasColumnName("prijemcejmeno");

                    b.Property<string>("PrijemceObchodniJmeno")
                        .HasColumnType("text")
                        .HasColumnName("prijemceobchodnijmeno");

                    b.Property<string>("PrijemceObec")
                        .HasColumnType("text")
                        .HasColumnName("prijemceobec");

                    b.Property<string>("PrijemceOkres")
                        .HasColumnType("text")
                        .HasColumnName("prijemceokres");

                    b.Property<string>("PrijemcePSC")
                        .HasColumnType("text")
                        .HasColumnName("prijemcepsc");

                    b.Property<int?>("PrijemceRokNarozeni")
                        .HasColumnType("integer")
                        .HasColumnName("prijemceroknarozeni");

                    b.Property<string>("PrijemceUlice")
                        .HasColumnType("text")
                        .HasColumnName("prijemceulice");

                    b.Property<string>("ProgramKod")
                        .HasColumnType("text")
                        .HasColumnName("programkod");

                    b.Property<string>("ProgramNazev")
                        .HasColumnType("text")
                        .HasColumnName("programnazev");

                    b.Property<List<Rozhodnuti>>("Rozhodnuti")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("rozhodnuti");

                    b.Property<string>("ZdrojNazev")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("zdrojnazev");

                    b.Property<string>("ZdrojUrl")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("zdrojurl");

                    b.HasKey("Id")
                        .HasName("pk_dotace");

                    b.ToTable("dotace", "exportready");
                });
#pragma warning restore 612, 618
        }
    }
}
