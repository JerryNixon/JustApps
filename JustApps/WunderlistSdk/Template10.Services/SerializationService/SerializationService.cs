namespace Template10.Services.SerializationService
{
    internal static class SerializationService
    {
        private static ISerializationService _json;
        public static ISerializationService Json => _json ?? (_json = new JsonSerializationService());
    }
}