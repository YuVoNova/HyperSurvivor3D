using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private GameObject ImpactParticle;
    [SerializeField]
    private GameObject MuzzleParticle;
    [SerializeField]
    private GameObject ProjectileParticle;
    [SerializeField]
    private GameObject ProjectileSound;

    [SerializeField]
    private GameObject AOEDamage;

    [SerializeField]
    private float CollideOffset = 0.15f;

    private float radius;
    private float detectionDistance;
    private Vector3 direction;
    private Vector3 originPosition;
    private Vector3 impactPosition;
    private RaycastHit hit;

    private GameObject impactObject;
    private GameObject aoeDamageObject;
    private ParticleSystem[] trails;
    private ParticleSystem trail;

    private Vector3 Velocity;
    private float Range;
    private int Damage;


    private void Start()
    {
        originPosition = transform.position;
        ProjectileParticle = Instantiate(ProjectileParticle, transform.position, transform.rotation) as GameObject;
        ProjectileParticle.transform.parent = transform;
        ProjectileSound = Instantiate(ProjectileSound, transform.position, transform.rotation) as GameObject;
        Destroy(ProjectileSound, 1.5f);
        if (MuzzleParticle)
        {
            MuzzleParticle = Instantiate(MuzzleParticle, transform.position, transform.rotation) as GameObject;
            Destroy(MuzzleParticle, 1.5f);
        }
    }

    private void FixedUpdate()
    {
        GetComponent<Rigidbody>().velocity = Velocity;

        if (GetComponent<Rigidbody>().velocity.magnitude != 0)
        {
            transform.rotation = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity);
        }

        if (Vector3.Distance(originPosition, transform.position) > Range)
        {
            Destroy(ProjectileParticle);
            Destroy(gameObject);
        }

        radius = transform.GetComponent<SphereCollider>().radius;

        direction = Vector3.Normalize(transform.GetComponent<Rigidbody>().velocity);

        detectionDistance = transform.GetComponent<Rigidbody>().velocity.magnitude * Time.deltaTime;

        if (Physics.SphereCast(transform.position, radius, direction, out hit, detectionDistance))
        {
            if (hit.transform.gameObject.layer == 7)
            {
                transform.position = hit.point + (hit.normal * CollideOffset);
                impactPosition = transform.position;
                impactPosition.y = 1.2f;

                impactObject = Instantiate(ImpactParticle, impactPosition, Quaternion.identity) as GameObject;

                trails = GetComponentsInChildren<ParticleSystem>();

                for (int i = 1; i < trails.Length; i++)
                {
                    trail = trails[i];

                    if (trail.gameObject.name.Contains("Trail"))
                    {
                        trail.transform.SetParent(null);
                        Destroy(trail.gameObject, 2f);
                    }
                }

                if (transform.tag == "Rocket")
                {
                    impactPosition.y = 0f;
                    aoeDamageObject = Instantiate(AOEDamage, impactPosition, Quaternion.identity) as GameObject;
                    aoeDamageObject.GetComponent<AOEDamage>().SetValues(Damage, 7.5f);
                    Destroy(aoeDamageObject, 0.5f);
                }
                else
                {
                    hit.transform.GetComponent<Enemy>().TakeDamage(Damage);
                }

                Destroy(ProjectileParticle, 2f);
                Destroy(impactObject, 2f);
                Destroy(gameObject);
            }
        }
    }

    public void SetValues(Vector3 velocity, float range, int damage)
    {
        Velocity = velocity;
        Range = range;
        Damage = damage;
    }
}
