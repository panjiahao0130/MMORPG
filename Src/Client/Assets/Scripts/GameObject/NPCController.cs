using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Data;
using Managers;
using System;
using Models;

public class NPCController : MonoBehaviour {
    public int npcId;

    SkinnedMeshRenderer render;
    Animator anim;
    Color orignColor;

    //是否处于互动状态
    private bool inInteractive = false;

    NpcDefine npc;

    

	void Start () {
        render = this.GetComponentInChildren<SkinnedMeshRenderer>();
        anim = this.gameObject.GetComponent<Animator>();
        orignColor = render.sharedMaterial.color;
        npc = NPCManager.Instance.GetNpcDefine(this.npcId);
        this.StartCoroutine(Actions());
        
	}
    

    IEnumerator Actions()
    {
        while (true)
        {
            if (inInteractive)
                yield return new WaitForSeconds(2f);
            else
                yield return new WaitForSeconds(UnityEngine.Random.Range(5f, 10f));
            this.Ralax();
        }
    }
    

    private void Ralax()
    {
        anim.SetTrigger("Relax");
    }

    void Interactive()
    {
        if (!inInteractive)
        {
            inInteractive = true;
            StartCoroutine(DoInteraction());
        }
    }

    IEnumerator DoInteraction()
    {
        yield return FaceToPlayer();
        if (NPCManager.Instance.Interactive(npc))
        {
            anim.SetTrigger("Talk");
        }
        yield return new WaitForSeconds(3f);
        inInteractive = false;
    }

    IEnumerator FaceToPlayer()
    {
        Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position - this.transform.position).normalized;
        while (Mathf.Abs(Vector3.Angle(this.gameObject.transform.forward, faceTo)) > 5)
        {
            this.gameObject.transform.forward = Vector3.Lerp(this.gameObject.transform.forward, faceTo, Time.deltaTime * 5f);
            yield return null;
        }
    }

    private void OnMouseDown()
    {
        Interactive();
    }

    private void OnMouseOver()
    {
        Highlight(true);
    }


    private void OnMouseEnter()
    {
        Highlight(true);
    }

    private void OnMouseExit()
    {
        Highlight(false);
    }

    void Highlight(bool highlight)
    {
        if (highlight)
        {
            if (render.sharedMaterial.color != Color.white)
                render.sharedMaterial.color = Color.white;
        }
        else
        {
            if (render.sharedMaterial.color != orignColor)
                render.sharedMaterial.color = orignColor;
        }
    }
}
