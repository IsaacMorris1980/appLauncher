using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IPackageService
{
    List<IApporFolder> Search { get; }
    PaginationObservableCollection Apps { get; }

    event EventHandler AppsRetrieved;

    Task LoadAppCollectionAsync();
    Task SaveAppCollectionAsync();
    Task<bool> LaunchApplication(string fullname);
    Task RescanForNewApplications();
    void RemoveFromSearch(string fullName);
}