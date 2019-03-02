using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ludo.GameService
{
    // Simple shared implementation of IUserIdArray
    internal class SlotArray : ISlotArray
    {
        private readonly string[] slots;

        // does a copy
        public SlotArray(ISlotArray slots)
        {
            var p = new string[slots.Length];
            for (int i = 0; i < p.Length; ++i)
                p[i] = slots[i];
            this.slots = p;
        }

        // does NOT copy!
        public SlotArray(string[] slots)
        {
            this.slots = slots ?? throw new ArgumentNullException(nameof(slots));
        }

        public string this[int i] => slots[i];

        public int Length => slots.Length;

        public int PlayerCount => slots.Length - slots.Count(string.IsNullOrEmpty);

        bool ISlotArray.IsEmpty => slots.All(string.IsNullOrEmpty);

        public IEnumerator<string> GetEnumerator()
            => slots.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => slots.GetEnumerator();
    }
}
