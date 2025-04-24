using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private Rigidbody bulletRigidbody;

    [Header("Bullet Constraints")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float damageRate = 3f;

    [Header("Interactive Objects Tags")]
    public string enemyTag = "enemy";
    public string enemyBossTag = "enemyBoss";

    [Header("Particle Effects Constraints")]
    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;

  

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("enemy"))
        {
            // Hit target
           Debug.Log("Collide with some thing..." + other.transform.name);
            GameObject temp = other.transform.parent.gameObject;
            Destroy(temp.transform.parent.gameObject);
            Instantiate(vfxHitGreen, transform.position, Quaternion.identity);
           // AudioManager.Instance.hapticFeedbackController.LightFeedback();
            Destroy(gameObject, lifetime);

        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("obstaclesLayer"))
        {
            // Hit Something else
            Instantiate(vfxHitRed, transform.position, Quaternion.identity);
            // Destroy(gameObject);
            Destroy(gameObject, lifetime);

        }
    }


}
