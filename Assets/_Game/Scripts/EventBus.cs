using UnityEngine.Events;

public static class EventBus
{
    public static UnityEvent OnMainButtonPressed = new();
    public static UnityEvent<GameController.State> OnStateChanged = new();
    public static UnityEvent OnCardWasUsed = new();
    public static UnityEvent<int> OnScoresEarned = new();
}

