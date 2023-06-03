using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<Bullet>()?.Push();
    }
}
