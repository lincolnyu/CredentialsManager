using CredentialsManagerLib.Helpers;
using JsonParser;
using JsonParser.JsonStructures;

namespace CredentialsManagerLib.Models
{
    public class Repository : IJsonSerializable
    {
        public Lookup<Provider> Providers { get; } = new Lookup<Provider>();

        public void FromJson(JsonNode jn)
        {
            Providers.Clear();
            Providers.LookupFromJson((JsonPairs)jn, CreateProviderFromJson);
        }

        public JsonNode ToJson() => Providers.LookupToJson();

        private Provider CreateProviderFromJson(JsonNode jn)
        {
            var provider = new Provider();
            provider.FromJson(jn);
            return provider;
        }
        
        public void LoadFromJsonString(string s)
        {
            var jn = Parser.ParseJson(s);
            FromJson(jn);
        }

        public string SaveToJsonString()
        {
            var jn = ToJson();
            return jn.ToString();
        }

        public string SaveToFormattedJsonString(int indent, int tabSize)
        {
            var jn = ToJson();
            return jn.ToString(indent, tabSize);
        }
    }
}
