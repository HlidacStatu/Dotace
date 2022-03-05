using System.ComponentModel.DataAnnotations;

namespace Common.IntermediateDb
{
    public class Cerpani
    {
        public string Id { get; set; }
        public decimal? CastkaSpotrebovana { get; set; }
        public int? Rok { get; set; }
        public int? GuessedYear { get; set; }
    }
}
