using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Detached.PatchTypes
{
    public class PatchJsonConverterFactory : JsonConverterFactory
    {
        readonly IPatchTypeInfoProvider _typeInfoProvider;

        public PatchJsonConverterFactory(IPatchTypeInfoProvider typeInfoProvider)
        {
            _typeInfoProvider = typeInfoProvider;
        }

        public PatchJsonConverterFactory()
        {
            _typeInfoProvider = new DefaultPatchTypeInfoProvider();
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return _typeInfoProvider.ShouldPatch(typeToConvert);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return (JsonConverter)Activator.CreateInstance(typeof(PatchJsonConverter<>).MakeGenericType(typeToConvert));
        }
    }
}