using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private PlayerMovement player;

    private Transform lookTarget;

    [SerializeField]
    private Transform objectToRotate;

    private Vector3 smDampVelocity;

    [SerializeField]
    private float rotationSpeed = 1f;

    [SerializeField]
    private bool shaking;

    [SerializeField]
    private float shakeAmount;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.GetPlayerReference();
        lookTarget = player.transform;

        if (!shaking)
            shakeAmount = 0;
    }

    private void LateUpdate()
    {
        int mult = 1;
        mult = lookTarget.position.x > transform.position.x ? 1 : -1;

        objectToRotate.right = Vector3.SmoothDamp(objectToRotate.right, (lookTarget.position + new Vector3(0, UnityEngine.Random.Range(-shakeAmount, shakeAmount),0) - transform.position) * mult, ref smDampVelocity, rotationSpeed);
    }
}
