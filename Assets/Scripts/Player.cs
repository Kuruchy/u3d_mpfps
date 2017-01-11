using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour {

    //Whenever the variable changes, it syncs it with all the clients and the server
    [SyncVar]
    private bool _isDead = false;

    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }
    [Header("Stats:")]
    [SerializeField]
    private int maxHealth = 100;
    [SyncVar]
    private int currentHealth;

    [Header("Dislable on Death:")]
    [SerializeField]
    private Behaviour[] dislableComponentsOnDeath;
    private bool[] wasEnabled;
    [SerializeField]
    private GameObject[] dislableGameObjectsOnDeath;

    [Header("Effects:")]
    [SerializeField]
    private GameObject deathEffect;
    [SerializeField]
    private GameObject spawnEffect;

    private bool firstSetup = true;

    public void SetupPlayer()
    {
        if (isLocalPlayer)
        {
            // Switch cameras
            GameManager.instance.SetSceneCameraAsMain(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }

        CmdBroadcastNewPlayerSetup();
    }

    [Command]
    private void CmdBroadcastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (firstSetup)
        {
            wasEnabled = new bool[dislableComponentsOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = dislableComponentsOnDeath[i].enabled;
            }

            firstSetup = false;
        }

        SetDefaults();
    }

    public void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.K))
        {
            RpcTakeDamage(1000);
        }
    }

    [ClientRpc]
    public void RpcTakeDamage(int _damage)
    {
        if (_isDead)
        {
            return;
        }

        currentHealth -= _damage;
        Debug.Log(transform.name + " now has " + currentHealth + " health.");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // Dislable components
        for (int i = 0; i < dislableComponentsOnDeath.Length; i++)
        {
            dislableComponentsOnDeath[i].enabled = false;
        }

        // Dislable gameobjects
        for (int i = 0; i < dislableGameObjectsOnDeath.Length; i++)
        {
            dislableGameObjectsOnDeath[i].SetActive(false);
        }

        // Dislable the collider
        Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = false;
        }

        // Spawn the death effect
        GameObject _deathEffectIns = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(_deathEffectIns, 2f);

        // Switch cameras
        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraAsMain(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        Debug.Log(transform.name + " is dead!");

        // StartCoroutine for calling an IEnumerator function
        StartCoroutine(Respawn());
    }


    //IEnumerator for yield seconds
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        // Switch cameras
        GameManager.instance.SetSceneCameraAsMain(false);
        GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);

        Transform _startPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _startPoint.position;
        transform.rotation = _startPoint.rotation;

        yield return new WaitForSeconds(0.1f);

        SetupPlayer();

        Debug.Log(transform.name + " respawned!");
    }

    public void SetDefaults()
    {

        isDead = false;

        currentHealth = maxHealth;

        // Enable behaviours
        for (int i = 0; i < dislableComponentsOnDeath.Length; i++)
        {
            dislableComponentsOnDeath[i].enabled = wasEnabled[i];
        }

        // Enable gameobjects
        for (int i = 0; i < dislableGameObjectsOnDeath.Length; i++)
        {
            dislableGameObjectsOnDeath[i].SetActive(true);
        }

        Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = true;
        }

        // Spawn the death effect
        GameObject _spawnEffectIns = Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_spawnEffectIns, 2f);
    }
}
