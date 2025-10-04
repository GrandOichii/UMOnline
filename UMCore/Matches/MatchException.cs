namespace UMCore;

[Serializable]
public class MatchException : Exception
{
    public MatchException(string message) : base(message) { }
    public MatchException(string message, Exception inner) : base(message, inner) { }
}