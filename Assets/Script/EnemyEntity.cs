using UnityEngine;

public class EnemyEntity : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float hp = 20f;
    public float moveSpeed = 3f;
    public float damageToPlayer = 10f;

    [Header("Drop Settings")]
    public GameObject expItemPrefab; // インスペクターでExpItemプレハブを登録
    public int minDropCount = 2;     // 最小ドロップ数
    public int maxDropCount = 5;     // 最大ドロップ数

    private Transform player;

    void Start()
    {
        // プレイヤーをタグで探す
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        // シンプルな追跡ロジック（必要に応じて）
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// ダメージを受ける処理（弾側から呼ばれる）
    /// </summary>
    public void TakeDamage(float amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // 経験値オーブをランダムな数生成してばらまく
        int dropCount = Random.Range(minDropCount, maxDropCount + 1);

        for (int i = 0; i < dropCount; i++)
        {
            if (expItemPrefab != null)
            {
                // 敵の位置に生成
                Instantiate(expItemPrefab, transform.position, Quaternion.identity);
            }
        }

        // 敵自身を消去
        Destroy(gameObject);
    }

    // プレイヤーとの接触判定
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // プレイヤーにダメージを与える処理などをここに書く
            Debug.Log("プレイヤーに衝突！");
        }
    }
}