using System.Collections;
using System.Collections.Generic;

namespace Ludo.GameService
{
    // Thread-safe (hopefully)
    public class GameStorage : IEnumerable<KeyValuePair<Id, GameData>>
    {
        // TODO: move to ctor? set minEncodeLength?
        private readonly IdStorage<GameData> ids = new IdStorage<GameData>();

        // TODO: inject validation?
        // TODO: Error codes.
        public bool TryCreateGame(GameData data, out Id id)
        {
            if (data == null)
            {   id = default;
                return false;
            }
            else
            {   id = ids.Add(data);
                return true;
            }
        }

        // Accepts partial Ids.
        public bool ContainsId(in Id id)
            => ids.Contains(in id);

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
