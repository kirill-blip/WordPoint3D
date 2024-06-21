using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _spawnPosition;
    [SerializeField] private GameObject _backgroundTilePrefab;
    [SerializeField] private Transform _backgroundParent;

    [SerializeField] private GameObject _backgroundDefaultTile;

    [Space(10f)]
    [SerializeField] private Transform _wordContainerInitPosition;

    [Space(10f)]
    [SerializeField] private float _duration = 1;
    [SerializeField] private float _letterDuration = 1;

    [Space(10f)]
    [SerializeField] private AudioClip[] _correctSounds;
    [SerializeField] private AudioClip _incorrectSound;

    private Player _player;
    private UserInterface _userInterface;
    private WordManager _wordManager;
    private LetterAudio _letterAudio;

    private Background _background;

    private Queue<Letter> _letterQueue;
    private bool _isLetterHandling;

    private void Awake()
    {
        _background = new Background(_spawnPosition.transform.position.z);
        _background.Push(_backgroundDefaultTile);
    }

    private void Start()
    {
        _letterQueue = new Queue<Letter>();

        _player = FindObjectOfType<Player>();
        _wordManager = FindObjectOfType<WordManager>();
        _userInterface = FindObjectOfType<UserInterface>();
        _letterAudio = FindObjectOfType<LetterAudio>();

        _userInterface.NextWordButtonPressed += () => StartCoroutine(NextWordButtonPressedHandler());
        _userInterface.GoButtonPressed += GoButtonPressedHandler;
        _userInterface.HelpButtonPressed += HelpButtonPressed;
    }

    private void HelpButtonPressed()
    {
        bool canPlayLetterAudio =
            !string.IsNullOrEmpty(_wordManager.CurrentWordContainer.GetWord())
            && _wordManager.CurrentWordContainer.IsShuffled();

        if (canPlayLetterAudio)
        {
            _letterAudio.PlayAudioLetter(_wordManager.CurrentWordContainer.GetWord());
        }

    }

    private IEnumerator NextWordButtonPressedHandler()
    {
        if (!_wordManager.HasNext())
        {
            yield break;
        }

        GameObject tile = CreateTile();

        _background.Push(tile);

        // Получить слово (Word Container)
        WordContainer wordContainer = _wordManager.GetNextWordContainer();

        wordContainer.SetPosition(_wordContainerInitPosition.position);
        // Нужно расположить WC на фон (кусок фона - BackgroundTile, который будет двигаться)
        wordContainer.SetParent(tile.transform);

        // Двинуть 
        yield return _background.Move(_duration);

        _userInterface.ActivateGoButton();
    }

    private GameObject CreateTile()
    {
        return Instantiate(_backgroundTilePrefab, _spawnPosition.transform.position, Quaternion.identity,
            _backgroundParent);
    }

    private void GoButtonPressedHandler()
    {
        DisassembleWord();
    }

    private void DisassembleWord()
    {
        _wordManager.CurrentWordContainer.ShuffleLetters(_letterDuration);
    }

    public void LetterClickedHandler(Letter letter)
    {
        letter.IsHandlerActive = false;
        _letterQueue.Enqueue(letter);

        if (!_isLetterHandling)
        {
            HandlerLetterQueue();
        }
    }

    private void HandlerLetterQueue()
    {
        if (_letterQueue.Count > 0)
        {
            StartCoroutine(DoHandlerLetterQueue(_letterQueue.Dequeue()));
        }
    }

    private IEnumerator DoHandlerLetterQueue(Letter letter)
    {
        if (_isLetterHandling)
        {
            throw new InvalidOperationException("Handler is busy");
        }

        _isLetterHandling = true;

        yield return _player.MoveWithEasingAsync(letter.transform.position);

        _player.TakeLetter(letter);

        // Нужно узнать, куда мы должны тащить букву.
        Vector3 targetLetterPosition = _wordManager.CurrentWordContainer.GetCurrentTargetPosition();

        yield return _player.MoveWithEasingAsync(targetLetterPosition);

        _wordManager.CurrentWordContainer.ApplyLetter(letter);

        if (_letterQueue.Count == 0 || _wordManager.CurrentWordContainer.IsDone())
        {
            yield return _player.MoveAway();
        }

        _isLetterHandling = false;

        if (_wordManager.CurrentWordContainer.IsDone())
        {
            if (_wordManager.CurrentWordContainer.IsCorrect())
            {
                _userInterface.HandleWordAssembled();

                if (!_wordManager.HasNext())
                {
                    _userInterface.HandleWordsOvered();
                }

                Vector3 to = new Vector3(0, 0, 8f);
                _background.Move(to, _duration);
                _player.MoveWithEasing(_player.transform.position - to, _duration);

                AudioSource.PlayClipAtPoint(_correctSounds[Random.Range(0, _correctSounds.Length)], Vector3.zero, 1f);
            }
            else
            {
                _userInterface.HandleWordAssembledIncorrect();
                AudioSource.PlayClipAtPoint(_incorrectSound, Vector3.zero, 1f);
                ReassembleWord();
            }
        }
        else
        {
            HandlerLetterQueue();
        }
    }

    private void ReassembleWord()
    {
        _wordManager.CurrentWordContainer.ShuffleLetters(_letterDuration);
    }
}