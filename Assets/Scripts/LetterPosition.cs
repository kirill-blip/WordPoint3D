using UnityEngine;

public class LetterPosition : MonoBehaviour
{
    private Letter _letter;

    public Letter Letter => _letter;

    public bool TrySetLetter(Letter letter)
    {
        if (_letter == null)
        {
            return true;
        }

        return false;
    }

    public void SetLetter(Letter letter)
    {
        _letter = letter;

        if (_letter is null)
        {
            return;
        }

        _letter.transform.SetParent(transform);
        _letter.transform.localPosition = Vector3.zero;
    }
}
