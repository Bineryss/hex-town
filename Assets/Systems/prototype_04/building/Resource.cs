using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Systems.Prototype_04.Building
{
    [CreateAssetMenu(fileName = "Resource", menuName = "Scriptable Objects/Resource")]
    public class Resource : SerializedScriptableObject
    {
        [ShowInInspector] public Guid Id => id;
        [ShowInInspector] public string Label => name;
        public ResourceType type;
        public ScoreResourceType scoreType;
        public int conversionRate = 1;
        private readonly Guid id = Guid.NewGuid();
    }

    public enum ResourceType
    {
        NONE,
        SHEEP,
        WOOL,
        WOOD,
        WHEAT,
        FLOUR,
        BREAD,
        PEOPLE,
        TOOLS,
        STONE,
        BRICK,
    }

    public enum ScoreResourceType
    {
        MATERIALS,
        FOOD,
        PEOPLE,
        GOLD,
    }
}