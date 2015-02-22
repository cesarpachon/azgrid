2012/06/26
QuadGrid Unity3D Component
V. 01
by Cesar Pachón (http://www.cesarpachon.com) 

project site: https://github.com/cesarpachon/azgrid
company site: http://www.cesarpachon.com

OVERVIEW
QuadGrid is a simple Unity Component that allow the creation and edition of 2D quad or tile based grids.
It is focused in being simple and lightweight. 
in particular, it only consume ONE gameObject, does not creates separated gameobject for each cell. 
it support the following features.
1. visualization and edition in both unity's EDIT and PLAY mode.
2. separated materials for tiles and lines.
3. hide/show either lines or tiles.
4. change width/height of tiles
5. change width of separator lines
6. generates OnTouchedCell events
7. works under rotations, scale and changes in position.

INTERNALS
it is implemented using two procedural generated Meshes, that are draw using Graphics.drawMesh method.

USAGE
create a empty game object, and attach the azquadgrid component.
in the inspector, assign materials, configure number of row and columns, change widht and heights..
if you want receive events, attach a custom script with a method like:
 public void OnTouchedCell(AZQuadGrid.AZQuadCell cell)
the struct cell holds row and column and also the center of the cell as a Vector3 struct. 

SAMPLE
check the sample scene acompaning the package.

DOCUMENTATION
check the video presentation in https://www.youtube.com/watch?v=ecupnLGvWAI

LICENSE AND WARRANTY
Licensed under LGPL V3


