using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Rigidbody2D arrowRb;
    public float speed = 2.5f;
    public float range = 1;
    private float timer;
    public int damage = 10;
    public float knockbackForce = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = range;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        arrowRb.linearVelocity = Vector2.right * speed;

        timer -= Time.deltaTime;
        if(timer <0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyHealth>())
        {
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.right * knockbackForce, ForceMode2D.Impulse);
            collision.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }



}
