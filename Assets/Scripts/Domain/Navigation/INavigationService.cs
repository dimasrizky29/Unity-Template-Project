using Cysharp.Threading.Tasks;

public interface INavigationService
{
    AppRoute CurrentRoute { get; }

    UniTask GoBack();
    bool CanGoBack { get; }
    UniTask<bool> HandleHardwareBack();


    UniTask NavigateTo(AppRoute targetRoute, string additionalNameScene = "", object payload = null);
    UniTask NavigateToInGame(AppRoute route, string env, object payload = null);
    void RegisterView(IPageContainer view);
}