using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class VersionDisplay : MonoBehaviour
{
    [SerializeField] private Text _VersionTxt;

    void Awake()
    {
        if (_VersionTxt)
        {
            _VersionTxt.text = "版本：" + Application.version;
        }
    }
}