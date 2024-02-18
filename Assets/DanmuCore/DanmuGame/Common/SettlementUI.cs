using Daan;
using DanMuGame;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class SettlementUI : PoolObject
{

    public List<Sprite> sprites;

    public Image settlementImg;
    public Text desc;
    public List<SettlementItem> settlementItems;
    public int colseTime = 30;
    protected float colseTimer = 0;
    protected int index = 0;
    public delegate void GameOver();
    public GameOver gameOver;
    protected bool isGameOver = false;
    private void Awake()
    {
        this.colseTimer = colseTime;
    }
    public override void OnSpawn()
    {
        this.index = 0;
        this.settlementItems.ForEach(item => { item.gameObject.SetActive(false); });
    }


    public void Show(string iconName)
    {
        this.isGameOver = true;
        this.settlementImg.sprite = this.sprites.Find(item => item.name == iconName);
    }


    public void ShowSettlementItem(string name, string desc, float value)
    {
        if (this.index >= this.settlementItems.Count)
        {
            return;
        }
        var settlementItem = this.settlementItems[this.index];
        settlementItem.SetUp(name, desc, value);
        settlementItem.gameObject.SetActive(true);
        this.index++;
    }

    void Update()
    {
        if (this.isGameOver)
        {
            this.colseTimer -= Time.deltaTime;
            if (this.colseTimer <= 0)
            {
                if (gameOver != null)
                {
                    this.gameOver();
                    gameOver = null;
                }
                this.isGameOver = false;
                ResourceManager.Instance.Despawn(this);
                this.colseTimer = this.colseTime;
            }
        }
    }
}
