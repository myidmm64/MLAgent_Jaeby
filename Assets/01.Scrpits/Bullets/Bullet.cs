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
    }

    public override void StartInit()
    {
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void BulletInit(GameArea area, Sprite bulletSprite, Transform parent, Vector3 position, Quaternion rot, float startSpeed, float? colliderRadius = null)
    {
        _area = area;
        transform.SetParent(area.BulletFactory);
        if (parent != null)
            transform.SetParent(parent);
        if (colliderRadius == null)
        {
            _col.radius = 0.16f;
        }
        else
        {
            if (colliderRadius.Value == 0f)
                _col.radius = 0.16f;
            else
                _col.radius = colliderRadius.Value;
        }
        transform.SetPositionAndRotation(position, rot);
        _spriteRenderer.sprite = bulletSprite;
        _speed = startSpeed;
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
    public Sprite sptire;
    public float colliderRadius;
    public float speed;
    public float angle;
}