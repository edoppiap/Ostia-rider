using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int globalMoney;
    public int record;

    public PlayerData(Player player)
    {
        globalMoney = player.globalMoney;
        record = player.record;
    }
}
