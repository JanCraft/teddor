using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProfile : Enemy {
    public string username;
    private NetworkingController nc;

    public override void Start() {
        nc = FindObjectOfType<NetworkingController>();
        level = 0;
    }
    public override void Update() {
        if (bleed > 0f) {
            bleed = Mathf.Lerp(bleed, 0f, .75f * Time.deltaTime);
            TakeDamage(Mathf.Min((1.5f + bleed) * maxhp * .05f, maxhp * .25f) * Time.deltaTime, null, false);
        }

        if (flaming > 0f) {
            flaming = Mathf.Lerp(flaming, 1f, 1.5f * Time.deltaTime);
            TakeDamage(Mathf.Min((2.5f + flaming) * maxhp * .1f, maxhp * .5f) * Time.deltaTime, null, false);
        }
    }

    public override void TakeDamage(float amount, PlayerController _, bool tdmg) {
        Dictionary<string, object> pkt = new Dictionary<string, object>();
        pkt.Add("name", username);
        pkt.Add("dmg", amount);
        nc.SendPacket("teddor:mp/damage", pkt);
    }
}
