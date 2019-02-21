namespace Ludo.WebAPI.Models
{
    public class ErrorCode
    {
        public int Code { get; set; }
        public string Desc { get; set; }

        public static implicit operator ErrorCode (GameService.Error error)
            => error == null ? null
            : new ErrorCode { Code = error.Code, Desc = error.Desc };
    }
}