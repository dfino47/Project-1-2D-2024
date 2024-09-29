using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthMaxPotion : MonoBehaviour
{
    #region HealthMaxPotion_variables
    [SerializeField]
    [Tooltip("the multiplier on the player's maximum health")]
    private int multiplier;
    #endregion

    #region Heal_functions
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        /* TODO Part 6.1: If this object collides with the player, heal the player by healAmount by calling the player's Heal() function.
         * After healing this health potion should be destroyed.
         * HINT: The variable, other, contains a reference to the object that collides with this health potion. */
        if(other.gameObject.CompareTag("Player")){
            PlayerController PlayerRef = other.gameObject.GetComponent<PlayerController>();
            PlayerRef.maxHealth *= multiplier;
            PlayerRef.Heal(PlayerRef.currHealth);
            FindObjectOfType<AudioManager>().Play("Potion");
            Destroy(gameObject);
        }
    }
    #endregion
}

