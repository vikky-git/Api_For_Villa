using System.Net;
using System.Reflection.Metadata;

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

        //Make A static Factorty Methods

        public static ApiResponse<TData> Create(bool _success,int _statuscode,string _message,TData? _data=default,
            object?_error=null)
        {
            return new ApiResponse<TData>()
            {
                Success = _success,
                StatusCode = _statuscode,
                Message = _message,
                Data = _data,
                Error = _error
            };
        }
        public static ApiResponse<TData> OK(TData data,string message)=>        
            Create(true, 200, message, _data: data);
        

        public static ApiResponse<TData> CreatedAt(TData data, string message)
        {
           return Create(true, 201, message, data);
        }
        public static ApiResponse<TData> NotFound(string message = "Resource Not Found") =>
            Create(false,404,message);

        public static ApiResponse<TData> BadRequest(string message, object? errors = null) =>
            Create(false, Convert.ToInt32(HttpStatusCode.BadRequest), message,_error: errors);  //for Bad request 400

        public static ApiResponse<TData> Conflict(string message) =>
            Create(false, 409, message);
        public static ApiResponse<TData> NoContent(string message="Operation Completed Successfully") =>
            Create(true, 204, message);

        public static ApiResponse<TData> Errors(int statuscode, string message, object? errors = null) =>
            Create(false, statuscode, message, _error: errors);
        /*What does _error: errors mean?
        It means:
            Pass the value of variable errors to the parameter named _error.*/

    }
}
