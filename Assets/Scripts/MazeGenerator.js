public class MazeGenerator extends MonoBehaviour {

public var wall : GameObject;
public var floor : GameObject;
public var ceiling : GameObject;
public var wallwTorch : GameObject;
public var wallwGrate : GameObject;
public var wallwArch : GameObject;
public var wallwGrateArch : GameObject;

var height : int;
var width : int;

var maze;
var walls;
var arr;
var num : int;
var done : boolean = false;

public function Start(){
	height = 100;
    width = 100;
    if (done==false){
		generate();
		done = true;
	}
	draw();
}

function generate(){
	var i : int;
	var j : int;
	var k : int;
	var l : int;

	k = 0;
	maze = new Array(height);
	arr = new Array(height*width);
	for (i=0; i < height; i++){
   		maze[i] = new Array(width);
   		for (j=0; j < width; j++){
   			maze[i][j] = new Array(1);
   			maze[i][j][0] = k;
   			k++;	
   		}
   	}
   	num = height*(width-1) + width*(height-1);
   	walls = new Array(num);
   	for (i=0; i < num; i++){
   		walls[i] = new Array(3);
   	}
    k = 0;
    var count : int;
    for (i=0; i < height; i++){
   		for (j=0; j < width; j++){
   			count = 0;
   			if ((j+1)<width){
   				walls[k][0] = maze[i][j][0];
   				walls[k][1] = maze[i][j+1][0];
   				count++;
   			}
   			if ((i+1)<height){
   				if (count==1){
   					walls[k+1][0] = maze[i][j][0];
   					walls[k+1][1] = maze[i+1][j][0];
   					count++;
   				}
   				else{
   					walls[k][0] = maze[i][j][0];
   					walls[k][1] = maze[i+1][j][0];
   					count++;
   				}	
   			}
   			if (count==1){
   				k++;
   			}
   			if (count==2){
   				k = k + 2;
   			}
   			if (k>(height*(width-1) + width*(height-1))){
   				i=height;
   				j=width;
   			}
   		}
   	}
   	RandomizeArray(walls); 
   	for (l=0; l < num; l++){
   		number = walls[l][0];
   		if (arr[number] != true){
   			arr[number] = true;
   			walls[l][2] = true;
   		} 
   	} 	  	  	 
}

function draw(){
	var n : int;
	var position;	
	var wallWidth : int = 28;
	var wallHeight : int = 0;
	var differenceHeight : int;
	var differenceWidth : int;	
	var rotation : Quaternion = Quaternion.identity;
	
	differenceHeight = 58 * height;
	differenceWidth = 58 * width;
	
	floor.isStatic = true;
	floor.transform.localScale = Vector3(10*width,10,10*height);
	Instantiate (floor, Vector3 (58*(width/2),0,58*(height/2)-29), rotation);
	
	ceiling.isStatic = true;
	ceiling.transform.localScale = Vector3(10*width,10,10*height);
	Instantiate (ceiling, Vector3 (58*(width/2),60,58*(height/2)-29), rotation);
	
	wall.isStatic = true;
	wallwGrate.isStatic = true;
	wallwArch.isStatic = true;
	wallwGrateArch.isStatic = true;
	
	for (n=0; n<width; n++){
		position = new Vector3 (wallWidth, 0, -28);
		Instantiate (wall, position, Quaternion.Euler(0,90,0));
		wallWidth = wallWidth + 58;
	}	
	wallWidth = 28;
	for (n=0; n<width; n++){
		position = new Vector3 (wallWidth, 0, -28+differenceHeight);
		Instantiate (wall, position, Quaternion.Euler(0,90,0));
		wallWidth = wallWidth + 58;
	}	
	wallHeight = 28;
	for (n=0; n<height; n++){
		position = new Vector3 (0, 0, -28+wallHeight);
		Instantiate (wall, position, rotation);
		wallHeight = wallHeight + 58;
	}
	wallHeight = 28;
	for (n=0; n<height; n++){
		position = new Vector3 (differenceWidth, 0, -28+wallHeight);
		Instantiate (wall, position, rotation);
		wallHeight = wallHeight + 58;
	}
	
	for (n=0; n<num; n++){	
	    if (walls[n][2]!=true){
	    	if ((walls[n][1]-walls[n][0])!=1){
		    	position = new Vector3 (((walls[n][0])%width+1)*58, 0, ((walls[n][1])/width)*58);
	    	    Instantiate (GetWall(), position, rotation);
	    	}
	    	else{
	    		position = new Vector3 (((walls[n][0])%width+1)*58-28, 0, ((walls[n][1])/width)*58+28);
	    		Instantiate (GetWall(), position, Quaternion.Euler(0,90,0));
	    	}
	    }
	}
}

static function RandomizeArray(arr : Array){
    for (var i = arr.length - 1; i > 0; i--) {
        var r = Random.Range(0,i);
        var tmp = arr[i];
        arr[i] = arr[r];
        arr[r] = tmp;
    }
}

function GetWall(){
	var random = Random.Range(0, 10);
	if (random>0)
			if (random>6)
					if (random>7)
							if (random>8)
								return wallwGrateArch;
							else
								return wallwArch;
					else
						return wallwGrate;
			else
				return wall;
	else
		return wall;
}

}