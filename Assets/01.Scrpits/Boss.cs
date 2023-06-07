using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private string StartPhase = "One";
    private readonly Vector2 MinBoundary = new Vector2(-4f, -4f);
    private readonly Vector2 MaxBoundary = new Vector2(4f, 4f);

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
    [Header("패턴1")]
    [SerializeField]
    private int _moveCount = 5;
    [SerializeField]
    private float _moveDelay = 0.5f;
    [SerializeField]
    private BulletData _circleShootData;
    [SerializeField]
    private float _circleShootDelay = 0.5f;

    [Space(20f)]
    [Header("패턴2")]
    [SerializeField]
    private BulletData _spinShootData;
    [SerializeField]
    private BulletData _bigBulletData;
    [SerializeField]
    private BulletData _spawnBulletData;
    [SerializeField]
    private BulletData _spawningData;
    [SerializeField]
    private int _spawnBulletCount = 2;
    [SerializeField]
    private float _spawnBulletStartAngle = 0f;
    [SerializeField]
    private float _spawnBulletAngle = 180f;

    private void Awake()
    {
        _gameArea.Player.OnEpisodeBeginAction += Init;
        _animator = transform.Find("Renderer").GetComponent<Animator>();
    }

    private void Init()
    {
        PatternsReset();

        MoveAnimationPlay(0);
        transform.DOKill();
        transform.position = _originPosition;
        _curHP = _maxHP;
        UIManager.Instance.UpdateBossHP(_gameArea, _maxHP, _curHP);
        Invoke("Phase" + StartPhase, 0f);
    }

    private void PatternsReset()
    {
        foreach (var seq in _bossSequences.Values)
            seq.Kill();
        foreach (var coroutine in _bossCoroutines.Values)
            StopCoroutine(coroutine);
        _bossSequences.Clear();
        _bossCoroutines.Clear();
    }

    private void PhaseOne()
    {
        _bossCoroutines.Add("PhaseOne", StartCoroutine(PhaseOneCoroutine()));
    }

    private void PhaseTwo()
    {
        _bossCoroutines.Add("PhaseTwo", StartCoroutine(PhaseTwoCoroutine()));
    }

    private IEnumerator PhaseOneCoroutine()
    {
        yield return new WaitForSeconds(1f);
        _bossCoroutines.Add("CircleShoot", StartCoroutine(CircleShootCoroutine()));
        RandomMove(_moveCount, PhaseTwo);
    }

    private IEnumerator PhaseTwoCoroutine()
    {
        PatternsReset();
        _bossCoroutines.Add("SpinBulletShoot", StartCoroutine(SpinBulletShoot()));
        yield return null;
    }

    private IEnumerator SpinBulletShoot()
    {
        for (int i = 0; i < 360; i += 5)
        {
            BulletUtility.BulletSpawn<NormalBullet>(_gameArea, _spinShootData, transform.position, Quaternion.Euler(0f, 0f, i));
            BulletUtility.BulletSpawn<NormalBullet>(_gameArea, _spinShootData, transform.position, Quaternion.Euler(0f, 0f, 360 - i));
            yield return new WaitForSeconds(0.025f);
        }
        RandomMove(3);

        for (int i = 0; i < 6; i++)
        {
            BulletUtility.BulletSpawn<NormalBullet>(_gameArea, _bigBulletData, transform.position, BulletUtility.LookTarget(transform.position, _gameArea.Player.transform.position));
            BulletUtility.BulletSpawn<NormalBullet>(_gameArea, _bigBulletData, transform.position, Quaternion.Euler(0f, 0f, 30f) * BulletUtility.LookTarget(transform.position, _gameArea.Player.transform.position));
            BulletUtility.BulletSpawn<NormalBullet>(_gameArea, _bigBulletData, transform.position, Quaternion.Euler(0f, 0f, -30f) * BulletUtility.LookTarget(transform.position, _gameArea.Player.transform.position));
            yield return new WaitForSeconds(_moveDelay * 0.5f);
        }

        for(int i = 0; i < 4; i++)
        {
            BulletUtility.BulletSpawn<MovingBulletSpawner>(_gameArea, _spawnBulletData, _originPosition, Quaternion.Euler(0f, 0f, 90f * i))
                .SpawnStart(0.5f, _spawningData, _spawnBulletCount, _spawnBulletStartAngle, _spawnBulletAngle);
        }

        for (int i = 0; i < 4; i++)
        {
            BulletUtility.BulletSpawn<MovingBulletSpawner>(_gameArea, _spawnBulletData, _originPosition, Quaternion.Euler(0f, 0f, 90f * i + 45f))
                .SpawnStart(1f, _spawningData, _spawnBulletCount, _spawnBulletStartAngle, _spawnBulletAngle);
        }
    }

    private void RandomMove(int count, Action Callback = null)
    {
        _bossSequences.Add("RandomMove", DOTween.Sequence());
        Sequence moveSeq = _bossSequences["RandomMove"];
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(MinBoundary.x, MaxBoundary.x), Random.Range(1.8f, MaxBoundary.y), 0f);
            moveSeq.Append(transform.DOMove(randomPosition, _moveDelay).OnStart(() =>
            {
                MoveAnimationPlay((randomPosition.x - transform.position.x) < 0 ? -1 : 1);
            }));
        }
        moveSeq.Append(transform.DOMove(_originPosition, _moveDelay))
            .OnComplete(() =>
            {
                StopCoroutine(_bossCoroutines["CircleShoot"]);
                MoveAnimationPlay(0);
                Callback?.Invoke();
            });
    }

    private IEnumerator CircleShootCoroutine()
    {
        BulletData data = _circleShootData;
        data.startSpeed *= 0.9f;
        data.endSpeed *= 0.9f;
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(_circleShootDelay);

            BulletUtility.CircleShoot<NormalBullet>(_gameArea, _circleShootData,
                transform.position, 36, 0f);

            BulletUtility.CircleShoot<NormalBullet>(_gameArea,
                data
                , transform.position, 36, 15f);
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
