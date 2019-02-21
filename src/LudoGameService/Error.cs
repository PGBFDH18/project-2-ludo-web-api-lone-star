namespace Ludo.GameService
{
    public class Error
    {
        public Error(int errCode, string description = null)
        {
            Code = errCode;
            Desc = description;
        }

        // Error code (required).
        public int Code { get; }

        // Optional human readable description.
        public string Desc { get; }
    }
}
