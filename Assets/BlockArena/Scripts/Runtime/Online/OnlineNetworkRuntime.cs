using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public static class OnlineNetworkRuntime
{
    public static NetworkManager EnsureNetworkManager()
    {
        if (NetworkManager.Singleton != null)
        {
            return NetworkManager.Singleton;
        }

        GameObject networkObject = new GameObject("OnlineNetworkManager");
        Object.DontDestroyOnLoad(networkObject);

        UnityTransport transport =
            networkObject.AddComponent<UnityTransport>();
        NetworkManager manager =
            networkObject.AddComponent<NetworkManager>();

        manager.NetworkConfig = new NetworkConfig
        {
            NetworkTransport = transport,
            EnableSceneManagement = true,
            ConnectionApproval = false
        };

        return manager;
    }

    public static string GetRoleLabel()
    {
        NetworkManager manager = NetworkManager.Singleton;

        if (manager == null || !manager.IsListening)
        {
            return "AĞ BAĞLANTISI HAZIRLANIYOR";
        }

        return manager.IsHost ? "HOST" : "CLIENT";
    }

    public static void Shutdown()
    {
        NetworkManager manager = NetworkManager.Singleton;

        if (manager == null)
        {
            return;
        }

        if (manager.IsListening)
        {
            manager.Shutdown();
        }

        Object.Destroy(manager.gameObject);
    }
}
