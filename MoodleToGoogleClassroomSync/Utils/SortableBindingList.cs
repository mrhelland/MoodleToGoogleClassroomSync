using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MoodleToGoogleClassroomSync.Utils {
    /// <summary>
    /// A BindingList that supports remembering the last sort state
    /// and automatically toggling between ascending/descending.
    /// </summary>
    public class SortableBindingList<T> : BindingList<T> {
        private bool _isSorted;
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;
        private PropertyDescriptor _sortProperty;

        public SortableBindingList() : base() { }
        public SortableBindingList(IList<T> list) : base(list) { }

        protected override bool SupportsSortingCore => true;
        protected override bool IsSortedCore => _isSorted;
        protected override PropertyDescriptor SortPropertyCore => _sortProperty;
        protected override ListSortDirection SortDirectionCore => _sortDirection;

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction) {
            var items = (List<T>)Items;

            // ✅ Always perform sort — even on first click
            if(_isSorted && _sortProperty == prop) {
                // Toggle sort direction
                _sortDirection = _sortDirection == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }
            else {
                _sortProperty = prop;
                _sortDirection = ListSortDirection.Ascending; // usually ascending on first click
            }

            // ✅ Always run sort, regardless of previous state
            var comparer = new PropertyComparer<T>(_sortProperty, _sortDirection);
            items.Sort(comparer);

            _isSorted = true;
            ResetBindings();
        }

        protected override void RemoveSortCore() {
            _isSorted = false;
        }
    }

    internal class PropertyComparer<T> : IComparer<T> {
        private readonly PropertyDescriptor _property;
        private readonly ListSortDirection _direction;

        public PropertyComparer(PropertyDescriptor property, ListSortDirection direction) {
            _property = property;
            _direction = direction;
        }

        public int Compare(T x, T y) {
            var xValue = _property.GetValue(x);
            var yValue = _property.GetValue(y);

            int result;
            if(xValue == null)
                result = yValue == null ? 0 : -1;
            else if(yValue == null)
                result = 1;
            else
                result = ((IComparable)xValue).CompareTo(yValue);

            return _direction == ListSortDirection.Ascending ? result : -result;
        }
    }
}
