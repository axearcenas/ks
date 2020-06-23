using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace KeepSafe.Extension
{
    public static class ObservableCollectionExtension
    {
        public static void UnselectAllItems<T>(this ObservableCollection<T> list) where T : ISelect
        {
            if (list.ToList().Exists(x => x.IsSelected))
            {
                list.FirstOrDefault(x => x.IsSelected).IsSelected = false;
            }
        }

        public static void UnselectItemById<T>(this ObservableCollection<T> list, int id) where T : ISelect
        {
            if (list.ToList().Exists(x => x.Id == id))
            {
                list.FirstOrDefault(x => x.Id == id).IsSelected = false;
            }
        }

        public static void SelectItemById<T>(this ObservableCollection<T> list, int id) where T : ISelect
        {
            if (list.ToList().Exists(x => x.Id == id))
            {
                list.FirstOrDefault(x => x.Id == id).IsSelected = true;
            }
        }

        public static void ClearDefault<T>(this ObservableCollection<T> list) where T : IDefault
        {
            if (list.ToList().Exists(x => x.IsDefault))
            {
                list.FirstOrDefault(x => x.IsDefault).IsDefault = false;
            }
        }

        public static void SetDefault<T>(this ObservableCollection<T> list, int id) where T : IDefault
        {
            if (list.ToList().Exists(x => x.Id == id))
            {
                list.FirstOrDefault(x => x.Id == id).IsDefault = true;
            }
        }

        public static void AddItem<T>(this ObservableCollection<T> list, T item) where T : IAddItem
        {
            if (list.ToList().Exists(x => x.Id == item.Id))
            {
                App.Log("Updating Item!");
                list.FirstOrDefault(x => x.Id == item.Id).Update(item);
            }
            else
            {
                App.Log("Adding Item!");
                list.Add(item);
            }
        }
    }
}
