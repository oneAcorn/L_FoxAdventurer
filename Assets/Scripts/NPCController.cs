using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NPCController : MonoBehaviour
{
    [Header("对话按钮")] public GameObject dialogBtnPrefab;
    [Header("对话按钮位置")] public Transform dialogBtnPos;
    [Header("对话文本")] public TextAsset dialogFile;
    [Space] public Image headerImg;
    public Text txt;
    [Space] public Sprite playerHeader;
    public Sprite npcHeader;


    private GameObject dialogBtn;

    private Canvas canvas;
    private int txtIndex;

    private List<String> txtList = new List<string>();

    //正在显示中的文字
    private string curShowingTxt;

    private void Start()
    {
        GetTextFromFile(dialogFile);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) //玩家来了
        {
            if (dialogBtn == null)
            {
                EnsureCanvas();
                dialogBtn = Instantiate(dialogBtnPrefab, canvas.transform);
                dialogBtn.transform.position = dialogBtnPos.position;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) //玩家走了
        {
            if (dialogBtn != null)
            {
                Destroy(dialogBtn);
                dialogBtn = null;
            }
            headerImg.transform.parent.gameObject.SetActive(false);
            txtIndex = 0;
            FindObjectOfType<PlayerController>().enabled = true;
        }
    }

    private void Update()
    {
        Dialog();
    }


    private void Dialog()
    {
        if (dialogBtn != null && Input.GetKeyDown(KeyCode.F) || (txtIndex > 0 && Input.GetKeyDown(KeyCode.Space)))
        {
            if (!string.IsNullOrEmpty(curShowingTxt)) //当前文本正在逐字播放中
            {
                StopAllCoroutines();
                txt.text = curShowingTxt;
                curShowingTxt = null;
                return;
            }

            // Debug.Log($"对话中:{txtIndex},{txtList.Count}");
            if (txtIndex >= txtList.Count)
            {
                headerImg.transform.parent.gameObject.SetActive(false);
                GameObject.FindObjectOfType<PlayerController>().enabled = true;
                txtIndex = 0;
                return;
            }

            if (txtIndex == 0)
            {
                headerImg.transform.parent.gameObject.SetActive(true);
                GameObject.FindObjectOfType<PlayerController>().enabled = false;
            }

            var headerSprite = txtList[txtIndex] == "P\r" ? playerHeader : npcHeader;
            txtIndex++;
            var dialogTxt = txtList[txtIndex];
            txtIndex++;
            headerImg.sprite = headerSprite;
            // txt.text = dialogTxt;
            // StopAllCoroutines();
            StartCoroutine(ShowTxt(dialogTxt));
            // Debug.Log($"txt {txtList[txtIndex]}");
        }
    }

    private IEnumerator ShowTxt(string txtStr)
    {
        txt.text = "";
        curShowingTxt = txtStr;
        foreach (var c in txtStr.ToCharArray())
        {
            txt.text += c;
            yield return new WaitForSeconds(0.06f);
        }

        curShowingTxt = null;
    }

    private void GetTextFromFile(TextAsset file)
    {
        txtList.Clear();
        txtIndex = 0;
        foreach (var line in file.text.Split('\n'))
        {
            if (!string.IsNullOrEmpty(line))
            {
                txtList.Add(line);
            }
        }
    }

    private void EnsureCanvas()
    {
        if (canvas != null) return;
        foreach (var c in FindObjectsOfType<Canvas>())
        {
            if (c.renderMode != RenderMode.WorldSpace) continue;
            canvas = c;
            break;
        }
    }
}