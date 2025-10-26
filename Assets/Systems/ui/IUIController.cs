namespace Systems.UI
{
    public interface IUIController
    {
        void HandleMouseInteraction(WorldNode node, bool isClick);
    }
}