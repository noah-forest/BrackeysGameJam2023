using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu]
public class ShopItem : ScriptableObject
{
    public string name;
    public string type;
    public GameObject prefab; // unit's prefab
    public Sprite treasureIcon; // item's image
    public string rarity;
    public string description;
}
