using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TitansGrasp", menuName = "Gems/PassiveGems/TitansGrasp")]
public class TitansGrasp : PassiveGem
{
    [SerializeField] protected int damageIncrease;

    public int ReturnDamageIncrease()
    {
        return damageIncrease;
    }

}
