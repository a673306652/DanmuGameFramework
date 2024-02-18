
using Chronos;
using Daan;
using System;
using UnityEngine;
public abstract class Bubble : PoolObject
{
    [HideInInspector]
    public RectTransform root;
    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public RectTransform bg;

    public float showTime;

    [SerializeField]
    protected Transform target;
    protected Vector3 followWorldPos;

    protected Action onCompleted;
    protected Timer timer;
    [HideInInspector]
    public bool isOpen = false;


    public bool worldToCanvas = true;

    public Timeline timeline;
    public bool mustInCanvas = false;

    public override void Init()
    {
        base.Init();
        this.timeline = this.GetComponentInChildren<Timeline>();
        this.root = this.transform as RectTransform;
        this.anim = this.GetComponentInChildren<Animator>();
        this.bg = this.transform.Find("背景").GetRectTransform();
    }

    protected abstract Vector2 GetSize();

    protected virtual void Update()
    {
        if (this.target != null)
        {
            if (!this.target.gameObject.activeInHierarchy)
            {
                this.target = null;
                this.Close();
            }
            else
            {
                this.followWorldPos = this.target.transform.position;
            }
        }
        if (this.timer != null && this.timeline != null)
        {
            this.timer.timeScale = this.timeline.timeScale;
        }
        if (this.worldToCanvas)
        {
            this.root.anchoredPosition = Camera.main.WorldToCanvasPoint(ResourceManager.Instance.canvas.GetRectTransform(), this.followWorldPos);
        }
        else
        {
            this.root.transform.position = this.followWorldPos;
        }
        this.bg.sizeDelta = this.GetSize();

        if (this.InCanvas(this.root.anchoredPosition) || this.mustInCanvas)
        {
            var d = Screen.width / 2f - Math.Abs(this.root.anchoredPosition.x);
            var d2 = d - this.bg.sizeDelta.x / 2;
            if (d2 < 0)
            {
                this.bg.anchoredPosition = this.bg.anchoredPosition.NewX(this.root.anchoredPosition.x > 0 ? d2 : -d2);
            }
            else
            {
                this.bg.anchoredPosition = this.bg.anchoredPosition.NewX(0);
            }
        }
        else
        {
            this.bg.anchoredPosition = this.bg.anchoredPosition.NewX(0);
        }
    }

    public bool InCanvas(Vector2 pos)
    {
        return new Rect(-Screen.width / 2F, -Screen.height / 2F, Screen.width, Screen.height).Contains(pos);
    }

    public void Follow(Transform tf)
    {
        if (null == tf) return;
        this.target = tf;
        this.followWorldPos = this.target.transform.position;
    }

    public void Follow(Vector3 worldPos)
    {
        this.followWorldPos = worldPos;
    }

    public void Open(float showTime = 3F, bool autoDespawn = true)
    {
        this.isOpen = true;
        this.anim.Play("Open");
        this.timer?.Cancel();
        this.showTime = showTime;
        this.timer = Timer.Register(this.showTime, () =>
         {
             this.Close();
         });

        if (autoDespawn)
        {
            if (this.onCompleted == null)
            {
                this.onCompleted = () =>
                {
                    ResourceManager.Instance.Despawn(this);
                };
            }
        }
        else
        {
            if (this.onCompleted != null) this.onCompleted = null;
        }
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        this.isOpen = false;
        this.timer?.Cancel();
    }



    public void Close()
    {
        if (!this.isOpen) return;
        this.isOpen = false;
        this.anim.Play("Close");
        this.timer?.Cancel();
        this.timer = Timer.Register(0.35F, () =>
       {
           this.onCompleted?.Invoke();
       });
    }

    public override void OnSpawn()
    {
        base.OnDespawn();
        this.transform.localScale = Vector3.zero;
        this.target = null;
        this.worldToCanvas = true;
    }

    public void OnCompleted(Action a)
    {
        this.onCompleted = a;
    }

}
