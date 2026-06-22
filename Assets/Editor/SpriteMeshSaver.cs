//using System;
//using UnityEditor;
//using UnityEngine;

//public class SpriteMeshSaver : MonoBehaviour
//{
//    [MenuItem("Tools/Save Selected Sprite as Mesh")]
//    static void SaveSpriteMesh()
//    {
//        // Проверяем выбранный объект
//        GameObject go = Selection.activeGameObject;
//        if (go == null)
//        {
//            Debug.LogWarning("Выберите объект с SpriteRenderer!");
//            return;
//        }

//        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
//        if (sr == null || sr.sprite == null)
//        {
//            Debug.LogWarning("На объекте нет SpriteRenderer или Sprite!");
//            return;
//        }

//        Sprite sprite = sr.sprite;

//        // Создаём новый Mesh
//        Mesh mesh = new Mesh();
//        Vector3[] vertices3D = new Vector3[sprite.vertices.Length];

//        for (int i = 0; i < vertices3D.Length; i++)
//        {
//            vertices3D[i] = sprite.vertices[i];
//        }

//        mesh.vertices = vertices3D;
//        mesh.triangles = Array.ConvertAll(sprite.triangles, t => (int)t);
//        mesh.uv = sprite.uv;

//        mesh.RecalculateBounds();
//        mesh.RecalculateNormals();

//        // Сохраняем как asset
//        string path = "Assets/" + sprite.name + "_Mesh.asset";
//        AssetDatabase.CreateAsset(mesh, path);
//        AssetDatabase.SaveAssets();

//        Debug.Log("Mesh сохранён как asset: " + path);
//    }
//}
