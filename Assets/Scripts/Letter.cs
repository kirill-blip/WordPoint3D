using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Letter : MonoBehaviour
{
    [SerializeField] private Color _targetColor;
    [SerializeField] private float _emissiveIntensity = 5f;

    [Space(10f)]
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _duration = .5f;
    [SerializeField] private TextMeshPro[] _texts;

    private Collider _collider;
    private Rigidbody _rigidbody;
    private Renderer _renderer;

    private bool _isMoving;
    private Color _initialColor;

    public char LetterValue { get; private set; }
    public bool IsHandlerActive { get; set; }
    public Vector3 InitialContainerPosition { get; private set; }
    
    private UnityEvent<Letter> _clickHandler;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
        _renderer = GetComponent<Renderer>();

        _initialColor = _renderer.material.color;

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

    public void Jump()
    {
        transform.DOJump(transform.position, _jumpForce, 1, _duration);
    }

    public IEnumerator ChangeLettersColorTo(Color color)
    {
        float elapsedTime = 0.0f;
        float duration = 2f;

        Color from = _renderer.material.color;
        Color to = color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float time = elapsedTime / duration;

            Color newColor = Color.Lerp(from, to, time);

            _renderer.material.color = newColor;

            yield return null;
        }

        _renderer.material.color = to;
    }

    public void ChangeLettersColorToDefault()
    {
        StartCoroutine(ChangeLettersColorTo(_initialColor));
    }

    public void ChangeLettersColorToTransparent()
    {
        StartCoroutine(ChangeLettersColorTo(_targetColor));
    }
}
