using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class JsonDictionaryConverter : ValueConverter<Dictionary<string, object>, string>
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public JsonDictionaryConverter()
        : base(
            v => JsonSerializer.Serialize(v, Options), 
            v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, Options) ?? new Dictionary<string, object>()
        )
    {
    }
}