using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public abstract class Bullet : PoolableObject
{
    [SerializeField]
    private SpriteRenderer _spriteRenderer = null;
    [SerializeField]
    private CircleCollider2D _col = null;
    protected float _speed = 0f;

    protected GameArea _area = null;

    public override void PopInit()
    {
    }

    public override void PushInit()
    {
        transform.localScale = Vector3.one;
        _col.radius = 0.16f;
        DOTween.Kill(this);
    }

    public override void StartInit()
    {
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void BulletInit(GameArea area, BulletData data, Vector3 position, Quaternion rot)
    {
        _area = area;
        transform.SetParent(area.BulletFactory);
        if (data.colliderRadius == 0f)
            _col.radius = 0.16f;
        transform.SetPositionAndRotation(position, rot);
        _spriteRenderer.sprite = data.sptire;
        _speed = data.startSpeed;
        if (data.moveEaseType == Ease.Unset)
            return;
        DOTween.To(() => _speed, x => _speed = x, data.endSpeed, data.duration).SetEase(data.moveEaseType);
    }

    public void Push()
    {
        PoolManager.Instance.Push(this);
    }

    private void Update()
    {
        MoveBullet();
    }

    protected abstract void MoveBullet();
}

[System.Serializable]
public struct BulletData
{
    public PoolType poolType;
    public Sprite sptire;
    public float colliderRadius;
    public float startSpeed;
    public float endSpeed;
    public float duration;
    public Ease moveEaseType;
}