using UnityEngine;
using UnityEngine.UI;

public class ExitPageView : BasePagePanel
{
    [SerializeField] private Button exitBtn;

    protected override void Awake()
    {
        base.Awake();
        exitBtn.onClick.AddListener(OnExitApplication);
    }

    private void OnExitApplication()
    {
        Application.Quit();
    }
}
