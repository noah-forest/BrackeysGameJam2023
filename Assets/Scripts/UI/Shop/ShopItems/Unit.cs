using Unity.VisualScripting;
using UnityEngine;

public enum UnitRarity
{
	Common = 2,
	Rare,
	Epic,
	Legendary
}

[CreateAssetMenu]
public class Unit : ScriptableObject
{
	public Sprite itemPreview;
	public Color previewColor;

	[Header("Information")]
	public string unitName;
	public UnitRarity unitRarity;

	public Vector2 spriteOffset;
}
