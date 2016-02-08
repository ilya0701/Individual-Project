
var controller : CharacterController;
var speed : float = 10.0;
var rotateSpeed : float = 0.5;
var sprintSpeed : float = 2.5;
var heavyBreath : AudioSource;
var footsteps : AudioSource; 
var runFootsteps : AudioSource; 
var torchSound : AudioSource; 
var torchLight : Light;
var flame : ParticleSystem;

private var timer = 0.0; 
 var bobbingSpeed = 0.10; 
 var bobbingAmount = 0.1; 
 var midpoint = 10; 

function Start () {
	Cursor.visible = false;
}

function Update () {
	footsteps.volume = 0;
	heavyBreath.volume = 0;
	runFootsteps.volume = 0;
	transform.Rotate(0, Input.GetAxis ("Mouse X") * rotateSpeed, 0);
//	if (transform.rotation.x<10 && transform.rotation.x>-10)
//		transform.Rotate(Input.GetAxis ("Mouse Y") * rotateSpeed, 0, 0);
	var forward : Vector3 = transform.TransformDirection(Vector3.forward);
	var curSpeed : float = speed * Input.GetAxis ("Vertical");
	if (curSpeed!=0)
		footsteps.volume = 1;
	if (Input.GetKey("left shift")){
		controller.SimpleMove(forward * curSpeed * sprintSpeed);
		heavyBreath.volume = 0.4;
		footsteps.volume = 0;
		runFootsteps.volume = 1;
		
	}	
	else{
		controller.SimpleMove(forward * curSpeed);
	}
	if (Input.GetMouseButtonDown(0) && torchLight.intensity==3){
		torchLight.intensity = 0;	
		flame.maxParticles = 0;
		torchSound.Stop();
	}	
	if (Input.GetMouseButtonDown(1) && torchLight.intensity==0){
		torchLight.intensity = 3;	
		flame.maxParticles = 20;
		torchSound.Play();
	}		
}

function HeadBob(){
	waveslice = 0.0; 
    horizontal = Input.GetAxis("Horizontal"); 
    vertical = Input.GetAxis("Vertical"); 
    if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0) { 
       timer = 0.0; 
    } 
    else { 
       waveslice = Mathf.Sin(timer); 
       timer = timer + bobbingSpeed; 
       if (timer > Mathf.PI * 2) { 
          timer = timer - (Mathf.PI * 2); 
       } 
    } 
    if (waveslice != 0) { 
       translateChange = waveslice * bobbingAmount; 
       totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical); 
       totalAxes = Mathf.Clamp (totalAxes, 0.0, 1.0); 
       translateChange = totalAxes * translateChange; 
       transform.localPosition.y = midpoint + translateChange; 
    } 
    else { 
       transform.localPosition.y = midpoint; 
    } 
}