using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasePassiveGem", menuName = "Gems/PassiveGems/BasePassiveGem")]
public class PassiveGem : BaseGem
{
    public string ReturnPrefix()
    {
        return namePartString;
    }
} 