using UnityEngine;

[CreateAssetMenu(fileName = "New HazardEntry", menuName = "HazardEntry")]
public class HazardEntrySO : ScriptableObject
{
    public string hazardName;
    public Sprite hazardIcon;
    
    [TextArea]
    public string hazardDescription;
}
