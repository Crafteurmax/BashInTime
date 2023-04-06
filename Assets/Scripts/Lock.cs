using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class Lock : MonoBehaviour
{
    [SerializeField] ConditionsManager conditionsManager;
    public String conditionLockName;

    [SerializeField] private GameObject NumberPrefab;
    [SerializeField] private GameObject ArrowPrefab;
    [SerializeField] public int NumOfNum;
    public int[] actualCode;
    private GameObject[] codeVisu;
    public int trueCode;
    private float numberHeight = 58.98f;
    private float numberWidth = 30f;
    private float arrowWidth = 1.53f;
    private bool isInAnimation = false;
    // Start is called before the first frame update
    void Start()
    {
        actualCode = new int[NumOfNum];
        codeVisu = new GameObject[NumOfNum];
        Transform numbersObjectTransform = transform.GetChild(1);
        Transform arrowsObjectTransform = transform.GetChild(2);
        float NtotalWidth = (NumOfNum - 1) * numberWidth;
        float AtotalWidth = (NumOfNum - 1) * arrowWidth;// 2.295
        for (var i = 0; i < NumOfNum; i++)
        {
            codeVisu[NumOfNum - (i + 1)] = Instantiate(NumberPrefab, Vector3.zero, NumberPrefab.transform.rotation, numbersObjectTransform);
            codeVisu[NumOfNum - (i + 1)].transform.SetLocalPositionAndRotation(new Vector3(-256.4f, (numberWidth * i) - NtotalWidth / 2, 0f), codeVisu[NumOfNum - (i + 1)].transform.rotation);
            codeVisu[NumOfNum - (i + 1)].name = "N" + i;

            Instantiate(ArrowPrefab, Vector3.zero, transform.rotation, arrowsObjectTransform);
            arrowsObjectTransform.GetChild(i).transform.SetLocalPositionAndRotation(new Vector3((arrowWidth * i) - AtotalWidth / 2, 0, 0), transform.rotation);
            int blackMagicThingButVeryImportantPleaseDontDeletIt = i; // don't know why but if you just put i then it pass a reference instead of value even if it's an int, unusable for delegate function, took so much time to figure it out please kiiiiiilllllll me
            arrowsObjectTransform.GetChild(i).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { ChangeNumber(-1, blackMagicThingButVeryImportantPleaseDontDeletIt); });
            arrowsObjectTransform.GetChild(i).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { ChangeNumber(1, blackMagicThingButVeryImportantPleaseDontDeletIt); });
            arrowsObjectTransform.GetChild(i).name = "Arrow" + (NumOfNum - (i + 1));

        }
    }

    public void OnDisable()
    {
        if (isCodeValide()) conditionsManager.SetCondition(conditionLockName, true);
    }

    private void ChangeNumber(int delta, int numID)
    {
        if (isInAnimation) return;
        actualCode[numID] += delta;
        Vector3 pos = codeVisu[numID].transform.localPosition;
        Vector3 dest;
        if (actualCode[numID] <= 9 && actualCode[numID] >= 0)
        {
            dest = new Vector3(pos.x + (delta * numberHeight), pos.y, pos.z);
        }
        else if (actualCode[numID] > 9)
        {
            dest = new Vector3(pos.x + ((delta - 10) * numberHeight), pos.y, pos.z);
            pos = new Vector3(pos.x + ((delta - 11) * numberHeight), pos.y, pos.z);
        }
        else if (actualCode[numID] < 0)
        {
            dest = new Vector3(pos.x + ((delta + 10) * numberHeight), pos.y, pos.z);
            pos = new Vector3(pos.x + ((delta + 11) * numberHeight), pos.y, pos.z);
        }
        else dest = Vector3.zero;

        codeVisu[numID].transform.SetLocalPositionAndRotation(pos, codeVisu[numID].transform.localRotation);
        StartCoroutine(Translate(codeVisu[numID].transform, dest, 0.5f));
        actualCode[numID] = actualCode[numID] % 10;
        if (actualCode[numID] < 0) actualCode[numID] += 10;

    }

    private IEnumerator Translate(Transform transObj, Vector3 destination, float duration)
    {
        isInAnimation = true;
        float beginTime = Time.time;
        Vector3 depart = transObj.localPosition;
        while (Time.time < beginTime + duration)
        {
            transObj.SetLocalPositionAndRotation(Vector3.Lerp(depart, destination, Mathf.SmoothStep(0, 1, (Time.time - beginTime) / duration)), transObj.localRotation);
            yield return null;
        }
        transObj.SetLocalPositionAndRotation(destination, transObj.localRotation);
        isInAnimation = false;
    }

    private bool isCodeValide()
    {
        int displayedCode = 0;
        for (var i = 0; i < NumOfNum; i++)
        {
            displayedCode += actualCode[NumOfNum - (i + 1)] * ((int)Mathf.Pow(10, i));
        }
        return displayedCode == trueCode;
    }

    [ContextMenu("test")]
    public void test()
    {
        //ChangeNumber(-1, ref 1);
    }

    // Update is called once per frame
    void Update()
    {

    }
}