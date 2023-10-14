//========================================================================================================================
// Edy Vehicle Physics - (c) Angel Garcia "Edy" - Oviedo, Spain
// Live demo: http://www.edy.es/unity/offroader.html
// 
// Terms & Conditions:
//  - Use for unlimited time, any number of projects, royalty-free.
//  - Keep the copyright notices on top of the source files.
//  - Resale or redistribute as anything except a final product to the end user (asset / library / engine / middleware / etc.) is not allowed.
//  - Put me (Angel Garcia "Edy") in your game's credits as author of the vehicle physics.
//
// Bug reports, improvements to the code, suggestions on further developments, etc are always welcome.
// Unity forum user: Edy
//========================================================================================================================
//
// CameraControl
//
// Manages the cameras of the scene, including main camera, map, mirrors, camera scripts and image effects.
//
//========================================================================================================================

#pragma strict

var Target : Transform;
var Target2 : Transform;

var MapCamera : Camera;
var DefaultCamera : int;

var showMirrors = true;
var MirrorLeftTex : GUITexture;
var MirrorRightTex : GUITexture;
var MirrorRearTex : GUITexture;

var enableImageEffects = true;
var ImageEffects : MonoBehaviour[];

private var m_cameraScript : MonoBehaviour[];
private var m_currCamera = DefaultCamera;		// C�mara por defecto. NO puede ser la SmoothLookAtFromPos
private var m_numCameras = 6;			// N�mero de scripts de c�mara controlados (hardcoded). La �ltima debe ser la SmothLookAtFromPos.

// Una variable para cada script que soportemos. 
// Es necesario ajustar cada script individualmente.

private var m_scrFixTo : CamFixTo;
private var m_scrSmoothFollow : CamSmoothFollow;
private var m_scrMouseOrbit : CamMouseOrbit;
private var m_scrSmoothLookAt : CamSmoothLookAt;
private var m_scrFreeView : CamFreeView;
private var m_scrSmoothLookAtFromPos : CamSmoothLookAtFromPos;

private var m_scrDriverFreeView : CamFreeView;
private var m_scrMapFollow : CamSmoothFollow;

private var m_lastTarget : Transform;
private var m_lastShowMirrors : boolean = showMirrors;
private var m_lastImageEffects : boolean = enableImageEffects;

private var m_targetCams : CarCameras;


// Localizar los componentes apropiados del Target y aplicarlos a las c�maras

private function UpdateTarget()
	{
	// En este punto Target y m_targetCams a�n apuntan al Target anterior. 
	// Desactivar sus c�maras espejo y el movimiento de la vista de conductor.
	
	if (m_targetCams)
		{
		if (m_targetCams.MirrorLeft) m_targetCams.MirrorLeft.enabled = false;
		if (m_targetCams.MirrorRight) m_targetCams.MirrorRight.enabled = false;
		if (m_targetCams.MirrorRear) m_targetCams.MirrorRear.enabled = false;
		
		if (m_scrDriverFreeView)
			m_scrDriverFreeView.enabled = false;
		}
	
	// Obtener los objetos necesarios del target
	
	m_targetCams = Target.GetComponent(CarCameras) as CarCameras;
	
	if (m_targetCams.DriverFront)
		m_scrDriverFreeView = m_targetCams.DriverFront.GetComponent(CamFreeView) as CamFreeView;
	else
		m_scrDriverFreeView = null;
		
	// Ajustar las camaras al target
	
	m_scrFixTo.Pos = m_targetCams.DriverFront;
	m_scrSmoothFollow.target = m_targetCams.CameraLookAtPoint;
	m_scrSmoothFollow.distance = m_targetCams.viewDistance;
	m_scrSmoothFollow.height = m_targetCams.viewHeight;
	m_scrSmoothFollow.rotationDamping = m_targetCams.viewDamping;
	
	m_scrMouseOrbit.target = m_targetCams.CameraLookAtPoint;
	m_scrMouseOrbit.distance = m_targetCams.viewDistance;
	m_scrMouseOrbit.distMinLimit = m_targetCams.viewMinDistance;
	m_scrMouseOrbit.yMinLimit = m_targetCams.viewMinHeight;
	
	m_scrSmoothLookAt.target = m_targetCams.CameraLookAtPoint;	

	if (m_scrMapFollow) m_scrMapFollow.target = m_targetCams.CameraLookAtPoint;	
	
	// El script de combinar dos targets se habilita o no seg�n Target2 est� establecido.
	// Target2 puede ser un Transform cualquiera. Si lleva un CarCameras, se usar�.	

	if (Target2)
		{
		var Target2Cameras : CarCameras;
		
		m_scrSmoothLookAtFromPos.pos = m_targetCams.CameraLookAtPoint;
		m_scrSmoothLookAtFromPos.positionZ = -m_targetCams.viewDistance;
		m_scrSmoothLookAtFromPos.positionY = m_targetCams.viewHeight / 2.0;
		
		Target2Cameras = Target2.GetComponent(CarCameras) as CarCameras;
		if (Target2Cameras)
			m_scrSmoothLookAtFromPos.target = Target2Cameras.CameraLookAtPoint;
		else
			m_scrSmoothLookAtFromPos.target = Target2;
		}
	else
		{
		// Si estaba seleccionada la c�mara ScrLookAtFromPos pero no ten�a Target2, establecer la c�mara por defecto.
		
		if (m_currCamera == m_numCameras-1)
			SwitchTo(DefaultCamera);
		}
		
	// Actualizar los espejos en el nuevo target
	
	UpdateMirrors();	
	}
	
	
function ClearMirrorTexture(Cam : Camera)
	{
	var oldClearFlags = Cam.clearFlags;
	var oldBackgroundColor = Cam.backgroundColor;
	var oldRect = Cam.rect;
	var oldCullingMask = Cam.cullingMask;
	
	Cam.clearFlags = CameraClearFlags.SolidColor;
	Cam.backgroundColor = Color(1.0, 1.0, 1.0, 1.0);	// Alpha 0.0=transparente, 1.0=opaco
	Cam.rect = Rect(0.0, 0.0, 1.0, 1.0);
	Cam.cullingMask = 0;
	
	Cam.Render();
	
	Cam.clearFlags = oldClearFlags;
	Cam.backgroundColor = oldBackgroundColor;
	Cam.rect = oldRect;
	Cam.cullingMask = oldCullingMask;
	}
	

function UpdateMirrors()
	{
	if (m_scrDriverFreeView)
		m_scrDriverFreeView.enabled = m_currCamera == 0;	
	
	// Determinar es adecuado mostrar u ocultar los espejos. Se muestran si:
	// - Disponemos de la textura corresponiente para mostrarlo.
	// - Estamos en vista del conductor.
	// - Est�n los espejos habilitados.	
	
	if (m_currCamera == 0 && showMirrors)
		{	
		if (MirrorLeftTex)
			if (m_targetCams.MirrorLeft)
				{
				m_targetCams.MirrorLeft.targetTexture = MirrorLeftTex.texture as RenderTexture; 
				m_targetCams.MirrorLeft.enabled = true;
				MirrorLeftTex.enabled = true;
				
				ClearMirrorTexture(m_targetCams.MirrorLeft);
				}
			else
				MirrorLeftTex.enabled = false;
		
		if (MirrorRightTex)
			if (m_targetCams.MirrorRight)
				{
				m_targetCams.MirrorRight.targetTexture = MirrorRightTex.texture as RenderTexture;
				m_targetCams.MirrorRight.enabled = true;
				MirrorRightTex.enabled = true;
				
				ClearMirrorTexture(m_targetCams.MirrorRight);
				}
			else
				MirrorRightTex.enabled = false;
			
		if (MirrorRearTex)
			if (m_targetCams.MirrorRear)
				{
				m_targetCams.MirrorRear.targetTexture = MirrorRearTex.texture as RenderTexture;
				m_targetCams.MirrorRear.enabled = true;
				MirrorRearTex.enabled = true;
				
				ClearMirrorTexture(m_targetCams.MirrorRear);
				}
			else
				MirrorRearTex.enabled = false;
		}
	else
		{
		if (m_targetCams.MirrorLeft) m_targetCams.MirrorLeft.enabled = false;
		if (m_targetCams.MirrorRight) m_targetCams.MirrorRight.enabled = false;
		if (m_targetCams.MirrorRear) m_targetCams.MirrorRear.enabled = false;
		
		if (MirrorLeftTex) MirrorLeftTex.enabled = false;
		if (MirrorRightTex) MirrorRightTex.enabled = false;
		if (MirrorRearTex) MirrorRearTex.enabled = false;
		}
	}
	

function Start ()
	{	
	m_currCamera = DefaultCamera;
	m_lastTarget = Target;
	
	// Obtener los scripts de la c�mara asociados y dejar la c�mara por defecto.
	// Hay que mantener el n�mero de c�maras ajustado al n�mero de componentes.
	// La �ltima c�mara debe ser SmoothLookAtFromPos (lleva tratamiento especial).
	
	m_scrFixTo = GetComponent(CamFixTo) as CamFixTo;
	m_scrSmoothFollow = GetComponent(CamSmoothFollow) as CamSmoothFollow;
	m_scrMouseOrbit = GetComponent(CamMouseOrbit) as CamMouseOrbit;
	m_scrSmoothLookAt = GetComponent(CamSmoothLookAt) as CamSmoothLookAt;
	m_scrFreeView = GetComponent(CamFreeView) as CamFreeView;
	m_scrSmoothLookAtFromPos = GetComponent(CamSmoothLookAtFromPos) as CamSmoothLookAtFromPos;
	
	m_cameraScript = new MonoBehaviour[m_numCameras];
	m_cameraScript[0] = m_scrFixTo;
	m_cameraScript[1] = m_scrSmoothFollow;
	m_cameraScript[2] = m_scrMouseOrbit;
	m_cameraScript[3] = m_scrSmoothLookAt;
	m_cameraScript[4] = m_scrFreeView;
	m_cameraScript[5] = m_scrSmoothLookAtFromPos;
	
	if (MapCamera)
		m_scrMapFollow = MapCamera.GetComponent(CamSmoothFollow) as CamSmoothFollow;
		
	// Desactivar las texturas para las c�maras espejo
	
	if (MirrorLeftTex) MirrorLeftTex.enabled = false;
	if (MirrorRightTex) MirrorRightTex.enabled = false;
	if (MirrorRearTex) MirrorRearTex.enabled = false;
	
	// Asignar las propiedades del Target a las c�maras. Puede no haber Target asignado al inicio.
	
	if (Target) UpdateTarget();
	
	// Establecer los efectos de imagen
	
	for (var i=0; i<ImageEffects.length; i++)
		ImageEffects[i].enabled = enableImageEffects;
	
	// Establecer la c�mara inicial.
	
	for (i=0; i<m_numCameras; i++)
		m_cameraScript[i].enabled = false;
	m_cameraScript[m_currCamera].enabled = true;
	}


function Update () 
	{
	if (m_lastTarget != Target)
		{		
		// Ajustar las c�maras al nuevo target
		
		UpdateTarget();
		m_lastTarget = Target;
		}
		
	if (m_lastShowMirrors != showMirrors)
		{
		UpdateMirrors();
		m_lastShowMirrors = showMirrors;
		}
		
	if (m_lastImageEffects != enableImageEffects)
		{
		for (var i=0; i<ImageEffects.length; i++)
			ImageEffects[i].enabled = enableImageEffects;
		m_lastImageEffects = enableImageEffects;
		}
		
	}
	

function Next ()
	{
	m_cameraScript[m_currCamera++].enabled = false;
	if (m_currCamera >= m_numCameras || (m_currCamera == m_numCameras-1 && !Target2)) m_currCamera = 0;
	m_cameraScript[m_currCamera].enabled = true;
	
	UpdateMirrors();
	}
	

function SwitchTo (Cam : int)
	{
	if (Cam < m_numCameras)
		{
		// Si es la c�mara del conductor, ya estaba seleccionada, y adem�s tiene un FreeView, reiniciar su rotaci�n a la original.
		// NOTA: Se usa FreeView con movimiento=0 y no MouseLook porque con FreeView se puede reiniciar la rotaci�n del transform, pero con MouseLook no.
		
		if (Cam == 0 && Cam == m_currCamera)
			{
			var DriverCam : CamFreeView = m_targetCams.DriverFront.GetComponent(CamFreeView) as CamFreeView;
			if (DriverCam)
				DriverCam.SetLocalEulerAngles(m_targetCams.getDriverViewAngles());
			}
		
		// Establecer la c�mara indicada
		
		m_cameraScript[m_currCamera].enabled = false;
		m_cameraScript[Cam].enabled = true;
		m_currCamera = Cam;
		
		UpdateMirrors();
		}	
	}
	

function ToggleMap ()
	{
	if (MapCamera)
		MapCamera.enabled = !MapCamera.enabled;
	}
	

