using Newtonsoft.Json;
using System;
using UnityEngine;

namespace WKLib.Core.Config.Converters;

internal sealed class ColorJsonConverter : JsonConverter<Color>
{
    public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
    {
        writer.WriteStartArray();
        writer.WriteValue(value.r);
        writer.WriteValue(value.g);
        writer.WriteValue(value.b);
        writer.WriteValue(value.a);
        writer.WriteEndArray();
    }

    public override Color ReadJson(
        JsonReader reader,
        Type objectType,
        Color existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        var arr = Newtonsoft.Json.Linq.JArray.Load(reader);

        float r = arr.Count > 0 ? arr[0].ToObject<float>() : 0f;
        float g = arr.Count > 1 ? arr[1].ToObject<float>() : 0f;
        float b = arr.Count > 2 ? arr[2].ToObject<float>() : 0f;
        float a = arr.Count > 3 ? arr[3].ToObject<float>() : 1f;

        return new Color(r, g, b, a);
    }
}