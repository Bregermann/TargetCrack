using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EndlessCarChase;
public class ObjectFade : MonoBehaviour
{

    public enum FadeType
    {
        Single,
        Whole,
        Parent,
        Children,
        Multiple
    }

    public FadeType fadeType;
    [Header("Custom Alpha Fade")]
    public bool customFade;
    [Range(0.0f, 1.0f)]
    public float fadeTo;
    [Header("Multiple Fade Type")]
    public GameObject[] otherObjects;
    public bool checkParentObject;
    //[HideInInspector]
    public bool faded;
    public FadeCheck fadeCheck;
    Renderer objectRenderer;
    GameObject highestParent;
    static ECCGameController gameController;
    
    // Start is called before the first frame update
    void Start()
    {
        if (gameController == null) gameController = GameObject.FindObjectOfType<ECCGameController>();
        
        //fadeCheck = GameObject.Find("Player").GetComponent<FadeCheck>();
        objectRenderer = GetComponent<Renderer>();
        highestParent = gameObject;
        while (highestParent.transform.parent != null)
        {
            highestParent = highestParent.transform.parent.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeCheck == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            if (obj != null)
                fadeCheck = obj.GetComponent<FadeCheck>();
            //fadeCheck = gameController.playerObject.gameObject.GetComponent<FadeCheck>();
            return;
        }

        if (fadeType == FadeType.Single)
        {
            if (fadeCheck.objectHit == gameObject)
            {
                if (!faded)
                {
                    foreach (Material material in objectRenderer.materials)
                    {
                        MaterialObjectFade.MakeFade(material);
                        Color newColor = material.color;
                        if (customFade) newColor.a = fadeTo;
                        else newColor.a = fadeCheck.fadeTo;
                        material.color = newColor;
                    }
                    faded = true;
                }
            }
            else
            {
                if (faded)
                {
                    foreach (Material material in objectRenderer.materials)
                    {
                        MaterialObjectFade.MakeOpaque(material);
                        Color newColor = material.color;
                        newColor.a = 1;
                        material.color = newColor;
                    }
                    faded = false;
                }
            }
        }
        
        if (fadeType == FadeType.Children)
        {
            if (fadeCheck.objectHit == gameObject)
            {
                if (!faded)
                {
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        foreach (Material material in transform.GetChild(i).GetComponent<Renderer>().materials)
                        {
                            MaterialObjectFade.MakeFade(material);
                            Color newColor = material.color;
                            if (customFade) newColor.a = fadeTo;
                            else newColor.a = fadeCheck.fadeTo;
                            material.color = newColor;
                        }
                    }
                    faded = true;
                }
            }
            else
            {
                if (faded)
                {
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        foreach (Material material in transform.GetChild(i).GetComponent<Renderer>().materials)
                        {
                            MaterialObjectFade.MakeOpaque(material);
                            Color newColor = material.color;
                            newColor.a = 1;
                            material.color = newColor;
                        }
                    }
                    faded = false;
                }
            }
        }
        
        if (fadeType == FadeType.Whole)
        {
            if (fadeCheck.parentObjectHit == highestParent)
            {
                if (!faded)
                {
                    GameObject findParent;
                    if (transform.parent != null) findParent = transform.parent.gameObject;
                    else findParent = null;
                    while (findParent != null)
                    {
                        if (findParent.GetComponent<Renderer>() != null)
                        {
                            foreach (Material material in findParent.GetComponent<Renderer>().materials)
                            {
                                MaterialObjectFade.MakeFade(material);
                                Color newColor = material.color;
                                if (customFade) newColor.a = fadeTo;
                                else newColor.a = fadeCheck.fadeTo;
                                material.color = newColor;
                            }
                        }
                        for (int i = 0; i < findParent.transform.childCount; i++)
                        {
                            foreach (Material material in findParent.transform.GetChild(i).GetComponent<Renderer>().materials)
                            {
                                MaterialObjectFade.MakeFade(material);
                                Color newColor = material.color;
                                if (customFade) newColor.a = fadeTo;
                                else newColor.a = fadeCheck.fadeTo;
                                material.color = newColor;
                            }
                        }
                        if (findParent.transform.parent != null)
                        {
                            findParent = findParent.transform.parent.gameObject;
                            if (findParent.GetComponent<Renderer>() == null) findParent = null;
                        }
                        else findParent = null;
                    }
                    faded = true;
                }
            }
            else
            {
                if (faded)
                {
                    GameObject findParent;
                    if (transform.parent != null) findParent = transform.parent.gameObject;
                    else findParent = null;
                    while (findParent != null)
                    {
                        if (findParent.GetComponent<Renderer>() != null)
                        {
                            foreach (Material material in findParent.GetComponent<Renderer>().materials)
                            {
                                MaterialObjectFade.MakeOpaque(material);
                                Color newColor = material.color;
                                newColor.a = 1f;
                                material.color = newColor;
                            }
                        }
                        for (int i = 0; i < findParent.transform.childCount; i++)
                        {
                            foreach (Material material in findParent.transform.GetChild(i).GetComponent<Renderer>().materials)
                            {
                                MaterialObjectFade.MakeOpaque(material);
                                Color newColor = material.color;
                                newColor.a = 1f;
                                material.color = newColor;
                            }
                        }
                        if (findParent.transform.parent != null)
                        {
                            findParent = findParent.transform.parent.gameObject;
                            if (findParent.GetComponent<Renderer>() == null) findParent = null;
                        }
                        else findParent = null;
                    }
                    faded = false;
                }
            }
        }

        if (fadeType == FadeType.Parent)
        {
            if (fadeCheck.parentObjectHit == highestParent)
            {
                if (!faded)
                {
                    foreach (Material material in objectRenderer.materials)
                    {
                        MaterialObjectFade.MakeFade(material);
                        Color newColor = material.color;
                        if (customFade) newColor.a = fadeTo;
                        else newColor.a = fadeCheck.fadeTo;
                        material.color = newColor;
                    }
                    foreach (Material material in transform.parent.GetComponent<Renderer>().materials)
                    {
                        MaterialObjectFade.MakeFade(material);
                        Color newColor = material.color;
                        if (customFade) newColor.a = fadeTo;
                        else newColor.a = fadeCheck.fadeTo;
                        material.color = newColor;
                    }
                    faded = true;
                }
            }
            else
            {
                if (faded)
                {
                    foreach (Material material in objectRenderer.materials)
                    {
                        MaterialObjectFade.MakeOpaque(material);
                        Color newColor = material.color;
                        newColor.a = 1;
                        material.color = newColor;
                    }
                    foreach (Material material in transform.parent.GetComponent<Renderer>().materials)
                    {
                        MaterialObjectFade.MakeOpaque(material);
                        Color newColor = material.color;
                        newColor.a = 1;
                        material.color = newColor;
                    }
                    faded = false;
                }
            }
        }

        if (fadeType == FadeType.Multiple)
        {
            if (!checkParentObject)
            {
                if (fadeCheck.objectHit == gameObject)
                {
                    foreach (Material material in objectRenderer.materials)
                    {
                        MaterialObjectFade.MakeFade(material);
                        Color newColor = material.color;
                        if (customFade) newColor.a = fadeTo;
                        else newColor.a = fadeCheck.fadeTo;
                        material.color = newColor;
                    }
                    for (int i = 0; i < otherObjects.Length; i++)
                    {
                        foreach (Material material in otherObjects[i].GetComponent<Renderer>().materials)
                        {
                            MaterialObjectFade.MakeFade(material);
                            Color newColor = material.color;
                            if (customFade) newColor.a = fadeTo;
                            else newColor.a = fadeCheck.fadeTo;
                            material.color = newColor;
                        }
                    }
                    faded = true;
                    
                }
                else
                {
                    if (faded)
                    {
                        foreach (Material material in objectRenderer.materials)
                        {
                            MaterialObjectFade.MakeOpaque(material);
                            Color newColor = material.color;
                            newColor.a = 1;
                            material.color = newColor;
                        }
                        for (int i = 0; i < otherObjects.Length; i++)
                        {
                            if (otherObjects[i].GetComponent<ObjectFade>() == null)
                            {
                                foreach (Material material in otherObjects[i].GetComponent<Renderer>().materials)
                                {
                                    MaterialObjectFade.MakeOpaque(material);
                                    Color newColor = material.color;
                                    newColor.a = 1;
                                    material.color = newColor;
                                }
                            }
                            else otherObjects[i].GetComponent<ObjectFade>().faded = true;
                        }
                        faded = false;
                    }
                }
            }
        

            if (checkParentObject)
            {
                if (fadeCheck.parentObjectHit == highestParent)
                {
                    foreach (Material material in objectRenderer.materials)
                    {
                        MaterialObjectFade.MakeFade(material);
                        Color newColor = material.color;
                        if (customFade) newColor.a = fadeTo;
                        else newColor.a = fadeCheck.fadeTo;
                        material.color = newColor;
                    }
                    for (int i = 0; i < otherObjects.Length; i++)
                    {
                        foreach (Material material in otherObjects[i].GetComponent<Renderer>().materials)
                        {
                            MaterialObjectFade.MakeFade(material);
                            Color newColor = material.color;
                            if (customFade) newColor.a = fadeTo;
                            else newColor.a = fadeCheck.fadeTo;
                            material.color = newColor;
                        }
                    }
                    faded = true;
                    
                }
                else
                {
                    if (faded)
                    {
                        foreach (Material material in objectRenderer.materials)
                        {
                            MaterialObjectFade.MakeOpaque(material);
                            Color newColor = material.color;
                            newColor.a = 1;
                            material.color = newColor;
                        }
                        for (int i = 0; i < otherObjects.Length; i++)
                        {
                            if (otherObjects[i].GetComponent<ObjectFade>() == null)
                            {
                                foreach (Material material in otherObjects[i].GetComponent<Renderer>().materials)
                                {
                                    MaterialObjectFade.MakeOpaque(material);
                                    Color newColor = material.color;
                                    newColor.a = 1;
                                    material.color = newColor;
                                }
                            }
                            else otherObjects[i].GetComponent<ObjectFade>().faded = true;
                        }
                        faded = false;
                    }
                }

            }
        }
    }
}
