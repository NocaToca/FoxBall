using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class LineDrawer : MonoBehaviour
{
    public float lineDecayRate = .35f;

    List<GameObject> babyObjects = new List<GameObject>();

    List<List<Vector3>> positions = new List<List<Vector3>>();
    List<List<Vector2>> edgePositions = new List<List<Vector2>>();
    LineRenderer lr;
    EdgeCollider2D edgey;

    float drawTime = 1.0f;
    float timeSinceLastDraw;

    int currentLineRendererIndex;
    bool drawingLine;

    List<LineRenderer> lineRenderers = new List<LineRenderer>();
    List<EdgeCollider2D> edgeColliders = new List<EdgeCollider2D>();
    List<float> clearTime = new List<float>();
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        edgey = GetComponent<EdgeCollider2D>();

        edgeColliders.Add(edgey);
        lineRenderers.Add(lr);

        positions.Add(new List<Vector3>());
        edgePositions.Add(new List<Vector2>());

        clearTime.Add(0.0f);

        //positions.Add(GameObject.FindWithTag("Player").transform.position);
        //positions.Add(transform.position);

        //lr.SetPositions(positions.ToArray());

        currentLineRendererIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < clearTime.Count; i++){
            clearTime[i] += Time.deltaTime;
        }
        

        DecayLines();
        

        if(Input.GetMouseButton(0)){
            if(timeSinceLastDraw > drawTime){
                
                timeSinceLastDraw = 0.0f;
            }
            DrawLine(currentLineRendererIndex);
            drawingLine = true;
        } else {
            if(drawingLine){
                CalculateNewCurrentLineRendererIndex();
                drawingLine = false;
            }
        }

        timeSinceLastDraw = Time.deltaTime;
    }

    void DecayLines(){
        for(int i = 0; i < positions.Count; i++){
            if(positions[i].Count == 0){
                clearTime[i] = 0.0f;
                edgeColliders[i].enabled = false;
            } else {
                if(clearTime[i] > lineDecayRate){
                    positions[i].RemoveAt(0);
                    DrawLineDontAdd(i);
                }
                edgeColliders[i].enabled = true;
            }
        }
    }

    void DrawLine(int index){
        float z = Camera.main.WorldToScreenPoint(GameObject.FindWithTag("Player").transform.position).z;

        Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1.0f));
        currentMousePos.z += 2.0f;

        edgePositions[index].Add(new Vector2(currentMousePos.x, currentMousePos.y));
        

        if(!positions[index].Contains(currentMousePos)){
            positions[index].Add(currentMousePos);
        } else {
            return;
        }

        lineRenderers[index].positionCount = positions[index].Count;
        lineRenderers[index].SetPositions(positions[index].ToArray());
        lineRenderers[index].Simplify(.03f);
        Vector3[] pos = new Vector3[lineRenderers[index].positionCount];
        lineRenderers[index].GetPositions(pos);
        Vector2[] posDownCast = new Vector2[pos.Length];

        for(int i = 0; i < pos.Length; i++){
            posDownCast[i] = new Vector2(pos[i].x, pos[i].y);
        }

        edgeColliders[index].points = posDownCast;
        
        //lr.Simplify(1.0f);
        //Debug.Log(currentMousePos);
    }

    void DrawLineDontAdd(int index){
        lineRenderers[index].positionCount = positions[index].Count;
        lineRenderers[index].SetPositions(positions[index].ToArray());
        lineRenderers[index].Simplify(.03f);
        Vector3[] pos = new Vector3[lineRenderers[index].positionCount];
        lineRenderers[index].GetPositions(pos);
        Vector2[] posDownCast = new Vector2[pos.Length];

        for(int i = 0; i < pos.Length; i++){
            posDownCast[i] = new Vector2(pos[i].x, pos[i].y);
        }

        edgeColliders[index].points = posDownCast;
    }

    void CalculateNewCurrentLineRendererIndex(){
        bool create = true;
        for(int i = 0; i < positions.Count; i++){
            if(positions[i].Count == 0){
                currentLineRendererIndex = i;
                create = false;
                break;
            }
        }
        if(create){
            CreateNewLine();
        }
        
    }

    void CreateNewLine(){
        GameObject baby = new GameObject();
        baby.transform.parent = this.gameObject.transform;

        //We need to make a new line renderer
        LineRenderer newLr = baby.AddComponent<LineRenderer>();
        newLr.startWidth = lr.startWidth;
        newLr.widthCurve = lr.widthCurve;

        newLr.material = lr.material;

        EdgeCollider2D edge2D = baby.AddComponent<EdgeCollider2D>();
        //edge2D.offset = edgey.offset;

        edgeColliders.Add(edge2D);
        lineRenderers.Add(newLr);

        clearTime.Add(0.0f);

        positions.Add(new List<Vector3>());
        edgePositions.Add(new List<Vector2>());

        currentLineRendererIndex = positions.Count - 1;

        babyObjects.Add(baby);
    }
}
