using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] protected float coolDown;
    protected float coolDownTimer;


    protected virtual void Update()
    {
        coolDownTimer -= Time.deltaTime;
    }


    public virtual bool CanUseSkill()
    {
        if (coolDownTimer < 0)
        {
            UseSkill();
            coolDownTimer = coolDown;
            return true;
        }

        Debug.Log("skill is cooldown");
        return false;
    }

    public virtual void UseSkill()
    {
        // do some skill specific things

    }
}