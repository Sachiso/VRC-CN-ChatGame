using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRC_StarterKit.SampleAssets.Scripts
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(MeshFilter))]
    [ExecuteInEditMode]
    public class UICubeMockGeomAutoMesh : MonoBehaviour
    {
        public Color color = new Color(0.278f, 0.278f, 0.278f, 1f);
        public float depth = 14;
        public Mesh mesh;
        
        public Vector3[] debugCorners;

        private RectTransform rectTransform;
        
        [SerializeField][HideInInspector]
        private Rect currentRect;
        [SerializeField][HideInInspector]
        private float currentDepth;
        [SerializeField][HideInInspector]
        private Color currentColor;

#if UNITY_EDITOR
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            UpdateMesh();
        }

        protected internal void UpdateMesh()
        {
            if (!rectTransform) rectTransform = GetComponent<RectTransform>();
            if (!rectTransform) return;

            var rect = rectTransform.rect;
            if (mesh && rect.Equals(currentRect) && color.Equals(currentColor) && Math.Abs(depth - currentDepth) < 0.001) return;
            currentRect = rect;
            currentDepth = depth;
            currentColor = color;
            
            float posZ = rectTransform.anchoredPosition3D.z;
            Vector3[] corners = new Vector3[4];
            rectTransform.GetLocalCorners(corners);
            debugCorners = corners;
            // 1 2
            // 0 3

            var vertList = new List<Vector3>();
            // 表面
            for (int i = 0; i < corners.Length; i++)
            {
                vertList.Add(new Vector3(corners[i].x, corners[i].y, 0));
            }
            // 裏面
            for (int i = 0; i < corners.Length; i++)
            {
                vertList.Add(new Vector3(corners[i].x, corners[i].y, depth));
            }
            
            // UV 表面 裏面
            Vector2[] uvs = new Vector2[] { 
                new Vector2 (0f, 0f),
                new Vector2 (0f, 1f),
                new Vector2 (1f, 1f),
                new Vector2 (1f, 0f),
                new Vector2 (0f, 0f),
                new Vector2 (0f, 1f),
                new Vector2 (1f, 1f),
                new Vector2 (1f, 0f)
            };
            
            int[] triangles = new int[] {
                // 表面
                0, 1, 2,
                0, 2, 3,
                // 裏面
                2+4, 1+4, 0+4,
                3+4, 2+4, 0+4,
                // 01 左側面
                0, 0+4, 1,
                0+4, 1+4,1,
                // 12 上側面
                1, 1+4, 2,
                1+4, 2+4, 2,
                // 23 右側面
                2, 2+4, 3,
                2+4, 3+4, 3,
                // 30 下側面
                3, 3+4, 0,
                3+4, 0+4, 0,
            };
            // 中心座標
            Vector3 center = new Vector3((corners[0].x + corners[2].x)/2, (corners[0].y + corners[2].y)/2 , depth*0.5f);
            // サイズ
            Vector3 size = new Vector3(Math.Abs(corners[2].x - corners[1].x), Math.Abs(corners[0].y - corners[1].y), depth * 0.99f);
            var uvs3 = new List<Vector3>();
            var uvs4 = new List<Vector3>();
            var colors = new List<Color>();
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs3.Add(center);
                uvs4.Add(size);
                colors.Add(color.linear);
            }
            
            if (!mesh) mesh = new Mesh();
            mesh.Clear();
            mesh.SetVertices(vertList);
            mesh.SetUVs(0, uvs);
            mesh.SetUVs(2, uvs3); // 中心座標 => UV3
            mesh.SetUVs(3, uvs4); // サイズ => UV4
            mesh.SetColors(colors);
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            var meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;
        }
#endif
    }

#if UNITY_EDITOR && !COMPILER_UDONSHARP
    [CustomEditor(typeof(UICubeMockGeomAutoMesh))]
    public class UICubeMockGeomAutoMeshEditor : UnityEditor.Editor
    {
        private void OnValidate()
        {
            var self = (UICubeMockGeomAutoMesh)target;
            self.UpdateMesh();
        }

        public override void OnInspectorGUI()
        {
            var self = (UICubeMockGeomAutoMesh)target;
            self.UpdateMesh();
            
            serializedObject.Update();

            bool execSaveCurrentMesh = false;
            if (!AssetDatabase.Contains(self.mesh)) execSaveCurrentMesh = GUILayout.Button("Save current mesh");

            DrawDefaultInspector();
            
            if (execSaveCurrentMesh) SaveCurrentMesh();
            
            serializedObject.ApplyModifiedProperties();
        }

        private void SaveCurrentMesh()
        {
            var self = (UICubeMockGeomAutoMesh)target;
            string path = EditorUtility.SaveFilePanelInProject("Save current mesh", "mesh", "asset", "");
            if (path.Length > 0)
            {
                AssetDatabase.CreateAsset(self.mesh, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
#endif
}