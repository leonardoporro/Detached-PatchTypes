using Newtonsoft.Json;
using System;

namespace Detached.PatchTypes.Newtonsoft
{
    public class PatchJsonConverter : JsonConverter
    {
        readonly IPatchTypeInfoProvider _typeInfoProvider;

        public PatchJsonConverter(IPatchTypeInfoProvider typeInfoProvider)
        {
            _typeInfoProvider = typeInfoProvider;
        }

        public override bool CanConvert(Type objectType)
        {
            return _typeInfoProvider.ShouldPatch(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, PatchTypeFactory.GetType(objectType));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}