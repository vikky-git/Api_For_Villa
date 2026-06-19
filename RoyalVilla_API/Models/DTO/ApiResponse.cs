namespace RoyalVilla_API.Models.DTO
{
    //This Class is used for make a common response FORMAT of ALL API
    public class ApiResponse<TData>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public TData? Data { get; set; }
        public Object? Error { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
