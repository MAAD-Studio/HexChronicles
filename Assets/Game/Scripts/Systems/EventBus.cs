using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the publishing and subscription of events throughout the application.
/// Ensures a decoupled architecture by allowing components to communicate indirectly.
/// </summary>
public class EventBus : Singleton<EventBus>
{
    private readonly Dictionary<Type, Action<object>> eventListeners = new Dictionary<Type, Action<object>>();
    private readonly object lockObject = new object(); // For thread-safety

    /// <summary>
    /// Subscribes to an event with a specific listener.
    /// Thread-safe.
    /// </summary>
    /// <typeparam name="T">The type of event to subscribe to.</typeparam>
    /// <param name="listener">The listener to invoke when the event is published.</param>
    public void Subscribe<T>(Action<object> listener)
    {
        var eventType = typeof(T);
        lock (lockObject)
        {
            if (!eventListeners.ContainsKey(eventType))
            {
                eventListeners[eventType] = null;
            }
            eventListeners[eventType] += listener;
        }
    }

    /// <summary>
    /// Unsubscribes a listener from a specific event.
    /// Thread-safe.
    /// </summary>
    /// <typeparam name="T">The type of event to unsubscribe from.</typeparam>
    /// <param name="listener">The listener to remove.</param>
    public void Unsubscribe<T>(Action<object> listener)
    {
        var eventType = typeof(T);
        lock (lockObject)
        {
            if (eventListeners.ContainsKey(eventType))
            {
                eventListeners[eventType] -= listener;
            }
        }
    }

    /// <summary>
    /// Publishes an event to all subscribed listeners.
    /// Catches and logs any exceptions thrown by listeners to prevent one listener's failure from affecting others.
    /// Thread-safe.
    /// </summary>
    /// <typeparam name="T">The type of event being published.</typeparam>
    /// <param name="eventInstance">The event instance to publish.</param>
    public void Publish<T>(T eventInstance)
    {
        var eventType = typeof(T);
        Action<object> listeners;
        lock (lockObject)
        {
            eventListeners.TryGetValue(eventType, out listeners);
        }

        if (listeners != null)
        {
            foreach (var listener in listeners.GetInvocationList())
            {
                try
                {
                    listener.DynamicInvoke(eventInstance);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error publishing event to listener: {ex.Message}");
                    // Consider logging the stack trace or handling the error further as needed.
                }
            }
        }
    }
}
