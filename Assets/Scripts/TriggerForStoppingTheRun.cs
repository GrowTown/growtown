using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerForStoppingTheRun : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UI_Manager.Instance.IsPlayerInSecondZone = true;
           /* var Tzc=this.gameObject.transform.GetChild(0).gameObject.GetComponent<TriggerZoneCallBacks>();
            if (Tzc.fieldID == 2)
            {
                other.gameObject.GetComponent<CamerasSwitch>().SwitchToCam(1);
            }
            else if (Tzc.fieldID == 1)
            {
                other.gameObject.GetComponent<CamerasSwitch>().SwitchToCam(3);
            }
            else if (Tzc.fieldID == 0)
            {
                other.gameObject.GetComponent<CamerasSwitch>().SwitchToCam(4);

            }
            else
            {
                if (!GameManager.Instance.isShowingnewLand)
                    other.gameObject.GetComponent<CamerasSwitch>().SwitchToCam(2);
            }*/
        }
        else
        {

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UI_Manager.Instance.IsPlayerInSecondZone = false;

          /* var CamSwitch=other.gameObject.GetComponent<CamerasSwitch>();
            var newPos=CamSwitch.activeCamera.transform.position;
            CamSwitch.SwitchToCam(0);
            CamSwitch.activeCamera.transform.position = newPos;*/
        }
    }
}
