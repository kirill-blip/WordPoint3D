using UnityEngine;
using UnityEngine.Events;

public class WordObject : MonoBehaviour
{
    public event UnityAction ObjectClicked;

    private void OnMouseDown()
    {
        ObjectClicked?.Invoke();
    }
}
