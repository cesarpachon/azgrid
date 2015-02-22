using UnityEngine;
using System.Collections;

public class OnTouchedCellScript : MonoBehaviour 
{
	
	public GameObject target;

	public void OnTouchedCell(AZQuadGrid.AZQuadCell cell)
	{
		Debug.Log("touched: "+ cell.x + " "+cell.y + " with center "+ cell.center.ToString());
		target.transform.position = cell.center;
	}
}
