using TMPro;
using UnityEngine;

public class HintOfWordTextUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    public void SetWord(string word)
    {
        _text.text = word;
    }
}
