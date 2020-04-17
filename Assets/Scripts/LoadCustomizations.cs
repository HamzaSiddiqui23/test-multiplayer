using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LoadCustomizations : MonoBehaviourPunCallbacks
{
    public List<GameObject> HairModels;
    public List<GameObject> BeardModels;
    public GameObject Glasses;
    public GameObject bodyRenderer;
    Material shirt;
    Material pants;
    Material shoes;
    CharacterCustomizer cc;

    private void Awake()
    {
        if (!photonView.IsMine)
            return;
        cc = GameObject.FindGameObjectWithTag("Customization").GetComponent<CharacterCustomizer>();
        GetClothesMats();
        SetCustomizations();
    }

    void GetClothesMats()
    {
        if (!photonView.IsMine)
            return;
        var mats = bodyRenderer.GetComponent<Renderer>().materials;
        foreach (var i in mats)
        {
            if (i.name == "Shirt (Instance)")
            {
                shirt = i;
            }
            if (i.name == "Jeans (Instance)")
            {
                pants = i;
            }
            if (i.name == "Shoes (Instance)")
            {
                shoes = i;
            }
        }
    }

    void SetCustomizations()
    {
        if (!photonView.IsMine)
            return;
        shirt.SetColor("_BaseColor", cc.currentShirtColor);
        pants.SetColor("_BaseColor", cc.currentPantsColor);
        shoes.SetColor("_BaseColor", cc.currentShoesColor);
        HairModels[cc.currentHairModel].SetActive(true);
        BeardModels[cc.currentBeardModel].SetActive(true);
        UpdateHairColor();
        if (cc.showGlasses)
            Glasses.SetActive(true);
        else
            Glasses.SetActive(false);
    }

    public void UpdateHairColor()
    {
        if (!photonView.IsMine)
            return;
        if (cc.currentHairModel != HairModels.Count - 1)
            HairModels[cc.currentHairModel].GetComponent<Renderer>().material.SetColor("_BaseColor", cc.currentHairColor);
        if (cc.currentBeardModel != BeardModels.Count - 1)
            BeardModels[cc.currentBeardModel].GetComponent<Renderer>().material.SetColor("_BaseColor", cc.currentHairColor);
    }
}
