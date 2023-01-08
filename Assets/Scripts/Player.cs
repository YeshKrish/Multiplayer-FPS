using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private int maxHealth = 100;

    private NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    // Start is called before the first frame update
    void Awake()
    {
        SetDefaults();
    }

    public void SetDefaults()
    {
        currentHealth.Value = maxHealth; 
    }


    public void TakeDamage(int _amount)
    {
        currentHealth.Value -= _amount;

        Debug.Log(transform.name + "now has" + currentHealth.Value + "health");
    }
}
