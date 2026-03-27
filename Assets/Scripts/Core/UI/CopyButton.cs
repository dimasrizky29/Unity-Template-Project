using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

[RequireComponent(typeof(Button))]
public class CopyButton : MonoBehaviour
{
    public TextMeshProUGUI id;

    private void Awake()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(CopyClipBoard);
    }

    public void CopyClipBoard()
    {
        GUIUtility.systemCopyBuffer = id.text;

        // Playsound with static
        GlobalResolver.Instance.Resolve<IAudioService>().PlayOneShot("ButtonGeneral");
    }
}
