using UnityEngine;
using VContainer;
using VContainer.Unity;

public class AuthLifetimeScope : LifetimeScope
{
    [Header("Scene Components")]
    [SerializeField] private ScenePageContainer scenePageContainer;
    [SerializeField] private AuthView authView;

    protected override void Configure(IContainerBuilder builder)
    {
        // 1. Daftarkan Komponen Unity (MonoBehaviour)
        builder.RegisterComponent(scenePageContainer);
        builder.RegisterComponent(authView);

        // 2. Daftarkan Logika (Plain C#)
        // VContainer akan otomatis memberikan 'authView' ke constructor Presenter ini
        builder.Register<AuthFlowPresenters>(Lifetime.Scoped);
    }
}
