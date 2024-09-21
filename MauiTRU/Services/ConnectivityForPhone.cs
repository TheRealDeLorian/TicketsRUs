using MauiTRU.Database;

namespace MauiTRU.Services;

public class ConnectivityForPhone
{
    private bool _forceDisconnect = false;
    public bool ForceDisconnect { get => _forceDisconnect; set => _forceDisconnect = value; }

    public ConnectivityForPhone()
    {
        _forceDisconnect = Preferences.Get(Constants.OfflineModeKey, Constants.DefaultOfflineMode);
    }

    public NetworkAccess GetNetworkAccess()
    {
        if (_forceDisconnect)
            return NetworkAccess.None;
        else
            return Connectivity.Current.NetworkAccess;
    }
}
