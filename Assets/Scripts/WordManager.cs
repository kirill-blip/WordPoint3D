using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class WordManager : MonoBehaviour
{
    [SerializeField] private List<WordData> _wordDataList;
    [SerializeField] private WordContainer _wordContainerPrefab;
    [SerializeField] private UnityEvent<Letter> _letterClickHandler;

    public WordContainer CurrentWordContainer { get; private set; }

    public WordContainer GetNextWordContainer()
    {
        // Get random data
        var wordData = _wordDataList[Random.Range(0, _wordDataList.Count)];
        _wordDataList.Remove(wordData);

        // Init container
        CurrentWordContainer = Instantiate(_wordContainerPrefab);
        CurrentWordContainer.SetData(wordData, _letterClickHandler);

        return CurrentWordContainer;
    }

    public bool HasNext()
    {
        return _wordDataList.Count != 0;
    }
}