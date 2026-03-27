public interface IPagePanel : IViewPanel
{
    // Menerima data dari NavigationService (Deep Linking)
    void SetPayload(object payload);

    // Kontrol internal untuk Sub-Panel
    void SwitchSubPanel(string subPanelId, bool addToHistory = true);

    // Fungsi 'Back' lokal (cek history internal dulu baru global)
    void Back();

    bool TryBackSubPanel();
}
