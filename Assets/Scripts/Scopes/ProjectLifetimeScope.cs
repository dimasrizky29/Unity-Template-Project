using Infrastructure.Networking;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ProjectLifetimeScope : LifetimeScope
{
    [SerializeField] private GlobalUIView globalUIView;
    [SerializeField] private AudioLibrary audioLibrary;
    [SerializeField] private NavigationConfig navigationConfig;

    protected override void Configure(IContainerBuilder builder)
    {
        // Gunakan Singleton karena kita ingin .env hanya dibaca SEKALI selama game jalan
        builder.Register<IApiConfig, ApiConfig>(Lifetime.Singleton);

        // --- 1. Infrastructure (API & network) ---
        // Singleton: Karena kita cuma butuh 1 koneksi API
        builder.Register<IHttpClient, BaseHttpClient>(Lifetime.Singleton);
        builder.Register<IGameApiService, GameApiService>(Lifetime.Singleton);
        // Implementasi Auth API yang membungkus ApiService
        builder.Register<IAuthApiService, AuthApiService>(Lifetime.Singleton);
        // Implementasi WebSocket Service yang membungkus koneksi WebSocket
        builder.Register<WebSocketService>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.Register<SocketMessageDispatcher>(Lifetime.Singleton).AsImplementedInterfaces();

        // --- 2. State Management (Data) ---
        // Singleton: Token user harus tersimpan terus
        builder.Register<IAuthStore, AuthStore>(Lifetime.Singleton);

        // --- 3. Audio Service ---
        builder.RegisterInstance(audioLibrary);
        builder.Register<IAudioService, AudioService>(Lifetime.Singleton);

        // --- 4. Navigation Service ---
        builder.RegisterInstance(navigationConfig);
        builder.Register<INavigationService, NavigationService>(Lifetime.Singleton);

        // --- 5. Global UI Service ---
        builder.Register<IGlobalUIService, GlobalUIPresenter>(Lifetime.Singleton);
        builder.RegisterComponent(globalUIView);

        // --- 6. User Service & Repository ---
        builder.Register<IUserApiService, UserApiService>(Lifetime.Singleton);
        builder.Register<IUserRepository, UserRepository>(Lifetime.Singleton);

        // -- Deeplink Service
        builder.Register<DeepLinkService>(Lifetime.Singleton).AsImplementedInterfaces();

        // --- Entry Point ---
        builder.RegisterEntryPoint<AppBootstrap>();

        // Set Resolver (Danger, only for button sound or small utility)
        builder.RegisterBuildCallback(resolver =>
        {
            GlobalResolver.Instance = resolver;
        });

        // register mobile input device
        builder.RegisterEntryPoint<MobileInputListener>();
    }
}
