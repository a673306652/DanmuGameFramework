//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using Daan;
//using DanMuGame;

//public class BilibiliAnalyzerPanel : MonoBehaviour
//{
//    public Transform 基于总人数;
//    public Transform 基于参与人数;
//    public Transform 基于付费人数;

//    public BilibiliAnalyzerClient client;


//    void Update()
//    {
//        this.UpdateDetail(this.基于总人数, this.client.基于总人数);
//        this.UpdateDetail(this.基于参与人数, this.client.基于参与人数);
//        this.UpdateDetail(this.基于付费人数, this.client.基于付费人数);
//        this.UpdateBase();
//    }


//    void UpdateBase()
//    {
//        this.transform.Find<Text>("基础数据/总人数").text = $"总人数：{MyMath.Color.GetColorString(this.client.总人数.ToString(), Color.green)}";
//        this.transform.Find<Text>("基础数据/参与人数").text = $"参与人数：{MyMath.Color.GetColorString(this.client.参与人数.ToString(), Color.green)}";
//        this.transform.Find<Text>("基础数据/付费人数").text = $"付费人数：{MyMath.Color.GetColorString(this.client.付费人数.ToString(), Color.green)}";
//        this.transform.Find<Text>("基础数据/付费总额").text = $"付费总额：{MyMath.Color.GetColorString(this.client.付费总额.ToString(), Color.green)}电池";
//    }

//    void UpdateDetail(Transform root, BilibiliAnalyzerClient.Data data)
//    {
//        root.transform.Find<Text>("参与率").text = $"参与率：{MyMath.Color.GetColorString($"{(data.参与率 * 100)}%", Color.green)}";
//        root.transform.Find<Text>("付费率").text = $"付费率：{MyMath.Color.GetColorString($"{(data.付费率 * 100)}%", Color.green)}";
//        root.transform.Find<Text>("人均时长（平均值）").text = $"人均时长（平均值）：{MyMath.Color.GetColorString(data.人均时长.ToString(), Color.green)}分钟";
//        root.transform.Find<Text>("人均时长（中位数）").text = $"人均时长（中位数）：{MyMath.Color.GetColorString(data.人均时长_中位数.ToString(), Color.green)}分钟";
//        root.transform.Find<Text>("用户价值（LTV）").text = $"用户价值（LTV）：{MyMath.Color.GetColorString(data.用户价值.ToString(), Color.green)}电池";
//    }
//}
