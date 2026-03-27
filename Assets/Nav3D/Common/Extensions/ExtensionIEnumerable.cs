using System;
using System.Collections.Generic;
using System.Linq;

namespace Nav3D.Common
{
    public static class ExtensionIEnumerable
    {
        //specific list copy implementation
        public static List<T> Copy<T>(this List<T> _List)
        {
            return new List<T>(_List);
        }

        public static Dictionary<K, V> Copy<K, V>(this Dictionary<K, V> _Dictionary)
        {
            Dictionary<K, V> dictionary = new Dictionary<K, V>(_Dictionary.Count);

            _Dictionary.ForEach(_Kvp => dictionary.Add(_Kvp.Key, _Kvp.Value));

            return dictionary;
        }

        public static void AddRange<T>(this HashSet<T> _HashSet, IEnumerable<T> _Enumerable)
        {
            _Enumerable.ForEach(_Element => _HashSet.Add(_Element));
        }
        
        public static void AddRange<K, V>(this Dictionary<K, V> _Dictionary, Dictionary<K, V> _OtherDictionary)
        {
            _OtherDictionary.ForEach(_Kvp => _Dictionary.Add(_Kvp.Key, _Kvp.Value));
        }

        //Generic template
        public static P Copy<P, T>(this P _Enumerable) where P : IEnumerable<T>, IList<T>, new()
        {
            P enumerable = new P();

            foreach (T element in _Enumerable)
            {
                enumerable.Add(element);
            }

            return enumerable;
        }

        public static void ForEach<T>(this IEnumerable<T> _Enumerable, Action<T> _Action)
        {
            foreach (T item in _Enumerable)
                _Action(item);
        }

        public static T MinBy<T>(this IEnumerable<T> _Enumerable, Func<T, float> _MinFunc)
        {
            return _Enumerable.OrderBy(_Elemet => _MinFunc(_Elemet)).First();
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> _Enumerable)
        {
            return _Enumerable == null || !_Enumerable.Any();
        }
    }
}