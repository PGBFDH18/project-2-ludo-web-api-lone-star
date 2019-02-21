using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Ludo.GameService
{
    internal class UserArray : IUserIdArray
    {
        private readonly string[] players;

        public UserArray(IUserIdArray players)
        {
            var p = new string[players.Length];
            for (int i = 0; i < p.Length; ++i)
                p[i] = players[i];
            this.players = p;
        }

        // does NOT copy!
        public UserArray(string[] players)
        {
            this.players = players ?? throw new ArgumentNullException(nameof(players));
        }

        public string this[int i] => players[i];

        public int Length => players.Length;

        public IEnumerator<string> GetEnumerator()
            => players.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => players.GetEnumerator();
    }
}
