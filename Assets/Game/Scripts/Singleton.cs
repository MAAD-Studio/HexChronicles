using UnityEngine;

/// <summary>
/// A generic Singleton class for MonoBehaviour types.
/// Ensures that only one instance of the specified type exists.
/// </summary>
/// <typeparam name="T">Type of the singleton class.</typeparam>
public class Singleton<T> : MonoBehaviour where T : Component
{
    // Static variable to hold the instance of the type T.
    private static T _instance; // Use static so that the instance is shared across all instances of the class
    private static object _lock = new object();
    private static bool _wasCreatedOnce = false;

    /// <summary>
    /// Public property to access the singleton instance.
    /// Creates an instance if one does not already exist.
    /// </summary>
    public static T Instance // An accessor property to check if the instance exists
    {
        get
        {
            lock (_lock)
            {
                // if it was already destroyed, don't try to create a new one
                if (_wasCreatedOnce) return _instance;
                // If _instance is null, find any object of type T in the scene.
                _instance ??= FindAnyObjectByType<T>();
                // If still null, create a new GameObject and add the component T to it.
                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    // Name the GameObject after the type T for easy identification.
                    go.name = typeof(T).Name;
                    _instance = go.AddComponent<T>();
                }
                _wasCreatedOnce = true;
                // Return the instance.
                return _instance;
            }
        }
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// Initializes the singleton instance.
    /// </summary>
    public virtual void Awake() // Use virtual so that derived classes can override
    {
        lock (_lock)
        {
            // Check if _instance is null (i.e., no instance already exists).
            if (_instance == null)
            {
                _wasCreatedOnce = true;
                // Assign this instance to _instance and ensure it persists across scenes.
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // If this isn't the first instance, destroy this GameObject.
                Destroy(gameObject);
            }
        }
    }
}