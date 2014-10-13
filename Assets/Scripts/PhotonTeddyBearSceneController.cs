using UnityEngine;
using System.Collections;

public class PhotonTeddyBearSceneController : Photon.MonoBehaviour
{
	[SerializeField] private Transform playerPrefab;

	void Awake ()
	{
		PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity, 0);
	}
}
