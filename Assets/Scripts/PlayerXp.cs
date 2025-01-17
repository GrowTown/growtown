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
            UI_Manager.Instance.PlayerLevel.UpdatetingthePlayerLevel(_playerXp);
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
        /*  switch (currentAction)
          {
              case PlayerAction.Clean:

                  Debug.Log("Cleanig");
                  break;
              case PlayerAction.Seed:
                  AddingXP(4);
                  // Debug.Log("Seeding");
                  break;
              case PlayerAction.Water:
                  AddingXP(2);
                  break;
              case PlayerAction.Harvest:
                  AddingXP(8);
                  break;
          }*/
    }
}
