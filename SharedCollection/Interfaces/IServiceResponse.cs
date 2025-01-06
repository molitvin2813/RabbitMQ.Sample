namespace SharedCollection.Interfaces
{
    public interface IServiceResponse
    {
        bool Success { get; set; }
        string Message { get; set; }
    }
}
