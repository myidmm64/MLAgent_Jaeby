using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBulletSpawner : Bullet
{

    public void SpawnStart(float time, BulletData data, int count, float startAngle, float angle)
    {
        StartCoroutine(SpawnCoroutine(time, data, count, startAngle, angle));
    }

    private IEnumerator SpawnCoroutine(float time, BulletData data, int count, float startAngle, float angle)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);
            BulletUtility.AngleShoot<NormalBullet>(_area, data, transform.position, count, transform.localRotation.eulerAngles.z + startAngle, angle);
        }
    }

    public override void PushInit()
    {
        base.PushInit();
        StopAllCoroutines();
        transform.DOKill();
    }

    protected override void MoveBullet()
    {
        transform.Translate(Vector2.up * _speed * Time.deltaTime);
    }
}
