using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBullet : Bullet
{

    protected override void MoveBullet()
    {
        transform.Translate(Vector2.up * _speed * Time.deltaTime);
    }
}