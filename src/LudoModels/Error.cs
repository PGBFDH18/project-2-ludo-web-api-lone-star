using System;

namespace Ludo.API.Models
{
    public readonly partial struct Error : IEquatable<Error>
    {
        public Error(int code, string desc = null)
        {
            Code = code;
            Desc = desc;
        }

        // Error code (required).
        public int Code { get; }

        // Optional human readable description or other additional detail(s).
        public string Desc { get; }

        public override bool Equals(object obj)
            => obj is Error ec && Equals(ec);

        public bool Equals(Error other)
            => Code == other.Code;

        public override int GetHashCode() => Code;

        public override string ToString()
            => Desc ?? Code.ToString();

        public static bool operator ==(Error error, int code)
            => error.Code == code;
        public static bool operator !=(Error error, int code)
            => error.Code != code;

        public static implicit operator Error(int errCode)
            => new Error(errCode); // TODO: auto-map default descriptions based on error code

        public static implicit operator Error((int, string) errPair)
            => new Error(errPair.Item1, errPair.Item2);

        // a lot of if statements test for NoError, so lets simplify:
        public static implicit operator bool(Error err)
            => err.Code != Codes.E00NoError;
    }
}