namespace ReimbursementTrackingApplication.Exceptions{
    public class UnauthorizedException:Exception
    {
        string _message;
        public UnauthorizedException()
        {
            _message = $"Unauthorized";
        }
        override public string Message => _message;
    }
}
