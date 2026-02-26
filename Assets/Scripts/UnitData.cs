using UnityEngine;

[CreateAssetMenu(fileName = "NewUnit", menuName = "SiegeGame/Unit")]
public class UnitData : ScriptableObject
{
    public string unitName;
    public int maxHealth = 100; // Maksimum can
    public float speed = 3.5f;
    public float attackRange = 2f;
    public float attackSpeed = 1.0f; // Saniyede kaç vuruþ yapacak?
    public int damage = 20;
    public GameObject modelPrefab;
}