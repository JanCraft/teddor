using System.Collections;
using UnityEngine;

public class PlayerCombatAbilities : MonoBehaviour {
    public PlayerCombat combat;
    public PlayerUI ui;

    public int soulCharge;
    public Animation soulShardAnimation;
    public AudioSource soulShardAudio;
    public static bool soulShardAnimationPause;

    private float abilityCD = 0f;
    private float cdreduce = 0f;
    private float cdreducetime = 0f;

    private void Update() {
        if (abilityCD > combat.stats.ability.GetCooldown()) abilityCD = combat.stats.ability.GetCooldown();

        float tfill = 1f - (abilityCD / combat.stats.ability.GetCooldown());
        if (ui.abilityProgress.fillAmount > tfill) {
            ui.abilityProgress.fillAmount = Mathf.Lerp(ui.abilityProgress.fillAmount, 0f, 5f * Time.deltaTime);
        } else {
            ui.abilityProgress.fillAmount = tfill;
        }
        Color targetCol = Color.white;
        if (abilityCD <= 0f)
            targetCol = ui.abilityActiveColor;

        ui.abilityProgress.color = Color.Lerp(ui.abilityProgress.color, targetCol, 15f * Time.deltaTime);
        if (abilityCD > 0f) abilityCD -= Time.deltaTime;
        if (Input.GetMouseButtonDown(1) && abilityCD <= 0f) {
            abilityCD = combat.stats.ability.GetCooldown();
            abilityCD -= cdreduce;
            if (abilityCD < 1f) abilityCD = 1f;
            combat.stats.ability.Perform(combat);
        }

        if (cdreduce > 0f && cdreducetime > 0f) {
            cdreducetime -= Time.deltaTime;
        } else if (cdreduce > 0f) {
            cdreduce = 0f;
        }
    }

    public IEnumerator DisableSoulShardAnimPause() {
        yield return new WaitForSeconds(1f);
        soulShardAnimationPause = false;
        combat.stats.soulShard.Perform(combat);
    }

    public void ReduceCD(float reduce, float time) {
        if (cdreduce == 888.888f) return; // wtf ?!?!?
        cdreduce += reduce;
        cdreducetime = Mathf.Max(cdreducetime, time);
        abilityCD -= reduce;
        if (abilityCD < 1f) abilityCD = 1f;
    }

    public void ResetCD() {
        abilityCD = combat.stats.ability.GetCooldown();
    }

    public void ZeroCD() {
        abilityCD = 0f;
    }
}