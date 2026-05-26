using UnityEngine;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    public float currentHP;
    public float maxHP = 100f;
    public float attackDamage = 10f;
    public float fireRate = 0.5f;
    public float moveSpeed = 7f;
    public float expMultiplier = 1.0f;
    public float damageReduction = 0f;
    public int shotCount = 1;

    // 回収範囲を固定値に設定
    [Header("Fixed Stats")]
    public float pickupRange = 3f;

    [Header("Timers")]
    public float barrierTimer = 0f;
    public float bombTimer = 0f;

    public Dictionary<UpgradeType, int> upgradeLevels = new Dictionary<UpgradeType, int>();

    private void Start()
    {
        currentHP = maxHP;
        UpdatePickupRangeVisual(); // 固定の範囲で一度だけ表示を更新
    }

    private void Update()
    {
        if (barrierTimer > 0) barrierTimer -= Time.deltaTime;
        if (bombTimer > 0) bombTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.B) && bombTimer <= 0)
        {
            ExecuteBomb();
        }
    }

    public void ApplyUpgrade(UpgradeData data)
    {
        if (!upgradeLevels.ContainsKey(data.type)) upgradeLevels[data.type] = 0;
        upgradeLevels[data.type]++;

        // 【安全対策】文字からすべての半角スペースを取り除く（"Exp Gain" -> "ExpGain" に自動変換）
        string typeName = data.type.ToString().Replace(" ", "");

        if (typeName.StartsWith("ExpGain")) expMultiplier += data.value;
        else if (typeName.StartsWith("DamageReduce")) damageReduction += data.value;
        else if (typeName.StartsWith("Damage")) attackDamage += data.value;
        else if (typeName.StartsWith("MaxHP")) { maxHP += data.value; currentHP += data.value; }
        else if (typeName.StartsWith("Heal")) currentHP = Mathf.Min(currentHP + data.value, maxHP);

        switch (data.type)
        {
            case UpgradeType.FireRate:
                fireRate = Mathf.Max(0.1f, fireRate - data.value);
                break;
            case UpgradeType.MoveSpeed:
                moveSpeed += data.value;
                break;
            case UpgradeType.ShotCount:
                shotCount = Mathf.Min(upgradeLevels[data.type], 5);
                break;
                // PickUpRange のケースを削除
        }
    }

    void ExecuteBomb()
    {
        int lv = upgradeLevels.ContainsKey(UpgradeType.Bomb) ? upgradeLevels[UpgradeType.Bomb] : 0;
        if (lv <= 0) return;

        bombTimer = 360f - (lv * 60f); // Lv1=300, Lv2=240, Lv3=180

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var e in enemies) Destroy(e);

        GameObject[] bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        foreach (var b in bullets) Destroy(b);
    }

    // --- 視覚化（固定範囲） ---
    public Transform pickupRangeVisual;
    void UpdatePickupRangeVisual()
    {
        if (pickupRangeVisual != null)
        {
            float visualScale = pickupRange * 0.5f;
            pickupRangeVisual.localScale = new Vector3(visualScale, visualScale, 1f);
        }
    }
}