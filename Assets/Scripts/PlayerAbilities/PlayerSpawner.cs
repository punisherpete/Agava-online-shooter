using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace PlayerAbilities
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private float _cooldown;

        private void Start()
        {
            Spawn();
        }

        public void SpawnPlayer(PlayerHealth player)
        {
            Spawn(player);
            //StartCoroutine(SpawnWithCooldown(player));
        }

        private IEnumerator SpawnWithCooldown(PlayerHealth player)
        {
            yield return new WaitForSeconds(_cooldown);

            Spawn(player);
        }

        private void Spawn(PlayerHealth player)
        {
            int spawnId = Random.Range(0, _spawnPoints.Length - 1);
            player.transform.position = _spawnPoints[spawnId].position;
            player.EnableObject();
        }

        private void Spawn()
        {
            int spawnId = Random.Range(0, _spawnPoints.Length - 1);
            //_playerPrefab.GetComponent<PlayerInfo>().ToPlayer();
            PhotonNetwork.Instantiate(_playerPrefab.name,
                _spawnPoints[spawnId].position, Quaternion.identity, 0);
        }
    }
}
