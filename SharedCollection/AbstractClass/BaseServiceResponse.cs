using SharedCollection.Interfaces;

namespace SharedCollection.AbstractClass
{
    public abstract class BaseServiceResponse : IServiceResponse
    {
        public BaseServiceResponse()
        {
            Success = true;
            Message = string.Empty;
        }

        public BaseServiceResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public bool Success { get; set; }

        public string Message { get; set; }
    }
}