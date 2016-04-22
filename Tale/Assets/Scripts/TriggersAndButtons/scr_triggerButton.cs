using UnityEngine;
using System.Collections;

public class scr_triggerButton : MonoBehaviour
{

    // Use this for initialization
    public GameObject[] affectedGameObjects;
    public float buttonDuration;
    private float buttonCooldown;
    public bool triggerActivatorsOnly;
    public LayerMask triggerActiveMask;

    void Start()
    {
        buttonCooldown = 0;
    }

    // Update is called once per frame
    void Update()
    {
        buttonCooldown -= Time.deltaTime;
    }
    void OnTriggerStay(Collider colli)
    {
        if (!triggerActivatorsOnly)
        {
            if (colli.gameObject.CompareTag("Player"))
            {
                if (buttonCooldown < 0)
                {
                    for (int i = 0; i < affectedGameObjects.Length; i++)
                    {
                        scr_triggerReciever triggerReciever = affectedGameObjects[i].GetComponent<scr_triggerReciever>();
                        if (triggerReciever != null)
                        {
                            triggerReciever.SetTriggerDuration(buttonDuration);
                            print("Cooldown Set For" + colli.gameObject.name + "with  " + buttonDuration);
                        }
                    }
                    buttonCooldown = buttonDuration - 0.1f;
                }
            }
        }
        else
        {
            if (colli.gameObject.CompareTag("Trigger_activator"))
            {
                if (buttonCooldown < 0)
                {
                    for (int i = 0; i < affectedGameObjects.Length; i++)
                    {
                        scr_triggerReciever triggerReciever = affectedGameObjects[i].GetComponent<scr_triggerReciever>();
                        if (triggerReciever != null)
                        {
                            triggerReciever.SetTriggerDuration(buttonDuration);
                            print("Cooldown Set For" + colli.gameObject.name + "with  " + buttonDuration);
                        }
                    }
                    buttonCooldown = buttonDuration - 0.1f;
                }
            }
        }
    }
}
