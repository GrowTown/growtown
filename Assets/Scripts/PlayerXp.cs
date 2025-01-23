using UnityEngine;

public  class PlayerXp:MonoBehaviour
{
    int _playerXp = 0;

    public int CurrentPlayerXpPoints
    { 
        get=>_playerXp;
        set
        {
            _playerXp = value;
            UI_Manager.Instance.playerXpTxt.text = _playerXp.ToString();
            UI_Manager.Instance.PlayerLevel.UpdatePlayerLevel(_playerXp);
        }
    }
    
    internal void AddingXP(int xp)
    {
        CurrentPlayerXpPoints += xp;
       
    }

    public void SuperXp(int Xp)
    {
        if (!UI_Manager.Instance.isSuperXpEnable)
        {
            AddingXP(Xp);
        }
        else
        {
            Xp += Xp;
            AddingXP(Xp);
        }
    }
}
