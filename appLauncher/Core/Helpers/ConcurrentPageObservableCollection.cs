using appLauncher.Core.CustomEvent;
using appLauncher.Core.Model;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace appLauncher.Core.Helpers
{
    public class PageObservableCollection : ObservableCollection<AppTile>
    {
        private List<List<AppTile>> _items;
        private int _pagenum;
        private int _appsperpage;
        private ObservableCollection<AppTile> allapps { get; set; } = new ObservableCollection<AppTile>();
        private ReadOnlyObservableCollection<AppTile> _itemsReadOnly
        {
            get
            {

                return new ReadOnlyObservableCollection<AppTile>(new ObservableCollection<AppTile>(allapps.OrderBy(x => x.appname)));
            }
        }

        public PageObservableCollection(IEnumerable<AppTile> collection) : base(collection)
        {
            GlobalVariables.NumofApps += new AppPageSizeChangedDelegate(NumofApps);
            GlobalVariables.PageNumChanged += new PageChangedDelegate(PagedChanged);
            GlobalVariables.Numofpages += GlobalVariables_Numofpages;
            allapps = new ObservableCollection<AppTile>(collection);

        }

        private void GlobalVariables_Numofpages(PageNumChangedArgs e)
        {

            for (int i = 0; i < e.numofpages; i++)
            {
                _items.Add(allapps.Skip(_pagenum * _appsperpage).Take(_appsperpage).ToList());
            }
        }

        private void PagedChanged(PageChangedEventArgs e)
        {
            _pagenum = e.PageIndex;
        }

        private void NumofApps(AppPageSizeChangedEventArgs e)
        {
            _appsperpage = e.AppPageSize;
        }

        public void Move(DraggedItem e)
        {
            var a = ((e.newpage * _appsperpage) + e.indexonnewpage);
            allapps.Move(e.initialindex, a);

        }
        public List<AppTile> GetPageApps()
        {
            return _items[_pagenum];
        }
        public List<AppTile> GetInternalList()
        {
            return allapps.ToList();
        }


    }
}
