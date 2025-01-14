using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Umbraco.Cms.Core.PropertyEditors;

/// <summary>
///     Represents the 'startNode' value for the <see cref="MultiNodePickerConfiguration" />
/// </summary>
[DataContract]
public class MultiNodePickerConfigurationTreeSource
{
    [JsonPropertyName("type")]
    [DataMember(Name = "type")]
    public string? ObjectType { get; set; }

    [JsonPropertyName("query")]
    [DataMember(Name = "query")]
    public string? StartNodeQuery { get; set; }

    [DataMember(Name = "dynamicRoot")]
    public DynamicRoot? DynamicRoot { get; set; }

    [JsonPropertyName("id")]
    [DataMember(Name = "id")]
    public Udi? StartNodeId { get; set; }
}

[DataContract]
public class DynamicRoot
{
    [DataMember(Name = "originAlias")]
    public string OriginAlias { get; set; } = string.Empty;

    [DataMember(Name = "originKey")]
    public Guid? OriginKey { get; set; }

    [DataMember(Name = "querySteps")]
    public QueryStep[] QuerySteps { get; set; } = Array.Empty<QueryStep>();
}

[DataContract]
public class QueryStep
{
    [DataMember(Name = "alias")]
    public string Alias { get; set; } = string.Empty;

    [DataMember(Name = "anyOfDocTypeKeys")]
    public IEnumerable<Guid> AnyOfDocTypeKeys { get; set; } = Array.Empty<Guid>();
}

