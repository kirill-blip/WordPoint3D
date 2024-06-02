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

    [Space(10f)] [SerializeField] private Transform _wordContainerInitPosition;

    [Space(10f)] [SerializeField] private float _duration = 1;
    [SerializeField] private float _letterDuration = 1;

    [Space(10f)] [SerializeField] private AudioClip[] _correctSounds;
    [SerializeField] private AudioClip _incorrectSound;

    private Player _player;
    private UserInterface _userInterface;
    private WordManager _wordManager;

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

        _userInterface.GoButtonPressed += GoButtonPressedHandler;
        _userInterface.NextWordButtonPressed += NextWordButtonPressedHandler;
    }

    private void NextWordButtonPressedHandler()
    {
        if (!_wordManager.HasNext())
        {
            return;
        }

        GameObject tile = CreateTile();

        _background.Push(tile);

        // Получить слово (Word Container)
        WordContainer wordContainer = _wordManager.GetNextWordContainer();

        wordContainer.SetPosition(_wordContainerInitPosition.position);
        // Нужно расположить WC на фон (кусок фона - BackgroundTile, который будет двигаться)
        wordContainer.SetParent(tile.transform);

        // Двинуть 
        _background.Move(_duration);
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

        // if (_wordFromLetters.Length == _wordManager.GetCurrentWordData().Word.Length)
        // {
        //     if (_wordFromLetters.ToLower().Trim() == _wordManager.GetCurrentWordData().Word.ToLower().Trim())
        //     {
        //         _letterPositions.ForEach(x => x.transform.SetParent(_wordManager.CurrentWordContainer.transform));
        //
        //         Vector3 movePosition = new Vector3(0, -8f, 0);
        //
        //         List<Task> tasks = new()
        //         {
        //             _wordManager.CurrentWordContainer.transform
        //                 .DOMove(_wordManager.CurrentWordContainer.transform.position + movePosition, _duration)
        //                 .SetEase(Ease.Linear).AsyncWaitForCompletion(),
        //             _player.MoveWithEasingAsync(_player.transform.position + movePosition, _duration)
        //         };
        //
        //         foreach (GameObject item in _backgroundTiles)
        //         {
        //             tasks.Add(item.transform.DOMove(item.transform.position + movePosition, _duration)
        //                 .SetEase(Ease.Linear).AsyncWaitForCompletion());
        //         }
        //
        //         await Task.WhenAll(tasks);
        //
        //         // WordAssembled?.Invoke();
        //         _userInterface.HandleWordAssembled();
        //
        //         AudioSource.PlayClipAtPoint(_correctSounds[Random.Range(0, _correctSounds.Length)], Vector3.zero, 1f);
        //     }
    }

    private void ReassembleWord()
    {
        _wordManager.CurrentWordContainer.ShuffleLetters(_letterDuration);
    }
}