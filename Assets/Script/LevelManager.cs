using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    [Header("Leveling Settings")]
    public float currentExp = 0;
    public float expToNextLevel = 100;
    public int currentLevel = 1;

    [Header("Upgrade Data")]
    public List<UpgradeData> allUpgrades; // インスペクターで全UpgradeDataアセットを登録

    [Header("UI References")]
    private VisualElement levelUpScreen;
    private VisualElement cardContainer;
    private ProgressBar expBar;

    // レア度の出現重み（合計100）
    private Dictionary<Rarity, int> rarityWeights = new Dictionary<Rarity, int>
    {
        { Rarity.Common, 45 },      // 45%
        { Rarity.Uncommon, 30 },    // 30%
        { Rarity.Rare, 15 },        // 15%
        { Rarity.SuperRare, 7 },    // 7%
        { Rarity.Legendary, 3 }     // 3%
    };

    void OnEnable()
    {
        // UI Documentから要素を取得
        var uiDoc = GetComponent<UIDocument>();
        var root = uiDoc.rootVisualElement;

        levelUpScreen = root.Q<VisualElement>("LevelUpScreen");
        cardContainer = root.Q<VisualElement>("CardContainer");
        expBar = root.Q<ProgressBar>("ExpBar");

        // 初期状態は非表示
        if (levelUpScreen != null) levelUpScreen.style.display = DisplayStyle.None;

        UpdateUI();
    }

    /// <summary>
    /// 経験値オーブを取得したときに呼ばれる
    /// </summary>
    public void GainExp(float amount)
    {
        var stats = Object.FindFirstObjectByType<PlayerStats>();
        float multiplier = (stats != null) ? stats.expMultiplier : 1.0f;

        currentExp += amount * multiplier;

        if (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
        UpdateUI();
    }

    void LevelUp()
    {
        currentLevel++;
        currentExp -= expToNextLevel;
        expToNextLevel *= 1.2f; // 次のレベルの必要量を増加

        // ゲーム停止とUI表示
        Time.timeScale = 0;
        if (levelUpScreen != null) levelUpScreen.style.display = DisplayStyle.Flex;

        GenerateUpgradeCards();
    }

    void GenerateUpgradeCards()
    {
        if (cardContainer == null)
        {
            Debug.LogError("【エラー】UI Builder内の 'CardContainer' が見つかりません。名前を確認してください。");
            return;
        }

        cardContainer.Clear();

        // 今回の選択肢として選ばれたリスト
        List<UpgradeData> selectedUpgrades = new List<UpgradeData>();

        // 3枚のカードを生成
        for (int i = 0; i < 3; i++)
        {
            UpgradeData picked = GetRandomUpgrade(selectedUpgrades);
            if (picked != null)
            {
                selectedUpgrades.Add(picked);
                CreateCardUI(picked);
            }
            else
            {
                Debug.LogWarning($"【抽選失敗リトライ】{i + 1}枚目の正規抽選に漏れました。登録データを確認してください。");
            }
        }
    }

    /// <summary>
    /// レア度に基づいた重み付け抽選（PlayerStatsの空辞書対策済み）
    /// </summary>
    UpgradeData GetRandomUpgrade(List<UpgradeData> excludeList)
    {
        var stats = Object.FindFirstObjectByType<PlayerStats>();

        // 1. レア度をロール
        Rarity rolledRarity = RollRarity();

        // 2. 該当レア度かつ、まだ今回の選択肢リストに入っていないものを抽出
        var possible = allUpgrades.Where(u =>
            u.rarity == rolledRarity &&
            !excludeList.Contains(u)
        ).ToList();

        // 3. 最大レベルチェック（stats側の辞書が空でも安全に判定）
        possible = possible.Where(u => {
            if (u.maxLevel <= 0) return true; // maxLevelが0以下の場合は「無制限」とみなす

            int currentLv = 0;
            if (stats != null && stats.upgradeLevels != null && stats.upgradeLevels.ContainsKey(u.type))
            {
                currentLv = stats.upgradeLevels[u.type];
            }

            return currentLv < u.maxLevel;
        }).ToList();

        // 【救済1】該当がない場合はCommonで再試行（フォールバック）
        if (possible.Count == 0)
        {
            possible = allUpgrades.Where(u => u.rarity == Rarity.Common && !excludeList.Contains(u)).ToList();

            possible = possible.Where(u => {
                if (u.maxLevel <= 0) return true;
                int currentLv = 0;
                if (stats != null && stats.upgradeLevels != null && stats.upgradeLevels.ContainsKey(u.type))
                {
                    currentLv = stats.upgradeLevels[u.type];
                }
                return currentLv < u.maxLevel;
            }).ToList();
        }

        // 【救済2】それでも候補がゼロなら、除外リストだけを考慮して全アセットから強制抽出
        if (possible.Count == 0)
        {
            possible = allUpgrades.Where(u => !excludeList.Contains(u)).ToList();
        }

        if (possible.Count == 0) return null;

        return possible[Random.Range(0, possible.Count)];
    }

    Rarity RollRarity()
    {
        int roll = Random.Range(0, 100);
        int cumulative = 0;
        foreach (var weight in rarityWeights)
        {
            cumulative += weight.Value;
            if (roll < cumulative) return weight.Key;
        }
        return Rarity.Common;
    }

    void CreateCardUI(UpgradeData data)
    {
        // ボタンとしてカードを作成
        Button card = new Button();
        card.AddToClassList("upgrade-card");
        card.AddToClassList(data.GetRarityClassName());

        // 【安全対策】USS側が読み込めていなくても、最低限見えるサイズと色をC#側から保証
        card.style.width = 150;
        card.style.height = 230;
        card.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 0.95f);
        card.style.flexDirection = FlexDirection.Column;
        card.style.alignItems = Align.Center;
        card.style.justifyContent = Justify.Center;
        card.style.marginLeft = 15;
        card.style.marginRight = 15;
        card.style.paddingLeft = 15;
        card.style.paddingRight = 15;
        card.style.borderTopWidth = 3;
        card.style.borderBottomWidth = 3;
        card.style.borderLeftWidth = 3;
        card.style.borderRightWidth = 3;
        card.style.borderTopColor = Color.gray;
        card.style.borderBottomColor = Color.gray;
        card.style.borderLeftColor = Color.gray;
        card.style.borderRightColor = Color.gray;

        // --- 名前の改行処理 ---
        string processedName = data.upgradeName;
        if (processedName.Contains("(")) processedName = processedName.Replace("(", "\n(");
        if (processedName.Contains("（")) processedName = processedName.Replace("（", "\n（");

        Label nameLabel = new Label(processedName);
        // USSの専用クラスを適用
        nameLabel.AddToClassList("upgrade-name-label");

        Label rarityLabel = new Label($"[{data.rarity.ToString()}]");
        rarityLabel.style.color = data.GetRarityColor();
        rarityLabel.style.fontSize = 16;
        rarityLabel.style.whiteSpace = WhiteSpace.Normal;

        Label descLabel = new Label(data.description);
        descLabel.style.whiteSpace = WhiteSpace.Normal;
        descLabel.style.color = Color.white;
        descLabel.style.fontSize = 14;
        descLabel.style.marginTop = 10;

        card.Add(nameLabel);
        card.Add(rarityLabel);
        card.Add(descLabel);

        // クリックイベント
        card.clicked += () => ApplyUpgradeAndResume(data);

        cardContainer.Add(card);
    }

    void ApplyUpgradeAndResume(UpgradeData data)
    {
        var stats = Object.FindFirstObjectByType<PlayerStats>();
        if (stats != null)
        {
            stats.ApplyUpgrade(data);
        }

        // UIを閉じてゲーム再開
        levelUpScreen.style.display = DisplayStyle.None;
        Time.timeScale = 1;
    }

    void UpdateUI()
    {
        if (expBar != null)
        {
            expBar.value = currentExp;
            expBar.highValue = expToNextLevel;
            expBar.title = $"LV {currentLevel} - {(int)currentExp} / {(int)expToNextLevel}";
        }
    }
}