using DG.Tweening;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _playerSpeed = 1f;
    
    private bool IsMoving { get; set; }

    public IEnumerator MoveWithEasingAsync(Vector3 toPosition)
    {
        if (IsMoving)
        {
            yield break;
        }

        IsMoving = true;

        toPosition.y = 0;

        float duration = CalculateDuration(toPosition);
        yield return transform.DOMove(toPosition, duration).SetEase(Ease.Linear).WaitForCompletion();

        IsMoving = false;
    }

    public void TakeLetter(Letter letter)
    {
        letter.transform.SetParent(transform);
        letter.SetTriggerToCollider();
    }

    private float CalculateDuration(Vector3 to)
    {
        float distance = Vector3.Distance(transform.position, to);
        return distance / _playerSpeed;
    }

    public IEnumerator MoveAway()
    {
        return MoveWithEasingAsync(transform.position + new Vector3(0, 0, -1.25f));
    }
}