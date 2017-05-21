using JsonParser.JsonStructures;
using CredentialsManagerLib.Helpers;

namespace CredentialsManagerLib.Models
{
    public class Account : IJsonSerializable
    {
        public Lookup<object> KeyValues { get; } = new Lookup<object>();

        public string UserName
        {
            get => KeyValues.TryGetValue("userName", out var un) ? un as string : null;
            set => SetValue("userName", value);
        }
        public string Password
        {
            get => KeyValues.TryGetValue("password", out var pw) ? pw as string : null;
            set => SetValue("password", value);
        }

        public void FromJson(JsonNode jn) => KeyValues.LookupFromJsonValues((JsonPairs)jn);
        public JsonNode ToJson() => KeyValues.LookupToJsonValues();

        private void SetValue(string key, object val)
        {
            if (val != null)
            {
                KeyValues[key] = val;
            }
            else
            {
                KeyValues.Remove(key);
            }
        }
    }
}
