using UnityEngine;
using System.Collections;
using System;

public class SPHUnity : MonoBehaviour {

	private SPHSimulation m_fluidSim;
	private Vector3 m_gravity;
	private ParticleSystem m_particleSystem;

	private CollisionResolver m_collisionSolver;

	private ParticleEmitter emitter;
	private ParticleRenderer prenderer;
	private Particle[] particles;
	private GameObject cam;
	private Material defaultmat;
	private Material noglowmat;

	public  Rect SimDomain = new Rect (0.1f, 0.1f, 6.1f, 6.1f);
	public Vector3 Grav = new Vector3 (0.0f, -9.81f, 0);
	public float gas = 1.205f;
	public float dampen = 0.001f;
	public float CellSpace;
	public float particleMass;

	public void Awake()
	{
		CellSpace = (SimDomain.width + SimDomain.height)/32.0f;
		particleMass = (CellSpace * 20.0f);
		m_gravity = new Vector3(0.0f, -9.81f,0.0f) * particleMass;
		m_fluidSim = new SPHSimulation (CellSpace, SimDomain);
		m_collisionSolver = new CollisionResolver ();
		m_collisionSolver.Bounciness = 0.2f;
		m_collisionSolver.Friction = 0.01f;

		m_particleSystem = new ParticleSystem ();
		float freq = 40;
		int maxPart = 100;
		m_particleSystem.MaxParticles = maxPart;
		m_particleSystem.MaxLife = (int)((double)maxPart / freq / 0.01f);

		FluidParticleEmitter emitter = new FluidParticleEmitter ();
		emitter.Position = new Vector3 (SimDomain.xMax / 2, SimDomain.yMax - SimDomain.yMax / 8,0.0f);
		emitter.VelocityMin = 2.5f;
		emitter.VelocityMax = 3.0f;
		emitter.Direction = new Vector3 (0.8f, -0.25f, 0.0f);
		emitter.Distribution = SimDomain.width * 0.1f;
		emitter.Frequency = freq;
		emitter.ParticleMass = particleMass;
		m_particleSystem.Emitters.Add (emitter);
	}

	public void Start()
	{
		emitter = (ParticleEmitter)gameObject.GetComponent (typeof(UnityEngine.ParticleEmitter));
		emitter.maxEmission = m_particleSystem.MaxParticles;
		emitter.minEmission = m_particleSystem.MaxParticles;
		emitter.maxSize = 0.1f;
		emitter.minSize = 0.1f;
		emitter.emit = false;

		prenderer = (ParticleRenderer)gameObject.AddComponent (typeof(ParticleRenderer));
		prenderer.material.color = new Color (1f, .817f, .39f, 1);
		defaultmat = (Material)Resources.Load("Default-Particle");
		noglowmat = (Material)Resources.Load("SoapBubble");

		cam = GameObject.Find ("Main Camera");
		timeleft = updateInterval;
	}

	public void FixedUpdate()
	{
		m_collisionSolver.Solve ();

		m_particleSystem.Update (0.01f);

		m_collisionSolver.Solve (ref m_particleSystem.Particles);

		m_fluidSim.Calculate (ref m_particleSystem.Particles, m_gravity, 0.01f);

		int d = m_particleSystem.Particles.Count - emitter.particles.Length;

		if (d > 0)
		{
			emitter.Emit (d);
		}

		particles = emitter.particles;

		FluidParticle p;

		for(int i = 0; i < m_particleSystem.Particles.Count;i++)
		{
			p = (FluidParticle) m_particleSystem.Particles[i];
			particles[i].position = new Vector3(p.Position.x,p.Position.y,p.Position.z);
			particles[i].color = Color.white;
			particles[i].energy = 1f;
		}
		emitter.particles = particles;
	}

	private float UItime = 0;
	private float UItimeNext = 0.5f;
	
	public void OnGUI() {
		
		GUI.Box(new Rect(0,0,220,80),"");
		
		GUI.Label(new Rect(11,0,80,20), fpsstr);

		m_particleSystem.MaxParticles =
			(int) GUI.HorizontalSlider(new Rect(10,25,200,20), m_particleSystem.MaxParticles, 0, 500);
		
		int pcount = m_particleSystem.Particles.Count;
		int pmax = m_particleSystem.MaxParticles;
		GUI.Label(new Rect(10, 35, 200, 20), pcount + " of " + pmax + " Particles");
		
		if (pcount > pmax) {
			for (int i = pcount-1; i >= pmax; i--) {
				m_particleSystem.Particles.RemoveAt(i);
			}
			emitter.particles = null;
		}
	}

	public  float updateInterval = 0.5F;
	
	private float accum   = 0; // FPS accumulated over the interval
	private int   frames  = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval
	private string fpsstr; // output string
	
	public void Update() {
		timeleft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		++frames;
		
		// Interval ended - update GUI text and start new interval
		if( timeleft <= 0.0 )
		{
			// display two fractional digits (f2 format)
			float fps = accum/frames;
			string format = System.String.Format("{0:F2} FPS",fps);
			fpsstr = format;
			
			timeleft = updateInterval;
			accum = 0.0F;
			frames = 0;
		}
		
	}
}
