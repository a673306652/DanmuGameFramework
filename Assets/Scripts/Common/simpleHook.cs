using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class simpleHook : MonoBehaviour
{
    [SerializeField] private float hookSpeed;
    [SerializeField] private float playerSpeed; 
    public Transform pPoint;
    private Vector3 p1;
    private Vector3 p2;
    public LineRenderer lr;
    public PlayerController2p5D player;
    public static simpleHook instance;
    public bool onHook;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        player = PlayerController2p5D.instance;
    }

    public void hook2Target(Vector3 point)
    {
        lr.gameObject.SetActive(true);
        StartCoroutine(posGo(point));
    }


    private IEnumerator posGo(Vector3 target)
    {
        onHook = true;
        player.DisableInput();
        p2 = p1;
        var t = 0f;
     
        while (t<1f)
        {
            p2 = Vector3.Lerp(p1,target,t);
            t += Time.deltaTime * hookSpeed;
            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(playerGo(target));
    }

    private IEnumerator playerGo(Vector3 target)
    {
        PlayerController2p5D.instance.sfx.VFXGo();
        var t = 0f;
        var tempPos = PlayerController2p5D.instance.transform.position;
        while (t<1f)
        {
            var playerPos = PlayerController2p5D.instance.transform.position;

            player.rb.isKinematic = true;
            
            PlayerController2p5D.instance.transform.position = Vector3.Lerp(tempPos, p2, t);
            
            t += Time.deltaTime * playerSpeed;
            yield return new WaitForEndOfFrame();
        }
        player.EnableInput();
        player.rb.isKinematic = false;
        onHook = false;
    }
    private void Update()
    {
        p1 = pPoint.position;
        if (!onHook)
        {
            p2 = p1;
        }
        lr.SetPositions(new Vector3[2]{p1,p2});
    }
}
