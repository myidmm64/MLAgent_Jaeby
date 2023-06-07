using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBulletSpawner : Bullet
{
    public override void BulletInit(GameArea area, Sprite bulletSprite, Transform parent, Vector3 position, Quaternion rot, float startSpeed, float? colliderRadius = null)
    {
        base.BulletInit(area, bulletSprite, parent, position, rot, startSpeed, colliderRadius);
    }

    public void SpawnStart(float time, BulletData data, int count, float startAngle, float angle)
    {
        StartCoroutine(SpawnCoroutine(time, data, count, startAngle, angle));
    }

    private IEnumerator SpawnCoroutine(float time, BulletData data, int count, float startAngle, float angle)
    {
        while(true)
        {
            yield return new WaitForSeconds(time);
            BulletUtility.AngleShoot<NormalBullet>(_area, data, PoolType.NormalBullet, _area.BulletFactory, transform.position,
                count, startAngle, angle);
        }
    }

    public override void PushInit()
    {
        base.PushInit();
        StopAllCoroutines();
        //StopCoroutine(_spawnCoroutine);
    }

    protected override void MoveBullet()
    {
        transform.Translate(Vector2.up * _speed * Time.deltaTime);
    }
}
