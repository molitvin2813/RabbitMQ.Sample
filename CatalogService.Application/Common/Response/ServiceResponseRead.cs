using SharedCollection.AbstractClass;

namespace CatalogService.Application.Common.Response
{
    public class ServiceResponseRead<T> : BaseServiceResponse
    {
        public ServiceResponseRead() : base() { }

        public ServiceResponseRead(T data, bool success, string message)
            : base(success, message)
        {
            Data = data;
        }

        public T? Data { get; set; }
    }
}
