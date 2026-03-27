using UnityEngine;

public interface IPageContainer
{
    public void HideAllPanels();
    public IViewPanel GetPanel(AppRoute route);
}
