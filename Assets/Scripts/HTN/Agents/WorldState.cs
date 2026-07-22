using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{
    public static WorldState Instance { get; private set; }

    [SerializeField] private List<LocationEntry> _entries;

    private Dictionary<LocationType, Transform> _locations;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _locations = new();

        foreach (var entry in _entries)
        {
            _locations.Add(entry.Type, entry.Transform);
        }
    }

    public Transform GetLocation(LocationType type)
    {
        return _locations[type];
    }
}
