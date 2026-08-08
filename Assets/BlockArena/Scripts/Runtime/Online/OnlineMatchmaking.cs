using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Multiplayer;
using UnityEngine;

public static class OnlineMatchmaking
{
    public enum SearchState
    {
        Idle,
        Searching,
        HumanFound,
        BotReady,
        Failed
    }

    private const int HumanSearchSeconds = 15;

    private static CancellationTokenSource cancellation;
    private static ISession session;
    private static DateTime searchDeadline;

    public static SearchState State { get; private set; } = SearchState.Idle;
    public static string StatusMessage { get; private set; } = "RAKİP ARAMAYA HAZIR";
    public static int SecondsRemaining =>
        State == SearchState.Searching
            ? Math.Max(
                0,
                (int)Math.Ceiling(
                    (searchDeadline - DateTime.UtcNow).TotalSeconds
                )
            )
            : 0;
    public static ISession Session => session;

    public static async Task StartSearchAsync()
    {
        await CancelSearchAsync();
        await OnlineServices.InitializeAsync();

        if (OnlineServices.State != OnlineServices.ConnectionState.Ready)
        {
            State = SearchState.Failed;
            StatusMessage = "ÇEVRİM İÇİ SERVİS HAZIR DEĞİL";
            return;
        }

        cancellation = new CancellationTokenSource();
        CancellationToken token = cancellation.Token;
        State = SearchState.Searching;
        searchDeadline = DateTime.UtcNow.AddSeconds(HumanSearchSeconds);
        StatusMessage = "GERÇEK OYUNCU ARANIYOR...";

        try
        {
            QuickJoinOptions quickJoin = new QuickJoinOptions
            {
                Timeout = TimeSpan.FromSeconds(3),
                CreateSession = true
            };
            SessionOptions sessionOptions = new SessionOptions
            {
                Name = "BlockArena-PVP",
                Type = "BlockArena-PVP",
                MaxPlayers = 2,
                IsPrivate = false
            };
            OnlineNetworkRuntime.EnsureNetworkManager();
            sessionOptions.WithRelayNetwork();

            session = await MultiplayerService.Instance.MatchmakeSessionAsync(
                quickJoin,
                sessionOptions
            );

            if (session.PlayerCount >= 2)
            {
                SetHumanFound();
                return;
            }

            while (DateTime.UtcNow < searchDeadline)
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(250, token);

                if (session.PlayerCount >= 2)
                {
                    SetHumanFound();
                    return;
                }
            }

            await LeaveCurrentSessionAsync();
            OnlineNetworkRuntime.Shutdown();
            State = SearchState.BotReady;
            StatusMessage = "OYUNCU BULUNAMADI • BOT HAZIR";
        }
        catch (OperationCanceledException)
        {
            State = SearchState.Idle;
            StatusMessage = "ARAMA İPTAL EDİLDİ";
        }
        catch (Exception exception)
        {
            await LeaveCurrentSessionAsync();
            OnlineNetworkRuntime.Shutdown();
            State = SearchState.Failed;
            StatusMessage = "EŞLEŞTİRME BAŞARISIZ";
            Debug.LogException(exception);
        }
    }

    public static async Task CancelSearchAsync()
    {
        if (cancellation != null)
        {
            cancellation.Cancel();
            cancellation.Dispose();
            cancellation = null;
        }

        await LeaveCurrentSessionAsync();
        OnlineNetworkRuntime.Shutdown();

        if (State == SearchState.Searching)
        {
            State = SearchState.Idle;
            StatusMessage = "ARAMA İPTAL EDİLDİ";
        }
    }

    private static void SetHumanFound()
    {
        State = SearchState.HumanFound;
        StatusMessage = "GERÇEK OYUNCU BULUNDU";
    }

    private static async Task LeaveCurrentSessionAsync()
    {
        if (session == null)
        {
            return;
        }

        try
        {
            await session.LeaveAsync();
        }
        catch (Exception exception)
        {
            Debug.LogWarning(exception.Message);
        }
        finally
        {
            session = null;
        }
    }
}
