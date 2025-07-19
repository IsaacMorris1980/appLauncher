using appLauncher.Core.Interfaces;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Windows.UI.Core;
using Windows.UI.Xaml.Automation.Peers; // For CoreDispatcher, if needed for UI thread updates

namespace appLauncher.Core.Model
{
    /// <summary>
    /// A specialized ObservableCollection that provides pagination functionality.
    /// It holds a full list of items and exposes a subset based on the current page.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    public class PaginationObservableCollection : ObservableCollection<IApporFolder>
    {
        // The full, unpaginated collection of items
        private List<IApporFolder> _originalCollection;

        // Properties for pagination state
        private int _itemsPerPage;
        public int ItemsPerPage
        {
            get => _itemsPerPage;
            set
            {
                if (_itemsPerPage != value && value > 0)
                {
                    _itemsPerPage = value;
                    UpdatePaginationProperties();
                }
            }
        }

        private int _currentPageNum;
        public int CurrentPageNum
        {
            get => _currentPageNum;
            private set
            {
                if (_currentPageNum != value)
                {
                    _currentPageNum = value;
                    // In a real ViewModel, this would typically raise PropertyChanged
                    // for the UI to update page number displays.
                }
            }
        }

        private int _numOfPages;
        public int NumOfPages
        {
            get => _numOfPages;
            private set
            {
                if (_numOfPages != value)
                {
                    _numOfPages = value;
                    // In a real ViewModel, this would typically raise PropertyChanged
                    // for the UI to update total page number displays.
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationObservableCollection{T}"/> class.
        /// </summary>
        /// <param name="originalCollection">The full collection of items to paginate.</param>
        /// <param name="itemsPerPage">The number of items to display per page.</param>
        public PaginationObservableCollection(IEnumerable<IApporFolder> originalCollection, int itemsPerPage = 0)
        {
            if (originalCollection == null)
                throw new ArgumentNullException(nameof(originalCollection));
            if (itemsPerPage <= 0)
                throw new ArgumentOutOfRangeException(nameof(itemsPerPage), "Items per page must be greater than zero.");

            _originalCollection = new List<IApporFolder>(originalCollection);
            _itemsPerPage = itemsPerPage;

            UpdatePaginationProperties();
            SetCurrentPage(1); // Initialize to the first page
        }

        /// <summary>
        /// Gets the original, unpaginated collection.
        /// This is often used for operations like searching or re-sorting the full list.
        /// </summary>
        /// <returns>The full list of items.</returns>
        public List<IApporFolder> GetOriginalCollection()
        {
            return _originalCollection;
        }

        /// <summary>
        /// Updates the internal original collection and re-applies pagination.
        /// Call this when the underlying data changes (e.g., after a rescan).
        /// </summary>
        /// <param name="newCollection">The new full collection of items.</param>
        public void UpdateOriginalCollection(IEnumerable<IApporFolder> newCollection)
        {
            _originalCollection = new List<IApporFolder>(newCollection);
            UpdatePaginationProperties();
            SetCurrentPage(Math.Min(CurrentPageNum, NumOfPages > 0 ? NumOfPages : 1)); // Stay on current page if possible, else first
        }

        /// <summary>
        /// Recalculates the total number of pages based on the original collection size and items per page.
        /// </summary>
        private void UpdatePaginationProperties()
        {
            NumOfPages = (int)Math.Ceiling((double)_originalCollection.Count / ItemsPerPage);
            if (NumOfPages == 0 && _originalCollection.Any()) // Handle case where there's content but less than 1 full page
            {
                NumOfPages = 1;
            }
            else if (!_originalCollection.Any()) // Handle empty collection
            {
                NumOfPages = 0;
            }
        }

        /// <summary>
        /// Sets the current page and updates the displayed items in the ObservableCollection.
        /// This is the core pagination logic.
        /// </summary>
        /// <param name="pageNumber">The 1-based page number to display.</param>
        public void SetCurrentPage(int pageNumber)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (NumOfPages > 0 && pageNumber > NumOfPages) pageNumber = NumOfPages;
            if (NumOfPages == 0 && pageNumber > 0) pageNumber = 0; // If no pages, set to 0 or handle as empty state

            if (CurrentPageNum == pageNumber && this.Any()) // If already on this page and not empty, no need to update
            {
                // Edge case: if collection was cleared externally, but pageNum is same, force refresh
                if (this.Count != GetItemsForPage(pageNumber).Count())
                {
                    UpdateDisplayedItems(pageNumber);
                }
                return;
            }

            CurrentPageNum = pageNumber;
            UpdateDisplayedItems(pageNumber);
        }
        /// <summary>
        /// Helper method to get the subset of items for a given page.
        /// </summary>
        /// <param name="pageNumber">The 1-based page number.</param>
        /// <returns>An enumerable of items for the specified page.</returns>
        private IEnumerable<IApporFolder> GetItemsForPage(int pageNumber)
        {
            if (!_originalCollection.Any() || pageNumber == 0) return Enumerable.Empty<IApporFolder>();

            int startIndex = (pageNumber - 1) * ItemsPerPage;
            // Ensure startIndex is not out of bounds
            if (startIndex >= _originalCollection.Count) return Enumerable.Empty<IApporFolder>();

            int count = Math.Min(ItemsPerPage, _originalCollection.Count - startIndex);
            return _originalCollection.Skip(startIndex).Take(count);
        }
        /// <summary>
        /// Clears the current items and adds the items for the new page.
        /// Ensures UI updates happen on the UI thread.
        /// </summary>
        /// <param name="pageNumber">The 1-based page number.</param>
        private async void UpdateDisplayedItems(int pageNumber)
        {
            // Ensure UI updates happen on the UI thread.
            // This is crucial if SetCurrentPage is called from a background thread.
            if (Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.HasThreadAccess)
            {
                PerformUIUpdate(pageNumber);
            }
            else
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () => PerformUIUpdate(pageNumber)
                );
            }
        }

        private void PerformUIUpdate(int pageNumber)
        {
            this.Clear(); // Clear current items

            foreach (var item in GetItemsForPage(pageNumber))
            {
                this.Add(item); // Add items for the new page
            }
        }

        /// <summary>
        /// Moves to the next page, if available.
        /// </summary>
        public void MoveNextPage()
        {
            if (CurrentPageNum < NumOfPages - 1)
            {
                SetCurrentPage(CurrentPageNum + 1);
            }
        }

        /// <summary>
        /// Moves to the previous page, if available.
        /// </summary>
        public void MovePreviousPage()
        {
            if (CurrentPageNum >= 1)
            {
                SetCurrentPage(CurrentPageNum - 1);
            }
        }

        // You might also want methods to add/remove items from the original collection
        // and then call UpdateOriginalCollection or re-calculate pagination.
        public void AddItemToOriginal(IApporFolder item)
        {
            _originalCollection.Add(item);
            UpdateOriginalCollection(_originalCollection); // Re-paginate after adding
        }

        public void RemoveItemFromOriginal(IApporFolder item)
        {
            _originalCollection.Remove(item);
            UpdateOriginalCollection(_originalCollection); // Re-paginate after removing
        }

        public void SortOriginalCollection(IComparer<IApporFolder> comparer)
        {
            _originalCollection.Sort(comparer);
            UpdateOriginalCollection(_originalCollection); // Re-paginate after sorting
        }

        /// <summary>
        /// Gets the current index of an item within the original collection.
        /// </summary>
        /// <param name="item">The item to find.</param>
        /// <returns>The index of the item in the original collection, or -1 if not found.</returns>
        public new int IndexOf(IApporFolder item)
        {
            return _originalCollection.IndexOf(item);
        }

        /// <summary>
        /// Moves an item within the original collection and re-paginates.
        /// </summary>
        /// <param name="oldIndex">The original index of the item.</param>
        /// <param name="newIndex">The new desired index of the item.</param>
        public void MoveItemInOriginal(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || oldIndex >= _originalCollection.Count ||
                newIndex < 0 || newIndex >= _originalCollection.Count)
            {
                return; // Invalid indices
            }

            IApporFolder itemToMove = _originalCollection[oldIndex];
            _originalCollection.RemoveAt(oldIndex);
            _originalCollection.Insert(newIndex, itemToMove);

            UpdateOriginalCollection(_originalCollection); // Re-paginate after move
        }
        public void Search(string searchText)
        {
            if (string.IsNullOrEmpty(searchText) || string.IsNullOrWhiteSpace(searchText))
            {
                UpdateDisplayedItems(this.CurrentPageNum);
                return;
            }
            var searched = _originalCollection.Where(x => x.Name.ToLower().Contains(searchText.ToLower()));
            if (searched.Any())
            {
                this.Clear();
                foreach (var item in searched)
                {
                    this.Add(item);
                }
            }
        }
        public List<IApporFolder> ReturnFavorites()
        {
            return _originalCollection.Where(x => x.Favorite).ToList();
        }
        public List<IApporFolder> ReturnMostUsed()
        {
            return _originalCollection.Where(x => x.LaunchedCount > 5).ToList();
        }
    }
}
