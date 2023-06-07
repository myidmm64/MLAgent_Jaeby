using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameArea : MonoBehaviour
{
    [SerializeField]
    private PlayerAgent _player = null;
    public PlayerAgent Player => _player;

    [SerializeField]
    private Transform _bulletFactory = null;
    public Transform BulletFactory => _bulletFactory;

    private int score = 0;
    public int Score => score;

    private void Start()
    {
        _player.OnEpisodeBeginAction += DestoryAllBullet;
        StartCoroutine(ScoreCoroutine());
    }

    private IEnumerator ScoreCoroutine()
    {
        while(true)
        {
            score += 13;
            UIManager.Instance.UpdateScore(this);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public List<T> GetBullets<T>() where T : Bullet
    {
        List<T> result = new List<T>();
        result.AddRange(_bulletFactory.GetComponentsInChildren<T>());
        return result;
    }

    public void DestoryAllBullet()
    {
        foreach(var bullet in GetBullets<Bullet>())
        {
            bullet.Push();
        }
    }
}
