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
// CarControl
//
// Main script for controlling and configuring the vehicle. All parameters that affect the behavior & handling 
// are defined here (except suspension parameters: spring, damper, distance, radius, which are configured on each wheel).
//
//========================================================================================================================

#pragma strict

// Objetos controlados

var WheelFL : CarWheel;
var WheelFR : CarWheel;
var WheelRL : CarWheel;
var WheelRR : CarWheel;
var AntiRollFront : CarAntiRollBar;
var AntiRollRear : CarAntiRollBar;

var CenterOfMass : Transform;

// Curvas de fricci�n de las ruedas

var ForwardWheelFriction : CarFrictionCurve;
var SidewaysWheelFriction : CarFrictionCurve;
var optimized = false;				// En modo optimizado los par�metros de las curvas de fricci�n no se pueden cambiar en tiempo real (no responden adecuadamente).

// Par�metros de entrada

var steerInput = 0.0;
var motorInput = 0.0;
var brakeInput = 0.0;
var handbrakeInput = 0.0;
var gearInput : int = 1;

// Par�metros de rendimiento y comportamiento

var steerMax = 45.0;
var motorMax = 1.0;			// Cantidad m�xima de slip de aceleraci�n antes de TC
var brakeMax = 1.0;			// Cantidad m�xima de slip de frenado antes de ABS

var autoSteerLevel = 1.0;	// ESP
var autoMotorMax = true;	// TC
var autoBrakeMax = true;	// ABS
var antiRollLevel = 1.0;	// Barras estabilizadoras

var motorPerformancePeak = 5.0;		// m/s de aceleraci�n m�xima en modo 4x4. En tracci�n simple es necesario subir el motorForceFactor
var motorForceFactor = 1.0;
var motorBalance = 0.5;				// 0.0 = tracci�n delantera, 1.0 = tracci�n trasera, 0.5 = 4x4.
var brakeForceFactor = 1.5;
var brakeBalance = 0.5;				// 0.0 = todo adelante, 1.0 = todo atr�s, 0.5 = mismo nivel.

var sidewaysDriftFriction = 0.35;
var staticFrictionMax = 1500.0;		// Control de fricci�n - ver CarWheel.
var frontSidewaysGrip = 1.0;
var rearSidewaysGrip = 1.0;

// Par�metros de configuraci�n

var serviceMode = false;			// Si est� activado se asignan los par�metros de grip tal cual. Permite hacer las pruebas de las curvas de fricci�n.

var airDrag = 3 * 2.2;		// Coeficiente aerodin�mico (ej. corvette 0.30) * Area frontal
var frictionDrag = 30.0;	// 30 veces la resistencia del aire (as� airDrag ser� m�s importante a partir de 30 m/s = 100 Km/h)

var rollingFrictionSlip = 0.075;

// MODO EXPERIMENTAL V3:
// - Reduce la tracci�n a medida que se aproxima a la velocidad m�xima. Alcanzando �sta la tracci�n es 0.

var tractionV3 = false;
var maxSpeed = 40.0;


// ------------------------------------------------
// Datos privados + telemetr�a

private var m_motorSlip = 0.0;
private var m_brakeSlip = 0.0;

private var m_frontMotorSlip = 0.0;
private var m_frontBrakeSlip = 0.0;
private var m_rearMotorSlip = 0.0;
private var m_rearBrakeSlip = 0.0;

private var m_steerL = 0.0;
private var m_steerR = 0.0;
private var m_steerAngleMax = 0.0;

private var m_maxRollAngle = 45.0;

function getGear() : String { return gearInput>0? "D" : gearInput<0? "R" : "N"; }
function getMotor() : float { return motorInput; }
function getBrake() : float { return brakeInput; }
function getHandBrake() : float { return handbrakeInput; }

function getSteerL() : float { return m_steerL; }
function getSteerR() : float { return m_steerR; }

function getMaxRollAngle() : float { return m_maxRollAngle; }


function OnEnable ()
	{
	ApplyEnabled(WheelFL, true);
	ApplyEnabled(WheelFR, true);
	ApplyEnabled(WheelRL, true);
	ApplyEnabled(WheelRR, true);
	}
	
function OnDisable ()
	{
	ApplyEnabled(WheelFL, false);
	ApplyEnabled(WheelFR, false);
	ApplyEnabled(WheelRL, false);
	ApplyEnabled(WheelRR, false);
	}
	

function Start ()
	{
	// Centro de Masas (CoM)
	// LocalScale es necesario para aplicar cualquier escalado que se haya aplicado al modelo.

	if (CenterOfMass) rigidbody.centerOfMass = Vector3(CenterOfMass.localPosition.x * transform.localScale.x, CenterOfMass.localPosition.y * transform.localScale.y, CenterOfMass.localPosition.z * transform.localScale.z);	
	
	// Punto de equilibrio lateral (estimado)
	
	var WheelC : WheelCollider = WheelFL.GetComponent(WheelCollider) as WheelCollider;
	var V = rigidbody.centerOfMass - transform.InverseTransformPoint(WheelC.transform.position);	
	var h = Mathf.Abs((V.y + WheelC.radius + WheelC.suspensionDistance/2.0) * transform.localScale.y);
	var l = Mathf.Abs(V.x * transform.localScale.x);
	m_maxRollAngle = Mathf.Atan2(l, h) * Mathf.Rad2Deg;

	// Otros datos
	
	rigidbody.maxAngularVelocity = 10;
	rigidbody.useConeFriction = false;
	
	// En modo optimizado aplicar los par�metros de las ruedas ahora, entonces recalcular los datos costosos
	
	if (optimized)
		{
		ApplyCommonParameters(WheelFL);
		ApplyCommonParameters(WheelFR);
		ApplyCommonParameters(WheelRL);
		ApplyCommonParameters(WheelRR);
		WheelFL.RecalculateStuff();
		WheelFR.RecalculateStuff();
		WheelRL.RecalculateStuff();
		WheelRR.RecalculateStuff();
		}
	}


function FixedUpdate ()
	{
	//======================================================================
	// 1. Aplicar par�metros a las ruedas
	//======================================================================

	// Par�metros de las ruedas que se establecen una vez en el script de control para no tener que ir rueda por rueda
	
	ApplyCommonParameters(WheelFL);
	ApplyCommonParameters(WheelFR);
	ApplyCommonParameters(WheelRL);
	ApplyCommonParameters(WheelRR);

	// P�metros para ruedas individuales seg�n configuraci�n actual
	
	ApplyFrontParameters(WheelFL);
	ApplyFrontParameters(WheelFR);
	ApplyRearParameters(WheelRL);
	ApplyRearParameters(WheelRR);
	
	//======================================================================
	// 2. Aplicar tracci�n / frenado
	//======================================================================

	m_motorSlip = motorInput * motorMax;
	m_brakeSlip = brakeInput * brakeMax;
	
	if (gearInput == 0) m_motorSlip = 0;
	else if (gearInput < 0) m_motorSlip = -m_motorSlip;
	
	// Balance de tracci�n

	if (serviceMode)
		{
		m_frontMotorSlip = m_motorSlip;
		m_rearMotorSlip = m_motorSlip;
		}
	else
	if (motorBalance >= 0.5)		// reducir tracci�n adelante
		{
		m_frontMotorSlip = m_motorSlip * (1.0-motorBalance) * 2.0;
		m_rearMotorSlip = m_motorSlip;
		}
	else	// Reducir tracci�n atr�s
		{
		m_frontMotorSlip = m_motorSlip;
		m_rearMotorSlip = m_motorSlip * motorBalance * 2.0;		
		}

	// Balance de frenado
	
	if (serviceMode)
		{
		m_frontBrakeSlip = m_brakeSlip;
		m_rearBrakeSlip = m_brakeSlip;
		}
	else
	if (brakeBalance >= 0.5)  // reducir frenos adelante
		{
		m_frontBrakeSlip = m_brakeSlip * (1.0-brakeBalance) * 2.0;
		m_rearBrakeSlip = m_brakeSlip;
		}
	else	// Reducir frenos atr�s
		{
		m_frontBrakeSlip = m_brakeSlip;
		m_rearBrakeSlip = m_brakeSlip * brakeBalance * 2.0;
		}

	ApplyTraction(WheelFL, m_frontMotorSlip, m_frontBrakeSlip, 0.0);
	ApplyTraction(WheelFR, m_frontMotorSlip, m_frontBrakeSlip, 0.0);
	ApplyTraction(WheelRL, m_rearMotorSlip, m_rearBrakeSlip, handbrakeInput);
	ApplyTraction(WheelRR, m_rearMotorSlip, m_rearBrakeSlip, handbrakeInput);

	//======================================================================
	// 3. Aplicar direcci�n
	//======================================================================
	
	// autoSteerLevel determina el �ngulo en el que las ruedas ofrecen la m�xima fuerza de giro (peakSlip) a la velocidad actual.
	// Se coge como referencia una rueda cualquiera delantera.

	if (autoSteerLevel > 0.0) 
		{
		var peakSlip = WheelFL.getSidewaysPeakSlip();
		var forwardSpeed = Mathf.Abs(transform.InverseTransformDirection(rigidbody.velocity).z * autoSteerLevel);
		
		if (forwardSpeed > peakSlip)
			m_steerAngleMax = 90.0 - Mathf.Acos(peakSlip / forwardSpeed) * Mathf.Rad2Deg;
		else
			m_steerAngleMax = steerMax;
		}
	else 
		m_steerAngleMax = steerMax;

	// La direcci�n de giro de cada rueda se calcula en Update para que se visualice con suavidad,
	// incluso en c�mara lenta.

	WheelFL.getWheelCollider().steerAngle = m_steerL;
	WheelFR.getWheelCollider().steerAngle = m_steerR;
		
	//======================================================================
	// 4. Fuerzas de resistencia
	//======================================================================
	
	// Resistencia aerodin�mica (drag) y resistencia a rodar (rr)
	//
	// Fdrag = -Cdrag * V * |V|
	// Frr = -Crr * V
	
	// Cdrag =  0.5 * Cd * A * rho
	// 	Cd = coeficiente aerodin�mico (ej. corvette 0.30)
	//	A = �rea frontal del veh�culo
	//	rho = densidad del aire = 1.29 kg/m3
	//
	// Crr = 30 veces Cdrag (as� Fdrag ser� m�s importante a partir de 30 m/s = 100 Km/h)
	
	if (!serviceMode)
		{
		var Cdrag = 0.5 * airDrag * 1.29 ; // * (motorMax+1) / 2;
		var Crr = frictionDrag * Cdrag;
		
		var Fdrag = -Cdrag * rigidbody.velocity * rigidbody.velocity.magnitude;
		var Frr = -Crr * rigidbody.velocity;
		
		rigidbody.AddForce(Fdrag + Frr);
		}
		
	//======================================================================
	// 5. Ajustes t�cnicos adicionales
	//======================================================================
	
	// Barras estabilizadoras
	
	if (AntiRollFront) AntiRollFront.AntiRollFactor = antiRollLevel;
	if (AntiRollRear) AntiRollRear.AntiRollFactor = antiRollLevel;	
	}
	

function Update()
	{
	// Calcular los �ngulos de giro de las ruedas para que los cambios de direcci�n se visualicen con suavemente,
	// incluso en c�mara lenta. FixedUpdate coge los valores que haya en el momento.
	
	CalculateSteerAngles();
	}
	

	
function ApplyEnabled(Wheel : CarWheel, enable : boolean)
	{
	// Necesario comprobar con null, ya que el OnDisable se invoca al finalizar la aplicaci�n, 
	// y el objeto puede no estar ya disponible aunque su referencia sea no nula.
	
	if (Wheel != null) Wheel.enabled = enable;
	}
		
	
function ApplyCommonParameters(Wheel : CarWheel)
	{
	Wheel.ForwardWheelFriction = ForwardWheelFriction;
	Wheel.SidewaysWheelFriction = SidewaysWheelFriction;
	Wheel.brakeForceFactor = brakeForceFactor;
	Wheel.motorForceFactor = motorForceFactor;
	Wheel.performancePeak = motorPerformancePeak;
	Wheel.sidewaysDriftFriction = sidewaysDriftFriction;
	Wheel.staticFrictionMax = staticFrictionMax;
	
	Wheel.serviceMode = serviceMode;
	Wheel.optimized = optimized;
	}

function ApplyFrontParameters(Wheel : CarWheel)
	{
	Wheel.sidewaysForceFactor = frontSidewaysGrip;
	}
	
function ApplyRearParameters(Wheel : CarWheel)
	{
	Wheel.sidewaysForceFactor = rearSidewaysGrip;
	}

function ApplyTraction(Wheel : CarWheel, motorSlip : float, brakeSlip : float, handBrakeInput : float)
	{
	var slipPeak = Wheel.getForwardPeakSlip();
	var slipMax = Wheel.getForwardMaxSlip();
	
	var Hit : WheelHit;
	var slip : float;
	
	// Tracci�n
	
	var motor = Mathf.Abs(motorSlip);	// motor = [0..motorMax]
	
	if (Wheel.getWheelCollider().GetGroundHit(Hit))
		{
		var steerRot = Quaternion.AngleAxis(Wheel.getWheelCollider().steerAngle, Wheel.transform.up);
		var wheelDir = steerRot * Wheel.transform.forward;	

		var pointV = rigidbody.GetPointVelocity(Hit.point);
		if (Hit.collider.attachedRigidbody)
			pointV -= Hit.collider.attachedRigidbody.GetPointVelocity(Hit.point);
		
		var v = Mathf.Abs(Vector3.Dot(pointV, wheelDir));
		
		if (v + slipPeak <= motorMax)
			{
			slip = motor - v;
			
			if (slip < 0) 
				slip = 0;
			else
			if (autoMotorMax && slip > slipPeak)
				slip = slipPeak;
			}
		else
			{
			var maxSlip : float;
			
			if (tractionV3)			
				maxSlip = Mathf.Lerp(slipPeak, 0, Mathf.InverseLerp(motorMax-slipPeak, maxSpeed, v));
			else
				maxSlip = slipPeak;
			slip = maxSlip * motor / motorMax;
			}
			
		if (motorSlip < 0)
			slip = -slip;
		}
	else
		slip = motorSlip;		
		
	// Frenos
		
	if (autoBrakeMax && brakeSlip > slipPeak)
		brakeSlip = slipPeak;		
		
	brakeSlip = Mathf.Max(brakeSlip, handBrakeInput * slipMax);
	
// OJO: ESTO NO QUEDA BIEN! al darle slip maximo nuestro c�digo hace la correcci�n de la curva, pero aun as� mete un toque de sobrefuerza al cruzar el l�mite.
//	Esperar a ver si ponen el clamped friction mode, ah� deber�a funcionar bien.
//	if (brakeSlip >= slipMax)			// Al indicar Infinity cuando se bloquea la rueda, Hit.forwardSlip devolver� el valor de Slip absoluto del terreno,
//		brakeSlip = Mathf.Infinity;		// en la misma forma en que devuelve Hit.sidewaysSlip.	
//	else							
		if (motorInput == 0.0) brakeSlip += rollingFrictionSlip / brakeForceFactor;	

	Wheel.motorInput = slip;
	Wheel.brakeInput = brakeSlip;
	}
	


function CalculateSteerAngles()
	{
	// -------- Giro de direcci�n ruedas delanteras
		
	// Calcular el �ngulo de ambas ruedas para el giro perfecto. 
	// Se asume que las distancias entre las ruedas son iguales delante-detr�s e izquierda-derecha.
	// Valores m�ximos de giro: -90..+90
	
    var B = (WheelFL.transform.position-WheelFR.transform.position).magnitude;
    var H = (WheelFR.transform.position-WheelRR.transform.position).magnitude;	

	if (steerInput > 0.0)			// Giro a la derecha
		{
		m_steerR = steerMax * steerInput;
		if (m_steerR > m_steerAngleMax) m_steerR = m_steerAngleMax;
		m_steerL = Mathf.Rad2Deg * Mathf.Atan( 1.0 / (Mathf.Tan((90 - m_steerR) * Mathf.Deg2Rad) + B / H));
		}
	else if (steerInput < 0.0)		// Giro a la izquierda
		{
		m_steerL = steerMax * steerInput;
		if (m_steerL < -m_steerAngleMax) m_steerL = -m_steerAngleMax;
		
		m_steerR = -Mathf.Rad2Deg * Mathf.Atan( 1.0 / (Mathf.Tan((90 + m_steerL) * Mathf.Deg2Rad) + B / H));
		}
	else
		{
		m_steerL = 0;
		m_steerR = 0;
		}
	}