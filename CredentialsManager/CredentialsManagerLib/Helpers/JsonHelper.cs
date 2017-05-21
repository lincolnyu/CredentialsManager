using CredentialsManagerLib.Models;
using JsonParser.JsonStructures;
using System;

namespace CredentialsManagerLib.Helpers
{
    public static class JsonHelper
    {
        public static JsonPairs LookupToJson<TValue>(this Lookup<TValue> lookup) where TValue : IJsonSerializable
        {
            var jps = new JsonPairs();
            lookup.LookupToJson(jps);
            return jps;
        }

        public static void LookupToJson<TValue>(this Lookup<TValue> lookup, JsonPairs jps) where TValue : IJsonSerializable
        {
            foreach (var kvp in lookup)
            {
                var key = kvp.Key;
                var value = kvp.Value;
                var cjn = value.ToJson();
                jps.KeyValues.Add(key, cjn);
            }
        }

        public static void LookupFromJson<TValue>(this Lookup<TValue> lookup, JsonPairs jps, Func<JsonNode, TValue> createValue)
        {
            foreach (var kvp in jps.KeyValues)
            {
                var key = kvp.Key;
                var jn = kvp.Value;
                var val = createValue(jn);
                lookup.Add(key, val);
            }
        }

        public static JsonPairs LookupToJsonValues(this Lookup<object> lookup)
        {
            var jps = new JsonPairs();
            lookup.LookupToJsonValues(jps);
            return jps;
        }

        public static void LookupToJsonValues(this Lookup<object> lookup, JsonPairs jps)
        {
            foreach (var kvp in lookup)
            {
                var key = kvp.Key;
                var value = kvp.Value;
                JsonNode jv;
                switch (value)
                {
                    case int ival:
                        jv = new JsonValue<Numeric> { Value = new Numeric(ival.ToString()) };
                        break;
                    case bool bval:
                        jv = new JsonValue<bool> { Value = bval };
                        break;
                    default:
                        jv = new JsonValue<string> { Value = value.ToString() };
                        break;
                }
                jps.KeyValues.Add(key, jv);
            }
        }

        public static void LookupFromJsonValues(this Lookup<object> lookup, JsonPairs jps, Predicate<Tuple<string, JsonNode>> canAdd = null)
        {
            foreach(var kvp in jps.KeyValues)
            {
                var key = kvp.Key;
                var jv = kvp.Value;
                if (canAdd?.Invoke(new Tuple<string, JsonNode>(key, jv)) == false)
                {
                    continue;
                }
                object val;
                switch (jv)
                {
                    case JsonValue<Numeric> jvn:
                        if (int.TryParse(jvn.Value.Value, out var ival))
                        {
                            val = ival;
                        }
                        else
                        {
                            val = jvn.ToString();
                        }
                        break;
                    case JsonValue<bool> jvb:
                        val = jvb.Value;
                        break;
                    default:
                        val = jv.ToString();
                        break;
                }
                lookup.Add(key, val);
            }
        }
    }
}
