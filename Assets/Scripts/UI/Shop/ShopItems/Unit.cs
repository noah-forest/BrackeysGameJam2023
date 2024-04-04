using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu]
public class Unit : ScriptableObject
{
	public Sprite itemPreview;
	public Color previewColor;

	[Header("Information")]
	public string unitName;
	public UnitStats unitStats;
	public Vector2 spriteOffset;
}
