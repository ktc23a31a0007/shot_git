using UnityEngine;

/// <summary>
/// 強化の種類を定義
/// </summary>
public enum UpgradeType
{
    ExpGainC,        // 獲得経験値増加 (Common)
    ExpGainUC,       // 獲得経験値増加 (Uncommon)
    ExpGainR,        // 獲得経験値増加 (Rare)
    ExpGainSR,       // 獲得経験値増加 (SuperRare)
    ExpGainL,        // 獲得経験値増加 (Legendary)
    DamageC,         // 攻撃力UP (Common)
    DamageUC,        // 攻撃力UP (Uncommon)
    DamageR,         // 攻撃力UP (Rare)
    DamageSR,        // 攻撃力UP (SuperRare)
    DamageL,         // 攻撃力UP (Legendary)
    FireRate,        // 連射UP (最大10)
    MaxHPC,          // 体力UP (Common)
    MaxHPUC,         // 体力UP (Uncommon)
    MaxHPR,          // 体力UP (Rare)
    MaxHPSR,         // 体力UP (SuperRare)
    MaxHPL,          // 体力UP (Legendary)
    HealUC,          // 体力回復 (Uncommon)
    HealR,           // 体力回復 (Rare)
    HealSR,          // 体力回復 (SuperRare)
    DamageReduceC,   // 被ダメージ減少 (Common)
    DamageReduceUC,  // 被ダメージ減少 (Uncommon)
    DamageReduceR,   // 被ダメージ減少 (Rare)
    DamageReduceSR,  // 被ダメージ減少 (SuperRare)
    DamageReduceL,   // 被ダメージ減少 (Legendary)
    MoveSpeed,       // 移動速度UP (最大5)
    ShotCount,       // 弾の発射数増加 (最大5)
    Barrier,         // バリア (最大3)
    Bomb             // ボム (最大3, CT: 300/240/180)
}

/// <summary>
/// レア度を定義
/// </summary>
public enum Rarity
{
    Common,      // 白
    Uncommon,    // 緑
    Rare,        // 青
    SuperRare,   // 紫
    Legendary    // 金
}

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Game/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    [Header("基本情報")]
    public string upgradeName;       // 強化の名前
    [TextArea] public string description; // UIに表示する説明文
    public UpgradeType type;         // 強化のカテゴリー
    public Rarity rarity;            // レア度

    [Header("パラメータ")]
    public float value;              // 上昇値や係数（例: 攻撃力+5なら 5）
    public int maxLevel;             // 最大レベル（0の場合は無制限）

    /// <summary>
    /// レア度に基づいた色を返す（UIの装飾用）
    /// </summary>
    public Color GetRarityColor()
    {
        return rarity switch
        {
            Rarity.Common => Color.white,
            Rarity.Uncommon => new Color(0.2f, 1f, 0.2f), // 緑
            Rarity.Rare => new Color(0.2f, 0.6f, 1f),    // 青
            Rarity.SuperRare => new Color(0.7f, 0.2f, 1f), // 紫
            Rarity.Legendary => new Color(1f, 0.8f, 0f),   // 金
            _ => Color.white
        };
    }

    /// <summary>
    /// レア度に基づいたCSSクラス名を返す（USSとの連携用）
    /// </summary>
    public string GetRarityClassName()
    {
        return "rarity-" + rarity.ToString().ToLower();
    }
}