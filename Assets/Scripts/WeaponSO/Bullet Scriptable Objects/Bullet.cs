using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletDataSO bulletData;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Worm"))
        {
            Worm worm = collision.transform.GetComponent<Worm>();

            if (worm == null)
                return;

            //worm.TakeDamage(bulletData.damage);
        }

        ExecuteBulletHole(collision);
    }

    private void ExecuteBulletHole(Collider2D collision)
    {
        if (!collision.transform.CompareTag("Ground"))
            return;

        Ground ground = collision.transform.GetComponent<Ground>();

        ground.ExplosionHole(gameObject.GetComponent<CapsuleCollider2D>(), bulletData.explosionRadius);
        Destroy(gameObject);
    }
}
