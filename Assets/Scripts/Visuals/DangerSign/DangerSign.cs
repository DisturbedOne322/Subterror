using UnityEngine;

public class DangerSign : MonoBehaviour
{
    //private Animator animator;

    //private readonly string IN_DANGER_ANIM = "IsInDanger";

    //[SerializeField]
    //private GameObject dangerSign;

    //private Ghost ghost;
    //private PlayerMovement player;

    //private bool isInDanger = false;

    //private void Awake()
    //{
    //    animator = GetComponent<Animator>();
    //}

    //// Start is called before the first frame update
    //void Start()
    //{
    //    ghost = GameManager.Instance.GetGhostReference();
    //    player = GameManager.Instance.GetPlayerReference();
    //    ghost.OnBurstAttack += Ghost_OnAttack;
    //    ghost.OnBurstAttackEnd += Ghost_OnAttackEnd;
    //}

    //private void OnDestroy()
    //{
    //    ghost.OnBurstAttack -= Ghost_OnAttack;
    //    ghost.OnBurstAttackEnd -= Ghost_OnAttackEnd;
    //}

    //private void Ghost_OnAttackEnd()
    //{
    //    isInDanger = false;
    //    dangerSign.SetActive(false);
    //    animator.SetBool(IN_DANGER_ANIM, false);
    //}

    //private void Ghost_OnAttack()
    //{
    //    isInDanger = true;
    //    dangerSign.SetActive(true);
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if(isInDanger)
    //    {
    //        transform.position = player.transform.position;
    //        Vector3 lookDirection = ghost.transform.position - transform.position;
    //        float angle = Vector3.SignedAngle(transform.up, lookDirection.normalized, Vector3.forward);
    //        transform.Rotate(0, 0, angle);
    //    }
    //}
}
