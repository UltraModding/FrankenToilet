using UnityEngine;
using TMPro;

// For MonoSingleton

public class LmsCountdown : MonoBehaviour
{
    public float startTimeInSeconds = 113f;
    public TextMeshProUGUI timerText;

    float currentTime;
    bool triggered;

    void Start()
    {
        currentTime = startTimeInSeconds;
        UpdateDisplay();
        triggered = false;
    }

    void Update()
    {
        if (currentTime <= 0f) return;

        currentTime -= Time.unscaledDeltaTime;
        if (currentTime < 0f) currentTime = 0f;

        UpdateDisplay();

        if (currentTime <= 0f && !triggered)
        {
            triggered = true;
            var nm = MonoSingleton<NewMovement>.Instance;
            if (nm != null)
            {
                nm.GetHurt(9999, false);
            }
        }
    }

    void UpdateDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
