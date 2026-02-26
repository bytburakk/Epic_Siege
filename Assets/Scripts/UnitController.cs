using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.UI;

public class UnitController : MonoBehaviour
{
    [Header("Veri Ayarlarý")]
    public UnitData data;
    public Transform target;
    public LayerMask enemyLayer;
    public Slider healthSlider;

    private NavMeshAgent agent;
    private Transform currentEnemy;
    private bool isAttacking = false;
    private Animator anim;

    [Header("Can Sistemi")]
    public int currentHealth;
    private bool isDead = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        // DATA KONTROLÜ VE CAN ATAMASI
        if (data != null)
        {
            currentHealth = data.maxHealth;
            agent.speed = data.speed;
            // stoppingDistance'ý direkt attackRange yapýyoruz
            agent.stoppingDistance = data.attackRange;

            if (healthSlider != null)
            {
                healthSlider.maxValue = data.maxHealth;
                healthSlider.value = currentHealth;
            }
        }
        else
        {
            Debug.LogError(gameObject.name + ": UnitData atanmamýþ! Can 0 kalýr.");
        }

        StartCoroutine(SetInitialTarget());
    }

    IEnumerator SetInitialTarget()
    {
        yield return new WaitForSeconds(0.1f);
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh && target != null)
        {
            agent.SetDestination(target.position);
        }
    }

    void Update()
    {
        if (isDead || agent == null || !agent.isOnNavMesh) return;

        DetectEnemy();

        // 1. HEDEF VE HAREKET MANTIÐI
        if (currentEnemy != null)
        {
            agent.SetDestination(currentEnemy.position);

            // MESAFE KONTROLÜ
            float distance = Vector3.Distance(transform.position, currentEnemy.position);

            // Menzile girdi mi? (+0.2f tolerans)
            if (distance <= agent.stoppingDistance + 0.2f)
            {
                if (!isAttacking)
                {
                    StartCoroutine(AttackRoutine());
                }
            }
            else if (isAttacking && distance > agent.stoppingDistance + 0.5f)
            {
                // Düþman çok uzaklaþýrsa kovalamaya geri dön
                StopAttacking();
            }
        }
        else
        {
            // Düþman yoksa kaleye git
            if (target != null) agent.SetDestination(target.position);
            if (isAttacking) StopAttacking();
        }
    }

    void DetectEnemy()
    {
        // 5 birimlik alanda düþman tara
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 8f, enemyLayer);

        if (hitColliders.Length > 0)
        {
            // En yakýndaki düþmaný seç (Opsiyonel: Daha detaylý seçim eklenebilir)
            currentEnemy = hitColliders[0].transform;
        }
        else
        {
            currentEnemy = null;
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        anim.SetBool("isAttacking", true);
        agent.isStopped = true; // Saldýrý anýnda ayaklarý yere çivile

        while (isAttacking && currentEnemy != null && !isDead)
        {
            // Düþmana bak
            Vector3 lookPos = currentEnemy.position;
            lookPos.y = transform.position.y; // Yukarý-aþaðý bakmasýn
            transform.LookAt(lookPos);

            // HASAR VERME
            UnitController enemyScript = currentEnemy.GetComponent<UnitController>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(data.damage);
                Debug.Log(gameObject.name + " vurdu! Verilen Hasar: " + data.damage);
            }

            // Saldýrý hýzý kadar bekle
            yield return new WaitForSeconds(data.attackSpeed > 0 ? data.attackSpeed : 1f);

            // Mesafe kontrolü
            if (currentEnemy == null || Vector3.Distance(transform.position, currentEnemy.position) > agent.stoppingDistance + 0.5f)
            {
                break;
            }
        }

        StopAttacking();
    }

    void StopAttacking()
    {
        isAttacking = false;
        if (anim != null) anim.SetBool("isAttacking", false);
        if (agent != null && agent.isOnNavMesh) agent.isStopped = false;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log(gameObject.name + " Hasar aldý! Kalan Can: " + currentHealth);

        if (healthSlider != null) healthSlider.value = currentHealth;

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        StopAllCoroutines(); // Saldýrý korutinini durdur
        anim.SetTrigger("Die");

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        // Collider'ý kapat ki diðerleri bu "cesede" saldýrmaya çalýþmasýn
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;

        Destroy(gameObject, 2.5f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 8f); // Görüþ mesafesi
        Gizmos.color = Color.yellow;
        if (data != null) Gizmos.DrawWireSphere(transform.position, data.attackRange); // Saldýrý mesafesi
    }
}