using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

public class MenuManager : MonoBehaviourPunCallbacks
{

    public InputField playerName;
    public Button PlayButton;
    public CharacterCustomizer cc;
    // Start is called before the first frame update
    void Start()
    {
        PlayButton.enabled = false;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnConnectedToMaster()
    {
        PlayButton.enabled = true;
    }

    public void Play()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOps = new RoomOptions();
        roomOps.IsVisible = true;
        roomOps.IsOpen = true;
        PhotonNetwork.NickName = playerName.text;
        string roomName = "Room" + Random.Range(0, 1000);
        PhotonNetwork.CreateRoom(roomName, roomOps);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(cc.currentHairColor.ToString());
        ExitGames.Client.Photon.Hashtable style = new ExitGames.Client.Photon.Hashtable();
        style.Add("hairColor",  "#" + ColorUtility.ToHtmlStringRGBA(cc.currentHairColor));
        style.Add("shirtColor", "#" + ColorUtility.ToHtmlStringRGBA(cc.currentShirtColor));
        style.Add("pantsColor", "#" + ColorUtility.ToHtmlStringRGBA(cc.currentPantsColor));
        style.Add("shoesColor", "#" + ColorUtility.ToHtmlStringRGBA(cc.currentShoesColor));
        style.Add("beardModel", cc.currentBeardModel);
        style.Add("hairModel", cc.currentHairModel);
        style.Add("Show Glasses", cc.showGlasses);
        PhotonNetwork.LocalPlayer.SetCustomProperties(style);
        PhotonNetwork.LoadLevel(1);
    }

}
