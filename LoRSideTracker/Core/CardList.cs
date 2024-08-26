using System;
using System.Collections;
using System.Collections.Generic;

namespace LoRSideTracker
{
    /// <summary>
    /// CardList IEnumerator
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CardListEnum<T> : IEnumerator<T>
    {
        CardList<T> TheCardList;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int Position = -1;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="list"></param>
        public CardListEnum(CardList<T> list)
        {
            TheCardList = list;
        }

        /// <summary>
        /// IEnumerator MoveToNext
        /// </summary>
        /// <returns>tru if position still in range</returns>
        public bool MoveNext()
        {
            Position++;
            return (Position < TheCardList.Count);
        }

        /// <summary>
        /// IEnumerator Reset
        /// </summary>
        public void Reset()
        {
            Position = -1;
        }

        /// <summary>
        /// IDosposable Dospose, does nothing
        /// </summary>
        void IDisposable.Dispose()
        {
        }

        /// <summary>
        /// IEnumarator Current accessor
        /// </summary>
        object IEnumerator.Current
        {
            get => Current;
        }

        /// <summary>
        /// Specialized Current accessor
        /// </summary>
        public T Current
        {
            get
            {
                try
                {
                    return TheCardList[Position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }

    /// <summary>
    /// List of cards, always sorted, allows duplicates
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CardList<T> : IEnumerable<T>
    {
        class CardKey : IComparable, ICloneable
        {
            public readonly int Cost;
            public readonly string Name;

            public CardKey(int cost, string name)
            {
                Cost = cost;
                Name = name;
            }

            public int CompareTo(object obj)
            {
                CardKey other = (CardKey)obj;
                int result = Cost.CompareTo(other.Cost);
                return (result == 0) ? Name.CompareTo(other.Name) : result;
            }

            public object Clone()
            {
                return MemberwiseClone();
            }
        }

        SortedList<CardKey, T> TheList;

        /// <summary>Number of items </summary>
        public int Count { get => TheList.Count; }

        /// <summary>Value accessor</summary>
        public T this[int n] { get => TheList.Values[n]; }

        /// <summary>
        /// Constructor
        /// </summary>
        public CardList()
        {
            TheList = new SortedList<CardKey, T>(new DuplicateKeyComparer<CardKey>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cost"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Add(int cost, string name, T value)
        {
            TheList.Add(new CardKey(cost, name), value);
        }

        void Add(CardKey key, T value)
        {
            TheList.Add(key, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void AddRange(CardList<T> other)
        {
            foreach (var kvp in other.TheList)
            {
                TheList.Add(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Clear the list
        /// </summary>
        public void Clear()
        {
            TheList.Clear();
        }

        /// <summary>
        /// Remove specific item
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            TheList.RemoveAt(index);
        }

        /// <summary>
        /// Check if this deck is same as the other deck
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool SequenceEquals(CardList<T> other)
        {
            if (Count != other.Count) return false;
            for (int i = 0; i < Count; i++)
            {
                // We only focus on keys here, value contents are not considered
                CardKey a = TheList.Keys[i];
                CardKey b = other.TheList.Keys[i];
                if (a.Cost != b.Cost || !a.Name.Equals(b.Name))
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// Extract all matching instances. IMPORTANT: First list items are copied to resulting list.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="fn">Comparator function, returns 0 for matches</param>
        /// <param name="combineFn">If specified, function that takes matching members of a and b and produces resulting value</param>
        /// <returns></returns>
        public static CardList<T> Extract(ref CardList<T> a, ref CardList<T> b, Func<T, T, int> fn, Func<T, T, T> combineFn = null)
        {
            // Example:
            // CardList<CardInPlay>.Extract(a, b, (x, y) => 
            // {
            //     int result = x.TheCard.Cost - y.TheCard.Cost;
            //     if (result == 0) result = x.TheCard.Name.CompareTo(y.TheCard.Name);
            //     return result;
            // });

            CardList<T> result = new CardList<T>();
            int i = 0;
            int j = 0;
            while (i < a.Count && j < b.Count)
            {
                int cmpResult = fn(a[i], b[j]);
                if (cmpResult > 0)
                {
                    j++;
                }
                else if (cmpResult < 0)
                {
                    i++;
                }
                else
                {
                    if (combineFn != null)
                    {
                        result.Add(a.TheList.Keys[i], combineFn(a[i], b[j]));
                    }
                    else
                    {
                        result.Add(a.TheList.Keys[i], a[i]);
                    }
                    a.RemoveAt(i);
                    b.RemoveAt(j);
                }
            }

            return result;
        }

        /// <summary>
        /// Extract subset based on predicate
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        public CardList<T> GetSubset(Predicate<T> fn)
        {
            CardList<T> result = new CardList<T>();
            for (int i = 0; i < Count; i++)
            {
                if (fn(this[i]))
                {
                    result.Add(TheList.Keys[i], TheList.Values[i]);
                }
            }
            return result;
        }

        /// <summary>
        /// Extract subset based on predicate
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        public CardList<T> ExtractSubset(Predicate<T> fn)
        {
            CardList<T> result = new CardList<T>();
            int i = 0;
            while (i < Count)
            {
                if (fn(this[i]))
                {
                    result.Add(TheList.Keys[i], TheList.Values[i]);
                    RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            return result;
        }

        /// <summary>
        /// Get two sets based on precidate
        /// </summary>
        /// <param name="setWhenTrue"></param>
        /// <param name="setWhenFalse"></param>
        /// <param name="fn"></param>
        public void Split(ref CardList<T> setWhenTrue, ref CardList<T> setWhenFalse, Predicate<T> fn)
        {
            foreach (var card in TheList)
            {
                if (fn(card.Value))
                {
                    setWhenTrue.Add(card.Key, card.Value);
                }
                else
                {
                    setWhenFalse.Add(card.Key, card.Value);

                }
            }
        }

        /// <summary>
        /// Find index of first member that matches given predicate criterion
        /// </summary>
        /// <param name="fn"></param>
        /// <param name="startIndex"></param>
        /// <returns>member index, or -1 if not found</returns>
        public int FindIndex(Predicate<T> fn, int startIndex = 0)
        {
            CardList<T> result = new CardList<T>();
            for (int i = startIndex; i < Count; i++)
            {
                if (fn(this[i]))
                {
                    return i;
                }
            }
            // Not found
            return -1;

        }

        /// <summary>
        /// Clone function
        /// </summary>
        /// <returns></returns>
        public CardList<T> Clone()
        {
            CardList<T> result = new CardList<T>();
            foreach (var card in TheList)
            {
                result.Add(card.Key, card.Value);
            }
            return result;
        }

        /// <summary>
        /// IEnumarable interface
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new CardListEnum<T>(this);
        }

        /// <summary>
        /// IEnumarable interface
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
