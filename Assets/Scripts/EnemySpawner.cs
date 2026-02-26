using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Ayarlar")]
    public GameObject enemyPrefab; // Düþman askerinin Prefab'ý
    public Transform playerBase;   // Hedef: Oyuncunun Kalesi
    public float spawnInterval = 5f; // Kaç saniyede bir asker çýksýn?

    private float timer;

    void Start()
    {
        timer = spawnInterval; // Ýlk askerin çýkmasý için zamanlayýcýyý kur
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnEnemy();
            timer = spawnInterval; // Zamanlayýcýyý sýfýrla
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        // Düþmaný spawner konumunda oluþtur
        GameObject newEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

        // Düþmana hedef olarak oyuncu kalesini ver
        UnitController controller = newEnemy.GetComponent<UnitController>();
        if (controller != null)
        {
            controller.target = playerBase;
        }
    }
}