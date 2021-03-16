using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Detached.PatchTypes
{
    public class PatchJsonConverter<TModel> : JsonConverter<TModel>
    {
        public override TModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return (TModel)JsonSerializer.Deserialize(ref reader, PatchTypeFactory.GetType(typeof(TModel)), options);
        }

        public override void Write(Utf8JsonWriter writer, TModel value, JsonSerializerOptions options)
        {
            JsonConverter<TModel> converter = (JsonConverter<TModel>)options.GetConverter(typeof(TModel));
            converter.Write(writer, value, options);
        }
    }
}