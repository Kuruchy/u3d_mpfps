using System;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour {

    private const string PLAYER_TAG = "Player";
    
    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    private void Start()
    {
        if(cam == null)
        {
            Debug.Log("PlayerShoot: no camera");
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();
    }

    private void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

        // Dont update if game is in pause
        if (PauseMenu.IsOn)
            return;

        if(currentWeapon.fireRate <= 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0, 1f/currentWeapon.fireRate);
            }else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
        
    }

    // Is called on the server when a player shoots
    [Command]
    void CmdOnShoot()
    {
        RpcDoShootingEffect();
    }

    // Is called on all clients ordered from server
    [ClientRpc]
    void RpcDoShootingEffect()
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
    }

    // Is called on the server when we hit something
    [Command]
    void CmdOnhit(Vector3 _pos, Vector3 _normal)
    {
        RpcDoHitEffect(_pos, _normal);
    }
    
    // Is called on all clients ordered from server
    [ClientRpc]
    void RpcDoHitEffect(Vector3 _pos, Vector3 _normal)
    {
        GameObject _hitEffect = Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
        Destroy(_hitEffect, 2f);
    }

    [Client]
    private void Shoot()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        // Call the Shooting method on server
        CmdOnShoot();

        Debug.Log("Shoot");
        RaycastHit _hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask)){
           
            if (_hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShoot(_hit.collider.name, currentWeapon.damage);
            }
        }

        // We hit something, call the onhit method on the server
        CmdOnhit(_hit.point, _hit.normal);

    }

    // Command implies that the function works on the server side
    [Command]
    void CmdPlayerShoot(string _PlayerID, int _damage)
    {
        Debug.Log(_PlayerID + " has been shot!");

        Player _player = GameManager.GetPlayer(_PlayerID);

        _player.RpcTakeDamage(_damage);
    }
}
