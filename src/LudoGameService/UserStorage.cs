using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Ludo.API.Service
{
    // Thread-safe (hopefully)
    public class UserStorage : IEnumerable<KeyValuePair<Id, UserData>>
    {
        // TODO: move to ctor? set minEncodeLength?
        private readonly IdStorage<UserData> ids = new IdStorage<UserData>();
        private readonly ConcurrentDictionary<string, Id> usernames = new ConcurrentDictionary<string, Id>();

        // TODO: inject validation here?
        // TODO: Error codes.
        public bool TryCreateUser(string userName, out Id id)
        {
            if (string.IsNullOrEmpty(userName))
            {   id = default;
                return false;
            }
            if (usernames.TryAdd(userName, default))
            {   
                var data = new UserData { UserName = userName };
                id = ids.Add(data);
                if (usernames.TryUpdate(userName, id, default))
                    return true;
                ids.TryRemove(id, out _);
                return false;
            }
            id = default;
            return false;
        }

        // This returns true even for users which are in the process of being created.
        public bool ContainsUserName(string userName)
            => usernames.ContainsKey(userName);

        // This returns true even for users which are in the process of being created.
        // Accepts partial Ids.
        public bool ContainsId(in Id id)
            => ids.Contains(in id);

        // Accepts partial Ids.
        public bool TryGetUserName(in Id id, out string userName)
            => ids.TryGet(in id, out var data) & (userName = data?.UserName) != null;

        // This returns true only for users which have been fully created.
        public bool TryGetId(string userName, out Id id)
            => usernames.TryGetValue(userName, out id) && id.HasValue;

        // "slow" locking operation!
        public ICollection<Id> CreateIdSnapshot()
            => usernames.Values;

        public IEnumerator<KeyValuePair<Id, UserData>> GetEnumerator()
            => ids.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ids.GetEnumerator();
    }
}
