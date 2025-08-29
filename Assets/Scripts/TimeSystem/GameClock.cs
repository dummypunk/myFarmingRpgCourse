using UnityEngine;
using TMPro;

public class GameClock : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText = null;
    [SerializeField] private TextMeshProUGUI dateText = null;
    [SerializeField] private TextMeshProUGUI seasonText = null;
    [SerializeField] private TextMeshProUGUI yearText = null;
    
    private void OnEnable()
    {
        EventHandler.AdvanceGameMinute += UpdateGameTime;
    }

    private void OnDisable()
    {
        EventHandler.AdvanceGameMinute -= UpdateGameTime;
    }

    private void UpdateGameTime(int gameYear, Season gameSeason, int gameDay, string dayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        //Update Time
        gameMinute = gameMinute - gameMinute % 10;

        string ampm = "";
        string minute;

        if (gameHour <= 12)
        {
            ampm = "am";
        }
        else
        {
            ampm = "pm";
        }

        if (gameHour >= 13)
        {
            gameHour -= 12;
        }

        if (gameMinute < 10)
        {
            minute = "0" + gameMinute;
        }
        else
        {
            minute = gameMinute.ToString();
        }
        
        string time = gameHour.ToString() + ":" + minute + ampm;
        
        timeText.SetText(time);
        dateText.SetText(dayOfWeek + "," + gameDay.ToString()); 
        seasonText.SetText(gameSeason.ToString()); 
        yearText.SetText("Year" + gameYear); 
    }
}
