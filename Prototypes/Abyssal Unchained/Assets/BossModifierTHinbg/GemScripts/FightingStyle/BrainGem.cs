using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseBrainGem", menuName = "Gems/BrainGems/BaseBrainGem")]
public class BrainGem : BaseGem
{
    public string ReturnTitle()
    {
        return namePartString;
    }

    public virtual void Execute(BossStats stats, PassiveGem passive, FightingStyleGem atkGem)
    {
        Debug.Log(stats.ReturnBaseHealth());
    }
}
