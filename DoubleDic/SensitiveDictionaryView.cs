using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DoubleDic.Utilities;
using JetBrains.Annotations;

namespace DoubleDic
{
    internal class SensitiveDictionaryView<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        [NotNull] private readonly IReadOnlyDictionary<TKey, TValue> _dictionaryImplementation;

        [NotNull] [ItemCanBeNull] public IEnumerable<TKey> SensitiveKeys => _sensitiveKeys;
        [NotNull] [ItemCanBeNull] private readonly HashSet<TKey> _sensitiveKeys;

        [NotNull] private readonly Func<TKey, TValue> _replacementFunc;

        internal SensitiveDictionaryView(
            IReadOnlyDictionary<TKey, TValue> source,
            IEnumerable<TKey> sensitiveKeys,
            Func<TKey, TValue> replacementFunc)
        {
            // TODO - Do we want to expose this class? If not then can probably skip these null checks.
            _replacementFunc = Preconditions.CheckNotNull(replacementFunc, nameof(replacementFunc));
            _dictionaryImplementation = Preconditions.CheckNotNull(source, nameof(source));
            _sensitiveKeys = new HashSet<TKey>(sensitiveKeys ?? Enumerable.Empty<TKey>());
        }

        [PublicAPI]
        public void AddSensitive([CanBeNull] TKey key) => _sensitiveKeys.Add(key);

        [PublicAPI]
        public void RemoveSensitive([CanBeNull] TKey key) => _sensitiveKeys.Remove(key);

#region IReadOnlyDictionary Support

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        public int Count => _dictionaryImplementation.Count;

        public bool ContainsKey(TKey key) => _dictionaryImplementation.ContainsKey(key);

        public bool TryGetValue(TKey key, out TValue value)
        {
            if(!_dictionaryImplementation.ContainsKey(key))
            {
                value = default;
                return false;
            }

            value = this[key];
            return true;
        }

        public TValue this[TKey key] => _sensitiveKeys.Contains(key)
            ? _replacementFunc(key)
            : _dictionaryImplementation[key];

        public IEnumerable<TKey> Keys => _dictionaryImplementation.Keys;

        public IEnumerable<TValue> Values => _dictionaryImplementation.Keys.Select(key => this[key]);

#endregion

        internal class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            [NotNull] private readonly SensitiveDictionaryView<TKey, TValue> _source;

            [NotNull] private readonly IEnumerator<KeyValuePair<TKey, TValue>> _enumeratorImplementation;

            internal Enumerator(SensitiveDictionaryView<TKey, TValue> source)
            {
                _source = source;
                _enumeratorImplementation = source.GetEnumerator();
            }

            public void Dispose() => _enumeratorImplementation.Dispose();

            public bool MoveNext()
            {
                return _enumeratorImplementation.MoveNext();
            }

            public void Reset() => _enumeratorImplementation.Reset();

            public KeyValuePair<TKey, TValue> Current =>
                new KeyValuePair<TKey, TValue>(
                    _enumeratorImplementation.Current.Key,
                    _source[_enumeratorImplementation.Current.Key]);

            object IEnumerator.Current => Current;
        }

    }
}