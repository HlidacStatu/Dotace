using System.Text.Json.Serialization;

namespace Common;

public class DataGovResult
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    [JsonPropertyName("data")]
    public DatumC Data { get; set; }
    
    public class DatumC
    {
        public class DatasetsC
        {
            public class DatumC
            {
                public class DistributionC
                {
                    [JsonPropertyName("accessURL")]
                    public string AccessURL { get; set; }
                }
                [JsonPropertyName("distribution")]
                public List<DistributionC> Distribution { get; set; }
            }
            
            [JsonPropertyName("data")]
            public List<DatumC> Data { get; set; }
        }
        
        [JsonPropertyName("datasets")]
        public DatasetsC Datasets { get; set; }
        
    }
    



    
    


}