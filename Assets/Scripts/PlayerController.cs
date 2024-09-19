using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region Movement_variables
    public float moveSpeed = 3;
    float x_input;
    float y_input;
    #endregion

    #region Physics_components
    Rigidbody2D PlayerRB;
    #endregion

    #region Health_variables
    public float maxHealth = 5;
    float currHealth = 5;
    public Slider hp_slider;
    #endregion

    #region Animation_components
    Animator anim;
    #endregion

    #region Unity_functions
    private void Awake() {
        /* TODO: Update your Awake function to initialize all variables needed. This includes your attackTimer, and your HPSlider.value.*/
        attackTimer = 0;
        currHealth = maxHealth;
        hp_slider.value = currHealth / maxHealth;
        /* TODO 4.1: Set HPSlider.value to a ratio between the 
            player's current health and maximum health. */
            
        PlayerRB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    private void Update() {
        if(isAttacking == true) {
            return;
        }
        if(Input.GetKeyDown(KeyCode.E)) {
            Interact();
        }
        /*TODO 1.1: Write an Update function that will call the Move() helper function while also updating the x_input and y_input values.
        You will also need to edit this function when you call attacks, and interacting with chests.*/
        x_input = Input.GetAxisRaw("Horizontal");
        y_input = Input.GetAxisRaw("Vertical");

        Move();
        /* TODO 1.2: Check if the attack key is being pressed. If so, attack by calling your Attack() function
         * IMPORTANT:  You will need to use `Input.GetKeyDown(KeyCode key)` to determine if the key is being pressed
        */
        
        if (Input.GetKeyDown(KeyCode.J)) {        
            if (attackTimer < 0){
                Attack();
                attackTimer = attackSpeed;
            }
        }
        attackTimer -= Time.deltaTime;
        /* TODO 1.3: Modify your attack conditional statement to only attack when attackTimer < 0. Otherwise, decrement the attackTimer. */
    }
    #endregion



    #region Attack_variables
    public float damage = 2;
    public float attackSpeed = 1;
    float attackTimer;
    public float hitboxTiming = 0.1f;
    public float endAnimationTiming = 0.1f;
    bool isAttacking;
    Vector2 currDirection;
    #endregion

    #region Attack_functions
    private void Attack()
    {
        // TODO 1.3: Set the attackTimer to attackSpeed to reset the attack cooldown 
        Debug.Log("Attacking now");
        Debug.Log(currDirection);
        StartCoroutine(AttackRoutine());

    }

    IEnumerator AttackRoutine()
    {
        /* Note: You will need to edit this function in the animation section, the enemy section, and in the sound section.*/
        isAttacking = true;
        FindObjectOfType<AudioManager>().Play("PlayerAttack");
        PlayerRB.velocity = Vector2.zero;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(hitboxTiming);
        Debug.Log("Casting hitbox now");
        RaycastHit2D[] hits = Physics2D.BoxCastAll(PlayerRB.position + currDirection, Vector2.one, 0f, Vector2.zero);

        foreach (RaycastHit2D hit in hits)
        {
            if(hit.transform.CompareTag("Enemy"))
            {
                Debug.Log("Tons of Damage");
                /* TODO 3.2: Call TakeDamage() inside of the enemy's Enemy script using
                the "hit" reference variable */
                hit.transform.GetComponent<Enemy>().TakeDamage(damage);
            }
        }

        yield return new WaitForSeconds(hitboxTiming);
        isAttacking = false;
    }
    
    #endregion

    #region Movement_functions
    private void Move()
    {
        /*TODO 1.1: Edit the Move() function which will set PlayerRB.velocity to a vector based on which input the player is pressing.*/

        if (x_input > 0) {
            PlayerRB.velocity = Vector2.right * moveSpeed;
            currDirection = Vector2.right;
        }
        else if (x_input < 0) {
            PlayerRB.velocity = Vector2.left * moveSpeed;
            currDirection = Vector2.left;
        }
        else if (y_input > 0) {
            PlayerRB.velocity = Vector2.up * moveSpeed;
            currDirection = Vector2.up;
        }
        else if (y_input < 0) {
            PlayerRB.velocity = Vector2.down * moveSpeed;
            currDirection = Vector2.down;
        }
        else {
            PlayerRB.velocity = Vector2.zero;
        }


        /* TODO 1.4: Set currDirection to the correct Vector direction i.e. Vector2.left.
         * HINT: there are four cardinal directions. */


        /* DO NOT MODIFY ANYTHING BELOW THIS LINE UNLESS YOU REALLY KNOW WHAT YOU'RE DOING */

        if (x_input == 0 && y_input == 0)
        {
            anim.SetBool("Moving", false);
        }
        else
        {
            anim.SetBool("Moving", true);

        }

        anim.SetFloat("DirX", currDirection.x);
        anim.SetFloat("DirY", currDirection.y);

    }
    #endregion

    #region Health_functions

    public void TakeDamage(float value)
    {
        /* TODO 3.1: Adjust currHealth when the player takes damage
        IMPORTANT: What happens when the player's health reaches 0? */
        currHealth -= value;
        hp_slider.value = currHealth / maxHealth;
        FindObjectOfType<AudioManager>().Play("PlayerHurt");
        Debug.Log("currHealth = " + currHealth);
        if (currHealth <=0){
            Die();
        }

        /* TODO 4.1: Update the value of HPSlider after the player's health changes. */
    }

    public void Heal(float value)
    {
        /* TODO 3.1: Adjust currHealth when the player heals
        IMPORTANT: What happens when the player's health surpasses their max health? Should currHealth be above maxHealth?*/
        if (currHealth >= maxHealth){
            return;
        }
        currHealth += value;
        hp_slider.value = currHealth / maxHealth;
        Debug.Log("Healing! Health is now: " + currHealth);
        /* TODO 4.1: Update the value of HPSlider after the player's health changes. */
    }

    public void Die()
    {
        FindObjectOfType<AudioManager>().Play("PlayerDeath");
        Destroy(this.gameObject);
        GameObject.FindWithTag("GameController").GetComponent<GameManager>().LoseGame();
    }
    #endregion

    #region Interact_functions
    private void Interact()
    {
        /* TODO 6.3: Use a BoxCastAll raycast to check what is infront of the player. 
         * If there is a chest game object, open the chest by calling it's Open() function */
        RaycastHit2D[] hits = Physics2D.BoxCastAll(PlayerRB.position + currDirection, new Vector2(0.5f, 0.5f), 0f, Vector2.zero, 0f);
        foreach (RaycastHit2D hit in hits) {
            if (hit.transform.CompareTag("Chest")){
                Debug.Log("Triggered Chest");
                hit.transform.GetComponent<Chest>().Open();
            }
        }
    }
    #endregion
}
