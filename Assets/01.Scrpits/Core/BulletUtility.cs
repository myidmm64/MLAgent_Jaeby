using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BulletUtility
{
    /// <summary>
    /// 일정한 각으로 발사
    /// </summary>
    public static List<T> AngleShoot<T>(BulletData bulletData, PoolType poolType, Transform parent, Vector2 pos, int count, float startAngle, float angle, float spriteAngle = 0f) where T : Bullet
    {
        List<T> result = new List<T>();
        for(int i = 0 ; i < count; i++)
        {
            T bullet = PoolManager.Instance.Pop<T>(poolType);
            bullet.BulletInit(bulletData.sptire, parent, pos, 
                Quaternion.Euler(0, 0, ((startAngle - angle * (count / 3)) + i * angle) + spriteAngle), bulletData.speed);
            result.Add(bullet);
        }
        return result;
    }
}
