namespace Systems.UI
{
    public interface IUIController
    {
        IUIModeSegment UIModeSegment { get; }
        void Initialize();
        void HandleMouseInteraction(WorldNode node, bool isClick);
    }
}