namespace Modules.WorldBuilder
{
    using System.Collections;
    using System.Collections.Generic;
    using NaughtyAttributes;
    using UnityEngine;
    // [ExecuteInEditMode]
    public class SimpleRect : MonoBehaviour
    {
        [Header("Preview")]
        [SerializeField] private bool _Preview;
        [SerializeField] private float _PreviewSize = 0.2f;
        [Header("Select Your Mesh")]
        [Tooltip("角模型")]
        [SerializeField] private Mesh _AngleMesh;
        [Tooltip("线模型")]
        [SerializeField] private Mesh _LineMesh;
        [Tooltip("UI材质")]
        [SerializeField] private Material _Material;
        [Tooltip("自动吸附体积对象")]
        [SerializeField] private GameObject _SizeRef;

        [Header("Set Key-Value")]
        [Range(0, 50)]
        [SerializeField] private float _Width;
        [Range(0, 50)]
        [SerializeField] private float _Height;

        [SerializeField] private Vector3 AngleScale = Vector3.one;
        [SerializeField] private Vector3 LineScale = Vector3.one;
        [SerializeField] private float AngleRotation;
        [SerializeField] private float LineRotation;
        public float Width { get { return _Width; } set { _Width = value; UpdatePos(); } }
        public float Height { get { return _Height; } set { _Height = value; UpdatePos(); } }
        public float LineSpace;

        private GameObject? RU;
        private GameObject? LU;
        private GameObject? RD;
        private GameObject? LD;
        private List<GameObject> ULine = new List<GameObject>();
        private List<GameObject> LLine = new List<GameObject>();
        private List<GameObject> DLine = new List<GameObject>();
        private List<GameObject> RLine = new List<GameObject>();
        private GameObject transParent;

        private void UpdatePos()
        {
            var centerPos = transform.position;
            var LUp = new Vector3(-_Width, 0, _Height);
            var RUp = new Vector3(_Width, 0, _Height);
            var LDp = new Vector3(-_Width, 0, -_Height);
            var RDp = new Vector3(_Width, 0, -_Height);

            if (LU == null)
            {
                return;
            }
            LU.transform.position = centerPos + LUp;
            RU.transform.position = centerPos + RUp;
            LD.transform.position = centerPos + LDp;
            RD.transform.position = centerPos + RDp;

            LD.transform.localScale = AngleScale;
            LU.transform.localScale = AngleScale;
            RU.transform.localScale = AngleScale;
            RD.transform.localScale = AngleScale;

            LD.transform.localEulerAngles = new Vector3(0, AngleRotation, 0);
            LU.transform.localEulerAngles = new Vector3(0, AngleRotation + 90, 0);
            RU.transform.localEulerAngles = new Vector3(0, AngleRotation + 180, 0);
            RD.transform.localEulerAngles = new Vector3(0, AngleRotation + 270, 0);

            for (int i = 0; i < LLine.Count; i++)
            {
                LLine[i].transform.localScale = LineScale;
                LLine[i].transform.localEulerAngles = new Vector3(0, LineRotation + 90, 0);
            }
            for (int i = 0; i < DLine.Count; i++)
            {
                DLine[i].transform.localScale = LineScale;
                DLine[i].transform.localEulerAngles = new Vector3(0, LineRotation, 0);
            }
            for (int i = 0; i < ULine.Count; i++)
            {
                ULine[i].transform.localScale = LineScale;
                ULine[i].transform.localEulerAngles = new Vector3(0, LineRotation, 0);
            }
            for (int i = 0; i < RLine.Count; i++)
            {
                RLine[i].transform.localScale = LineScale;
                RLine[i].transform.localEulerAngles = new Vector3(0, LineRotation + 90, 0);
            }
        }

        [Button(enabledMode: EButtonEnableMode.Always)]
        private void AutoSetAnchorAndSize()
        {
            if (null != _SizeRef)
            {
                Vector3 centerPos, size;
                RectUtils.GetCenterPos(_SizeRef, out centerPos, out size);
                _Width = size.x / 2f;
                _Height = size.z / 2f;
                var anchorP = centerPos.y - size.y / 2f;
                transform.position = new Vector3(centerPos.x, anchorP, centerPos.z);
            }
        }
        private GameObject InstantiateMesh(string name, Mesh m, Material mat, Transform parent)
        {
            var go = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));
            var mf = go.GetComponent<MeshFilter>();
            var mr = go.GetComponent<MeshRenderer>();
            go.transform.SetParent(parent, true);
            mf.mesh = m;
            mr.material = mat;
            return go;
        }

        [Button(enabledMode: EButtonEnableMode.Always)]
        private void CreatePreviewMesh()
        {
            ClearMesh();
            if (transParent != null)
            {
                DestroyImmediate(transParent.gameObject);
            }
            transParent = new GameObject("SR_New");
            transParent.transform.SetParent(transform);
            transParent.transform.localPosition = Vector3.zero;
            transParent.transform.localScale = Vector3.one;
            transParent.transform.localEulerAngles = Vector3.zero;

            var centerPos = transform.position;
            var LUp = new Vector3(-_Width, 0, _Height);
            var RUp = new Vector3(_Width, 0, _Height);
            var LDp = new Vector3(-_Width, 0, -_Height);
            var RDp = new Vector3(_Width, 0, -_Height);

            for (int i = 0; i < 4; i++)
            {
                var a = InstantiateMesh("Angle", _AngleMesh, _Material, transParent.transform);
                var w = (i - 1) % 2 == 0; //˫��false ����true
                var h = i >= 2;

                a.transform.position = centerPos + new Vector3(_Width * (w ? -1f : 1f), 0, _Height * (h ? -1 : 1));

                if (i == 0)
                {
                    RU = a;
                    a.name = "RU";
                }
                if (i == 1)
                {
                    LU = a;
                    a.name = "LU";
                }
                if (i == 2)
                {
                    RD = a;
                    a.name = "RD";
                }
                if (i == 3)
                {
                    LD = a;
                    a.name = "LD";
                }
            }

            if (null == _LineMesh)
            {
                UpdatePos();
                return;
            }

            var wCount = Mathf.RoundToInt(_Width * 2 / LineSpace);
            if (wCount <= 2)
            {
                var u = InstantiateMesh("Line", _LineMesh, _Material, transParent.transform);
                var d = InstantiateMesh("Line", _LineMesh, _Material, transParent.transform);
                u.transform.localEulerAngles = new Vector3(0, LineRotation, 0);
                d.transform.localEulerAngles = new Vector3(0, LineRotation, 0);
                u.transform.position = centerPos + new Vector3(0, 0, _Height);
                d.transform.position = centerPos + new Vector3(0, 0, -_Height);
                u.transform.localScale = LineScale;
                d.transform.localScale = LineScale;
                ULine.Add(u);
                DLine.Add(d);
            }
            else
            {
                for (int i = 1; i < wCount; i++)
                {
                    var u = InstantiateMesh("Line", _LineMesh, _Material, transParent.transform);
                    var d = InstantiateMesh("Line", _LineMesh, _Material, transParent.transform);
                    u.transform.localScale = LineScale;
                    d.transform.localScale = LineScale;
                    u.transform.localEulerAngles = new Vector3(0, LineRotation, 0);
                    d.transform.localEulerAngles = new Vector3(0, LineRotation, 0);
                    u.transform.position = centerPos + new Vector3(-Width + LineSpace * i, 0, _Height);
                    d.transform.position = centerPos + new Vector3(-Width + LineSpace * i, 0, -_Height);
                    ULine.Add(u);
                    DLine.Add(d);
                }
            }


            var hCount = Mathf.RoundToInt(_Height * 2 / LineSpace);

            if (hCount <= 2)
            {
                var l = InstantiateMesh("Line", _LineMesh, _Material, transParent.transform);
                var r = InstantiateMesh("Line", _LineMesh, _Material, transParent.transform);
                l.transform.localScale = LineScale;
                r.transform.localScale = LineScale;
                l.transform.localEulerAngles = new Vector3(0, LineRotation + 90, 0);
                r.transform.localEulerAngles = new Vector3(0, LineRotation + 90, 0);
                r.transform.position = centerPos + new Vector3(Width, 0, 0);
                l.transform.position = centerPos + new Vector3(-Width, 0, 0);
                LLine.Add(l);
                RLine.Add(r);
            }
            else
            {
                for (int i = 1; i < hCount; i++)
                {
                    var l = InstantiateMesh("Line", _LineMesh, _Material, transParent.transform);
                    var r = InstantiateMesh("Line", _LineMesh, _Material, transParent.transform);
                    l.transform.localScale = LineScale;
                    r.transform.localScale = LineScale;
                    l.transform.localEulerAngles = new Vector3(0, LineRotation + 90, 0);
                    r.transform.localEulerAngles = new Vector3(0, LineRotation + 90, 0);
                    r.transform.position = centerPos + new Vector3(Width, 0, -Height + LineSpace * i);
                    l.transform.position = centerPos + new Vector3(-Width, 0, -Height + LineSpace * i);
                    LLine.Add(l);
                    RLine.Add(r);
                }
            }

            UpdatePos();
        }

        [Button(enabledMode: EButtonEnableMode.Always)]
        private void ClearMesh()
        {
            if (LU == null) return;
            DestroyImmediate(RU);
            DestroyImmediate(LU);
            DestroyImmediate(RD);
            DestroyImmediate(LD);

            for (int i = 0; i < ULine.Count; i++)
            {
                DestroyImmediate(ULine[i].gameObject);
            }
            for (int i = 0; i < RLine.Count; i++)
            {
                DestroyImmediate(RLine[i].gameObject);
            }
            for (int i = 0; i < DLine.Count; i++)
            {
                DestroyImmediate(DLine[i].gameObject);
            }
            for (int i = 0; i < LLine.Count; i++)
            {
                DestroyImmediate(LLine[i].gameObject);
            }

            ULine.Clear();
            RLine.Clear();
            DLine.Clear();
            LLine.Clear();
            if (transParent != null)
            {
                DestroyImmediate(transParent.gameObject);
            }
        }

        [Button(enabledMode: EButtonEnableMode.Always)]
        private void SyncToRectAnim()
        {
            var sra = GetComponent<SimpleRect_Anim>();
            if (null == sra)
            {
                sra = gameObject.AddComponent<SimpleRect_Anim>();
            }
            sra.Width = _Width;
            sra.Height = _Height;
        }

        private Vector3 TransWorld2Object(Vector3 targetPos, Vector3 centerPos)
        {
            return targetPos - centerPos;
        }


#if UNITY_EDITOR
        void OnValidate()
        {
            // UpdatePos();
        }
        void OnDrawGizmos()
        {
            if (_Preview)
            {
                var centerPos = transform.position;

                var RU = new Vector3(_Width, 0, _Height);
                var LU = new Vector3(-_Width, 0, _Height);
                var RD = new Vector3(_Width, 0, -_Height);
                var LD = new Vector3(-_Width, 0, -_Height);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(centerPos + RU, _PreviewSize);
                Gizmos.DrawSphere(centerPos + LU, _PreviewSize);
                Gizmos.DrawSphere(centerPos + RD, _PreviewSize);
                Gizmos.DrawSphere(centerPos + LD, _PreviewSize);

                Gizmos.DrawLine(centerPos + RU, centerPos + LU);
                Gizmos.DrawLine(centerPos + LU, centerPos + LD);
                Gizmos.DrawLine(centerPos + LD, centerPos + RD);
                Gizmos.DrawLine(centerPos + RD, centerPos + RU);
            }
        }
#endif
    }
}