using UnityEngine;

[System.Serializable]
public class LocationEntry
{
    [SerializeField] private LocationType type;
    [SerializeField] private Transform transform;

    public LocationType Type => type;
    public Transform Transform => transform;
}
