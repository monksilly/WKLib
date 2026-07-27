using System;
using System.Collections.Generic;
using WKLib.Utilities;

namespace WKLib.API.Events;

public class EventService
{
    private readonly WKLibAPI _owner;
    private readonly List<(HookId hook, Action wrapped)> _subscriptions = new();

    internal EventService(WKLibAPI owner) => _owner = owner;

    public void Subscribe(HookId hook, Action callback, EventPriority priority = EventPriority.Normal)
    {
        WKLog.Debug($"[{_owner.DisplayName} ({_owner.GUID})] subscribing to {hook} with priority {priority}");
        Action wrapped = () => Invoke(hook, callback);
        _subscriptions.Add((hook, wrapped));
        GameEvents.Subscribe(hook, wrapped, priority);
    }

    private void Invoke(HookId hook, Action callback)
    {
        WKLog.Debug($"[{_owner.DisplayName} ({_owner.GUID})] invoking {hook} callback");
        try
        {
            callback();
        }
        catch (Exception ex)
        {
            WKLog.Error($"[{_owner.DisplayName} ({_owner.GUID})] threw in {hook} callback: {ex}");
        }
    }

    internal void UnsubscribeAll()
    {
        foreach (var (hook, wrapped) in _subscriptions)
            GameEvents.Unsubscribe(hook, wrapped);
        _subscriptions.Clear();
    }
}