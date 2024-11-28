namespace ReimbursementTrackingApplication.Exceptions
{
    public class UnauthorizedException : Exception
    {
        string _message;
        public UnauthorizedException(string message)
        {
            _message = message;
        }
        override public string Message => _message;
    }
}
