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
    private Transform spawnPoint;

    [SerializeField]
    private int maxHealth = 100;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    private NetworkVariable<int> _currentHealth = new NetworkVariable<int>();
    public int currentHealth
    {
        get { return _currentHealth.Value; }
        protected set { _currentHealth.Value = value; }
    }

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
        isDead = false;

        currentHealth = maxHealth;

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
        if (isDead)
            return;

        currentHealth -= _amount;

        Debug.Log(transform.name + "has" + currentHealth + "health");

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

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
        StartCoroutine(Respawn(transform.name));
    }

    private IEnumerator Respawn(string _playerID)
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnDelay);
        SpawningServerRpc(_playerID);
       
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawningServerRpc(string _playerID)
    {
        Transform spawnedPointTransform = Instantiate(spawnPoint);
        spawnedPointTransform.GetComponent<NetworkObject>().Spawn(true);
        SpawnClientRpc(_playerID);   
    }

    [ClientRpc]
    void SpawnClientRpc(string _playerID)
    {
        Player _player = GameManager.GetPlayer(_playerID);
        Debug.Log("The player who dies is:" + _player.transform.name);
        _player.transform.position = spawnPoint.position;
        Debug.Log("I am Respawned" + _player.name + "Position:" + _player.transform.position);
        SetDefaults();
    }
}
