
function Start () {
	var rend = GetComponent.<Renderer>();
	var MazeGeneratorScript = GameObject.FindWithTag('GameController').gameObject.GetComponent('MazeGenerator');
	rend.material.mainTextureScale = Vector2 (MazeGeneratorScript.width,MazeGeneratorScript.height);
}
