var controller : CharacterController;
var speed : float = 1.0;
var rotateSpeed : float = 0.5;
var sprintSpeed : float = 2.5;

private var timer = 0.0; 
 var bobbingSpeed = 0.10; 
 var bobbingAmount = 0.1; 
 var midpoint = 10; 
 var v = new Vector3 (0,0,15); 

function Start () {

	//Cursor.visible = false;
	transform.Translate(v);
	transform.Rotate(90, 0, 0);

}

function Update () {

	transform.Rotate(0, Input.GetAxis ("Mouse X") * rotateSpeed, 0);
	transform.Rotate(Input.GetAxis ("Mouse Y") * rotateSpeed, 0, 0);
	var forward : Vector3 = transform.TransformDirection(Vector3.forward);
	var curSpeed : float = speed * Input.GetAxis ("Vertical");

	if (Input.GetKey("left shift")){
		controller.Move(forward * curSpeed * sprintSpeed);
		
	}	
	else{
		controller.Move(forward * curSpeed);
	}	
}