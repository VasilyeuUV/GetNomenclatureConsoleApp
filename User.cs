using System.Text.Json.Serialization;

namespace GetNomenclatureConsoleApp
{
    internal class User : IDepartamentable
    {
        [JsonPropertyName("department")]
        public Department[]? Departments { get; set; }
    }
}
