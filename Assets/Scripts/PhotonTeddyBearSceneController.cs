using UnityEngine;
using System.Collections;

public class PhotonTeddyBearSceneController : Photon.MonoBehaviour
{
	[SerializeField] private Transform playerPrefab;
	[SerializeField] private Transform bearPrefab;

	void Awake ()
	{
		PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity, 0);

		if (PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.Instantiate(bearPrefab.name, Vector3.zero, Quaternion.identity, 0);
		}
	}
}
