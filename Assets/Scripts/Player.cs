using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class Player : NetworkBehaviour
{
    private NetworkVariable<bool> _isDead = new NetworkVariable<bool>(false);
    public bool isDead
    {
        get { return _isDead.Value; }
        protected set { _isDead.Value = value; }
    }

    [SerializeField]
    private GameObject spawnPoint;

    [SerializeField]
    private int maxHealth = 100;

    [SerializeField]
    private GameObject spwanPoint;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    private NetworkVariable<int> currentHealth = new NetworkVariable<int>();


    public void Setup()
    {
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }

        SetDefaults();
    }

    private void Update()
    {
        if (!IsLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamageClientRpc(40);
        }
    }

    public void SetDefaults()
    {
        _isDead.Value = false; 

        currentHealth.Value = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        Collider _col = GetComponent<Collider>();
        if(_col != null)
        {
            _col.enabled = true;
        }
    }

    [ClientRpc]
    public void TakeDamageClientRpc(int _amount)
    {
        if (_isDead.Value)
            return;

        currentHealth.Value -= _amount;

        Debug.Log(transform.name + "now has" + currentHealth.Value + "health");

        if(currentHealth.Value <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _isDead.Value = true;

        //Disable Components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = false;
        }

        Debug.Log(transform.name + " is Dead");

        //call respawn method
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f);

        Transform spawnedPointTransform =  Instantiate(spwanPoint.transform);
        spawnedPointTransform.GetComponent<NetworkObject>().Spawn(true);
        transform.position = spwanPoint.transform.position;
        Debug.Log("I am Respawned");
        SetDefaults();       
    }  
}
