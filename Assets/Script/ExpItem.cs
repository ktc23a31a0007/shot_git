using UnityEngine;

public class ExpItem : MonoBehaviour
{
    public float expValue = 10f;       // このオーブ1個の経験値量
    public float moveSpeed = 12f;      // 吸い寄せられるスピード
    private Transform player;
    private bool isFollowing = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 【修正】FindFirstObjectByType を使用（警告対策＆高速化）
        var stats = Object.FindFirstObjectByType<PlayerStats>();
        if (stats != null) player = stats.transform;

        // 【ばらまき演出】出現時にランダムな方向に飛ばす
        Vector2 scatterDir = Random.insideUnitCircle.normalized * Random.Range(3f, 6f);
        rb.AddForce(scatterDir, ForceMode2D.Impulse);

        // 0.5秒後に空気抵抗を強くして、その場に留まるようにする
        Invoke("EnableDrag", 0.5f);
    }

    void EnableDrag() { rb.linearDamping = 5f; }

    void Update()
    {
        if (player == null) return;

        // プレイヤーの回収範囲（固定値）を取得
        var stats = player.GetComponent<PlayerStats>();
        if (stats == null) return;

        float pickupRange = stats.pickupRange;
        float distance = Vector2.Distance(transform.position, player.position);

        // 範囲内に入ったら吸い寄せモードON
        if (distance < pickupRange) isFollowing = true;

        if (isFollowing)
        {
            rb.linearVelocity = Vector2.zero; // 物理挙動を止める

            // 【修正】isKinematic から bodyType への変更（警告解消）
            rb.bodyType = RigidbodyType2D.Kinematic;

            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }

        // プレイヤーに重なったら経験値獲得
        if (distance < 0.3f)
        {
            // 【修正】FindFirstObjectByType を使用
            var levelManager = Object.FindFirstObjectByType<LevelManager>();
            if (levelManager != null)
            {
                levelManager.GainExp(expValue);
            }
            Destroy(gameObject);
        }
    }

    // 画面外に出たら消える（メモリ節約）
    void OnBecameInvisible() { Destroy(gameObject); }
}