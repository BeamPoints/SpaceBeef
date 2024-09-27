using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] int weaponNumber = -1;
    [SerializeField] FireRateType fireRateType;
    [SerializeField] AudioSource weaponSound;
    [SerializeField] AudioSource noAmmoSound;
    [SerializeField] bool isShotgun = false;
    [SerializeField] List<Transform> firePoints;
    [SerializeField] GameObject bulletTrailPrefab;
    [SerializeField] int maxDistance = 0;
    [SerializeField] float fireRate = 0;
    [SerializeField] float recoil = 0;
    [SerializeField] int ammo = 0;
    [SerializeField] LayerMask layerToCollide;
    [SerializeField] Collider trigger;
    [SerializeField] int laserDamage = 1;
    [SerializeField] float laserTickTime;

    float timeToFire = 0;
    Vector2 destination;
    Vector2 origin;
    LineRenderer laser;
    PlayerController owner = null;
    public PlayerController Owner { get { return owner; } set { owner = value; } }
    bool needsDestruction = false;
    [SerializeField] float timeToDestruction = 3.0f;
    float destructionTimer = 0.0f;
    bool canStun = false;
    public bool CanStun { get { return canStun; } set { canStun = value; } }

    float laserMatPower = 0.0f;
    private float laserTick;

    private void Awake()
    {
        timeToFire = fireRate;
        Color transparent = new Color(1, 1, 1, 0);
        Color nonTransparent = new Color(1, 1, 1, 1);
        laser = GetComponent<LineRenderer>();
    }

    public int Ammo { get { return ammo; } }
    public float RecoilForce { get { return recoil; } }

    public void CallStopLaser()
    {
        if (laser && laser.enabled)
        {
            laser.enabled = false;
            maxDistance = 0;
        }
    }

    private void Update()
    {
        if (needsDestruction)
        {
            destructionTimer -= Time.deltaTime;
            if (destructionTimer <= 0.0f)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            destructionTimer = timeToDestruction;
        }
    }

    public void ActivateCollisionWithOwner()
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), owner.GetComponent<Collider>(), false);
    }

    /// <summary>
    /// Calls the Shoot function. (TODO: vielleicht umbenennen, oder in shoot() umwandeln)
    /// <paramref name="_isSemiAuto"/> Semiauto shooting
    /// <para>-Talis / (Patrick)</para>
    /// </summary>
    /// <param name="_isSemiAuto"></param>
    public void CallShoot(bool _isSemiAuto)
    {
        timeToFire += Time.deltaTime;

        if (fireRateType == FireRateType.SEMIAUTO)
        {
            if (ammo >= 1)
            {
                if (isShotgun && _isSemiAuto)
                {
                    if (timeToFire >= fireRate)
                    {
                        Shoot();
                        ammo--;
                        timeToFire = 0.0f;
                    }
                }
                if (!isShotgun && _isSemiAuto)
                {
                    Shoot();
                    ammo--;
                }
            }
            else
            {
                if (noAmmoSound != null && !noAmmoSound.isPlaying)
                    noAmmoSound.Play();
            }
        }
        else if (fireRateType == FireRateType.AUTOMATIC)
        {
            if (ammo >= 1)
            {
                if (timeToFire >= fireRate)
                {
                    Shoot();
                    ammo--;

                    timeToFire = 0.0f;
                }
            }
            else
            {
                if (noAmmoSound != null && !noAmmoSound.isPlaying)
                    noAmmoSound.Play();
            }
        }
        else if (fireRateType == FireRateType.LASER)
        {
            if (ammo >= 1)
            {
                if (timeToFire >= fireRate)
                {
                    if (!laser.enabled)
                    {
                        laser.enabled = true;
                    }
                    ammo--;
                    Shoot();

                    timeToFire = 0.0f;

                    if (laser != null)
                    {
                        Ray ray = new Ray(firePoints[0].position, firePoints[0].forward);
                        laser.SetPosition(0, ray.origin);
                        laser.SetPosition(1, ray.origin + (firePoints[0].forward * 0.5f));
                        laserMatPower = Mathf.Lerp(-0.5f, 0.5f, Mathf.PingPong(Time.time * 0.1f, 0.5f));
                        laser.material.SetFloat("_Power", laserMatPower);

                        RaycastHit hit;
                        Physics.Raycast(firePoints[0].position, firePoints[0].forward, out hit, maxDistance, layerToCollide, QueryTriggerInteraction.Ignore);
                        
                        if (hit.collider != null)
                        {
                            laser.SetPosition(2, hit.point);
                            
                            if (hit.collider.gameObject.GetComponent<Health>() != null)
                            {
                                var Health = hit.collider.gameObject.GetComponent<Health>();

                                laserTick += Time.deltaTime;

                                if(laserTick > laserTickTime)
                                {
                                    Health.GetDamage(laserDamage);
                                    laserTick = 0.0f;
                                }
                            }
                        }
                        else
                        {
                            laser.SetPosition(2, ray.GetPoint(maxDistance));
                            maxDistance += 2;
                        }
                    }

                }
            }
            else
            {
                CallStopLaser();
                StopLaserSound();
                if (noAmmoSound != null && !noAmmoSound.isPlaying)
                    noAmmoSound.Play();
            }
        }

        if (firePoints.Count > 0)
        {
            origin = firePoints[0].position;
            destination = firePoints[0].forward;
            Debug.DrawRay(origin, firePoints[0].forward, Color.red);
        }
    }

    void Shoot()
    {
        if (weaponSound != null)
        {
            if (fireRateType != FireRateType.LASER)
            {
                weaponSound.Play();
            }
            else
            {
                if (!weaponSound.isPlaying)
                {
                    weaponSound.Play();
                }
            }
        }

        //GetComponentInParent<Rigidbody>().AddRelativeForce(new Vector3(0f, 0f, -recoil), ForceMode.Impulse);
        if (GetComponentInParent<Rigidbody>())
        {
            transform.parent.GetComponent<Rigidbody>().AddRelativeForce(
            new Vector3(-recoil, 0f, 0f), ForceMode.Impulse);
            GetComponentInParent<PlayerController>().Player.SetVibration(0, 0.5f, 0.1f);
            GetComponentInParent<PlayerController>().Player.SetVibration(1, 0.75f, 0.1f);
        }

        Effect();
    }

    /// <summary>
    /// Aktiviert den Rigidbody und wirft die Waffe des Spielers mit Physik (AddForce).
    /// <para>-Talis</para>
    /// </summary>
    /// <param name="_throwForce"> Wie stark die Waffe geworfen werden soll</param>
    public void ThrowWeapon(float _throwForce)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(transform.forward * _throwForce, ForceMode.Impulse);
        GetComponent<Collider>().enabled = true;
        Physics.IgnoreCollision(GetComponent<Collider>(), owner.GetComponent<Collider>(), true);
        transform.localScale = new Vector3(50f, 50f, 50f);
        GetComponentInParent<PlayerController>().WeaponEquipped = false;
        transform.parent = null;
        needsDestruction = true;
        canStun = true;
        if (laser != null)
        {
            CallStopLaser();
        }
    }

    void Effect()
    {
        if (bulletTrailPrefab != null)
        {
            if (isShotgun)
            {
                List<GameObject> shotgunBullets = new List<GameObject>();

                foreach (var firePoint in firePoints)
                {
                    GameObject shotgunBullet = Instantiate(bulletTrailPrefab, firePoint.position, firePoint.rotation);
                    shotgunBullets.Add(shotgunBullet);
                }

                foreach (GameObject bullet in GameObject.FindGameObjectsWithTag("Bullet"))
                {
                    foreach (GameObject shotgunBullet in shotgunBullets)
                    {
                        Physics.IgnoreCollision(bullet.GetComponent<Collider>(), shotgunBullet.GetComponent<Collider>());
                    }
                }
            }
            else
            {
                Instantiate(bulletTrailPrefab, firePoints[0].position, firePoints[0].rotation);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && !other.GetComponent<PlayerController>().WeaponEquipped && other.GetComponent<PlayerController>().GiveWeapon)
        {
            // Ziemlich hässlich gelöst, weil die models kaputt sind aber keine Zeit zum verbessern :S
            switch (weaponNumber)
            {
                case 0: //Pistol
                    other.GetComponent<PlayerController>().GunPoint.localPosition = new Vector3(0.68f, 0.44f, -0.22f);
                    break;
                case 1: //Rifle
                    other.GetComponent<PlayerController>().GunPoint.localPosition = new Vector3(0.47f, 0.31f, -0.08f);
                    break;
                case 2: //Shotgun
                    other.GetComponent<PlayerController>().GunPoint.localPosition = new Vector3(0.38f, 0.315f, -0.11f);
                    break;
                case 3: //Laser
                    other.GetComponent<PlayerController>().GunPoint.localPosition = new Vector3(0.48f, 0.315f, -0.12f);
                    break;
                    //case 4: //Sniper
                    //	other.GetComponent<PlayerController>().GunPoint.localPosition = new Vector3(0.30f, 0.31f, -0.08f);
                    //	break;
            }
            GetComponent<Collider>().enabled = false;


            other.GetComponentInChildren<Animator>().SetInteger("State", weaponNumber);

            GetComponent<Rigidbody>().isKinematic = true;

            transform.position = other.GetComponent<PlayerController>().GunPoint.position;
            transform.rotation = other.GetComponent<PlayerController>().GunPoint.rotation;
            transform.parent = other.transform;
            transform.localScale = other.GetComponent<Transform>().localScale * 10;

            other.GetComponent<PlayerController>().WeaponEquipped = true;

            needsDestruction = false;
            canStun = false;
        }
    }

    public void StopLaserSound()
    {
        if(fireRateType == FireRateType.LASER)
        {
            if (weaponSound.isPlaying)
            {
                weaponSound.Stop();
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag != "Player")
        {
            canStun = false;
        }
    }
}

public enum FireRateType
{
    AUTOMATIC,
    SEMIAUTO,
    LASER
}
