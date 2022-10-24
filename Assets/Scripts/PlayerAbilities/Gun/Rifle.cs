using Photon.Pun;
using PlayerAbilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Rifle : Gun
{
    [SerializeField] private float _damage;
    [SerializeField] private ParticleSystem _shootParticle;
    [SerializeField] private float _recoilForceXMax;
    [SerializeField] private float _recoilForceYMax;
    [SerializeField] private float _recoilForceXMin;
    [SerializeField] private float _recoilForceYMin;
    [SerializeField] private int _maxAmmoCount;

    private int _maxAmmoQuanity;

    private void Start()
    {
        _ammoQuanity = _maxAmmo;
        _maxAmmoQuanity = _maxAmmoCount;
    }

    private void FixedUpdate()
    {
        if (PhotonView != null)
        {
            if (PhotonView.IsMine)
            {
                if (Input.GetMouseButton(0))
                {
                    Shoot(Camera);
                }
            }
        }
    }

    public override void Shoot(Camera camera)
    {
        if (_ammoQuanity > 0 && _canShoot)
        {
            _shootParticle.Play();
            MouseLook.Shoot(_recoilForceXMin, _recoilForceYMin, _recoilForceXMax, _recoilForceYMax);
            ShootSound.Play();

            Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            ray.origin = camera.transform.position;

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.TryGetComponent(out PlayerHealth playerHealth))
                {
                    if (playerHealth.PhotonView.IsMine == false)
                    {
                        playerHealth.ApplyDamage(_damage, PhotonNetwork.LocalPlayer);
                        OnHit();
                    }
                }
                else
                {
                    PhotonView.RPC(nameof(ShootRpc), RpcTarget.All, hit.point, hit.normal);
                }
            }

            StartCoroutine(CountdownShoot());
            _ammoQuanity--;
            _maxAmmoQuanity--;

            if (_maxAmmoQuanity <= 0)
            {
                _ammoQuanity = _maxAmmo;
                _maxAmmoQuanity = _maxAmmoCount;
                _canShoot = true;
                OnEmptyAmmo();
            }
        }
        else
        {
            if (_ammoQuanity == 0 && _canShoot)
            {
                Reload();
                Debug.Log("Reloading");
            }
        }
    }
}
