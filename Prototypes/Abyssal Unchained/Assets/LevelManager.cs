using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] BossStats boss;
    [SerializeField] TextMeshProUGUI bossName;
    public Player player;

    public static LevelManager instance;

    private void Start()
    {
        if(instance != null)
        {
            Destroy(this);
        }
        instance = this;


        Debug.Log(boss.ReturnNameToPrint());
        bossName.text = boss.ReturnNameToPrint();
        Debug.Log(bossName.text);
    }
}
