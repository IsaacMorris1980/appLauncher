using appLauncher.Core.Model;

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace appLauncher.Core.Helpers
{
    public class ObservableList : List<AppTile>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private int _pagenum;
        private int _numperpage;
        private List<AppTile> _orginalcollection;
        private int startindex => _pagenum * _numperpage;
        private int endindex => startindex + _numperpage;
        public ObservableList() : base()
        {
            this._pagenum = 0;
            this._numperpage = 0;
            this._orginalcollection = new List<AppTile>();

        }
        public ObservableList(IEnumerable<AppTile> collection) : base(collection)
        {
            this._pagenum = 0;
            this._numperpage = 1;
            this._orginalcollection = new List<AppTile>(collection);
        }
        public void RecalculatePageItems()
        {
            Clear();
            for (int i = this.startindex; i < this.endindex; i++)
            {
                if (this._orginalcollection.Count > i)
                {
                    base.Insert(i - this.startindex, this._orginalcollection[i]);
                }
            }

        }
        public new void Add(AppTile value)
        {
            _orginalcollection.Add(value);
        }
        public new void Insert(int index, AppTile value)
        {

            if ((index >= this.startindex) && (index <= this.endindex))
            {
                base.Insert(index, value);
                if (Count > _numperpage)
                    base.RemoveAt(this.endindex);
            }
            else
            {
                _orginalcollection.Insert(index, value);
            }
        }
        protected new void RemoveAt(int index)
        {

            //Check if the Index is with in the current Page range then remove from the collection as bellow. And remove from the originalCollection also
            if ((index >= startindex) && (index < endindex))
            {
                base.RemoveAt(index - startindex);

                if (Count <= _numperpage)
                    base.Insert(endindex - 1, _orginalcollection[index + 1]);
            }

            _orginalcollection.RemoveAt(index);
        }
        public int PageSize
        {
            get { return _numperpage; }
            set
            {
                if (value >= 0)
                {
                    _numperpage = value;
                    RecalculatePageItems();
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("PageSize"));
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }
        public int GetIndexof(AppTile app)
        {
            return _orginalcollection.IndexOf(app);
        }

        public int CurrentPage
        {
            get { return _pagenum; }
            set
            {
                if (value >= 0)
                {
                    _pagenum = value;
                    RecalculatePageItems();
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("CurrentPage"));
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }
        public List<AppTile> GetInternalList()
        {
            return _orginalcollection;
        }
    }
}
