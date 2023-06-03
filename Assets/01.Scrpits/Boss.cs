using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private GameArea _gameArea = null;

    [SerializeField]
    private int _maxHP = 100;
    private int _curHP = 0;

    public Action DieAction = null;

    private void Awake()
    {
        _gameArea.Player.OnEpisodeBeginAction += Init;
    }

    private void Init()
    {
        _curHP = _maxHP;
        UIManager.Instance.UpdateBossHP(_gameArea, _maxHP, _curHP);
    }

    public void Damage(int value)
    {
        _curHP -= value;
        _curHP = Mathf.Clamp(_curHP, 0, _maxHP);
        UIManager.Instance.UpdateBossHP(_gameArea, _maxHP, _curHP);

        if(_curHP <= 0)
        {
            DieAction?.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBullet"))
        {
            collision.GetComponent<Bullet>()?.Push();
            Damage(1);
        }
    }
}
