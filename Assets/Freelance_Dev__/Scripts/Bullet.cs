using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 50f;
    public float lifetime = 3f;
    public float damageRate = 0.5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            collision.gameObject.SetActive(false); // Deactivate the enemy
            Destroy(gameObject); // Destroy this object
        }
        else if (collision.gameObject.CompareTag("bossEnemy"))
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(damageRate); // Deactivate the bossEnemy
            Destroy(gameObject); // Destroy this object
        }
    }
}
