using System;
using System.Collections.Generic;
using UnityEngine;

public class IconManager : MonoBehaviour
{
    /// <summary>
    /// Set up all the icons in the Unity Scene, and then they're pre-loaded for easy use
    /// This only really works if you don't have too many total assets 
    /// </summary>
    public List<Icon> iconRepository;
    /// <summary>
    /// Indexed version of iconRepository, built from iconRepository at runtime
    /// </summary>
    private readonly Dictionary<string, Icon> _indexedRepository = new Dictionary<string, Icon>();
    public Sprite defaultIcon; // use this if we get a request for a non-existent icon;

    public enum IconTypes { Spell, Class, Item, Status, Encounter, Level, Misc }

    public void Start()
    {
        foreach (var eachIcon in iconRepository)
            _indexedRepository[eachIcon.id] = eachIcon;
    }

    public Sprite GetIconById(string id, IconTypes expectedType = IconTypes.Misc)
    {
        Icon output;
        if (_indexedRepository.TryGetValue(id, out output))
        {
            if (expectedType != IconTypes.Misc && output.type != expectedType) // For now, just a warning
                Debug.LogWarning("Icon: " + id + " does not match expected type: " + expectedType);
            return output.sprite;
        }
        return defaultIcon;
    }
}

[Serializable]
public class Icon
{
    public string id;
    public Sprite sprite;
    public IconManager.IconTypes type;
}
