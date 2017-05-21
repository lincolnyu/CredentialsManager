using JsonParser.JsonStructures;

namespace CredentialsManagerLib.Models
{
    public interface IJsonSerializable
    {
        void FromJson(JsonNode jn);
        JsonNode ToJson();
    }
}
