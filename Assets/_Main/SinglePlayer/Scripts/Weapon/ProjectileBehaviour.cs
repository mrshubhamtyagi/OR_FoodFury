using FoodFury;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    [SerializeField] private bool rotate = false;
    [SerializeField] private float rotateAmount = 45;
    [SerializeField] private GameObject muzzlePrefab;
    [SerializeField] private GameObject hitPrefab;
    [SerializeField] private List<ObjectSpawn> trails;
    [SerializeField] private List<ParticleSystem> particles;

    private Weapon weapon;
    private float speed;

    private Vector3 offset;
    private bool collided;
    private Rigidbody rb;

    void OnEnable()
    {
        collided = false;
        rb = GetComponent<Rigidbody>();
        speed = Mathf.Max(GameController.Instance.Rider.Vehicle.MaxSpeed + 5, 25);
        foreach (ObjectSpawn objectSpawn in trails)
        {
            Instantiate(objectSpawn, transform);
        }

    }
    public void SpawnMuzzleVFX()
    {
        if (muzzlePrefab != null)
        {
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward + offset;
            var ps = muzzleVFX.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(muzzleVFX, ps.main.duration);
            else
            {
                var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(muzzleVFX, psChild.main.duration);
            }
        }
    }


    public void SetWeapon(Weapon weapon) => this.weapon = weapon;


    void FixedUpdate()
    {

        if (rotate)
            transform.Rotate(0, 0, rotateAmount, Space.Self);
        if (speed != 0 && rb != null)
            rb.position += (transform.forward + offset) * (speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag != "Bullet" && !collided)
        {
            collided = true;
            Debug.Log(collider.gameObject.name);

            if (trails.Count > 0)
            {
                for (int i = 0; i < trails.Count; i++)
                {
                    if (trails[i] != null)
                    {
                        // trails[i].transform.parent = null;
                        var ps = trails[i].GetComponent<ParticleSystem>();
                        if (ps != null)
                        {
                            ps.Stop();
                            Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
                        }
                    }
                }
            }

            speed = 0;
            GetComponent<Rigidbody>().isKinematic = true;

            //ContactPoint contact = collider.ClosestPoint;
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, transform.position - collider.ClosestPoint(transform.position));
            Vector3 pos = collider.ClosestPoint(transform.position);

            if (hitPrefab != null)
            {
                var hitVFX = Instantiate(hitPrefab, pos, rot);

                var ps = hitVFX.GetComponent<ParticleSystem>();
                if (ps == null)
                {
                    var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                    Destroy(hitVFX, psChild.main.duration);
                }
                else
                    Destroy(hitVFX, ps.main.duration);
            }

            DestroyParticle();
        }

    }

    public void DestroyParticle()
    {
        //if (GetComponent<Weapon>().collidedWithRider == false)
        //{
        //    foreach (ParticleSystem particle in particles)
        //    {
        //        particle.Stop();
        //    }
        //    transform.GetChild(0).localScale = Vector3.one;

        //    weapon.DestroyInstance();

        //}
        //else
        //{
        //    foreach (ParticleSystem particle in particles)
        //    {
        //        particle.Stop();
        //    }
        //}
    }


}
