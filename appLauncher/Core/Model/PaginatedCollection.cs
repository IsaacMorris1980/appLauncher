using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appLauncher.Model
{
    
        [Serializable]
        public class PaginationObservableCollection : ObservableCollection<finalAppItem>
        {
            private ObservableCollection<finalAppItem> originalCollection;
            [NonSerialized]
            private int Page;
            [NonSerialized]
            private int CountPerPage;

        public PaginationObservableCollection(IEnumerable<finalAppItem> collection) : base(collection)
        {
            
                Page = 0;
                CountPerPage = 1;
                originalCollection = new ObservableCollection<finalAppItem>(collection);
                RecalculateThePageItems();
            }


            private void RecalculateThePageItems()
            {
                ClearItems();

                int startIndex = Page * CountPerPage;
                int endIndex = startIndex + CountPerPage;
                for (int i = startIndex; i < endIndex; i++)
                {
                    if (originalCollection.Count > i)
                        base.InsertItem(i - startIndex, originalCollection[i]);
                }
            }









            protected override void InsertItem(int index, finalAppItem item)
            {
                int startIndex = Page * CountPerPage;
                int endIndex = startIndex + CountPerPage;

                //Check if the Index is with in the current Page then add to the collection as bellow. And add to the originalCollection also
                if ((index >= startIndex) && (index < endIndex))
                {
                    base.InsertItem(index - startIndex, item);

                    if (Count > CountPerPage)
                        base.RemoveItem(endIndex);
                }

                if (index >= Count)
                    originalCollection.Add(item);
                else
                    originalCollection.Insert(index, item);
            }

            protected override void RemoveItem(int index)
            {
                int startIndex = Page * CountPerPage;
                int endIndex = startIndex + CountPerPage;
                //Check if the Index is with in the current Page range then remove from the collection as bellow. And remove from the originalCollection also
                if ((index >= startIndex) && (index < endIndex))
                {
                    this.RemoveAt(index - startIndex);

                    if (Count <= CountPerPage)
                        base.InsertItem(endIndex - 1, originalCollection[index + 1]);
                }

                originalCollection.RemoveAt(index);
            }
        public ObservableCollection<finalAppItem> GetFinalAppItems()
        {
            
        }
            public int PageSize
            {
                get { return Count; }
                set
                {
                    if (value >= 0)
                    {
                        CountPerPage = value;
                        RecalculateThePageItems();
                        OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("PageSize"));
                    }
                }
            }

            public int CurrentPage
            {
                get { return Page; }
                set
                {
                    if (value >= 0)
                    {
                        Page = value;
                        RecalculateThePageItems();
                        OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("CurrentPage"));
                    }
                }
            }
        }
    }