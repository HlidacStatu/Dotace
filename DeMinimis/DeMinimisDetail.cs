using System;
using Dapper.Contrib.Extensions;
#pragma warning disable 1591

// ReSharper disable InconsistentNaming
// ReSharper disable All

namespace DeMinimis
{
    [Table("dotace")]
    public class DeMinimisDetail
    {
        [ExplicitKey]
        public int Id { get; set; }
        public int Prijemce_SZRId { get; set; }
        public string? Ico { get; set; }
        public string? JmenoPrijemce { get; set; }
        public string? RdmOblastKod { get; set; }
        public decimal PodporaCzk { get; set; }
        public decimal PodporaEur { get; set; }
        public DateTime PodporaDatum { get; set; }
        public int PodporaForma_Kod { get; set; }
        public string? PodporaFormaText { get; set; }
        public string? PodporaUcel { get; set; }
        public string? ProjektId { get; set; }
        public string? CjPrijemce { get; set; }
        public int StavKod { get; set; }
        public string? StavKodText { get; set; }
        public DateTime? Insdat { get; set; }
        public DateTime? Edidat { get; set; }
        public int Poskytovatel_SZRId { get; set; }
        public string? PoskytovatelOjm { get; set; }
        public string? PoskytovatelIco { get; set; }
        public int PravniAktPoskytnutiId { get; set; }
        public string? PravniAktPoskytnutiText { get; set; }
        public string? RezimPodporyId { get; set; }

        public static string CreateSqlTableCommand()
        {
            return @"Create table dotace
                (
                ""Id"" integer, 
                ""Prijemce_SZRId"" integer, 
                ""Ico"" text, 
                ""JmenoPrijemce"" text, 
                ""RdmOblastKod"" text, 
                ""PodporaCzk"" decimal(20,2), 
                ""PodporaEur"" decimal(20,2), 
                ""PodporaDatum"" date, 
                ""PodporaForma_Kod"" integer, 
                ""PodporaFormaText"" text, 
                ""PodporaUcel"" text, 
                ""ProjektId"" text, 
                ""CjPrijemce"" text, 
                ""StavKod"" integer, 
                ""StavKodText"" text, 
                ""Insdat"" date, 
                ""Edidat"" date, 
                ""Poskytovatel_SZRId"" integer, 
                ""PoskytovatelOjm"" text, 
                ""PoskytovatelIco"" text, 
                ""PravniAktPoskytnutiId"" integer, 
                ""PravniAktPoskytnutiText"" text, 
                ""RezimPodporyId"" text
                )";
        }
        
            
    }
}