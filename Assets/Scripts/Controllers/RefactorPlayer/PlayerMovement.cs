using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public PlayerCombat combat;

    public float speed = 6f;
    public float bSpeed = 6f;
    private float speedmult = 1f;
    private float speedmulttime;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    private float velocity = 0f;
    private float dashTime = 0f;
    public LayerMask groundLayer;
    public Transform cam;
    public CharacterController controller;
    public Animation anim;

    public Transform waterVortexTPPoint;

    void Start() {

    }

    void Update() {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(horizontal, 0f, vertical).normalized;
        bSpeed = Mathf.Lerp(bSpeed, speed, 7.5f * Time.deltaTime);
        if (dir.magnitude >= 0.1f) {
            if (!anim.isPlaying) anim.Play();
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * bSpeed * speedmult * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashTime <= 0f) {
            combat.iframes = .25f;
            bSpeed *= 5f;
            combat.attackCD = .1f;
            dashTime = .5f;
        }

        velocity -= Time.deltaTime * 9.81f;
        controller.Move(Vector3.up * velocity * Time.deltaTime);
        if (controller.isGrounded && velocity < 0f) velocity = -.2f;
        if (Input.GetKeyDown(KeyCode.Space) && Physics.CheckSphere(transform.position + Vector3.down * .25f, .1f, groundLayer)) {
            velocity = 9.81f * .66f;
        }

        if (dashTime > 0f) dashTime -= Time.deltaTime;

        if (speedmult > 1f) {
            if (speedmulttime > 0f) {
                speedmulttime -= Time.deltaTime;
            } else {
                speedmult = 1f;
                speedmulttime = 0f;
            }
        }
    }

    public void AddSpeedMult(float power, float time) {
        speedmult += power;
        speedmulttime = Mathf.Max(speedmulttime, time);
    }

    public void Teleport(Vector3 worldPos) {
        controller.enabled = false;
        transform.position = worldPos;
        controller.enabled = true;
    }

    public void Teleport(Transform worldPos) {
        controller.enabled = false;
        transform.position = worldPos.position;
        controller.enabled = true;
    }

    public void TeleportFade(Transform worldPos) {
        LoadingScreen.FadeInOutTeleport(1f, this, worldPos.position);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("WaterVortex")) {
            Teleport(waterVortexTPPoint);
            FindObjectOfType<VortexBefall>().Init();
        }
    }
}