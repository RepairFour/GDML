using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BaseAttackModifierGem", menuName = "Gems/AttackModifierGems/BaseAttackModifierGem")]
public class FightingStyleGem : BaseGem
{
    public string ReturnPostFix()
    {
        return namePartString;
    }
 
}
