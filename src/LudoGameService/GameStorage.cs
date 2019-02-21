using System.Collections;
using System.Collections.Generic;

namespace Ludo.GameService
{
    // Thread-safe (hopefully)
    public class GameStorage : IEnumerable<KeyValuePair<Id, GameData>>
    {
        // TODO: move to ctor? set minEncodeLength?
        private readonly IdStorage<GameData> ids = new IdStorage<GameData>();

        // inject validation?
        public Id CreateGame(GameData data)
            => data == null ? default : ids.Add(data);

        // Accepts partial Ids.
        public bool ContainsId(in Id id)
            => ids.Contains(in id);

        // Accepts partial Ids.
        public GameData TryGet(in Id id)
            => ids.TryGet(in id, out var data) ? data : null;

        //public bool TryGetUserName(Id id, out string userName)
        //    => ids.TryGet(id, out var data) & (userName = data?.UserName) != null;
        
        // "slow" locking operation!
        public ICollection<Id> CreateIdSnapshot()
            => ids.CreateIdSnapshot();

        public IEnumerator<KeyValuePair<Id, GameData>> GetEnumerator()
            => ids.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ids.GetEnumerator();
    }
}
