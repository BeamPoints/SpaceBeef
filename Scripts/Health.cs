using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private AudioSource damageClipPrefab;
    [SerializeField] private AudioSource deathClipPrefab;
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private GameObject toSpawnOnDamage;
    [SerializeField] private GameObject toSpawnOnDeathPrefab;
    [SerializeField] private List<GameObject> objectsToDestroy;

    private bool isWall;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {

    }

    /// <summary>
    ///  Führt z.B. eine Methode aus, die den Tod des gameObjects darstellt -BZ
    /// </summary>
    private void DeathSequence()
    {
        if (deathClipPrefab != null)
            Instantiate(deathClipPrefab, transform.position, Quaternion.identity);

        if (objectsToDestroy == null)
            return;
        
        if (toSpawnOnDeathPrefab != null)
            Instantiate(toSpawnOnDeathPrefab, transform.position, Quaternion.Euler(180.0f, 0.0f, 0.0f));

        foreach (var toDestroy in objectsToDestroy)
            Destroy(toDestroy);

        Destroy(gameObject);
    }

    /// <summary>
    /// Sind die HP bei 0 oder niedriger, wird DeathSequence() ausgeführt.
    /// Liegen sie höher als die Max Health, wird ResetHealth() ausgeführt. -BZ
    /// </summary>
    private void HealthCheck()
    {       
        if (CurrentHealth <= 0)
        {
            DeathSequence();
        }
        else
        {
            if(toSpawnOnDamage != null)
                Instantiate(toSpawnOnDamage, transform.position + Vector3.up, Quaternion.identity);

            if (damageClipPrefab != null)
                Instantiate(damageClipPrefab, transform.position, Quaternion.identity);
        }
    }

    /// <summary>
    /// Zieht ein HP ab -BZ
    /// </summary>
    public void DecrementHealth()
    {
        currentHealth--;
        HealthCheck();
    }

    /// <summary>
    /// Fügt 1 HP hinzu -BZ
    /// </summary>
    public void IncrementHealth()
    {
        currentHealth++;
        HealthCheck();
    }

    public int MaxHealth
    {
        get { return maxHealth; }
    }

    public int CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            currentHealth = value;
            HealthCheck();
        }
    }

    public void GetDamage(int _damage)
    {
        CurrentHealth -= _damage;
        HealthCheck();
    }
}
