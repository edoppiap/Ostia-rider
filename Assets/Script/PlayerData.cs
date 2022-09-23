using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string id;
    public int globalMoney;
    public int record;

    public PlayerData(Player player)
    {
        id = player.id;
        globalMoney = player.globalMoney;
        record = player.record;
    }
}
