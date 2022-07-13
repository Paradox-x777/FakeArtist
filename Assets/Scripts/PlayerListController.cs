using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListController : MonoBehaviour
{

    [Header("Players")]
    public string PlayerPrefabLocation;

    [Header("Components")]
    public PhotonView photonView;

    Dictionary<int,GameObject> PlayersInList = new Dictionary<int, GameObject>();
    private Color colorInactive = new Color(32, 99, 159);
    private Color colorActive = Color.red;

    void Start()
    {
        
    }

    [PunRPC]
    public void UpdateList()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerObj = PhotonNetwork.Instantiate(PlayerPrefabLocation, new Vector3(0, 0), Quaternion.identity, 0);
            var label = playerObj.transform.Find("Nickname").GetComponent<TextMeshProUGUI>();
            label.text = player.NickName;

            if (player.ActorNumber == GameManager.instance.ActivePlayer) { 
                var userPanel = playerObj.transform.GetComponent<Image>();
                userPanel.color = new Color(1f, 0.9f, 0.5f, 0.4f);
            }
            
            if(player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                label.color = Color.red;
            }

            playerObj.transform.parent = gameObject.transform;
        }
        
    }
}
