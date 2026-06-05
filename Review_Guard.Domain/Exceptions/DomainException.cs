namespace Review_Guard.Domain.Exceptions;

public class DomainException : Exception
{
    public string ErrorCode { get; }

    public string MessageKey { get; }

    public DomainException(
        string errorCode,
        string messageKey)
        : base(messageKey)
    {
        ErrorCode = errorCode;
        MessageKey = messageKey;
    }
}