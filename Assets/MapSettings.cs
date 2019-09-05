using UnityEngine;

[CreateAssetMenu()]
public class MapSettings : ScriptableObject {

    [Range(0,1)]
    public float percentLand;
    
}