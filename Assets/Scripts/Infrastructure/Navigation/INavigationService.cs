using Cysharp.Threading.Tasks;

public interface INavigationService
{
    AppRoute CurrentRoute { get; }

    UniTask GoBack();
    UniTask NavigateTo(AppRoute targetRoute);
    void RegisterView(ScenePageContainer view);
}