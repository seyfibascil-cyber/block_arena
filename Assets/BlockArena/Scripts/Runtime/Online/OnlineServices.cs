using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;

public static class OnlineServices
{
    public enum ConnectionState
    {
        Offline,
        Connecting,
        Ready,
        Failed
    }

    private static Task initializationTask;

    public static ConnectionState State { get; private set; } =
        ConnectionState.Offline;

    public static string StatusMessage { get; private set; } =
        "BAĞLANTI BEKLENİYOR";

    public static string PlayerId =>
        AuthenticationService.Instance.IsSignedIn
            ? AuthenticationService.Instance.PlayerId
            : string.Empty;

    public static Task InitializeAsync()
    {
        if (initializationTask != null)
        {
            return initializationTask;
        }

        initializationTask = InitializeInternalAsync();
        return initializationTask;
    }

    public static async Task RetryAsync()
    {
        initializationTask = null;
        await InitializeAsync();
    }

    private static async Task InitializeInternalAsync()
    {
        State = ConnectionState.Connecting;
        StatusMessage = "ÇEVRİM İÇİ SERVİSE BAĞLANILIYOR...";

        try
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                await UnityServices.InitializeAsync();
            }

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            State = ConnectionState.Ready;
            StatusMessage = "ÇEVRİM İÇİ SERVİS HAZIR";
        }
        catch (Exception exception)
        {
            State = ConnectionState.Failed;
            StatusMessage = "BAĞLANTI KURULAMADI";
            UnityEngine.Debug.LogException(exception);
        }
    }
}
