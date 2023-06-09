using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Timeline;

public class PlayerAgent : Agent
{
    [SerializeField]
    private GameArea _gameArea = null;

    [SerializeField]
    private BulletData _normalBulletData;
    [SerializeField]
    private BulletData _specialBulletData;

    [SerializeField]
    private float _normalSpeed = 4f;
    [SerializeField]
    private float _slowSpeed = 2f;
    [SerializeField]
    private float _shootDelay = 0.1f;

    [SerializeField]
    private int _maxHp = 3;
    public int MaxHp => _maxHp;
    private int _hp = 0;
    public int HP => _hp;

    [SerializeField]
    private int _maxBomb = 3;
    public int MaxBomb => _maxHp;
    private int _curBomb = 0;
    public int CurBomb => _curBomb;

    private Animator _animator = null;
    private Animator _obAnimator = null;
    private SpriteRenderer _colliderSpriteRenderer = null;
    private Sequence _fadeSeq = null;
    private Rigidbody2D _rigid = null;

    private Boss _boss = null;

    public Action OnEpisodeBeginAction = null;

    private MoveState _moveState = MoveState.None;

    public override void Initialize()
    {
        Application.runInBackground = true;

        _colliderSpriteRenderer = transform.Find("Collider").GetComponent<SpriteRenderer>();
        _animator = transform.Find("Renderer").GetComponent<Animator>();
        _obAnimator = _animator.transform.Find("Obs").GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody2D>();
        _moveState = MoveState.Normal;
        _boss = _gameArea.transform.Find("Boss").GetComponent<Boss>();
        _boss.DieAction += EndEpisode;
    }

    public override void OnEpisodeBegin()
    {
        StopAllCoroutines();
        StartCoroutine(ShootCoroutine());

        _hp = _maxHp;
        transform.position = _gameArea.transform.position + new Vector3(0f,-3f,0f);
        OnEpisodeBeginAction?.Invoke();
    }

    private IEnumerator ShootCoroutine()
    {
        while (true)
        {
            List<NormalBullet> bull = BulletUtility.AngleShoot<NormalBullet>(_gameArea, _normalBulletData, transform.position
                , 6, 0f, 3f);

            int spe = UnityEngine.Random.Range(1, 4);
            List<NormalBullet> bullets = BulletUtility.AngleShoot<NormalBullet>(_gameArea, _specialBulletData, transform.position
                , spe, 0f, 6f);

            for (int i = 0; i < bull.Count; i++)
                bull[i].transform.SetParent(_gameArea.PlayerBulletFactory);
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].transform.SetParent(_gameArea.PlayerBulletFactory);
                bullets[i].transform.localScale = new Vector3(2f, 0.5f, 1f);
            }

            yield return new WaitForSeconds(_shootDelay);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        MoveState oldState = _moveState;
        _moveState = actions.DiscreteActions.Array[0] == 1 ? MoveState.Slow : MoveState.Normal;
        _obAnimator.SetInteger("Shift", actions.DiscreteActions.Array[0]);

        float speed = _normalSpeed;
        if (_moveState == MoveState.Slow)
            speed = _slowSpeed;

        if (oldState != _moveState)
        {
            float targetAlpha = _moveState == MoveState.Slow ? 1f : 0f;
            if (_fadeSeq != null)
                _fadeSeq.Kill();
            _fadeSeq = DOTween.Sequence();
            _fadeSeq.Append(_colliderSpriteRenderer.DOFade(targetAlpha, 0.25f));
        }

        _rigid.velocity = new Vector2(actions.ContinuousActions.Array[0], actions.ContinuousActions.Array[1]).normalized * speed;
        int hori = Mathf.RoundToInt(actions.ContinuousActions.Array[0]);
        _animator.SetInteger("Horizontal", hori);

        AddReward(0.001f);
        if (actions.ContinuousActions.Array[2] < 0.5f)
            AddReward(0.002f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuous = actionsOut.ContinuousActions;
        continuous.Array[0] = (float)Input.GetAxisRaw("Horizontal");
        continuous.Array[1] = (float)Input.GetAxisRaw("Vertical");
        continuous.Array[2] = (float)Mathf.Abs(_gameArea.boss.transform.position.x - transform.position.x);

        var discreate = actionsOut.DiscreteActions;
        discreate.Array[0] = Input.GetKey(KeyCode.LeftShift) ? 1 : 0;
    }

    public void Damaged(int value)
    {
        _hp = Mathf.Clamp(_hp - value, 0, _maxHp);
        UIManager.Instance.UpdatePlayerHP(_gameArea);
        if (_hp <= 0)
        {
            AddReward(-3f);
            EndEpisode();
        }
        else
        {
            AddReward(-1f * value);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            _gameArea.DestoryAllBullet(BulletTag.Bullet);
            Damaged(1);
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            EndEpisode();
        }
    }
}

public enum MoveState
{
    None,
    Normal,
    Slow
}
