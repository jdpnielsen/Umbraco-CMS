// Copyright (c) Umbraco.
// See LICENSE for more details.

using System.Text.Json.Nodes;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Editors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Strings;

namespace Umbraco.Cms.Core.PropertyEditors;

/// <summary>
///     Represents a slider editor.
/// </summary>
[DataEditor(
    Constants.PropertyEditors.Aliases.Slider,
    ValueEditorIsReusable = true)]
public class SliderPropertyEditor : DataEditor
{
    private readonly IIOHelper _ioHelper;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SliderPropertyEditor" /> class.
    /// </summary>
    public SliderPropertyEditor(IDataValueEditorFactory dataValueEditorFactory, IIOHelper ioHelper)
        : base(dataValueEditorFactory)
    {
        _ioHelper = ioHelper;
        SupportsReadOnly = true;
    }

    /// <inheritdoc />
    protected override IDataValueEditor CreateValueEditor()
        => DataValueEditorFactory.Create<SliderPropertyValueEditor>(Attribute!);

    /// <inheritdoc />
    protected override IConfigurationEditor CreateConfigurationEditor() =>
        new SliderConfigurationEditor(_ioHelper);

    internal class SliderPropertyValueEditor : DataValueEditor
    {
        private readonly IJsonSerializer _jsonSerializer;

        public SliderPropertyValueEditor(
            IShortStringHelper shortStringHelper,
            IJsonSerializer jsonSerializer,
            IIOHelper ioHelper,
            DataEditorAttribute attribute)
            : base(shortStringHelper, jsonSerializer, ioHelper, attribute) =>
            _jsonSerializer = jsonSerializer;

        public override object? ToEditor(IProperty property, string? culture = null, string? segment = null)
        {
            // value is stored as a string - either a single integer value
            // or a two integer values separated by comma (for range sliders)
            var value = property.GetValue(culture, segment);
            if (value is not string stringValue)
            {
                return null;
            }

            var parts = stringValue.Split(Constants.CharArrays.Comma);
            var parsed = parts
                .Select(s => int.TryParse(s, out var i) ? i : (int?)null)
                .Where(i => i != null)
                .Select(i => i!.Value)
                .ToArray();

            return parts.Length == parsed.Length && parsed.Length is 1 or 2
                ? new SliderRange { From = parsed.First(), To = parsed.Last() }
                : null;
        }

        public override object? FromEditor(ContentPropertyData editorValue, object? currentValue)
        {
            // FIXME: do not rely explicitly on concrete JSON implementation here - consider creating an object deserialization method on IJsonSerializer (see also MultiUrlPickerValueEditor)
            if (editorValue.Value is not JsonNode jsonNode)
            {
                return null;
            }

            SliderRange? range = _jsonSerializer.Deserialize<SliderRange>(jsonNode.ToJsonString());
            return range?.ToString();
        }

        internal class SliderRange
        {
            public int From { get; set; }

            public int To { get; set; }

            public override string ToString() => From == To ? $"{From}" : $"{From},{To}";
        }
    }
}
