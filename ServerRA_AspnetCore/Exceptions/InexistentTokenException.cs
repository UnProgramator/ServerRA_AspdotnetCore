namespace ServerRA_AspnetCore.Exceptions
{
    public class InexistentTokenException : Exception
    {
        public InexistentTokenException(string? mesage) : base(mesage) { }
    }
}
