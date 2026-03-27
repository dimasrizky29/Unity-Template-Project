using UnityEngine;

public class QualityControl : MonoBehaviour
{
    //public TMP_Dropdown dropdown;
    public GameObject[] active;

    private void Start()
    {
        int value = PlayerPrefs.GetInt("quality", QualitySettings.GetQualityLevel());

        SetQualityObj(value);
    }

    public void Btn_ChangeQuality(int value)
    {
        QualitySettings.SetQualityLevel(value);
        PlayerPrefs.SetInt("quality", value);

        SetQualityObj(value);
    }

    private void SetQualityObj(int value) 
    {
        foreach (var obj in active)
            obj.SetActive(false);

        active[value].SetActive(true);
    }
}
