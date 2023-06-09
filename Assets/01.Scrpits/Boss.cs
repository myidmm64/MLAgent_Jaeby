using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private string StartPhase = "One";
    private readonly Vector2 MinBoundary = new Vector2(-4.2f, -4.5f);
    private readonly Vector2 MaxBoundary = new Vector2(4.2f, 4.5f);

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
    private float _moveIdleTime = 0.5f;
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

    [Space(20f)]
    [Header("패턴3")]
    [SerializeField]
    private BulletData _blueBulletData;
    [SerializeField]
    private BulletData _blueBulletSpawningData;
    [SerializeField]
    private float _spawnSpeed = 0.25f;
    [SerializeField]
    private List<BulletData> _boundBulletDatas = new List<BulletData>();
    [SerializeField]
    private int _boundBulletCountWidth = 8;
    [SerializeField]
    private int _boundBulletCountHeight = 16;

    [Space(20f)]
    [Header("패턴4")]
    [SerializeField]
    private BulletData _fourCircleShootData;
    [SerializeField]
    private BulletData _fourSpinShootData;
    [SerializeField]
    private BulletData _bombShootData;


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
        transform.position = _gameArea.transform.position + _originPosition;
        _curHP = _maxHP;
        UIManager.Instance.UpdateBossHP(_gameArea, _maxHP, _curHP);
        PhaseOne();
        //Invoke("Phase" + StartPhase, 0f);
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
        PatternsReset();
        _bossCoroutines.Add("PhaseOne", StartCoroutine(PhaseOneCoroutine()));
    }

    private void PhaseTwo()
    {
        PatternsReset();
        transform.DOMove(_gameArea.transform.position + _originPosition, _moveDelay);

        //_gameArea.Player.GetComponent<PlayerAgent>().AddReward(0.5f);
        _bossCoroutines.Add("PhaseTwo", StartCoroutine(PhaseTwoCoroutine()));
    }

    private void PhaseThree()
    {
        PatternsReset();
        transform.DOMove(_gameArea.transform.position + _originPosition, _moveDelay);

        //_gameArea.Player.GetComponent<PlayerAgent>().AddReward(0.5f);
        _bossCoroutines.Add("PhaseThree", StartCoroutine(PhaseThreeCoroutine()));
    }

    private void PhaseFour()
    {
        PatternsReset();
        transform.DOMove(_gameArea.transform.position + _originPosition, _moveDelay);

        //_gameArea.Player.GetComponent<PlayerAgent>().AddReward(0.5f);
        _bossCoroutines.Add("PhaseFour", StartCoroutine(PhaseFourCoroutine()));
    }

    private IEnumerator PhaseOneCoroutine()
    {
        yield return new WaitForSeconds(1f);
        _bossCoroutines.Add("CircleShoot", StartCoroutine(CircleShootCoroutine()));
        RandomMove(_moveCount, PhaseTwo);
    }

    private IEnumerator PhaseTwoCoroutine()
    {
        _bossCoroutines.Add("SpinBulletShoot", StartCoroutine(SpinBulletShoot()));
        yield return null;
    }

    private IEnumerator PhaseThreeCoroutine()
    {
        for (int i = 0; i < 4; i++)
            BulletUtility.BulletSpawn<MovingBulletSpawner>(_gameArea, _spawnBulletData, _gameArea.transform.position + _originPosition, Quaternion.Euler(0f, 0f, 90f * i))
                .SpawnStart(0.5f, _spawningData, _spawnBulletCount, _spawnBulletStartAngle, _spawnBulletAngle);
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < 4; i++)
            BulletUtility.BulletSpawn<MovingBulletSpawner>(_gameArea, _spawnBulletData, _gameArea.transform.position + _originPosition, Quaternion.Euler(0f, 0f, 90f * i + 45f))
                .SpawnStart(0.5f, _spawningData, _spawnBulletCount, _spawnBulletStartAngle, _spawnBulletAngle);
        yield return new WaitForSeconds(4f);

        BulletUtility.BulletSpawn<MovingBulletSpawner>(_gameArea, _blueBulletData, _gameArea.transform.position + _originPosition, Quaternion.Euler(0f, 0f, 180f))
            .SpawnStart(_spawnSpeed, _blueBulletSpawningData, _spawnBulletCount, _spawnBulletStartAngle, _spawnBulletAngle);

        yield return new WaitForSeconds(3.5f);
        BulletUtility.BoundaryShoot<NormalBullet>(_gameArea, _boundBulletDatas[0], MinBoundary, MaxBoundary,
            _gameArea.Player.transform.position, _boundBulletCountWidth, _boundBulletCountHeight);
        yield return new WaitForSeconds(2.5f);
        BulletUtility.BoundaryShoot<NormalBullet>(_gameArea, _boundBulletDatas[1], MinBoundary, MaxBoundary,
            _gameArea.Player.transform.position, _boundBulletCountWidth, _boundBulletCountHeight);
        yield return new WaitForSeconds(2.5f);
        BulletUtility.BoundaryShoot<NormalBullet>(_gameArea, _boundBulletDatas[2], MinBoundary, MaxBoundary,
            _gameArea.Player.transform.position, _boundBulletCountWidth, _boundBulletCountHeight);
        yield return new WaitForSeconds(4f);

        PhaseFour();
    }

    private IEnumerator PhaseFourCoroutine()
    {
        RandomMove(8);
        Vector2 spawnPosition = transform.position;
        for(int i = 0; i < 5; i++)
        {
            BulletUtility.CircleShoot<NormalBullet>(_gameArea, _fourCircleShootData,
                spawnPosition, 36, 0f + i % 2 == 0 ? 5f : 0f);
            yield return new WaitForSeconds(0.5f - i * 0.05f);
        }
        for (int i = 0; i < 720; i += 7)
        {
            BulletUtility.BulletSpawn<NormalBullet>(_gameArea, _fourSpinShootData, transform.position, Quaternion.Euler(0f, 0f, i));
            BulletUtility.BulletSpawn<NormalBullet>(_gameArea, _fourSpinShootData, transform.position, Quaternion.Euler(0f, 0f, 360 - i));
            yield return new WaitForSeconds(0.025f);
        }
        yield return new WaitForSeconds(4f);

        for(int k = 0; k <7; k++)
        {
            BulletData data = _bombShootData;
            float leftRandom = Random.Range(15f, 45f);
            float rightRandom = Random.Range(-15f, -45f);
            for (int i = 0; i < 4; i++)
            {
                BulletUtility.BulletSpawn<NormalBullet>(_gameArea, data, transform.position,
                    BulletUtility.LookTarget(transform.position, _gameArea.Player.transform.position));
                BulletUtility.BulletSpawn<NormalBullet>(_gameArea, data, transform.position,
                    Quaternion.Euler(0f, 0f, leftRandom) * BulletUtility.LookTarget(transform.position, _gameArea.Player.transform.position));
                BulletUtility.BulletSpawn<NormalBullet>(_gameArea, data, transform.position,
                    Quaternion.Euler(0f, 0f, rightRandom) * BulletUtility.LookTarget(transform.position, _gameArea.Player.transform.position));
                data.startSpeed *= 0.9f;
                data.endSpeed *= 0.9f;
            }
            yield return new WaitForSeconds(0.5f - k * 0.05f);
        }

        PhaseOne();
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

        PhaseThree();
    }

    private void RandomMove(int count, Action Callback = null)
    {
        transform.DOKill();
        _bossSequences.Add("RandomMove", DOTween.Sequence());
        Sequence moveSeq = _bossSequences["RandomMove"];
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPosition = _gameArea.transform.position + new Vector3(Random.Range(MinBoundary.x, MaxBoundary.x), Random.Range(1.8f, MaxBoundary.y), 0f);
            moveSeq.Append(transform.DOMove(randomPosition, _moveDelay).OnStart(() =>
            {
                MoveAnimationPlay((randomPosition.x - transform.position.x) < 0 ? -1 : 1);
            }));
            moveSeq.AppendInterval(_moveIdleTime);
        }
        moveSeq.Append(transform.DOMove(_gameArea.transform.position + _originPosition, _moveDelay))
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
            _gameArea.Player.GetComponent<PlayerAgent>().AddReward(10f);
            DieAction?.Invoke();
        }
        else
        {
            _gameArea.Player.GetComponent<PlayerAgent>().AddReward(0.001f);
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
