using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSpawner : MonoBehaviour
{
    public GameObject unitPrefab; // Project panelindeki mavi kapsül
    public Transform targetBase; // Düþman kalesi

    void Update()
    {
        // Yeni Input System ile Space tuþuna basýlýp basýlmadýðýný kontrol eder
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SpawnUnit();
        }
    }

    void SpawnUnit()
    {
        // Askeri spawner'ýn olduðu yerde oluþtur
        GameObject newUnit = Instantiate(unitPrefab, transform.position, Quaternion.identity);

        // Askerin hedef kalesini belirle
        newUnit.GetComponent<UnitController>().target = targetBase;
    }
}