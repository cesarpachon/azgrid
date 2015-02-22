
using UnityEngine;
using System.Collections;
using System.IO;

[ExecuteInEditMode]
public class AZQuadGrid : MonoBehaviour 
{
	
	public struct AZQuadCell
	{
		public int x;
		public int y;
		public Vector3 center;
	};
	
	
	public Material lineMaterial;
	public Material quadMaterial;
	
	public bool renderQuads=true;
	public bool renderLines=true;
	
	
	[HideInInspector] [SerializeField] int numRows=3;
	[ExposeProperty] 
	public int Rows
	{
		set{ numRows = Mathf.Clamp(value, 0, 100); rebuild();}
		get{ return numRows;}
	}
	
	[HideInInspector] [SerializeField] int numColumns=3;
	[ExposeProperty] 
	public int Columns
	{
		set{ numColumns = Mathf.Clamp(value, 0, 100); rebuild();}
		get{ return numColumns;}
	}
	
	[HideInInspector] [SerializeField] float CellWidth=1.0f;
	[ExposeProperty] 
	public float cellWidth
	{
		set{ CellWidth = Mathf.Abs(value); rebuild();}
		get{ return CellWidth;}
	}
		
	[HideInInspector] [SerializeField] float CellHeight=1.0f;
	[ExposeProperty] 
	public float cellHeight
	{
		set{ CellHeight = Mathf.Abs(value); rebuild();}
		get{ return CellHeight;}
	}
	
	/**
	 * line width is substracted to cellwidth, cellheight (takes space from the cell)
	 */
	[HideInInspector] [SerializeField] float LineWidth=0.1f;
	[ExposeProperty] 
	public float lineWidth
	{
		set{ LineWidth = Mathf.Clamp(value, 0.0f, Mathf.Min(CellWidth, CellHeight)); rebuild();}
		get{ return LineWidth;}
	}
	
	private bool dorebuild=false;
	
	private Mesh lineMesh;
	private Mesh quadMesh;
	
	private Vector3[] vertices;
	//private Vector3[] normals;
	//we need two uvsets, both of the same size(==vertices.size)
    private Vector2[] lineUvs;
    private Vector2[] quadUvs;
    private int[] lineTriangles;
    private int[] quadTriangles;
	
	
	private BoxCollider boxcollider;
	
	
	private AZQuadCell currCell;
	
	public void rebuild()
	{
		dorebuild =  true;
	}
	//--------------
	
	// Use this for initialization
	void Start () 
	{
		
		boxcollider = this.GetComponent<BoxCollider>();
		_rebuild();
	}
	
	
	// Update is called once per frame
	void Update () {
		
		if(dorebuild) _rebuild ();	
		

		if(renderLines)
			Graphics.DrawMesh(lineMesh, transform.localToWorldMatrix, lineMaterial, gameObject.layer); 
			//Graphics.DrawMesh(lineMesh, transform.position, transform.rotation,  lineMaterial, gameObject.layer);
		if(renderQuads)
			Graphics.DrawMesh(quadMesh, transform.localToWorldMatrix, quadMaterial, gameObject.layer); 
			//Graphics.DrawMesh(quadMesh, transform.position, transform.rotation,  quadMaterial, gameObject.layer);
	}
	//---------------------------
	
	
	private void _rebuild()
	{
		//StreamWriter writer = new StreamWriter("C:\\debug.txt");
		
		
		lineMesh  = new Mesh();
		quadMesh  = new Mesh();
		
		
		int numPoints = numRows*numColumns;
		int numCorners = (numRows+1)*(numColumns+1);
		int sizeVertexBuffer = numCorners * 16;//each point has 16 vertex
		int numLines = numRows+1+numColumns+1; //needed lines to draw the grid
		int sizeLineTriangleBuffer = numLines*2*3; //each line needs two triangles, each triangle three index
		int sizeQuadTriangleBuffer = numPoints*2*3; //each quad needs two triangles, each triangle three index
		
		
		vertices = new Vector3[sizeVertexBuffer];
		//normals = new Vector3[sizeVertexBuffer];
		quadUvs = new Vector2[sizeVertexBuffer];
		lineUvs =new Vector2[sizeVertexBuffer];
		lineTriangles = new int[sizeLineTriangleBuffer];
		quadTriangles = new int[sizeQuadTriangleBuffer];
		
		//mesh is centered in the parent transform
		
		//we create for each point in the grid (each cell corner) a QUADLET (a small square of vertex)
		//we need to iterate over an additional row and column, in order to create quadlets for the final
		//bottm and right edges.
		//LineWidth;
		float lineWidth2 = LineWidth/2.0f;
		
		float orix = -(CellWidth*numColumns)/2.0f; 
		float oriy = -(CellHeight*numRows)/2.0f;
		
		float posx = orix;
		float posy = oriy;
		
		int row, col;
		int i = 0;
		for(row = 0; row<numRows+1;row++)
		{
			posx = orix; 
			for(col = 0; col<numColumns+1; col++)
			{
				
				//writer.WriteLine("cell "+row+","+col+" i "+ i + " pos "+ posx + ","+posy);
				
				//posx, posy is the CENTER of the quadlet..ie
				
				//vertex 0
				vertices[i].x = posx-lineWidth2;
				vertices[i].y = 0.0f; 
				vertices[i].z = posy-lineWidth2;
				
				//writer.WriteLine((i)+": vertex 0: "+(posx-lineWidth2)+", "+ (posy-lineWidth2));
				i++;
				
				
				//vertex 1
				vertices[i].x = posx+lineWidth2;
				vertices[i].y = 0.0f; 
				vertices[i].z = posy-lineWidth2;
				//writer.WriteLine((i)+": vertex 1: "+(posx+lineWidth2)+", "+ (posy-lineWidth2));
				i++;
				
				//vertex 2
				vertices[i].x = posx+lineWidth2;
				vertices[i].y = 0.0f; 
				vertices[i].z = posy+lineWidth2;
				//writer.WriteLine((i)+": vertex 2: "+(posx+lineWidth2)+", "+ (posy+lineWidth2));
				i++;
				
				//vertex 3
				vertices[i].x = posx-lineWidth2;
				vertices[i].y = 0.0f; 
				vertices[i].z = posy+lineWidth2;
				//writer.WriteLine((i)+": vertex 3: "+(posx-lineWidth2)+", "+ (posy+lineWidth2));
				i++;
				

				//after
				posx+=CellWidth;	
			}
			posy +=CellHeight;
		}
		
		//init normals
		/*for(i=0;i<sizeVertexBuffer;i++)
		{
			normals[i].x= 0.0f;
			normals[i].y=1.0f;
			normals[i].z=0.0f;
		}*/
		
		
		/*NOW compute the triangle INDEXES for quads
		  each quad (cell) is formed by the local vertex 2, 7, 8, 13 
		  in two triangles: 2,7,8  and 2, 8, 13
		 */
		int idquad = 0;
		for(row = 0; row<numRows;row++)
		{
			for(col = 0; col<numColumns; col++)
			{
				
				//writer.WriteLine(" quad : "+col+", "+ row);
				
				quadTriangles[idquad++] = _getVertexIndex(row, col)+2;
				quadTriangles[idquad++] = _getVertexIndex(row+1, col+1);//+0;
				quadTriangles[idquad++] = _getVertexIndex(row, col+1)+3;
				
				//writer.WriteLine (_getVertexIndex(row, col)+","+_getVertexIndex(row, col+1)+","+_getVertexIndex(row+1, col+1));

				quadTriangles[idquad++] = _getVertexIndex(row, col)+2;
				quadTriangles[idquad++] = _getVertexIndex(row+1, col)+1;
				quadTriangles[idquad++] = _getVertexIndex(row+1, col+1);//+0;

				//writer.WriteLine (_getVertexIndex(row, col)+","+_getVertexIndex(row+1, col+1)+","+_getVertexIndex(row+1, col));

			}
		}
		
		
		//now, build the triangle index for the lines 
		int idline=0;
		for(row = 0; row<=numRows;row++)
		{
			//writer.WriteLine(" line row : "+row);
			
			lineTriangles[idline++] = _getVertexIndex(row, 0);
			lineTriangles[idline++] = _getVertexIndex(row, numColumns)+2;
			lineTriangles[idline++] = _getVertexIndex(row, numColumns)+1;
			
			//writer.WriteLine(" "+_getVertexIndex(row, 0)+ " to "+ _getVertexIndex(row, numColumns));

			
			lineTriangles[idline++] = _getVertexIndex(row, 0);
			lineTriangles[idline++] = _getVertexIndex(row, 0)+3;
			lineTriangles[idline++] = _getVertexIndex(row, numColumns)+2;
			//writer.WriteLine(" "+_getVertexIndex(row, 0)+ " to "+ _getVertexIndex(row, numColumns));

		}
		
		//idline=0;
		for(col = 0; col<=numColumns;col++)
		{
			//writer.WriteLine(" line col : "+col);
			
			lineTriangles[idline++] = _getVertexIndex(0,col);
			lineTriangles[idline++] = _getVertexIndex(numRows,col)+2;
			lineTriangles[idline++] = _getVertexIndex(0,col)+1;
			
			//writer.WriteLine(" "+_getVertexIndex(0,col)+ " to "+ _getVertexIndex(numRows,col));

			
			lineTriangles[idline++] = _getVertexIndex(0,col);
			lineTriangles[idline++] = _getVertexIndex(numRows,col)+3;
			lineTriangles[idline++] = _getVertexIndex(numRows,col)+2;
			//writer.WriteLine(" "+_getVertexIndex(0,col)+ " to "+ _getVertexIndex(numRows,col));

		}
		
		
		/*quadUvs = new Vector2[sizeVertexBuffer];
		lineUvs =new Vector2[sizeVertexBuffer];
		lineTriangles = new int[sizeLineTriangleBuffer];
		*/
		
		
		lineMesh.vertices = vertices;
		quadMesh.vertices = vertices;
		
		//lineMesh.normals = normals;
		//quadMesh.normals = normals;
		lineMesh.RecalculateNormals();
		quadMesh.RecalculateNormals();
        
		lineMesh.uv = lineUvs;
		quadMesh.uv = quadUvs;
		
		lineMesh.triangles = lineTriangles;
		quadMesh.triangles = quadTriangles;
		
		//writer.Close();
		
		//update collider
		if(boxcollider != null)
		{
			Vector3 collsize = new Vector3();
			collsize.x = cellWidth*numColumns;
			collsize.y = 0.1f;
			collsize.z = cellHeight*numRows;
			boxcollider.size = collsize; 
		}
		
		dorebuild = false;
	}
	//----------------------
	
	private int _getVertexIndex(int row, int col)
	{
		return (row*(numColumns+1)*4) + ((col)*4);
	}
	//----------------------
	
	public void DrawGizmo()
	{
		foreach(Vector3 point  in vertices)
		{
			Gizmos.DrawLine(Vector3.zero, point);
		}
	}
	//------------------------
	
	public void OnMouseDown() 
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		Plane plane = new Plane(transform.up, transform.position);
		
		float distance = 0;
		if(plane.Raycast(ray, out distance))
		{
			
			Vector3 point = ray.GetPoint(distance);
			//Debug.DrawLine(ray.origin, point);
			//compute the selected cell
			//point = transform.worldToLocalMatrix*point;
			point = transform.InverseTransformPoint(point);
			
			float width = cellWidth*numColumns; 
			float width2 = width*0.5f;
			float height = cellHeight*numRows;
			float height2 = height*0.5f;

			Debug.Log ("point: " + point.ToString() + " w "+width + " w2 "+ width2 + " h "+ height + " h2 "+ height2);
			Debug.Log("div "+ ((point.x+width2) / cellWidth) + ". "+((point.z+height2)/cellHeight));
			
			//currCell.x = Mathf.Clamp(Mathf.FloorToInt((point.x+width2) / cellWidth), 0, numColumns-1);
			//currCell.y = Mathf.Clamp(Mathf.FloorToInt ((point.z+height2)/cellHeight), 0, numRows-1);
			
			currCell.x = Mathf.FloorToInt((point.x+width2) / cellWidth);
			currCell.y = Mathf.FloorToInt ((point.z+height2)/cellHeight);
		
			
			currCell.center.x = (currCell.x * CellWidth)+(CellWidth*0.5f)-width2;
			currCell.center.y = 0.0f;
			currCell.center.z = (currCell.y * CellHeight)+(CellHeight*0.5f)-height2;
			currCell.center = transform.TransformPoint(currCell.center);
			
			this.SendMessage("OnTouchedCell", currCell , SendMessageOptions.DontRequireReceiver);
		}				
        //Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
	}
	//----------------------------
		
}
