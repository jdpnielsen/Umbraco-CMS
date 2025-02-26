using System.Text.Json;
using System.Text.Json.Serialization;
using Umbraco.Cms.Core.Serialization;

namespace Umbraco.Cms.Infrastructure.Serialization;

/// <inheritdoc />
public sealed class SystemTextConfigurationEditorJsonSerializer : IConfigurationEditorJsonSerializer
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemTextConfigurationEditorJsonSerializer" /> class.
    /// </summary>
    public SystemTextConfigurationEditorJsonSerializer()
        => _jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            // in some cases, configs aren't camel cased in the DB, so we have to resort to case insensitive
            // property name resolving when creating configuration objects (deserializing DB configs)
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            Converters =
            {
                new JsonStringEnumConverter(),
                new JsonObjectConverter(),
                new JsonUdiConverter(),
                new JsonUdiRangeConverter(),
                new JsonBooleanConverter()
            }
        };

    /// <inheritdoc />
    public string Serialize(object? input) => JsonSerializer.Serialize(input, _jsonSerializerOptions);

    /// <inheritdoc />
    public T? Deserialize<T>(string input) => JsonSerializer.Deserialize<T>(input, _jsonSerializerOptions);
}
