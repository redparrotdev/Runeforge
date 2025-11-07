using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Engine.Events;

public static class EventManager
{
    private static bool _isDispatching = false;
    private static readonly HashSet<BaseEvent> _toDispatch = [];
    private static readonly Dictionary<Type, List<EventSubscription>> _eventsSubscribtions = [];

    public static EventSubscription Subscribe<T>(Action<T> callback) where T : BaseEvent
    {
        var eventType = typeof(T);
        if (!_eventsSubscribtions.TryGetValue(eventType, out var subscribtionsList))
        {
            _eventsSubscribtions[eventType] = [];
            subscribtionsList = _eventsSubscribtions[eventType];
        }

        var subscription = new EventSubscription(eventType, e => callback((T)e));
        subscribtionsList.Add(subscription);

        return subscription;
    }

    public static void Update(GameTime gameTime)
    {
        List<EventSubscription> inactiveSubscriptions = [];

        _isDispatching = true;

        foreach (var eventInstance in _toDispatch)
        {
            var eventType = eventInstance.GetType();

            if (!_eventsSubscribtions.TryGetValue(eventType, out var subscribtionsList))
            {
                continue;
            }

            foreach (var subscription in subscribtionsList.ToArray())
            {
                if (subscription.IsActive)
                {
                    subscription.Callback(eventInstance);
                }
                else
                {
                    inactiveSubscriptions.Add(subscription);
                }
            }
        }

        _toDispatch.Clear();

        _isDispatching = false;

        foreach (var inactiveSubscription in inactiveSubscriptions)
        {
            inactiveSubscription.Unsubscribe();
        }
    }

    public static void Dispatch(BaseEvent eventInstance)
    {
        _toDispatch.Add(eventInstance);
    }

    public sealed class EventSubscription
    {
        public readonly Type EventType;
        internal readonly Action<BaseEvent> Callback;

        public bool IsActive => _isActive;
        private bool _isActive = true;

        internal EventSubscription(Type eventType, Action<BaseEvent> callback)
        {
            EventType = eventType;
            Callback = callback;
        }

        public void Unsubscribe()
        {
            _isActive = false;

            if (_isDispatching) return;

            if (_eventsSubscribtions.TryGetValue(EventType, out var subscribtionsList))
            {
                subscribtionsList.Remove(this);
            }
        }
    }
}
