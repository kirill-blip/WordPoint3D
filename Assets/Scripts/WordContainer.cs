using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class WordContainer : MonoBehaviour
{
    [SerializeField] private Vector2 _leftTopBound;
    [SerializeField] private Vector2 _rightBottomBound;

    [Space(10f)]
    [SerializeField] private Letter _letterPrefab;
    [SerializeField] private Transform _letterPosition;
    [SerializeField] private Transform _wordObjectPosition;

    [Space(10f)]
    [SerializeField] private LayerMask _layerMask;

    [SerializeField] private TextMeshProUGUI _text;

    private List<Letter> _letters = new();
    private WordObject _wordObject;
    private WordData _wordData;

    private List<Letter> _appliedLetters = new();
    private List<Vector3> _letterPositions = new();

    private LetterAudio _letterAudio;

    private bool _isShuffled = false;

    private void ObjectClickedHandler()
    {
        _letterAudio ??= FindObjectOfType<LetterAudio>();

        if (!IsDone() && _isShuffled)
        {
            _text.text = GetWord();

            WordAudio.PlayWordAudio(GetWord());
            StartCoroutine(PlayLetterAudio());
        }
    }

    private IEnumerator PlayLetterAudio()
    {
        yield return new WaitForSeconds(.5f);

        _letterAudio.PlayAudioLetter(GetWord());

        yield return new WaitUntil(() => !_letterAudio.IsPlaying());

        _text.text = string.Empty;
    }

    public void SetData(WordData wordData, UnityEvent<Letter> letterClickHandler)
    {
        _letterPositions.Clear();
        _appliedLetters.Clear();

        _wordData = wordData;
        Instantiate(_wordData.WordObject, _wordObjectPosition);

        _wordObject = GetComponentInChildren<WordObject>();
        _wordObject.ObjectClicked += ObjectClickedHandler;

        for (int i = 0; i < _wordData.Word.Length; i++)
        {
            Vector3 position = _letterPosition.position + new Vector3(i, 0, 0);
            _letterPositions.Add(position);

            var letter = Instantiate(_letterPrefab, position, Quaternion.identity);
            letter.transform.SetParent(transform);
            letter.SetLetter(_wordData.Word[i]);
            letter.SetClickHandler(letterClickHandler);

            letter.gameObject.name = wordData.Word[i].ToString();

            _letters.Add(letter);
        }
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
    }

    public void ShuffleLetters(float letterDuration)
    {
        _isShuffled = true;

        List<Letter> lettersToMove = new List<Letter>();

        if (_appliedLetters.Count != 0)
        {
            for (int i = 0; i < _appliedLetters.Count; i++)
            {
                if (_appliedLetters[i].LetterValue != _letters[i].LetterValue)
                {
                    lettersToMove.Add(_appliedLetters[i]);

                    _appliedLetters[i] = null;
                }
            }
        }
        else
        {
            lettersToMove = _letters;
        }

        List<Vector3> positions = GeneratePositions(lettersToMove);

        for (int i = 0; i < lettersToMove.Count; i++)
        {
            lettersToMove[i].IsHandlerActive = true;
            lettersToMove[i].Move(positions[i], letterDuration);
        }
    }

    private List<Vector3> GeneratePositions(List<Letter> letters)
    {
        List<Vector3> result = new List<Vector3>();

        for (int i = 0; i < letters.Count; i++)
        {
            result.Add(GeneratePosition());
        }

        bool positionsChanged;

        do
        {
            positionsChanged = false;

            for (int i = 0; i < result.Count; i++)
            {
                Vector3 positionA = result[i];

                for (int j = i + 1; j < result.Count; j++)
                {
                    Vector3 positionB = result[j];

                    float distance = Vector3.Distance(positionA, positionB);

                    if (distance < 1.25f)
                    {
                        //Debug.Log("Positions " + positionA + " and " + positionB + " are within the distance threshold.");

                        result[j] = GeneratePosition();

                        //Debug.Log("Position " + positionB + " was regenerated to " + result[j]);

                        positionsChanged = true;
                    }
                }
            }
        } while (positionsChanged);

        return result;
    }

    private Vector3 GeneratePosition()
    {
        float x = Random.Range(_leftTopBound.x, _rightBottomBound.x);
        float z = Random.Range(_leftTopBound.y, _rightBottomBound.y);

        Vector3 position = new Vector3(x, 0, z);

        return position;
    }

    public bool IsDone()
    {
        if (!_appliedLetters.Any(x => x == null))
        {
            return _letters.Count == _appliedLetters.Count;
        }

        return false;
    }

    public void ApplyLetter(Letter letter)
    {
        if (_appliedLetters.Any(x => x == null))
        {
            for (int i = 0; i < _appliedLetters.Count; i++)
            {
                if (_appliedLetters[i] == null)
                {
                    _appliedLetters[i] = letter;
                    break;
                }
            }
        }
        else
        {
            _appliedLetters.Add(letter);
        }

        letter.transform.SetParent(transform);
    }

    public Vector3 GetCurrentTargetPosition()
    {
        int index = _appliedLetters.Count;

        if (_appliedLetters.Count != 0 && _appliedLetters.Any(x => x == null))
        {
            index = _appliedLetters.IndexOf(_appliedLetters.First(x => x == null));
        }

        return transform.TransformPoint(_letterPositions[index]);
    }

    public bool IsCorrect()
    {
        string userWord = string.Join("", _appliedLetters.Select(x => x.LetterValue).ToArray());

        return string.Equals(userWord, _wordData.Word, StringComparison.InvariantCultureIgnoreCase);
    }

    public string GetWord()
    {
        return _wordData.Word;
    }

    public bool IsShuffled()
    {
        return _isShuffled;
    }
}
