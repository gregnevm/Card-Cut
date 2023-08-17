using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameController : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Card _playcardPrefab;

    [Header("References")]
    [SerializeField] private Transform _startCardPosition;
    [SerializeField] private PathCreator _pathCreator;  

    [Header("Settings")]
    [SerializeField] private float  _xPositionDelta;
    [SerializeField] private float _torqueSpin = 200f;
    [SerializeField] private float _cardMoveSpeed = 0.5f;

    public State CurrentState { get; private set; }
    public Card CurrentCard { get; private set; }
    public int Score { get; private set; }
    

    private const float SMALL_DISTANCE_THRESHOLD = 0.01f;
    
   
    private Coroutine _currentAnimation;
    private bool _firstTickOfState = true;

    private void Start()
    {        
        CurrentState = State.Reload;
        EventBus.OnMainButtonPressed.AddListener(ButtonPressed);
        InitPathCreator();
    }

    private void Update()
    {
        StateHandler();       
    }

    private void StateHandler()
    {
        switch (CurrentState)
        {
            case State.ChooseRoute:
                ChooseRoute();
                break;
            case State.ChooseTorque:
                ChooseTorque();
                break;
            case State.ChoosePower:
                ChoosePower();
                break;
            case State.Strike:
                Strike();
                break;
            case State.Reload:
                Reload();
                break;
        }
    }

    private void NextState()
    {
        if (CurrentState != State.Strike)
        {
            CurrentState++;
            _firstTickOfState = true;
            Debug.Log(CurrentState);
            EventBus.OnStateChanged.Invoke(CurrentState);
        }
        else
        {
            CurrentState = State.Reload;
            Debug.Log(CurrentState);
            EventBus.OnStateChanged.Invoke(CurrentState);
            _firstTickOfState = true;
        }
    }

    private void InitPathCreator()
    {
        _pathCreator.startPoint = CurrentCard ? CurrentCard.transform : _pathCreator.startPoint;
        DrawRoute();
    }

    private void ChooseRoute()
    {
        if (_firstTickOfState)
        {
            _pathCreator.RedrawLine();
            _firstTickOfState = false;
            DrawRoute();            
            _pathCreator.EnableLine();
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            
                Vector3 curPosition = CurrentCard.transform.position;
                float deltaX = Input.GetTouch(0).deltaPosition.x;

                float lerpValue = Mathf.InverseLerp(-_xPositionDelta, _xPositionDelta, curPosition.x) + (deltaX / Screen.width);
                float newX = Mathf.Lerp(-_xPositionDelta, _xPositionDelta, lerpValue);

                Vector3 newPosition = new(newX, curPosition.y, curPosition.z);
                CurrentCard.transform.position = newPosition;
                DrawRoute();
            
        }
    }

    private void ChooseTorque()
    {
        if (_firstTickOfState)
        {
            _pathCreator.RedrawLine();
            _firstTickOfState = false;
            DrawRoute();
        }

        

        if (Input.touchCount>0 )
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved ) // defeat from random touches or UI using 
            {
                float normalizedTouchX = touch.position.x / Screen.width;
                float torqueAmount = Mathf.Lerp(-_torqueSpin, _torqueSpin, normalizedTouchX);

                CurrentCard.Rigidbody.AddRelativeTorque (Vector3.up*torqueAmount);                 

                float curveFactor = normalizedTouchX * 2f - 1f;
                DrawRoute(curveFactor);
            }
            
        }

    }

    private void ChoosePower()
    {
        if (_firstTickOfState)
        {
            _firstTickOfState = false;
        }
        // TODO: Implement logic there

        NextState();
    }

    private void Strike()
    {
        if (_firstTickOfState)
        {          
            _firstTickOfState = false;

            if (_currentAnimation != null)
            {
                StopCoroutine(_currentAnimation);
            }

            _currentAnimation = StartCoroutine(StrikeMoveRoutine(_pathCreator.GetPositions()));
        }
    }

    private IEnumerator StrikeMoveRoutine(List<Vector3> points)
    {
        foreach (Vector3 targetPoint in points)
        {
            while (Vector3.Distance(CurrentCard.transform.position, targetPoint) > SMALL_DISTANCE_THRESHOLD)
            {
                var position = Vector3.MoveTowards(CurrentCard.transform.position, targetPoint, Time.fixedDeltaTime * _cardMoveSpeed);
                CurrentCard.Rigidbody.MovePosition(position);                

                yield return new WaitForEndOfFrame();
            }
        }

        EndStrike(); 
    }

    private void EndStrike()
    {
        EventBus.OnCardWasUsed.Invoke();        
        CurrentCard.TurnOver();
        CurrentCard.transform.SetParent(_startCardPosition);
        NextState();
    }

    private void DrawRoute()
    {
        _pathCreator.startPoint = CurrentCard ? CurrentCard.transform : _startCardPosition;
        _pathCreator.UpdateLine(0f);
        _pathCreator.EnableLine();
    }

    private void DrawRoute(float curveFactor)
    {
        _pathCreator.startPoint = CurrentCard.transform;
        _pathCreator.UpdateLine(curveFactor);
    }

    private void Reload()
    {
        StopAllCoroutines();        
        if (_currentAnimation != null)
        {
            StopCoroutine(_currentAnimation);
        }

        if (CurrentCard == null)
        {
            CurrentCard = InitNewCard();
            CurrentCard.Rigidbody.AddTorque(_torqueSpin * Vector3.up, ForceMode.VelocityChange);
            NextState();
            EventBus.OnStateChanged.Invoke(CurrentState);
            return;
        }

        CurrentCard.TurnOver();
        CurrentCard = null;
    }

    private void ButtonPressed()
    {
        NextState();
    }

    private Card InitNewCard()
    {
        // TODO: Add more logic or replace it with Zenject or Factory
        Card card = Instantiate(_playcardPrefab, _startCardPosition.position, Quaternion.identity);        
        return card;
    }
}
