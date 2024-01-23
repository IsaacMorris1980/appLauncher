using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace appLauncher.Core.Extensions
{
    public static class ExtensionMethods
    {
        public static int Remove<T>(
            this ObservableCollection<T> coll, Func<T, bool> condition)
        {
            var itemsToRemove = coll.Where(condition).ToList();
            foreach (var itemToRemove in itemsToRemove)
            {
                coll.Remove(itemToRemove);
            }
            return itemsToRemove.Count;
        }
        public static int Remove<T>(
           this List<T> coll, Func<T, bool> condition)
        {
            List<T> itemsToRemove = coll.Where(condition).ToList();
            foreach (var itemToRemove in itemsToRemove)
            {
                coll.Remove(itemToRemove);
            }
            return itemsToRemove.Count;
        }

    }
}
