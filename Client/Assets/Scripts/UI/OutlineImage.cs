using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutlineImage : Graphic
{
    public float outline = 10f;
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;
        vertex.position = new Vector3(-width / 2, -height / 2);
        vh.AddVert(vertex); 
        vertex.position = new Vector3(-width / 2, height/2);
        vh.AddVert(vertex);
        vertex.position = new Vector3(width/2, height/2);
        vh.AddVert(vertex);
        
        vertex.position = new Vector3(width / 2, -height / 2);
        vh.AddVert(vertex);
        

        float widthSqr = outline * outline;
        float distanceSqr = widthSqr / 2f;
        float distance = Mathf.Sqrt(distanceSqr);
        Vector3 centre = new Vector3(-width / 2, -height / 2);
        vertex.position = centre +new Vector3(distance,distance); vh.AddVert(vertex);
        vertex.position = centre+ new Vector3(distance, height - distance); vh.AddVert(vertex);
        vertex.position = centre+new Vector3(width - distance, height - distance); vh.AddVert(vertex);
        vertex.position = centre+new Vector3(width - distance, distance); vh.AddVert(vertex);

        vh.AddTriangle(0, 1, 5);
        vh.AddTriangle(5, 4, 0);

        vh.AddTriangle(1, 2, 6);
        vh.AddTriangle(6,5,1);

        vh.AddTriangle(2,3,7);
        vh.AddTriangle(7,6,2); 
        
        vh.AddTriangle(3,0,4);
        vh.AddTriangle(4,7,3);


    }
}
