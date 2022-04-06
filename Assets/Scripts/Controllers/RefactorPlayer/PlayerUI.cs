using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
    public PlayerCombat combat;

    [Header("Ingame / Cinematic")]
    public GameObject ingameUI;
    public GameObject cinematicUI;

    [Header("Enemy HUD")]
    public GameObject enemyhud;
    public Text enemylvl;
    public Transform enemyhp;
    public Transform enemydmg;
    public Transform enemyshield;
    [HideInInspector] public bool removeEnemyHUD;

    [Header("Player HUD")]
    public Transform hpbar;
    public Transform hpdmgbar;
    public Transform hpshieldbar;
    public Text lvltxt;
    public Transform xpbar;

    [Header("Ability UI")]
    public Image abilityProgress;
    public Image abilityIcon;
    public Color abilityActiveColor;

    [Header("SS. UI")]
    public Transform soulShardGraphics;
    public Image soulChargeProgress;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        cinematicUI.SetActive(PlayerCombatAbilities.soulShardAnimationPause);
        ingameUI.SetActive(!PlayerCombatAbilities.soulShardAnimationPause);

        Vector3 hpbarscale = new Vector3(combat.stats.hp / combat.stats.maxhp, 1f, 1f);
        hpbar.localScale = hpbarscale;
        hpshieldbar.localScale = Vector3.Lerp(hpshieldbar.localScale, new Vector3(Mathf.Min(combat.shieldValue / combat.stats.maxhp, 1f), 1f, 1f), 5f * Time.deltaTime);
        hpdmgbar.localScale = Vector3.Lerp(hpdmgbar.localScale, hpbarscale, Time.deltaTime * 2.5f);
        lvltxt.text = "Lv. " + combat.stats.level;
        xpbar.localScale = new Vector3(Mathf.Clamp01(combat.stats.xp / combat.stats.xptonext), 1f, 1f);

        if (removeEnemyHUD && combat.lasthitenemy != null) {
            combat.lasthitenemy = null;
            removeEnemyHUD = false;
        }
        if (combat.lasthitenemy != null) {
            enemylvl.text = "Lv. " + combat.lasthitenemy.level;
            enemyhp.localScale = new Vector3(combat.lasthitenemy.hp / combat.lasthitenemy.maxhp, 1f, 1f);
            enemydmg.localScale = new Vector3(Mathf.Lerp(enemydmg.localScale.x, combat.lasthitenemy.hp / combat.lasthitenemy.maxhp, 2.5f * Time.deltaTime), 1f, 1f);
            enemyshield.localScale = new Vector3(combat.lasthitenemy.shield / combat.lasthitenemy.maxhp, 1f, 1f);
        }
        enemyhud.SetActive(combat.lasthitenemy != null);

        int ssgc = soulShardGraphics.transform.childCount;
        for (int i = 0; i < ssgc; i++) {
            if (combat.stats.soulShard.type <= 0) {
                soulShardGraphics.transform.GetChild(i).gameObject.SetActive(false);
                continue;
            }
            soulShardGraphics.transform.GetChild(i).gameObject.SetActive((int) combat.stats.soulShard.type == i + 1);
        }

        float tfill = (float) combat.combatAbilities.soulCharge / (float) combat.stats.soulShard.ChargeMax();
        if (soulChargeProgress.fillAmount > tfill) {
            soulChargeProgress.fillAmount = Mathf.Lerp(soulChargeProgress.fillAmount, 0f, 5f * Time.deltaTime);
        } else {
            soulChargeProgress.fillAmount = tfill;
        }
    }
}