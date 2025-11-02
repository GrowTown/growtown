// ------------------------------------------------------------------------------
//  ToolManager.cs
//  Maintains the player's currently equipped gathering tool and exposes helpers
//  to validate whether the tool can harvest a particular resource type.
// ------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ToolManager : MonoBehaviour
{
    [SerializeField] private ToolType startingTool = ToolType.None;

    private static readonly Dictionary<ResourceType, ToolType> resourceToolMap = new Dictionary<ResourceType, ToolType>
    {
        { ResourceType.Grass, ToolType.Sickle },
        { ResourceType.Stone, ToolType.Pickaxe },
        { ResourceType.Ore, ToolType.Hammer },
        { ResourceType.Tree, ToolType.Axe }
    };

    public ToolType CurrentTool { get; private set; }

    private void Awake()
    {
        EquipTool(startingTool);
    }

    public void EquipTool(ToolType tool)
    {
        CurrentTool = tool;
    }

    public bool HasToolFor(ResourceType type)
    {
        return resourceToolMap.TryGetValue(type, out var requiredTool) && CurrentTool == requiredTool;
    }

    public ToolType GetRequiredTool(ResourceType type)
    {
        if (resourceToolMap.TryGetValue(type, out var tool))
            return tool;

        return ToolType.None;
    }
}
