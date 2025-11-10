using UnityEngine;
using UnityEngine.UI;

public class GameTimerText : MonoBehaviour
{
    bool canCountUpTimer;
    float timer;
    Text timerText;

    private void Start()
    {
        timerText = GetComponent<Text>();
    }

    private void Update()
    {
        if (canCountUpTimer && (int)(timer / 60) < 100)
        {
            timer += Time.deltaTime;

            int minutes = (int)(timer / 60);
            int seconds = (int)(timer % 60);
            int tenths = (int)((timer * 1000) % 1000);
            timerText.text = $"{minutes:00}:{seconds:00}.{tenths:000}";
        }
    }

    public void StartCountUpTimer()
    {
        canCountUpTimer = true;
    }

    public void StopCountUpTimer()
    {
        canCountUpTimer = false;
    }
}
