using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Player : MonoBehaviour {
    //stats vars
    public float health = 100f;
    public float hunger = 100f;
    public float thirst = 100f;
    public float stamina = 100f;

    //max stats vars
    public float maxHealth = 100f;
    public float maxHunger = 100f;
    public float maxThirst = 100f;
    public float maxStamina = 100f;

    //min stats vars
    public float minHealth = 0;
    public float minHunger = 0;
    public float minThirst = 0;
    public float minStamina = 0;

    //changes frequencies
    public float healingTime = 20f;
    public float restingTime = 5f;
    public float tiringTime = 5f;
    public float hungryTime = 5f;
    public float thirstyTime = 5f;

    //changes values
    public float healingSpeed = 1f;
    public float restingSpeed = 1f;
    public float tiringSpeed = 1f;
    public float hungrySpeed = 1f;
    public float thirstySpeed = 1f;

    //stamina related vars
    public bool canSprint = true;
    public bool canJump = true;

    public bool wellFed = true;
    public bool canRest = true;//if isn't running or jumping
    public bool canCheckRest = true;

    public bool wasJumping = false;

    public float jumpEnergy = 10f;

    public float checkRestCooldownTime = 5f;

    public FirstPersonController fpc;

    public bool isResting = false;
    public bool isTiring = false;
    public bool isHealing = false;

    public void Start()
    {
        fpc = this.GetComponent<FirstPersonController>();
        //Setup hunger and thrist process
        InvokeRepeating("Hungry", 0f, hungryTime);
        InvokeRepeating("Thirsty", 0f, thirstyTime);
    }

    public void LoadStats (float _health, float _hunger, float _thirst, float _stamina)
    {
        health = _health;
        hunger = _hunger;
        thirst = _thirst;
        stamina = _stamina;
    }

    private void Update()
    {
        if(!fpc.m_IsWalking && Mathf.Sqrt(Mathf.Pow(fpc.m_Input.x, 2) + Mathf.Pow(fpc.m_Input.y, 2)) > 0)//if running
        {
            if (canCheckRest)//if running while checking is allowed
            {
                canRest = false;//prevent stamina and health regen
                StartCoroutine(CheckRestCooldown());//reset cooldown
            }
            //Consumme stamina
            if(!isTiring)
                InvokeRepeating("Tire", 0f, tiringTime);
        }
        else
        {
            //Stop consumming stamina
            if (isTiring)
            {
                CancelInvoke("Tire");
                isTiring = false;
            }
                
                
        }

        if (fpc.m_Jumping)//if jumping
        {
            if (canCheckRest)//if jumping while checking is allowed
            {
                canRest = false;//prevent stamina and health regen
                StartCoroutine(CheckRestCooldown());//reset cooldown
            }
            if(!wasJumping)
            {
                Jump();
                wasJumping = true;
            }
        }
        else
        {
            wasJumping = false;
        }
        if(stamina == 0)
        {
            canSprint = false;
        }
        else
        {
            canSprint = true;
            canJump = true;
        }

        //temporary, delete when custom char controler is ready
        fpc.cS = canSprint;
        fpc.cJ = canJump;

        if (canCheckRest && !fpc.m_Jumping && !(!fpc.m_IsWalking && Mathf.Sqrt(Mathf.Pow(fpc.m_Input.x, 2) + Mathf.Pow(fpc.m_Input.y, 2)) > 0))//if checking is allowed and isn't running or jumping
        {
            canRest = true;//allow stamina and health regen
        }

        if (hunger < 50f || thirst < 50f)
        {
            wellFed = false;
        }
        else
        {
            wellFed = true;
        }
        if(wellFed && canRest)
        {
            //Start resting and healing process
            if (!isHealing)
                InvokeRepeating("Heal", 0f, healingTime);
            if (!isResting)
                InvokeRepeating("Rest", 0f, restingTime);
        }
        else
        {
            //Stop resting and healing process
            if (isHealing)
            {
                CancelInvoke("Heal");
                isHealing = false;
            }
                
            if (isResting)
            {
                CancelInvoke("Rest");
                isResting = false;
            }
                
        }
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(10, Screen.height - 180, 200, 160), "");
        //Display stats
        GUI.Label(new Rect(20, Screen.height - 180 + 1 * 10 + 0 * 30, 180, 30), "Health: " + (int)health);
        GUI.Label(new Rect(20, Screen.height - 180 + 2 * 10 + 1 * 30, 180, 30), "Hunger: " + (int)hunger);
        GUI.Label(new Rect(20, Screen.height - 180 + 3 * 10 + 2 * 30, 180, 30), "Thirst: " + (int)thirst);
        GUI.Label(new Rect(20, Screen.height - 180 + 4 * 10 + 3 * 30, 180, 30), "Stamina: " + (int)stamina);
    }

    IEnumerator CheckRestCooldown()
    {
        //setup cooldown before we can recheck if is resting
        canCheckRest = false;
        yield return new WaitForSeconds(checkRestCooldownTime);
        canCheckRest = true;
    }

    private void Jump()
    {
        /*stamina -= jumpEnergy;
        if (stamina > maxStamina)
        {
            stamina = maxStamina;
        }
        if (stamina < minStamina)
        {
            stamina = minStamina;
        }*/
    }

    private void Heal()//Add "healingSpeed" value to health when called
    {
        isHealing = true;
        health += healingSpeed;
        if(health > maxHealth)
        {
            health = maxHealth;
        }
        if (health < minHealth)
        {
            health = minHealth;
        }
    }

    private void Rest ()//Add "restingSpeed" value to stamina when called
    {
        isResting = true;
        stamina += restingSpeed;
        if(stamina > maxStamina)
        {
            stamina = maxStamina;
        }
        if (stamina < minStamina)
        {
            stamina = minStamina;
        }
    }

    private void Tire()//Remove "tiringSpeed" value to stamina when called
    {
        isTiring = true;
        stamina -= tiringSpeed;
        if (stamina > maxStamina)
        {
            stamina = maxStamina;
        }
        if (stamina < minStamina)
        {
            stamina = minStamina;
        }
    }

    private void Hungry ()//Remove "hungrySpeed" value to hunger when called
    {
        hunger -= hungrySpeed;
        if(hunger > maxHunger)
        {
            hunger = maxHunger;
        }
        if (hunger < minHunger)
        {
            hunger = minHunger;
        }
    }

    private void Thirsty ()//Remove "thirstySpeed" value to thirst when called
    {
        thirst -= thirstySpeed;
        if(thirst > maxThirst)
        {
            thirst = maxThirst;
        }
        if (thirst < minThirst)
        {
            thirst = minThirst;
        }
    }
}
