using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Ludo.API.Service
{
    // Thread-safe
    // Use as component inside classes that manage Ids
    public sealed class IdStorage<TData> : IEnumerable<KeyValuePair<Id, TData>>
    {
        public IdStorage(byte minEncodeLength = Id.DefaultEncodedLength, long IdCounterStartingValue = 0L)
        {   _idCounter = IdCounterStartingValue;
            MinEncodeLength = minEncodeLength;
        }

        private long _idCounter;
        private readonly ConcurrentDictionary<Id, TData> dict
            = new ConcurrentDictionary<Id, TData>();

        public int MinEncodeLength { get; }

        // Number of ids issued by this instance. (NOT the same as number of users!)
        public long IdCounter => _idCounter;

        // Creates the next id.
        public Id Next() => new Id(IncrementIdCounter(), MinEncodeLength);

        public Id Add(TData data)
        {   Id id;
            do id = Next();
            while(!dict.TryAdd(id, data));
            return id;
        }

        // ALL these methods accept partial Ids, since they are expanded to complete ids as needed.

        public void AddOrSet(in Id id, TData data)
            => dict[id.GetComplete(MinEncodeLength)] = data;

        public bool TryAdd(in Id id, TData data)
            => dict.TryAdd(id.GetComplete(MinEncodeLength), data);

        public bool TryGet(in Id id, out TData data)
            => dict.TryGetValue(id, out data);

        public bool TryRemove(in Id id, out TData data)
            => dict.TryRemove(id, out data);

        public bool TryUpdate(in Id id, TData newValue, TData comparisonValue)
            => dict.TryUpdate(id.GetComplete(MinEncodeLength), newValue, comparisonValue); // Not sure .GetComplete() is needed here?

        public bool Contains(in Id id)
            => dict.ContainsKey(id);

        // "slow" locking operation!
        public ICollection<TData> CreateDataSnapshot()
            => dict.Values;

        // "slow" locking operation!
        public ICollection<Id> CreateIdSnapshot()
            => dict.Keys;

        public IEnumerator<KeyValuePair<Id, TData>> GetEnumerator()
            => dict.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => dict.GetEnumerator();

        // CARE: Interlocked.Increment can wrap around to a neg value.
        private long IncrementIdCounter()
            => Interlocked.Increment(ref _idCounter);
    }
}
