using SharedCollection.AbstractClass;

namespace OrderService.Application.Common.Response
{
    public class ServiceResponseError : BaseServiceResponse
    {
        public ServiceResponseError() : base() { }

        public ServiceResponseError(bool success, string message) : base(success, message) { }
    }
}
