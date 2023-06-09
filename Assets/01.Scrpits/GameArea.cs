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
    private Boss _boss = null;
    public Boss boss => _boss;

    [SerializeField]
    private Transform _bulletFactory = null;
    public Transform BulletFactory => _bulletFactory;

    [SerializeField]
    private Transform _playerBulletFactory = null;
    public Transform PlayerBulletFactory => _playerBulletFactory;

    private int score = 0;
    public int Score => score;

    private void Start()
    {
        _player.OnEpisodeBeginAction += () => DestoryAllBullet(BulletTag.All);
        _player.OnEpisodeBeginAction += () => score = 0;
        StartCoroutine(ScoreCoroutine());
    }

    private IEnumerator ScoreCoroutine()
    {
        while (true)
        {
            score += 13;
            UIManager.Instance.UpdateScore(this);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public List<T> GetBullets<T>(BulletTag tag) where T : Bullet
    {
        List<T> result = new List<T>();
        switch (tag)
        {
            case BulletTag.None:
                return null;
            case BulletTag.Player:
                result.AddRange(_playerBulletFactory.GetComponentsInChildren<T>());
                break;
            case BulletTag.Bullet:
                result.AddRange(_bulletFactory.GetComponentsInChildren<T>());
                break;
            case BulletTag.All:
                result.AddRange(_playerBulletFactory.GetComponentsInChildren<T>());
                result.AddRange(_bulletFactory.GetComponentsInChildren<T>());
                break;
            default:
                break;
        }
        return result;
    }

    public void DestoryAllBullet(BulletTag tag)
    {
        foreach (var bullet in GetBullets<Bullet>(tag))
            bullet.Push();
    }
}

public enum BulletTag
{
    None,
    Player,
    Bullet,
    All
}
