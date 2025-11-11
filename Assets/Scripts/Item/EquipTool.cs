using UnityEngine;

public class EquipTool : Equip
{
    public float attackRate;
    private bool attacking;
    public float attackDistance;
    public float attackDistanceOne;
    public float attackDistanceThree;
    public float useStamina;

    [Header("Resource Gathering")]
    public bool doesGatherResources;

    [Header("Combat")]
    public bool doesDealDamage;
    public int damage;

    private Animator animator;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();
        attackDistance = attackDistanceOne;
    }

    private void OnEnable()
    {
        CharacterManager.Instance.Player.controller.OnChangeView += ChangeView;
    }

    private void OnDisable()
    {
        if (CharacterManager.Instance != null && CharacterManager.Instance.Player != null && CharacterManager.Instance.Player.controller != null)
            CharacterManager.Instance.Player.controller.OnChangeView -= ChangeView;
    }

    public override void OnAttackInput()
    {
        if (!attacking)
        {
            if (CharacterManager.Instance.Player.condition.UseStamina(useStamina))
            {
                attacking = true;
                animator.SetTrigger("Attack");
                Invoke(nameof(OnCanAttack), attackRate);
            }
        }
    }

    void OnCanAttack()
    {
        attacking = false;
    }

    public void OnHit()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackDistance))
        {
            if (doesGatherResources && hit.collider.TryGetComponent(out Resource resource))
            {
                resource.Gather(hit.point, hit.normal);
            }

            if (hit.collider.TryGetComponent(out IDamagable damagable))
            {
                damagable.TakePhysicalDamage(damage);
            }
        }
    }

    private void ChangeView()
    {
        attackDistance = CharacterManager.Instance.Player.controller.IsthirdPersonView ? attackDistanceThree : attackDistanceOne;
    }
}