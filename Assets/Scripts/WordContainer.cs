using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class WordContainer : MonoBehaviour
{
    [SerializeField] private Vector2 _leftTopBound;
    [SerializeField] private Vector2 _rightBottomBound;

    [SerializeField] private Letter _letterPrefab;
    [SerializeField] private Transform _letterPosition;

    [SerializeField] private MeshRenderer _pictureMaterial;
    [SerializeField] private LayerMask _layerMask;

    private readonly List<Letter> _letters = new();
    private WordData _wordData;

    private List<Letter> _appliedLetters = new();
    private List<Vector3> _letterPositions = new();

    public void SetData(WordData wordData, UnityEvent<Letter> letterClickHandler)
    {
        _letterPositions.Clear();
        _appliedLetters.Clear();

        _wordData = wordData;
        _pictureMaterial.material = _wordData.Picture;

        for (int i = 0; i < _wordData.Word.Length; i++)
        {
            Vector3 position = _letterPosition.position + new Vector3(i, 0, 0);
            _letterPositions.Add(position);

            var letter = Instantiate(_letterPrefab, position, Quaternion.identity);
            letter.transform.SetParent(transform);
            letter.SetLetter(_wordData.Word[i]);
            letter.SetClickHandler(letterClickHandler);

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
        foreach (var letter in _letters)
        {
            Vector3 position = GeneratePosition();

            letter.IsHandlerActive = true;
            letter.Move(position, letterDuration);
        }
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
        return _letters.Count == _appliedLetters.Count;
    }

    public void ApplyLetter(Letter letter)
    {
        _appliedLetters.Add(letter);
        letter.transform.SetParent(transform);
    }

    public Vector3 GetCurrentTargetPosition()
    {
        return transform.TransformPoint(_letterPositions[_appliedLetters.Count]);
    }

    public bool IsCorrect()
    {
        string userWord = string.Join("", _appliedLetters.Select(x => x.LetterValue).ToArray());

        return string.Equals(userWord, _wordData.Word, StringComparison.InvariantCultureIgnoreCase);
    }
}