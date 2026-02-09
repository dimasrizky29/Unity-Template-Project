using Infrastructure.Networking;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ProjectLifetimeScope : LifetimeScope
{
    [SerializeField] private GlobalUIView globalUIView;
    [SerializeField] private NavigationConfig navigationConfig;

    protected override void Configure(IContainerBuilder builder)
    {
        // Gunakan Singleton karena kita ingin .env hanya dibaca SEKALI selama game jalan
        builder.Register<IApiConfig, ApiConfig>(Lifetime.Singleton);

        // --- 1. Infrastructure (API & Network) ---
        // Singleton: Karena kita cuma butuh 1 koneksi API
        builder.Register<IHttpClient, BaseHttpClient>(Lifetime.Singleton);
        builder.Register<IGameApiService, GameApiService>(Lifetime.Singleton);
        // Implementasi Auth API yang membungkus ApiService
        builder.Register<IAuthApiService, AuthApiService>(Lifetime.Singleton);

        // --- 2. State Management (Data) ---
        // Singleton: Token user harus tersimpan terus
        builder.Register<IAuthStore, AuthStore>(Lifetime.Singleton);

        // --- 3. Navigation Service ---
        builder.RegisterInstance(navigationConfig);
        builder.Register<INavigationService, NavigationService>(Lifetime.Singleton);

        // --- 3. Global UI Service ---
        builder.RegisterComponent(globalUIView);
        builder.Register<IGlobalUIService, GlobalUIService>(Lifetime.Singleton);

        // --- 4. Entry Point ---
        builder.RegisterEntryPoint<GameInitializer>();
    }
}
