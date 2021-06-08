using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStats : MonoBehaviour
{
    [Header("Base Stats", order = 0)]
    [SerializeField] protected int baseHealth;
    [SerializeField] protected int baseDamage;
    [SerializeField] protected int baseMoveSpeed;

    [Header("Boss Naming Variables", order = 1)]
    [SerializeField] protected string title;
    [SerializeField] protected string prefix;
    [SerializeField] protected string postfix;
    [SerializeField] protected string nameToPrint;

    [Header("Sockets", order = 2)]
    [SerializeField] protected PassiveGem alpha;

    [Header("Statistics", order = 4)]
    [SerializeField] protected int timesAttacked;

    public string ReturnNameToPrint()
    {
        nameToPrint = prefix + " " + title + " " + postfix;
        return nameToPrint;
    }

    public int ReturnBaseHealth()
    {
        return baseHealth;
    }

    private void Update()
    {
        
    }


}
