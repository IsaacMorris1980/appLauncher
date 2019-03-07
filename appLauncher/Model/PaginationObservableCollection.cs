using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using appLauncher.Model;
namespace appLauncher.Model

{
	public class PaginationObservableCollection<finalAppItem> : ObservableCollection<finalAppItem>
	{
		private List<finalAppItem> originalCollection;
		private int Page;
		private int CountPerPage;

		public  PaginationObservableCollection(IEnumerable<finalAppItem> collecton)
		{
			Page = 0;
			CountPerPage = 1;
			originalCollection = new List<finalAppItem>(collecton);
			RecalculateThePageItems();
		}

		public PaginationObservableCollection(int itemsPerPage)
		{
			Page = 0;
			CountPerPage = itemsPerPage;
			originalCollection = new List<finalAppItem>();
		}
		public PaginationObservableCollection()
		{
			Page = 0;
			CountPerPage = 1;
			originalCollection = new List<finalAppItem>();
		}
		private void RecalculateThePageItems()
		{
			Clear();

			int startIndex = Page * CountPerPage;

			for (int i = startIndex; i < startIndex + CountPerPage; i++)
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
				base.RemoveItem(index - startIndex);

				if (Count <= CountPerPage)
					base.InsertItem(endIndex - 1, originalCollection[index + 1]);
			}

			originalCollection.RemoveAt(index);
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
