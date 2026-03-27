public interface ISubPanel : IViewPanel
{
    // Identitas unik di dalam Page (misal: "Avatar", "Username")
    string SubPanelId { get; }

    // Mengenal siapa managernya agar bisa lapor (misal: minta Back)
    void Initialize(IPagePanel parent);
}