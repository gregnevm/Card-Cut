using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using TMPro;

public partial class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] List<StateImageDependency> _stateSprites;
    [SerializeField] Image _buttonImage;
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] TextMeshProUGUI _cardsCountText;

    [Header("Settings")]
    [SerializeField] int _cardsCount;
    private int _scores = 0;

    private GameController.State _currentState;
    private void Start()
    {       
        EventBus.OnStateChanged.AddListener(UpdateState);
        EventBus.OnCardWasUsed.AddListener(CardUsed);
        EventBus.OnScoresEarned.AddListener(ScoresEarned);
        UpdateCards();
        UpdateScores();
    }

    public void ButtonPressed()
    {
        EventBus.OnMainButtonPressed.Invoke();
    }

    private void UpdateState(GameController.State newState)
    {
        _currentState = newState;
        UpdateImage();
    }

    private void UpdateImage()
    {
        StateImageDependency dependency = _stateSprites.FirstOrDefault(dep => dep.state == _currentState);
        if (dependency.sprite != null)
        {
            _buttonImage.sprite = dependency.sprite;
        }
    }
    private void CardUsed()
    {
        _cardsCount--;
        UpdateCards();
    }
    void ScoresEarned(int scores)
    {
        _scores += scores;
        UpdateScores();
    }
    private void UpdateScores()
    {
        _scoreText.text = _scores.ToString();
    }
    private void UpdateCards()
    {
        _cardsCountText.text = _cardsCount.ToString();
    }
}
