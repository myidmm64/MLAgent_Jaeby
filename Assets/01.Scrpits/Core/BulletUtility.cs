using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BulletUtility
{
    /// <summary>
    /// 일정한 각으로 발사
    /// </summary>
    public static List<T> AngleShoot<T>(GameArea area, BulletData bulletData, PoolType poolType, Transform parent, Vector2 pos, int count, float startAngle, float angle) where T : Bullet
    {
        List<T> result = new List<T>();
        for(int i = 0 ; i < count; i++)
        {
            T bullet = PoolManager.Instance.Pop<T>(poolType);
            bullet.BulletInit(area, bulletData.sptire, parent, pos, 
                Quaternion.Euler(0, 0, ((startAngle - angle * (count / 3)) + i * angle)), bulletData.speed, bulletData.colliderRadius);
            result.Add(bullet);
        }
        return result;
    }

    public static List<T> CircleShoot<T>(GameArea area, BulletData bulletData, PoolType poolType, Transform parent, Vector2 pos, int count) where T : Bullet
    {
        List<T> result = new List<T>();
        float angleDistance = 360f / count;
        for (int i = 0; i < count; i++)
        {
            T bullet = PoolManager.Instance.Pop<T>(poolType);
            bullet.BulletInit(area, bulletData.sptire, parent, pos,
                Quaternion.Euler(0, 0, angleDistance * i), bulletData.speed, bulletData.colliderRadius);
            result.Add(bullet);
        }
        return result;
    }
}
