using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
	[SerializeField] float lifeTime = 5f;
	[SerializeField] int damage = 20;
	[SerializeField] int speed = 45;

	public Vector3 Velocity { get; set; }
	Vector3 previousPos = Vector3.zero;

	private void Start()
	{
		//Physics.IgnoreLayerCollision(0,14);
	}

	void Update ()
	{
		Destroy(gameObject, lifeTime);

		Velocity = (gameObject.transform.position - previousPos);
		previousPos = gameObject.transform.position;
	}

	private void FixedUpdate()
	{
		GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }

	private void OnCollisionEnter(Collision collision)
	{
		if(collision.collider.tag == "Player")
		{
			collision.collider.GetComponent<Rigidbody>().AddForce(Velocity * 10f, ForceMode.Impulse);
		}

		if (collision.gameObject.GetComponent<Health>() != null)
		{
			collision.gameObject.GetComponent<Health>().GetDamage(damage);
		}

		Destroy(gameObject);
	}
}
