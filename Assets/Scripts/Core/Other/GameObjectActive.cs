using UnityEngine;

public class GameObjectActive : MonoBehaviour
{
    public float timerToDisable = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetActive(float timerDisable = 0)
    {
        if (timerDisable > 0)
            timerToDisable = timerDisable;
        
        gameObject.SetActive(true);

        Invoke(nameof(Disable), timerToDisable);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
