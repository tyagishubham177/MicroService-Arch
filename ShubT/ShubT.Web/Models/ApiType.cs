namespace ShubT.Web.Models
{
    public class ProjectEnums
    {
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE,
            PATCH
        }

        public enum ContentType
        {
            Json,
            MultipartFormData,
        }
    }
}
