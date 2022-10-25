using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider),typeof(PhotonView))]
public class GunSpawner : MonoBehaviourPun
{
    [SerializeField] private List<Gun> _guns;
    [SerializeField] private Transform _showPoint;
    [SerializeField] private float _timerToSpawn;

    private SphereCollider _collider;
    private Gun _newGun;
    private Vector3 _rotation = new Vector3(0, 20, 0);
    private bool _canUse = true;
    private PhotonView _photonView;

    private void Start()
    {
        _newGun = _guns[GetRandomIndex()];
        _newGun.gameObject.SetActive(true);
        _collider = GetComponent<SphereCollider>();
        _photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        _showPoint.Rotate(_rotation * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_canUse)
        {
            if (other.TryGetComponent(out PlayerHand playerHand))
            {
                playerHand.SetNewGun(_newGun.GunID);
                _photonView.RPC(nameof(DisableWeapon), RpcTarget.All);
            }
        }
    }

    [PunRPC]
    private void DisableWeapon()
    {
        _newGun.gameObject.SetActive(false);
        _canUse = false;
        _collider.enabled = false;
        StartCoroutine(CountdownToSpawn());
    }

    private IEnumerator CountdownToSpawn()
    {
        yield return new WaitForSeconds(_timerToSpawn);
        _newGun = _guns[GetRandomIndex()];
        _newGun.gameObject.SetActive(true);
        _collider.enabled = true;
        _canUse = true;
    }

    private int GetRandomIndex()
    {
        return Random.Range(0, _guns.Count);
    }
}
