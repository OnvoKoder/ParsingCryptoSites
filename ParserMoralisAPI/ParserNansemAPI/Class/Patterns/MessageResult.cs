namespace ParserNansemAPI.Class.Patterns
{
    public class MessageResult
    {
        public string Message { get; }
        public bool IsError { get; }
        public MessageResult(string message, bool error)
        {
            Message = message;
            IsError = error;
        }
    }
}
