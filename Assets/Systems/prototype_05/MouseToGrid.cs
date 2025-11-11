using Systems.Core;
using Systems.Prototype_05.UI;
using UnityEngine;


namespace Systems.Prototype_05.Player
{

    public class MouseToGrid : MonoBehaviour
    {
        [SerializeField] private PlayerGridSelector playerGridSelector;

        void OnEnable()
        {
            playerGridSelector.OnChange += (node, clicked) => EventBus<TileSelectionChanged>.Raise(new TileSelectionChanged()
            {
                node = node,
                clicked = clicked
            });
        }
    }

    public struct TileSelectionChanged : IEvent
    {
        public WorldNode node;
        public bool clicked;
    }
}

