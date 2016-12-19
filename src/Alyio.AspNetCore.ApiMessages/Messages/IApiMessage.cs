namespace Alyio.AspNetCore.ApiMessages
{
    public interface IApiMessage
    {
        int StatusCode { get; }

        ApiMessage ApiMessage { get; }
    }
}
