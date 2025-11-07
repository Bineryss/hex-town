using UnityEngine;
using UnityEngine.UIElements;

namespace Systems.Prototype_05.UI.Debug
{
    public class VisualDebugger : MonoBehaviour
    {
        [SerializeField] private UIDocument document;


        private void DebugZone(VisualElement root)
        {
            root.Add(new DataIndicator());
        }


        void OnValidate()
        {
            if (document == null) return;

            VisualElement root = document.rootVisualElement;
            root.Clear();
            DebugZone(root);
        }
    }
}