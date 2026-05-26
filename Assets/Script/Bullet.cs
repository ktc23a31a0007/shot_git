using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public float damage = 10f;
    public float lifeSpan = 3f;

    void Start()
    {
        Destroy(gameObject, lifeSpan); // 念のため時間で消滅
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // 敵のスクリプトを取得
            EnemyEntity enemy = collision.GetComponent<EnemyEntity>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("敵にダメージを与えました！");
            }
            else
            {
                // ここが呼ばれる場合は、敵に EnemyEntity.cs がアタッチされていません
                Debug.LogError($"{collision.name} に EnemyEntity スクリプトが見つかりません！");
            }

            Destroy(gameObject); // 弾を消す
        }
    }
}