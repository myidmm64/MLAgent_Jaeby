using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private GameArea _gameArea = null;
    private Dictionary<string, Coroutine> _bossCoroutines = new Dictionary<string, Coroutine>();
    private Dictionary<string, Sequence> _bossSequences = new Dictionary<string, Sequence>();

    [SerializeField]
    private int _maxHP = 100;
    private int _curHP = 0;

    public Action DieAction = null;

    [SerializeField]
    private Vector3 _originPosition = Vector3.zero;
    private Animator _animator = null;

    [Space(20f)]
    [Header("1∆‰¿Ã¡Ó")]
    [SerializeField]
    private int _moveCount = 5;
    [SerializeField]
    private float _moveDelay = 0.5f;
    [SerializeField]
    private BulletData _circleShootData;
    [SerializeField]
    private float _circleShootDelay = 0.5f;

    private void Awake()
    {
        _gameArea.Player.OnEpisodeBeginAction += Init;
        _animator = transform.Find("Renderer").GetComponent<Animator>();
    }

    private void Init()
    {
        foreach (var seq in _bossSequences.Values)
            seq.Kill();
        _bossSequences.Clear();
        StopAllCoroutines();
        _bossCoroutines.Clear();

        transform.position = _originPosition;
        _curHP = _maxHP;
        UIManager.Instance.UpdateBossHP(_gameArea, _maxHP, _curHP);
        PhaseOne();
    }

    private void PhaseOne()
    {
        _bossCoroutines.Add("CircleShoot", StartCoroutine(CircleShootCoroutine()));
        _bossCoroutines.Add("RandomMove", StartCoroutine(RandomMoveCoroutine()));
    }

    private IEnumerator RandomMoveCoroutine()
    {
        yield return new WaitForSeconds(1f);

        _bossSequences.Add("RandomMove", DOTween.Sequence());
        Sequence moveSeq = _bossSequences["RandomMove"];
        for (int i = 0; i < _moveCount; i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-4f, 4f), Random.Range(1.8f, 4f), 0f);
            moveSeq.Append(transform.DOMove(randomPosition, _moveDelay));
            MoveAnimationPlay((randomPosition.x - transform.position.x) < 0 ? -1 : 1);
            yield return new WaitForSeconds(_moveDelay);
        }
        moveSeq.Append(transform.DOMove(_originPosition, _moveDelay));
        StopCoroutine(_bossCoroutines["CircleShoot"]);
        MoveAnimationPlay(0);
    }

    private IEnumerator CircleShootCoroutine()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(_circleShootDelay);

            BulletUtility.CircleShoot<NormalBullet>(_gameArea, _circleShootData, PoolType.NormalBullet, null,
                transform.position, 36);

            BulletUtility.CircleShoot<NormalBullet>(_gameArea,
                new BulletData { sptire = _circleShootData.sptire, speed = _circleShootData.speed * 0.9f }
                , PoolType.NormalBullet, null,
                transform.position, 36);
        }
    }

    public void Damage(int value)
    {
        _curHP -= value;
        _curHP = Mathf.Clamp(_curHP, 0, _maxHP);
        UIManager.Instance.UpdateBossHP(_gameArea, _maxHP, _curHP);

        if (_curHP <= 0)
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

    private void MoveAnimationPlay(int value)
    {
        switch (value)
        {
            case 0:
                _animator.Play("BossIdle");
                break;
            case -1:
                _animator.Play("BossLeftMove");
                break;
            case 1:
                _animator.Play("BossRIghtMove");
                break;
            default:
                break;
        }
        _animator.Update(0);
    }
}
