using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ludo.GameService
{
    // Simple shared implementation of IUserIdArray
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

        int IUserIdArray.OpenCount => 0;

        bool IUserIdArray.IsEmpty => players.All(string.IsNullOrEmpty);

        public IEnumerator<string> GetEnumerator()
            => players.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => players.GetEnumerator();
    }
}
