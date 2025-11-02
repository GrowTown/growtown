using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHammerManager : MonoBehaviour
{
    [Header("Hammer Settings")]
    public GameObject hammerPrefab;
    public Transform handSocket;
    public KeyCode equipKey = KeyCode.E;

    private GameObject equippedHammer;
    private bool hammerEquipped = false;

    void Update()
    {
        if (Input.GetKeyDown(equipKey))
        {
            if (!hammerEquipped)
                EquipHammer();
            else
                UnequipHammer(); // optional toggle off
        }
    }

    public void EquipHammer()
    {
        if (hammerPrefab == null || handSocket == null)
        {
            Debug.LogWarning("Hammer prefab or hand socket not assigned!");
            return;
        }

        equippedHammer = Instantiate(hammerPrefab, handSocket.position, handSocket.rotation, handSocket);
        equippedHammer.tag = "Hammer";

        Rigidbody rb = equippedHammer.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        hammerEquipped = true;
        Debug.Log("Hammer equipped!");
    }

    public void UnequipHammer()
    {
        if (equippedHammer != null)
            Destroy(equippedHammer);

        hammerEquipped = false;
        Debug.Log("Hammer unequipped.");
    }
}
