using SharedCollection.AbstractClass;

namespace OrderService.Application.Common.Response
{
    public class ServiceResponseWrite<T>
        : BaseServiceResponse
        where T : struct
    {
        public ServiceResponseWrite() : base() { }
        public ServiceResponseWrite(bool success, string message, T key) : base(success, message)
        {
            Key = key;
        }

        public T? Key { get; set; }
    }
}
