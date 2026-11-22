using appLauncher.Core.Commands;
using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace appLauncher.Core.ViewModels
{
    public class ItemDetailEditViewModel : ViewModelBase
    {
        private readonly IPackageService _packageService;
        private readonly INavigationService _navigationService;

        private IApporFolder _currentItem;
        public IApporFolder CurrentItem
        {
            get => _currentItem;
            set => SetProperty(ref _currentItem, value);
        }

        // Bridge property for XAML Text="{x:Bind ViewModel.ItemName, Mode=TwoWay}"
        public string ItemName
        {
            get => CurrentItem?.Name;
            set
            {
                if (CurrentItem != null && CurrentItem.Name != value)
                {
                    CurrentItem.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsApp => _currentItem is FinalTiles;
        public bool IsFolder => _currentItem is AppFolder;

        public FinalTiles AppItem => _currentItem as FinalTiles;
        public AppFolder FolderItem => _currentItem as AppFolder;

        private List<FinalTiles> _availableTiles = new List<FinalTiles>();
        public List<FinalTiles> AvailableTiles
        {
            get => _availableTiles;
            set => SetProperty(ref _availableTiles, value);
        }

        private FinalTiles _selectedAvailableTile;
        public FinalTiles SelectedAvailableTile
        {
            get => _selectedAvailableTile;
            set => SetProperty(ref _selectedAvailableTile, value);
        }

        private FinalTiles _selectedFolderApp;
        public FinalTiles SelectedFolderApp
        {
            get => _selectedFolderApp;
            set => SetProperty(ref _selectedFolderApp, value);
        }

        // Commands matching XAML bindings
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddAppToFolderCommand { get; }
        public ICommand RemoveAppFromFolderCommand { get; }

        public ItemDetailEditViewModel(IPackageService packageService, INavigationService navigationService)
        {
            _packageService = packageService ?? throw new ArgumentNullException(nameof(packageService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            SaveCommand = new RelayCommand(SaveItemChanges);
            CancelCommand = new RelayCommand(() => { if (_navigationService.CanGoBack) _navigationService.GoBack(); });
            AddAppToFolderCommand = new RelayCommand(AddAppToFolder, () => SelectedAvailableTile != null);
            RemoveAppFromFolderCommand = new RelayCommand(RemoveAppFromFolder, () => SelectedFolderApp != null);
        }

        public void Initialize(object navigationParameter)
        {
            if (navigationParameter is IApporFolder item)
            {
                CurrentItem = item;

                if (IsFolder)
                {
                    var allTiles = _packageService.Apps.GetOriginalCollection().OfType<FinalTiles>().ToList();
                    var folderAppNames = FolderItem.FolderApps.Select(x => x.Name).ToList();
                    AvailableTiles = allTiles.Where(t => !folderAppNames.Contains(t.Name)).ToList();
                }

                OnPropertyChanged(nameof(ItemName));
                OnPropertyChanged(nameof(IsApp));
                OnPropertyChanged(nameof(IsFolder));
                OnPropertyChanged(nameof(AppItem));
                OnPropertyChanged(nameof(FolderItem));
            }
        }

        private void SaveItemChanges()
        {
            if (IsApp && AppItem != null)
            {
                _packageService.Apps.UpdateApp(AppItem);
            }
            else if (IsFolder && FolderItem != null)
            {
                var folders = _packageService.Apps.OfType<AppFolder>().ToList();
                var apps = _packageService.Apps.OfType<FinalTiles>().ToList();

                if (!folders.Any(x => x.Name == FolderItem.Name))
                {
                    folders.Add(FolderItem);
                }

                List<IApporFolder> combined = new List<IApporFolder>();
                combined.AddRange(folders);
                combined.AddRange(apps);
                _packageService.Apps = new AppPaginationObservableCollection(combined.OrderBy(x => x.Name));
            }

            if (_navigationService.CanGoBack)
            {
                _navigationService.GoBack();
            }
        }

        private void AddAppToFolder()
        {
            if (SelectedAvailableTile != null && FolderItem != null)
            {
                FolderItem.FolderApps.Add(SelectedAvailableTile);
                AvailableTiles.Remove(SelectedAvailableTile);
                OnPropertyChanged(nameof(FolderItem));
                OnPropertyChanged(nameof(AvailableTiles));
            }
        }

        private void RemoveAppFromFolder()
        {
            if (SelectedFolderApp != null && FolderItem != null)
            {
                FolderItem.FolderApps.Remove(SelectedFolderApp);
                AvailableTiles.Add(SelectedFolderApp);
                OnPropertyChanged(nameof(FolderItem));
                OnPropertyChanged(nameof(AvailableTiles));
            }
        }
    }
}
