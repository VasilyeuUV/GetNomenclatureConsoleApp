using System.Text.Json.Serialization;

namespace GetNomenclatureConsoleApp
{
    internal class User
    {
        [JsonPropertyName("department")]
        public Department? Department { get; set; }

        [JsonPropertyName("source")]
        public Source? Source { get; set; }

        public bool IsActivated { get; set; }
    }
}
