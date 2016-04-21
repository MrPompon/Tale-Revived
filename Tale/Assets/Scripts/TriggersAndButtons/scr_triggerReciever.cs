using UnityEngine;
using System.Collections;

public class scr_TriggerReciever : MonoBehaviour
{

    // Use this for initialization
    private float triggerDuration;
    void Start()
    {
        triggerDuration = 0;
    }

    // Update is called once per frame
    void Update()
    {
        triggerDuration -= Time.deltaTime;
    }
    public float GetTriggerDuration()
    {
        return triggerDuration;
    }
    public void SetTriggerDuration(float p_duration)
    {
        triggerDuration = p_duration;
    }
}
