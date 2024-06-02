﻿using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Background
{
    private readonly float _spawnPositionZ;
    private readonly Queue<GameObject> _tiles = new();
    private const int MaxQueueCount = 3;

    public Background(float spawnPositionZ)
    {
        _spawnPositionZ = spawnPositionZ;
    }

    public void Push(GameObject tile)
    {
        _tiles.Enqueue(tile);
        Debug.Log(_tiles.Count);

        if (_tiles.Count > MaxQueueCount)
        {
            Debug.Log(_tiles.Count);

            var tileToDestroy = _tiles.Dequeue();
            Object.Destroy(tileToDestroy);
        }
    }

    public void Move(float duration)
    {
        foreach (GameObject backgroundItem in _tiles)
        {
            var endPosition = new Vector3(
                0, 0, backgroundItem.transform.position.z - _spawnPositionZ);
            backgroundItem.transform.DOMove(endPosition, duration).SetEase(Ease.Linear);
        }
    }

    public void Move(Vector3 to, float duration)
    {
        foreach (GameObject backgroundItem in _tiles)
        {
            var endPosition = new Vector3(
                0, 0, backgroundItem.transform.position.z - to.z);
            backgroundItem.transform.DOMove(endPosition, duration).SetEase(Ease.Linear);
        }
    }
}