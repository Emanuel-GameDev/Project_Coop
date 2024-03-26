using System.Collections;
using UnityEngine;

public class ObstacleEnemy : MonoBehaviour
{
    [Header("Variabili Ostacolo base")]

    [SerializeField, Tooltip("Il danno inflitto dall'ostacolo")]
    [Min(0)]
    protected float damage = 5;
    protected float staminaDamage = 1;
    [SerializeField, Tooltip("La forza di spinta dell'ostacolo")]
    [Min(0)]
    protected float pushStrength = 10000;
    [SerializeField, Tooltip("Il raggio del trigger dell'ostacolo")]
    [Min(1)]
    protected float triggerArea = 1;

    Animator animator;
    public Transform dealerTransform => transform;

    [SerializeField] protected AnimationCurve AnimationCurve;

    [SerializeField] protected bool active;
    private void Awake()
    {
        active = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected DamageData GetDamageData()
    {

        return new DamageData(damage, staminaDamage, null, false);
    }

    protected IEnumerator PushPlayer(PlayerCharacter player) //transform dell'oggetto,la forza
    {
        float timer = 0;
        float interpolationRatio;
        Vector3 startPoint = player.transform.position;

        Vector3 forceVector = (player.transform.position - transform.position).normalized;

        player.GetComponent<IDamageable>().TakeDamage(GetDamageData());



        if (player.gameObject.TryGetComponent(out Rigidbody2D rb))
        {

            while (timer < 1)
            {
                interpolationRatio = timer / 1;
                timer += Time.deltaTime;
                rb.MovePosition(Vector3.Lerp(startPoint, startPoint + (forceVector * pushStrength), AnimationCurve.Evaluate(interpolationRatio)));
                yield return new WaitForFixedUpdate();
            }


        }



        yield return null;
    }

    protected IEnumerator PushPlayerExplosion(PlayerCharacter player, float explosionRadius)
    {
        float timer = 0;
        float interpolationRatio;
        Vector2 startPoint = player.transform.position;

        Vector2 forceVector = (player.transform.position - transform.position);

        float forceFalloff = 1 - (forceVector.magnitude / explosionRadius);

        Debug.Log($"force falloff: {forceFalloff}");

        player.GetComponent<IDamageable>().TakeDamage(GetDamageData());



        if (player.gameObject.TryGetComponent(out Rigidbody2D rb))
        {
            while (timer < 1)
            {
                interpolationRatio = timer / 1;
                timer += Time.deltaTime;
                rb.MovePosition(Vector3.Lerp(startPoint, startPoint + (forceVector.normalized * pushStrength)+(forceVector.normalized * (forceFalloff <= 0 ? 0 : explosionRadius) * forceFalloff), AnimationCurve.Evaluate(interpolationRatio)));
                yield return new WaitForFixedUpdate();
            }



        }



    }
}
