using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Letter : MonoBehaviour
{
    [SerializeField] private TextMeshPro[] _texts;

    private Collider _collider;
    private bool _isMoving;

    public char LetterValue { get; private set; }

    private UnityEvent<Letter> _clickHandler;

    public bool IsHandlerActive { get; set; }

    public Vector3 InitialContainerPosition { get; private set; }

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        InitialContainerPosition = transform.position;
    }

    private void OnMouseDown()
    {
        if (IsHandlerActive)
        {
            _clickHandler?.Invoke(this);
        }
    }

    public void SetLetter(char letter)
    {
        LetterValue = letter;

        foreach (var text in _texts)
        {
            text.text = LetterValue.ToString();
        }
    }

    public void SetClickHandler(UnityEvent<Letter> handler)
    {
        _clickHandler = handler;
    }

    public void SetTriggerToCollider()
    {
        _collider.isTrigger = true;
    }

    public void Move(Vector3 position, float duration)
    {
        _collider.isTrigger = false;
        transform.DOLocalMove(position, duration).SetEase(Ease.Linear);
    }
}