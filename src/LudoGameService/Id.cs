using System;

namespace Ludo.API.Service
{
    // Ids are encoded left-to-right;
    // a numeric id of 1 with an encoding length of 3, would look like '100';
    // a numeric id of 4 with an encoding length of 3, would look like '400';
    // ...numeric 20 = 'k00' encoded, numeric 50 = 'e10', etc.
    public readonly struct Id : IEquatable<Id>, IEquatable<string>
    {
        public const int DefaultEncodedLength = 3;

        public readonly long Numeric;
        public readonly string Encoded;
        public bool HasValue => Encoded != null;
        // An Id is "complete" if it has both values (i.e. it's not partial and not default)
        public bool IsComplete => Numeric != PARTIAL && !IsPartialOrEmpty(Encoded);

        // ONLY use partial Ids as arguments in fast lookups and searches.
        // NEVER store a partial Id in a collection (call GetComplete)!
        // NEVER return a partial Id from a public API method!
        public Id(long numeric, int minimumEncodedLength = DefaultEncodedLength, bool partial = false)
        {   if (numeric < 0L)
                throw new ArgumentOutOfRangeException(nameof(numeric));
            Numeric = numeric;
            Encoded = partial ? string.Empty : new string(Encode(numeric, minimumEncodedLength));
        }
        public Id(string encoded, bool partial = false)
        {   Encoded = encoded;
            Numeric = partial ? PARTIAL : Decode(encoded);
        }

        public static Id Partial(string encoded) => new Id(encoded, partial: true);
        public static Id Partial(long numeric) => new Id(numeric, partial: true);

        // Gives a version of this Id that has both Encoding and Numeric value (i.e. not partial).
        // ALWAYS use this when you intend to store the id somewhere, and make sure it succeeds.
        // THROWS InvalidOperationException if !HasValue.
        public Id GetComplete(int minimumEncodedLength = DefaultEncodedLength)
        {
            if (Encoded == null) //!HasValue
                throw new InvalidOperationException("Id has no value.");
            if (Encoded.Length == 0)
                return new Id(Numeric, minimumEncodedLength);
            if (Numeric == PARTIAL)
                return new Id(Encoded);
            return this; // already complete
        }

        public static char PaddingCharacter => ALPHABET[0];

        public override bool Equals(object obj)
            => (obj is Id id && Equals(id))
            || (obj is string s && Equals(s))
            || (obj is long num && Equals(num));

        public bool Equals(Id other)
            => (Numeric == PARTIAL | other.Numeric == PARTIAL)
            ? !IsPartialOrEmpty(Encoded) && Encoded == other.Encoded
            : Numeric == other.Numeric;

        public bool Equals(string other)
            => !IsPartialOrEmpty(other) && other == Encoded;

        public bool Equals(long other)
            => other != PARTIAL && other == Numeric;

        public override int GetHashCode()
            => (Numeric == PARTIAL ? TryDecode(Encoded, out long num) ? num : 0L : Numeric).GetHashCode();

        public override string ToString()
            => IsPartialOrEmpty(Encoded) ? new string(Encode(Numeric, DefaultEncodedLength)) : Encoded;

        public static char[] Encode(long id, int minimumLength = DefaultEncodedLength)
        {   Recursive(id, 0, out char[] r);
            return r;

            void Recursive(long div, int i, out char[] result)
            {   if (div == 0L) // end reached
                {   if (i < minimumLength) // padding?
                        for (result = new char[minimumLength]; i < result.Length; ++i)
                            result[i] = PaddingCharacter;
                    else
                        result = new char[i];
                }
                else // recurse deeper
                {   Recursive(div / ALP_LENGTH, i + 1, out result); // going down...
                    result[i] = ALPHABET[unchecked((int)(div % ALP_LENGTH))]; // coming back up!
        }   }   }

        public static long Decode(string encoded)
        {   long result = 0L;
            long mul = 1L;
            for (int i = 0; i < encoded.Length; ++i)
            {   if (!TryDecode(encoded[i], out int v))
                    throw new ArgumentException($"Illegal character '{encoded[i]}'.");
                result += v * mul;
                mul = checked(mul * ALP_LENGTH);
            }
            return result;
        }

        public static bool TryDecode(string encoded, out long result)
        {   result = 0L;
            long mul = 1L;
            for (int i = 0; i < encoded.Length; ++i)
            {   if (mul > MULT_LIMIT)
                    return false;
                if (!TryDecode(encoded[i], out int v))
                    return false;
                result += v * mul;
                mul *= ALP_LENGTH;
            }
            return true;
        }

        // IMPORTANT: !IsComplete does NOT imply much !!
        // There are 3 options: empty, partial, complete. (empty == !HasValue)
        private bool IsPartialOrEmpty(string s)
            => string.IsNullOrEmpty(s);

        private static bool TryDecode(char c, out int value)
        {   if (c >= '0' & c <= '9')
                value = c - '0';
            else if (c >= 'a' & c <= 'z')
                value = c - 'a' + 10;
            else if (c == '-')
                value = 35; // ALP_LENGTH - 1
            else
                return (value = 0) != 0; // (always False)
            return true;
        }

        private const string ALPHABET = // DO NOT TOUCH!
            "01234" + "56789" + // 0 - 9
            "abcde" + "fghij" + // 10-19
            "klmno" + "pqrst" + // 20-29
            "uvxyz" + "-";      // 30-35
        private const long ALP_LENGTH = 36; // DO NOT TOUCH!
        private const long MULT_LIMIT = long.MaxValue / (ALP_LENGTH * 2);
        private const long PARTIAL = -1L;
    }
}
