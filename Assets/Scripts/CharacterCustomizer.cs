using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomizer : MonoBehaviour
{
    public List<Color> availableColors;
    public List<GameObject> hairModels;
    public List<GameObject> beardModels;
    public GameObject glasses;

    public GameObject bodyRenderer;
    Material shirt;
    Material pants;
    Material shoes;

    public int currentHairModel;
    public int currentBeardModel;

    int totalHairCount;
    int totalBeardCount;

    public Color currentHairColor;
    public Color currentShirtColor;
    public Color currentPantsColor;
    public Color currentShoesColor;

    public bool showGlasses;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        currentHairModel = 0;
        currentBeardModel = 0;
        totalHairCount = hairModels.Count;
        totalBeardCount = hairModels.Count;
        showGlasses = true;
        currentHairColor = availableColors[0];
        GetClothesMats();
    }

    void GetClothesMats()
    {
        var mats = bodyRenderer.GetComponent<Renderer>().materials;
        foreach(var i in mats)
        {
            if(i.name == "Shirt (Instance)")
            {
                shirt = i;
            }
            if(i.name == "Jeans (Instance)")
            {
                pants = i;
            }
            if(i.name == "Shoes (Instance)")
            {
                shoes = i;
            }
        }
    }

    public void SetGlasses()
    {
        if(showGlasses)
        {
            showGlasses = false;
            glasses.SetActive(false);
        }
        else
        {
            showGlasses = true;
            glasses.SetActive(true);
        }
    }

    public void SetColorOfHair(int index)
    {
        currentHairColor = availableColors[index];
        UpdateHairColor();
    }

    public void SetColorOfShirt(int index)
    {
        shirt.SetColor("_BaseColor", availableColors[index]);
        currentShirtColor = availableColors[index];
    }

    public void SetColorOfJeans(int index)
    {
        pants.SetColor("_BaseColor", availableColors[index]);
        currentPantsColor = availableColors[index];
    }

    public void SetColorOfShoes(int index)
    {
        shoes.SetColor("_BaseColor", availableColors[index]);
        currentShoesColor = availableColors[index];
    }

    public void NextHair()
    {
        if(currentHairModel + 1 < totalHairCount)
        {
            hairModels[currentHairModel].SetActive(false);
            hairModels[currentHairModel + 1].SetActive(true);
            currentHairModel++;
            UpdateHairColor();
        }
        else
        {
            hairModels[currentHairModel].SetActive(false);
            hairModels[0].SetActive(true);
            currentHairModel = 0;
            UpdateHairColor();
        }
    }
    public void PreviousHair()
    {
        if (currentHairModel - 1 >= 0)
        {
            hairModels[currentHairModel].SetActive(false);
            hairModels[currentHairModel - 1].SetActive(true); ;
            currentHairModel--;
            UpdateHairColor();
        }
        else
        {
            hairModels[currentHairModel].SetActive(false);
            hairModels[totalHairCount - 1].SetActive(true);
            currentHairModel = totalHairCount - 1;
            UpdateHairColor();
        }
    }

    public void NextBeard()
    {
        if (currentBeardModel + 1 < totalBeardCount)
        {
            beardModels[currentBeardModel].SetActive(false);
            beardModels[currentBeardModel + 1].SetActive(true);
            currentBeardModel++;
            UpdateHairColor();
        }
        else
        {
            beardModels[currentBeardModel].SetActive(false);
            beardModels[0].SetActive(true);
            currentBeardModel = 0;
            UpdateHairColor();
        }
    }
    public void PreviousBeard()
    {
        if (currentBeardModel - 1 >= 0)
        {
            beardModels[currentBeardModel].SetActive(false);
            beardModels[currentBeardModel - 1].SetActive(true); ;
            currentBeardModel--;
            UpdateHairColor();
        }
        else
        {
            beardModels[currentBeardModel].SetActive(false);
            beardModels[totalBeardCount - 1].SetActive(true);
            currentBeardModel = totalBeardCount - 1;
            UpdateHairColor();
        }
    }

    public void UpdateHairColor()
    {
        if (currentHairModel != totalHairCount - 1)
            hairModels[currentHairModel].GetComponent<Renderer>().material.SetColor("_BaseColor", currentHairColor);
        if (currentBeardModel != totalBeardCount - 1)
            beardModels[currentBeardModel].GetComponent<Renderer>().material.SetColor("_BaseColor", currentHairColor);
    }
}
