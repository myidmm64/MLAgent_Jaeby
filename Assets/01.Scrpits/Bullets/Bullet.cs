using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : PoolableObject
{
    private SpriteRenderer _spriteRenderer = null;
    protected float _speed = 0f;

    public override void PopInit()
    {
    }

    public override void PushInit()
    {
        transform.localScale = Vector3.one;
    }

    public override void StartInit()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void BulletInit(Sprite bulletSprite, Transform parent, Vector3 position, Quaternion rot, float startSpeed)
    {
        _spriteRenderer.sprite = bulletSprite;
        transform.SetParent(parent);
        transform.SetPositionAndRotation(position, rot);
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
    public float speed;
    public float angle;
}