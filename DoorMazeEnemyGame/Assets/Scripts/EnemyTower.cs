using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTower : MonoBehaviour
{ 
    public float currentHealth = 20f;

    public HealthBar healthBar;
    [SerializeField] private ParticleSystem towerSmoke;
    [SerializeField] private GameObject healthBarUI; 
    

    public void DealDamage(float damageToTake)
    {
        
        currentHealth -= (damageToTake * Time.deltaTime);

        healthBar.SetHealth(currentHealth);

        Debug.Log("Tower Health = " + currentHealth);
        if (currentHealth <= 0)
        {
            // particle effect
            Instantiate(towerSmoke, transform.position, Quaternion.identity);
            healthBarUI.SetActive(false); 
            Destroy(gameObject);
        }
    }
}
