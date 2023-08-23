using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

[CreateAssetMenu]
public class ShopItem : ScriptableObject
{
    public string name;
    public GameObject prefab; // unit's art
    public string rarity;
}
