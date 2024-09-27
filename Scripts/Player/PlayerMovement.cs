using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using RewiredConsts;

public class PlayerMovement : MonoBehaviour
{
	public Rigidbody rb = null;
    [SerializeField] float speed = 25f;

	Health health = null;
    float leftStickHorizontal = 0.0f;
    float leftStickVertical = 0.0f;
    float rightStickHorizontal = 0.0f;
    float rightStickVertical = 0.0f;
    float aimAngle = 0.0f;
    Quaternion aimRotation = Quaternion.identity;
    Vector3 velocity = Vector3.zero;
    Vector3 previousPos = Vector3.zero;

    PlayerController playerController = null;
	Rewired.Player player = null;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

	private void Start()
	{
        playerController = GetComponent<PlayerController>();
        
	}

	private void Update()
    {
        if(player == null)
        {
            player = playerController.Player;
            return;
        }

        if (health.CurrentHealth <= 0)
            return;

        if (!playerController.IsStunned)
        {
		    leftStickHorizontal = player.GetAxis(Action.MOVE_HORIZONTAL);
            leftStickVertical = player.GetAxis(Action.MOVE_VERTICAL);
		    rightStickHorizontal = player.GetAxis(Action.AIMHORIZONTAL);
            rightStickVertical = player.GetAxis(Action.AIMVERTICAL);
        }

		aimAngle = Mathf.Atan2(rightStickHorizontal, rightStickVertical) * Mathf.Rad2Deg;
        aimRotation = Quaternion.AngleAxis(-aimAngle + 90, Vector3.forward);
    }

    /// <summary>
    /// Handles the player movement and rotation. Left stick for movement and right stick for rotation.
    /// Call this Method in FixedUpdate because physics stuff.
    /// <para>-Talis</para>
    /// </summary>
    public void HandleMovementRotation()
    {
        velocity = rb.velocity;
        rb.velocity = new Vector3(Mathf.Lerp(velocity.x, speed * leftStickHorizontal, Time.fixedDeltaTime * 8f),
            Mathf.Lerp(velocity.y, speed * leftStickVertical, Time.fixedDeltaTime * 8f), 0f);

        if (rightStickHorizontal != 0 || rightStickVertical != 0)
        {
            rb.rotation = Quaternion.Slerp(rb.rotation, aimRotation, Time.fixedDeltaTime * 20f);
        }
    }
}