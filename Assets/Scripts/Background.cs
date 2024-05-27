using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Background
{
    private readonly float _spawnPositionY;
    private readonly Queue<GameObject> _tiles = new();
    private const int MaxQueueCount = 3;

    public Background(float spawnPositionY)
    {
        _spawnPositionY = spawnPositionY;
    }

    public void Push(GameObject tile)
    {
        _tiles.Enqueue(tile);
        if (_tiles.Count > MaxQueueCount)
        {
            var tileToDestroy = _tiles.Dequeue();
            Object.Destroy(tileToDestroy);
        }
    }

    public void Move(float duration)
    {
        foreach (GameObject backgroundItem in _tiles)
        {
            var endPosition = new Vector3(
                0, 0, backgroundItem.transform.position.z - _spawnPositionY);
            backgroundItem.transform.DOMove(endPosition, duration).SetEase(Ease.Linear);
        }
    }
}