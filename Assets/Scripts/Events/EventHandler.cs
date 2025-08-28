using System;
using System.Collections.Generic;

public delegate void MovementDelegate
    (float inputX, float inputY, bool isWalking, bool isRunning, bool isIdle,
     bool isCarrying, ToolEffect toolEffect,
     bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
     bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
     bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
     bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
     bool idleUp,bool idleDown,bool idleLeft,bool idleRight);

public static class EventHandler
{
    public static event Action<InventoryLocation, List<InventoryItem>> InventoryUpdatedEvent;

    public static void CallInventoryUpdatedEvent(InventoryLocation inventoryLocation,List<InventoryItem> inventoryItems)
    {
        if (InventoryUpdatedEvent != null)
            InventoryUpdatedEvent(inventoryLocation, inventoryItems);
    }

    //Movement Event
    public static event MovementDelegate MovementEvent;
    //Movement Event Call For Publishers
    public static void CallMovementEvent(float inputX, float inputY, bool isWalking, bool isRunning, bool isIdle,
    bool isCarrying, ToolEffect toolEffect,
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
    bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
    bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
    bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
    bool idleUp, bool idleDown, bool idleLeft, bool idleRight)
    {
        if (MovementEvent != null)
            MovementEvent(inputX, inputY,
                          isWalking, isRunning, isIdle, isCarrying,
                          toolEffect,
                          isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
                          isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
                          isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
                          isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
                          idleUp, idleDown, idleLeft, idleRight);
    }



    //Time Event 
    //Advance game minute
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameMinute;

    public static void CallAdvanceGameMinuteEvent(int gameYear, Season gameSeason, int gameDay, string gameDayofWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if(AdvanceGameMinute != null)
        {
            AdvanceGameMinute(gameYear, gameSeason, gameDay, gameDayofWeek, gameHour, gameMinute, gameSecond);
        }
    }

    //Advance game hour
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameHour;

    public static void CallAdvanceGameHourEvent(int gameYear, Season gameSeason, int gameDay, string gameDayofWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameMinute != null)
        {
            AdvanceGameHour(gameYear, gameSeason, gameDay, gameDayofWeek, gameHour, gameMinute, gameSecond);
        }
    }

    //Advance game Day
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameDay;

    public static void CallAdvanceGameDayEvent(int gameYear, Season gameSeason, int gameDay, string gameDayofWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameMinute != null)
        {
            AdvanceGameDay(gameYear, gameSeason, gameDay, gameDayofWeek, gameHour, gameMinute, gameSecond);
        }
    }


    //Advance game Season
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameSeason;

    public static void CallAdvanceGameSeasonEvent(int gameYear, Season gameSeason, int gameDay, string gameDayofWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameMinute != null)
        {
            AdvanceGameSeason(gameYear, gameSeason, gameDay, gameDayofWeek, gameHour, gameMinute, gameSecond);
        }
    }

    //Advance game Year
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameYear;

    public static void CallAdvanceGameYearEvent(int gameYear, Season gameSeason, int gameDay, string gameDayofWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameMinute != null)
        {
            AdvanceGameSeason(gameYear, gameSeason, gameDay, gameDayofWeek, gameHour, gameMinute, gameSecond);
        }
    }
}