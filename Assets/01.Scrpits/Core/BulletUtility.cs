using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BulletUtility
{
    public static Quaternion LookTarget(Vector2 start, Vector2 target, float spriteRotation = 90f)
    {
        Vector2 dir = target - start;
        float result = Mathf.Atan2(dir.y, dir.x);
        return Quaternion.AngleAxis(result * Mathf.Rad2Deg - spriteRotation, Vector3.forward);
    }

    public static T BulletSpawn<T>(GameArea area, BulletData bulletData, Vector2 pos, Quaternion rot) where T : Bullet
    {
        T bullet = PoolManager.Instance.Pop<T>(bulletData.poolType);
        bullet.BulletInit(area, bulletData, pos, rot);
        return bullet;
    }

    /// <summary>
    /// ������ ������ �߻�
    /// </summary>
    public static List<T> AngleShoot<T>(GameArea area, BulletData bulletData, Vector2 pos, int count, float startAngle, float angle) where T : Bullet
    {
        List<T> result = new List<T>();
        for (int i = 0; i < count; i++)
        {
            T bullet = PoolManager.Instance.Pop<T>(bulletData.poolType);
            bullet.BulletInit(area, bulletData, pos,
                Quaternion.Euler(0, 0, ((startAngle - angle * (count / 3)) + i * angle)));
            result.Add(bullet);
        }
        return result;
    }

    public static List<T> CircleShoot<T>(GameArea area, BulletData bulletData, Vector2 pos, int count, float startRotation) where T : Bullet
    {
        List<T> result = new List<T>();
        float angleDistance = 360f / count;
        for (int i = 0; i < count; i++)
        {
            T bullet = PoolManager.Instance.Pop<T>(bulletData.poolType);
            bullet.BulletInit(area, bulletData, pos,
                Quaternion.Euler(0, 0, angleDistance * i + startRotation));
            result.Add(bullet);
        }
        return result;
    }
}
