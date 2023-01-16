using Unity.Netcode;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player_setup))]
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

<<<<<<< HEAD
    [SerializeField]
    private GameObject[] disableGameObjectOnDeath;

    [SerializeField]
    private GameObject deathEffect;
    
    [SerializeField]
    private GameObject spawnEffect;

=======
>>>>>>> parent of 43e3892 (Explosion effects)
    private NetworkVariable<int> _currentHealth = new NetworkVariable<int>();
    public int currentHealth
    {
        get { return _currentHealth.Value; }
        protected set { _currentHealth.Value = value; }
    }

    private bool isFirstSetup = true;   

    public void SetupPlayer()
    {
        if (IsLocalPlayer)
        {
            //Here we switch camera
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<Player_setup>().playerUIInstance.SetActive(true);
        }

        BroadCastNewPlayerSetupServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void BroadCastNewPlayerSetupServerRpc()
    {
        SetupPlayerOnAllClientClientRpc();
    }

    [ClientRpc]
    private void SetupPlayerOnAllClientClientRpc()
    {
        if (isFirstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }

            isFirstSetup = false;
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

        //Enable the components 
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        ////Enable the GameObjects 
        //for (int i = 0; i < disableGameObjectOnDeath.Length; i++)
        //{
        //    disableGameObjectOnDeath[i].SetActive(true);
        //}

        ////Spawn effects
        //GameObject spawnEffectInstance = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
        //Destroy(spawnEffectInstance, 3f);

        //Enable the GameObjects 
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

<<<<<<< HEAD
        //Call death effect
        OnDeathEffectServerRpc();

        //Disable colliders
=======
>>>>>>> parent of 43e3892 (Explosion effects)
        Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = false;
        }

<<<<<<< HEAD
        //Here we switch camera
        if (IsLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<Player_setup>().playerUIInstance.SetActive(false);
        }

=======
>>>>>>> parent of 43e3892 (Explosion effects)
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

        //Spawn effects
        GameObject spawnEffectInstance = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(spawnEffectInstance, 3f);

        //Enable the GameObjects 
        for (int i = 0; i < disableGameObjectOnDeath.Length; i++)
        {
            disableGameObjectOnDeath[i].SetActive(true);
        }

        SetupPlayer();
    }

    //Is called on server when a bullet hits!
    //Takes in the hit point and normal of the surface

    [ServerRpc(RequireOwnership = false)]
    void OnDeathEffectServerRpc()
    {
        DoDeathEffectsClientRpc();
    }


    //is called on all clients when we bullet hits an object
    //We spawn cool effects here
    [ClientRpc]
    void DoDeathEffectsClientRpc()
    {

        GameObject deathEffectInstance = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(deathEffectInstance, 10f);

        //Disable GameObjects
        for (int i = 0; i < disableGameObjectOnDeath.Length; i++)
        {
            disableGameObjectOnDeath[i].SetActive(false);
        }

    }
}
