namespace MyTemplate.APIResponses;

public class Response
{
    public bool IsSuccess { get; }
    public Error Error { get;  } = default!;


    public Response(bool isSuccess, Error error)
    {

        if ((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Error= error;
    }
    public Response Success() => new Response(true,Error.None);

    public Response Failure(Error error) => new Response(false, error);

    public static Response<T> Success<T>(T data) => new(data,true,Error.None);
    public static Response<T> Failure<T>(Error error) => new(default,false,error);


}


public class Response<T>:Response
{
    private readonly T? _value;
    public Response(T? value,bool isSuccess,Error error):base(isSuccess,error)
    {
        _value = value;
    }

    public T Value => IsSuccess ? _value! : throw new InvalidOperationException("Cannot access the value of a failed response.");

}
