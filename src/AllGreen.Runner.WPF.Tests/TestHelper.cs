using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FluentAssertions;

namespace AllGreen.Runner.WPF.Tests
{
    public class TestHelper
    {
        static public void AttachPropertyChangedEvent(object obj, PropertyChangedEventHandler propertyChangedEventHandler)
        {
            ((INotifyPropertyChanged)obj).PropertyChanged += propertyChangedEventHandler;
        }

        static public PropertyChangedTester<T> TestPropertyChanged<T>(T obj) where T : INotifyPropertyChanged
        {
            return new PropertyChangedTester<T>(obj);
        }

        static public CollectionChangedTester<T, TItem> TestCollectionChanged<T, TItem>(T obj) where T : INotifyCollectionChanged, IEnumerable<TItem>
        {
            return new CollectionChangedTester<T, TItem>(obj);
        }

        public class PropertyChangedTester<T> where T : INotifyPropertyChanged
        {
            T _Object;
            List<string> _ChangedProperties = new List<string>();

            public PropertyChangedTester(T obj)
            {
                this._Object = obj;
                ((INotifyPropertyChanged)obj).PropertyChanged += (sender, e) => _ChangedProperties.Add(e.PropertyName);
            }

            public PropertyChangedTester<T> Action(Action<T> action)
            {
                _ChangedProperties.Clear();
                action.Invoke(_Object);
                return this;
            }
            public PropertyChangedTester<T> Action(System.Action action)
            {
                return this.Action(o => action.Invoke());
            }
            public PropertyChangedTester<T> Changes(string propertyName)
            {
                _ChangedProperties.Contains(propertyName).Should().BeTrue("PropertyChanged event wasn't called for " + propertyName);
                return this;
            }
            /*public PropertyChangedTester<T> Changes<TProperty>(Expression<Func<T, TProperty>> propertySelector)
            {
                string propertyName = NotifyPropertyChangedHelper.GetPropertyName<T, TProperty>(_Object, propertySelector);
                return Changes(propertyName);
            }*/
        }

        public class CollectionChangedTester<T, TItem> where T : INotifyCollectionChanged, IEnumerable<TItem>
        {
            T _Object;
            List<object> _AddedObjects = new List<object>();
            List<object> _RemovedObjects = new List<object>();
            List<object> _ReplacedObjectsOld = new List<object>();
            List<object> _ReplacedObjectsNew = new List<object>();
            List<object> _MovedObjectsOld = new List<object>();
            List<object> _MovedObjectsNew = new List<object>();
            bool _Reset = false;

            public CollectionChangedTester(T obj)
            {
                this._Object = obj;
                obj.CollectionChanged += (sender, e) => StoreAction(e);
            }

            private void StoreAction(NotifyCollectionChangedEventArgs e)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (object item in e.NewItems)
                            _AddedObjects.Add(item);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (object item in e.OldItems)
                            _RemovedObjects.Add(item);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        for (int i = 0; i < e.OldItems.Count; i++)
                        {
                            _ReplacedObjectsOld.Add(e.OldItems[i]);
                            _ReplacedObjectsNew.Add(e.NewItems[i]);
                        }
                        break;
                    case NotifyCollectionChangedAction.Move:
                        for (int i = 0; i < e.OldItems.Count; i++)
                        {
                            _MovedObjectsOld.Add(e.OldItems[i]);
                            _MovedObjectsNew.Add(e.NewItems[i]);
                        }
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        _Reset = true;
                        break;
                }
            }

            public CollectionChangedTester<T, TItem> Action(System.Action action)
            {
                return this.Action(o => action.Invoke());
            }
            public CollectionChangedTester<T, TItem> Action(Action<T> action)
            {
                _AddedObjects.Clear();
                _RemovedObjects.Clear();
                _ReplacedObjectsOld.Clear();
                _ReplacedObjectsNew.Clear();
                _MovedObjectsOld.Clear();
                _MovedObjectsNew.Clear();
                _Reset = false;
                action.Invoke(_Object);
                return this;
            }
            public CollectionChangedTester<T, TItem> Adds(TItem item)
            {
                _AddedObjects.ShouldAllBeEquivalentTo(new TItem[] { item }, "it should be added");
                return this;
            }
            public CollectionChangedTester<T, TItem> Removes(TItem item)
            {
                _RemovedObjects.ShouldAllBeEquivalentTo(new TItem[] { item }, "it should be removed");
                return this;
            }
            public CollectionChangedTester<T, TItem> Replaces(TItem oldItem, TItem newItem)
            {
                _ReplacedObjectsOld.ShouldAllBeEquivalentTo(new TItem[] { oldItem }, "it should be replaced");
                _ReplacedObjectsNew.ShouldAllBeEquivalentTo(new TItem[] { newItem }, "it should replace");
                return this;
            }
            public CollectionChangedTester<T, TItem> Resets()
            {
                _Reset.Should().BeTrue("Collection wasn't reset");
                return this;
            }
            public CollectionChangedTester<T, TItem> CountIs(int count)
            {
                (_Object as IEnumerable<TItem>).Should().HaveCount(count);
                return this;
            }

        }

        public class ObservableCollectionTester<TItem> where TItem : new()
        {
            ObservableCollection<TItem> _ObservableCollection;

            public ObservableCollectionTester(ObservableCollection<TItem> observableCollection)
            {
                _ObservableCollection = observableCollection;
            }

            public void RunTests()
            {
                TItem item1 = new TItem();
                TItem item2 = new TItem();
                TestHelper.TestCollectionChanged<ObservableCollection<TItem>, TItem>(_ObservableCollection)
                    .Action(c => c.Add(item1)).Adds(item1).CountIs(1)
                    .Action(c => c.Add(item2)).Adds(item2).CountIs(2)
                    .Action(c => c.Remove(item2)).Removes(item2).CountIs(1)
                    .Action(c => c.Clear()).Resets().CountIs(0);
            }
        }

        public static Mock<T> NewMockWithNotifyProperty<T>() where T : class
        {
            var res = new Mock<T>();
            res.As<INotifyPropertyChanged>().SetupAllProperties();
            res.SetupAllProperties();
            return res;
        }

    }
}
