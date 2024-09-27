using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressionSystem;
using RewiredConsts;

public class WeaponSpawner : MonoBehaviour
{
    enum AnimationState { IDLE, LASER, PISTOL, RIFLE, SHOTGUN };

    [SerializeField] private float spawnTime = 5;
	[SerializeField] private Transform spawnPos = null;
	[SerializeField] private bool isWeapon = false;
    [SerializeField] private bool firstInstantSpawn;
	[SerializeField] bool spawnOnStart = true;
    [SerializeField] private GameObject weaponPickupParticle;
    
    GameObject weaponPrefab;

    private int weapon = -1;
    private GameObject spawnedWeapon = null;
    private bool isWeaponEquipped = false;

//#####################################################################################################################
	void Start ()
	{
		if(spawnOnStart)
		{
			if(firstInstantSpawn)
				Invoke("SpawnWeapon", 0.1f);
			else
				Invoke("SpawnWeapon", spawnTime);
		}
    }
//#####################################################################################################################
	public void SpawnWeapon ()
	{
		if(!isWeapon)
		{
			weapon = Random.Range(0, ProgressionManager.Instance.WeaponPool.Count);
			weaponPrefab = ProgressionManager.Instance.WeaponPool[weapon];
            GameObject weaponSpawnerPrefab = ProgressionManager.Instance.WeaponSpawnerPool[weapon];
            spawnedWeapon = Instantiate(weaponSpawnerPrefab, spawnPos);
			isWeapon = true;
        }	
	}
	//#####################################################################################################################
	private void OnTriggerStay(Collider other)
	{
		if(other.tag == "Player" && isWeapon && other.GetComponent<PlayerController>().GiveWeapon)
		{
			// Ziemlich hässlich gelöst, weil die models kaputt sind aber keine Zeit zum verbessern :S
			switch(weapon)
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

			// check if player has no weapon equipped
			if(!other.GetComponent<PlayerController>().WeaponEquipped)
			{
				other.GetComponentInChildren<Animator>().SetInteger("State", weapon);
				GameObject weaponToEquip = Instantiate(weaponPrefab,
					other.GetComponent<PlayerController>().GunPoint.position,
					other.GetComponent<PlayerController>().GunPoint.rotation,
					other.transform);
				weaponToEquip.GetComponent<Transform>().localScale = other.GetComponent<Transform>().localScale * 10;
				weaponToEquip.GetComponent<WeaponManager>().Owner = other.GetComponent<PlayerController>();
				other.GetComponent<PlayerController>().WeaponEquipped = true;

                if(weaponPickupParticle != null)
                {
                    Instantiate(weaponPickupParticle, transform.position, Quaternion.identity);
                }

				Destroy(spawnedWeapon);
				isWeapon = false;
                Invoke("SpawnWeapon", spawnTime);
            }
        }
	}
}
