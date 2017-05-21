using JsonParser.JsonStructures;
using CredentialsManagerLib.Helpers;

namespace CredentialsManagerLib.Models
{
    public class Provider : IJsonSerializable
    {
        public Lookup<object> KeyValues { get; } = new Lookup<object>(); 
        public Lookup<Account> Accounts { get; } = new Lookup<Account>();

        public void FromJson(JsonNode jn)
        {
            KeyValues.LookupFromJsonValues((JsonPairs)jn, (t)=> t.Item1 != "accounts");
            if (((JsonPairs)jn).KeyValues.TryGetValue("accounts", out var js))
            {
                Accounts.LookupFromJson((JsonPairs)js, CreateAccountFromJson);
            }
        }

        public JsonNode ToJson()
        {
            var jps = KeyValues.LookupToJsonValues();
            var jpAccs = Accounts.LookupToJson();
            jps.KeyValues["accounts"] = jpAccs;
            return jps;
        }

        private Account CreateAccountFromJson(JsonNode jn)
        {
            var account = new Account();
            account.FromJson(jn);
            return account;
        }
    }
}
