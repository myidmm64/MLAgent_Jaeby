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
    /// 일정한 각으로 발사
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

    public static List<T> BoundaryShoot<T>(GameArea area, BulletData bulletData, Vector2 MinBoundary, Vector2 MaxBoundary, Vector3 targetVector, int width, int height) where T : Bullet
    {
        List<T> result = new List<T>();
        for (int i = 0; i < width; i++)
        {
            float stepX = (MaxBoundary.x - MinBoundary.x) / width;
            Vector3 downPos = area.transform.position + new Vector3(MinBoundary.x + stepX * i,
                MinBoundary.y);
            Vector3 upPos = area.transform.position + new Vector3(MinBoundary.x + stepX * i,
                MaxBoundary.y);
            //아래
            BulletSpawn<T>(area, bulletData, downPos
                , LookTarget(downPos, targetVector));
            //위
            BulletSpawn<T>(area, bulletData, upPos
                , LookTarget(upPos, targetVector));
        }

        for (int i = 0; i < height; i++)
        {
            float stepY = (MaxBoundary.y - MinBoundary.y) / height;
            Vector3 leftPos = area.transform.position + new Vector3(MinBoundary.x,
                MinBoundary.y + stepY * i);
            Vector3 rightPos = area.transform.position + new Vector3(MaxBoundary.x,
                MinBoundary.y + stepY * i);
            //왼쪽ㄱ
            BulletSpawn<T>(area, bulletData, leftPos
               , LookTarget(leftPos, targetVector));
            //오른쪽
            BulletSpawn<T>(area, bulletData, rightPos
                , LookTarget(rightPos, targetVector));
        }
        return result;
    }
}
