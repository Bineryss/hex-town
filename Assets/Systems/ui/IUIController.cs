namespace Systems.UI
{
    public interface IUIController
    {
        IUIModeSegment UIModeSegment { get; }
        void Initialize();
        void HandleMouseInteraction(WorldNode node, WorldNode prevNode, bool isClick);
        void Exit();
        void Activate();
    }
}