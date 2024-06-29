using System;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    [SerializeField] private bool _firstTimeShuffledWord = false;

    private WordManager _wordManager;
    private const string FIRST_TIME_SHUFFLED_WORD = "FirstTimeShuffledWord";

    private void Awake()
    {
#if !UNITY_EDITOR
        if (PlayerPrefs.HasKey(FIRST_TIME_SHUFFLED_WORD))
        {
            _firstTimeShuffledWord = Convert.ToBoolean(PlayerPrefs.GetInt(FIRST_TIME_SHUFFLED_WORD));
        }
#endif

        _wordManager = FindObjectOfType<WordManager>();
    }
}
