using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DoubleDic.Utilities;
using JetBrains.Annotations;

namespace DoubleDic
{
    [PublicAPI]
    public class DoubleDic<TKey, TValue>
    {
        [NotNull] public IReadOnlyDictionary<TKey, TValue> Exposed => _exposed;
        [NotNull] private readonly Dictionary<TKey, TValue> _exposed = new Dictionary<TKey, TValue>();

        [NotNull] public IReadOnlyDictionary<TKey, TValue> Redacted => _redacted;
        [NotNull] private readonly Dictionary<TKey, TValue> _redacted = new Dictionary<TKey, TValue>();

        [NotNull] public HashSet<TKey> SensitiveKeys { get; }

        [NotNull] private readonly Func<TKey, TValue> _replacementFun;

        [PublicAPI]
        public DoubleDic(
            Func<TKey, TValue> replacementFun,
            params TKey[] sensitiveKeys)
            : this(replacementFun, (IEnumerable<TKey>)sensitiveKeys) { }

        [PublicAPI]
        public DoubleDic(
            Func<TKey, TValue> replacementFun,
            IEnumerable<TKey> sensitiveKeys = default)
        {
            Preconditions.CheckNotNull(replacementFun, nameof(replacementFun));

            SensitiveKeys = new HashSet<TKey>(sensitiveKeys ?? Enumerable.Empty<TKey>());
            _replacementFun = replacementFun;
        }

        [PublicAPI]
        public DoubleDic(
            TValue replacementValue,
            params TKey[] sensitiveKeys)
            : this(replacementValue, (IEnumerable<TKey>)sensitiveKeys) { }

        [PublicAPI]
        public DoubleDic(
            [CanBeNull] TValue replacementValue,
            IEnumerable<TKey> sensitiveKeys = default)
            : this(key => replacementValue, sensitiveKeys) { }

        [PublicAPI]
        public TValue this[TKey key]
        {
            set
            {
                _redacted[key] = value;
                _exposed[key] = value;
            }
        }

        [PublicAPI]
        public bool Remove(TKey key)
        {
            _redacted.Remove(key);
            return _exposed.Remove(key);
        }

    }
}
