using System;

namespace KeepSafe.Extension
{
    public interface IObservableCollectionExtension
    {
        int Id { get; set; }
    }

    public interface ISelect : IObservableCollectionExtension
    {
        bool IsSelected { get; set; }
    }

    public interface IDefault : IObservableCollectionExtension
    {
        bool IsDefault { get; set; }
    }

    public interface IAddItem : IObservableCollectionExtension
    {
        void Update<T>(T item) where T : IAddItem;
    }
}
