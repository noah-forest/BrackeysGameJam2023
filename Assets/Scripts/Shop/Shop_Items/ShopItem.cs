using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ShopItem : ScriptableObject
{
    public string name;
    public string type;
    public GameObject art; // unit splash art, or item art
    public string rarity;
    public string description;
}
