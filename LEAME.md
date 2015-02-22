2012/06/26
Componente QuadGrid para Unity3D
V. 01
por Cesar Pachón (http://www.cesarpachon.com) 

sitio del proyecto: https://github.com/cesarpachon/azgrid
sitio de la compañía: http://www.cesarpachon.com

RESUMEN
QuadGrid es un sencillo componente para Unity3D que permite la creación y edición de una cuadrícula bidimensional. 
Se enfoca en ser sencillo y ligero.
En particular, sólo consume un GameObject. no crea GameObjects separados para cada celda. 
Soporta las siguientes características:
1. visualización y edición en modos EDICION y EJECUCION de Unity3D.
2. materiales separados para celdas y líneas.
3. mostrar/ocular celdas ó lineas.
4. cambiar el ancho/alto de las celdas.
5. cambiar el ancho de la línea de separación.
6. generar eventos OnTouchedCell
7. funciona aún si la cuadricula es rotada, escalada, o cambia de posición.


IMPLEMENTACION
Está implementada utilizando dos mallas generadas proceduralmente. para dibujar utiliza el método Graphics.DrawMesh.

USO
Debes crear un GameObject vacío, y asociarle el script AZQuadGrid.
En el inspector, puedes asignarle materiales, configurar el número de filas y columnas, cmabiar el ancho y el alto..
si quieres recibir eventos, asocia un script propio que tenga un método así: 
 public void OnTouchedCell(AZQuadGrid.AZQuadCell cell)
la estructura cell que se recibe por parámetros contiene la fila y la columna seleccionada, así como el centro de la celda como un Vector3.

EJEMPLO
Verifica la escena de ejemplo que acompaña este paquete.

DOCUMENTACION
verifica el video de presentación en  https://www.youtube.com/watch?v=ecupnLGvWAI

LICENCIA Y GARANTÍA
Todo el código está licenciado bajo LGPL V3


