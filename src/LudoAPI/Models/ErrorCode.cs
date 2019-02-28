using System;

namespace Ludo.WebAPI.Models
{
    public readonly struct ErrorCode : IEquatable<ErrorCode>
    {
        public ErrorCode(int code, string desc = null)
        {
            Code = code;
            Desc = desc;
        }

        public int Code { get; }
        public string Desc { get; }

        public override bool Equals(object obj)
            => obj is ErrorCode ec && Equals(ec);

        public bool Equals(ErrorCode other) => Code == other.Code;

        public override int GetHashCode() => Code;

        public override string ToString()
            => Desc ?? Code.ToString();

        public static bool operator ==(ErrorCode error, int code)
            => error.Code == code;
        public static bool operator !=(ErrorCode error, int code)
            => error.Code != code;

        public static implicit operator ErrorCode (GameService.Error error)
            => new ErrorCode(code:error.Code, desc:error.Desc);

        public static implicit operator ErrorCode (int errorCode)
            => new ErrorCode(errorCode);
    }
}