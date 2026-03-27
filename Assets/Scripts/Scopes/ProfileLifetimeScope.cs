using VContainer;
using VContainer.Unity;

public class ProfileLifetimeScope : LifetimeScope
{
    public ProfilePageView userProfileView;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<ProfilePresenter>(Lifetime.Scoped);
        builder.RegisterComponent(userProfileView);
    }
}
