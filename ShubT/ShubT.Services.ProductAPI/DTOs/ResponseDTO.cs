namespace ShubT.Services.ProductAPI.DTOs
{
    public class ResponseDTO
    {
        public object? Result { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string DisplayMessage { get; set; } = string.Empty;
    }
}
