using System.Collections.Generic;
using UnityEngine;

// Optional. Right-click in Project > Create > MixKitchen > Area.
// Make one for Luzon, Visayas and Mindanao and put the 5 dishes in each.
// Lets you use ONE gameplay scene for all three areas.
[CreateAssetMenu(fileName = "NewArea", menuName = "MixKitchen/Area")]
public class AreaData : ScriptableObject
{
    public string areaName;
    public List<Dish> dishes = new List<Dish>();
}