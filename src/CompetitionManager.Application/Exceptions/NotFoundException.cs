namespace CompetitionManager.Application.Exceptions;

public sealed class NotFoundException : Exception
{
    public NotFoundException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}