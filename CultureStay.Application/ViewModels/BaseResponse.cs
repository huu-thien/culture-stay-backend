namespace CultureStay.Application.ViewModels;

public class BaseResponse<T>
{
    public string Message { get; set; } = string.Empty;
    public T Data { get; set; } = default!;
    

}

public class BaseResponse
{
    public string Message { get; set; } = string.Empty;
}