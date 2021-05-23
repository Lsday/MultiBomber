using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Linq;

public class BlenderAssetProcessor : AssetPostprocessor
{

    public void OnPostprocessModel(GameObject obj)
    {

        //only perform corrections with blender files
        if (obj.GetComponent<Animation>() != null) return;

        ModelImporter importer = assetImporter as ModelImporter;
        if (Path.GetExtension(importer.assetPath) == ".blend")
        {
            // only processe files with the _RT suffix
            if (importer.assetPath.Contains("_RT"))
            {
                RotateObject(obj.transform);

                //Don't know why we need this...
                //Fixes wrong parent rotation
                obj.transform.rotation = Quaternion.identity;
            }
        }

    }

    //recursively rotate a object tree individualy
    private void RotateObject(Transform obj)
    {
        Vector3 objRotation = obj.eulerAngles;
        objRotation.x += 90f;
        obj.eulerAngles = objRotation;

        //if a meshFilter is attached, we rotate the vertex mesh data
        MeshFilter meshFilter = obj.GetComponent(typeof(MeshFilter)) as MeshFilter;
        if (meshFilter)
        {
            RotateMesh(meshFilter.sharedMesh);
        }

        //do this too for all our children
        //Casting is done to get rid of implicit downcast errors
        foreach (Transform child in obj)
        {
            RotateObject(child);
        }
    }

    //"rotate" the mesh data
    private void RotateMesh(Mesh mesh)
    {
        //switch all vertex z values with y values
        Vector3[] vertices = mesh.vertices;
        for (int index = 0; index < vertices.Length; index++)
        {
            vertices[index] = new Vector3(-vertices[index].x, vertices[index].z, vertices[index].y);
        }
        mesh.vertices = vertices;

        //recalculate other relevant mesh data
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}