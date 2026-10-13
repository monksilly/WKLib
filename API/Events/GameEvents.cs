using System;
using System.Collections.Generic;
using WKLib.Utilities;

namespace WKLib.API.Events;

public static class GameEvents
{
    private readonly struct Subscription
    {
        public readonly EventPriority Priority;
        public readonly long Order;
        public readonly Action Handler;

        public Subscription(EventPriority priority, long order, Action handler)
        {
            Priority = priority;
            Order = order;
            Handler = handler;
        }
    }

    private static readonly Dictionary<HookId, List<Subscription>> _handlers = new();
    private static long _nextOrder = 0;

    internal static void Subscribe(HookId hook, Action handler, EventPriority priority)
    {
        if (!_handlers.TryGetValue(hook, out var list))
        {
            list = new List<Subscription>();
            _handlers[hook] = list;
        }

        list.Add(new Subscription(priority, _nextOrder++, handler));

        // handlers are sorted by priority, then by order of subscription
        list.Sort((a, b) =>
        {
            int cmp = a.Priority.CompareTo(b.Priority);
            return cmp != 0 ? cmp : a.Order.CompareTo(b.Order);
        });
    }

    internal static void Unsubscribe(HookId hook, Action handler)
    {
        if (_handlers.TryGetValue(hook, out var list))
            list.RemoveAll(s => s.Handler == handler);
    }

    internal static void Raise(HookId hook)
    {
        WKLog.Debug($"Raising event {hook}");
        if (!_handlers.TryGetValue(hook, out var list) || list.Count == 0)
            return;

        foreach (var sub in list.ToArray())
            sub.Handler.Invoke();
    }
}