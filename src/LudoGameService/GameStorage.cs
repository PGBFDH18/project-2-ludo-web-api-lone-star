using System.Collections;
using System.Collections.Generic;

namespace Ludo.API.Service
{
    // Thread-safe (hopefully)
    public class GameStorage : IEnumerable<KeyValuePair<Id, Game>>
    {
        // TODO: move to ctor? set minEncodeLength?
        private readonly IdStorage<Game> ids = new IdStorage<Game>();

        // inject validation?
        public Id CreateGame(Game data)
            => data == null ? default : ids.Add(data);

        // Accepts partial Ids.
        public bool ContainsId(in Id id)
            => ids.Contains(in id);

        // Accepts partial Ids.
        public Game TryGet(in Id id)
            => ids.TryGet(in id, out var data) ? data : null;

        // Accepts partial Ids.
        internal void Remove(in Id id, out Game game)
            => ids.TryRemove(in id, out game);

        //public bool TryGetUserName(Id id, out string userName)
        //    => ids.TryGet(id, out var data) & (userName = data?.UserName) != null;
        
        // "slow" locking operation!
        public ICollection<Id> CreateIdSnapshot()
            => ids.CreateIdSnapshot();

        public IEnumerator<KeyValuePair<Id, Game>> GetEnumerator()
            => ids.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ids.GetEnumerator();
    }
}
