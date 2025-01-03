using UnityEngine;

public  class PlayerXp:MonoBehaviour
{
    int _playerXp=0;

    public int PlayerXpPoints
    { 
        get=>_playerXp; 
        set=>_playerXp = value;
    }
    
    internal void AddingXP(int xp)
    {
        PlayerXpPoints += xp;
        UI_Manager.Instance.playerXpTxt.text=PlayerXpPoints.ToString();
    }
}
