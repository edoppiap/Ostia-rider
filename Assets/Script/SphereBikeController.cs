using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

//controller arcade "a sfera": il giocatore controlla la rotazione di una sfera invisibile che, rotolando sul terreno,
//muove il motorino. Il vecchio motorino (corpo, ruote su joint, SuspensionBikeController, Sgommata, AnimationController)
//resta attivo come "burattino fisico": è un corpo libero, tirato verso la sfera da forze (in orizzontale, in direzione
//e nel rollio) come da un ammortizzatore molto rigido, mentre beccheggio e sospensioni sono tutti della fisica. Gas e freno arrivano anche al vecchio controller,
//la cui spinta applicata sotto il baricentro produce impennate, inchiodate e rimbalzi veri.
//Va messo sul Motorino accanto a SuspensionBikeController. Se è disattivato non fa niente e il gioco usa il vecchio
//controller da solo.
public class SphereBikeController : MonoBehaviour, IBikeControls
{
    [Header("Sfera")]
    public float sphereRadius = .5f;
    public float sphereMass = 120f;
    [Tooltip("Attrito tra sfera e terreno: è quello che trasforma la rotazione in movimento")]
    public float groundFriction = 1f;
    [Tooltip("Layer considerati terreno (esclusi waypoints, Player, Car)")]
    public LayerMask groundLayers = ~((1 << 6) | (1 << 7) | (1 << 11));
    public float groundCheckDistance = .25f;
    [Tooltip("Rende visibile la sfera, per vedere come rotola")]
    public bool showSphere = false;

    [Header("Motore (m/s e m/s², riferiti al rotolamento)")]
    [Tooltip("16 m/s = circa 58 km/h")]
    public float maxSpeed = 16f;
    [Tooltip("Accelerazione da fermo, cala fino a 0 alla velocità massima")]
    public float acceleration = 10f;
    public float brakeDeceleration = 20f;
    [Tooltip("Rallentamento della rotazione senza gas né freno")]
    public float coastDeceleration = 3f;
    public float maxReverseSpeed = 4f;
    public float reverseAcceleration = 6f;
    [Tooltip("Per quanto tenere premuto il freno da fermi prima di partire in retromarcia")]
    public float reverseDelay = .3f;
    [Tooltip("Sotto questa velocità, senza gas, il motorino si ferma del tutto")]
    public float stopSpeed = .8f;

    [Header("Sterzo e grip")]
    [Tooltip("Gradi al secondo di rotazione a sterzo pieno")]
    public float maxYawRate = 140f;
    [Tooltip("Quanto sterza in funzione della velocità (0 = fermo, 1 = velocità massima)")]
    public AnimationCurve steerBySpeed = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(.2f, 1f), new Keyframe(1f, .65f));
    [Tooltip("Quanto in fretta lo sterzo arriva a fine corsa")]
    public float steerResponse = 6f;
    [Tooltip("Quanto in fretta la rotazione della sfera si riallinea al muso: basso = più derapata")]
    public float grip = 6f;
    [Tooltip("Grip a sterzo pieno e velocità massima")]
    public float driftGrip = 1.5f;
    [Tooltip("Velocità da cui il grip inizia a calare in curva")]
    public float driftMinSpeed = 6f;

    [Header("Anti-rimbalzo")]
    [Tooltip("Accelerazione extra verso il terreno quando si è a terra")]
    public float stickToGround = 20f;
    public float airGravityMultiplier = 2f;
    public float maxDepenetrationVelocity = 2f;
    [Tooltip("Rimbalzo della sfera su atterraggi e dossi: 0 = nessuno, 1 = elastico")]
    [Range(0f, 1f)]
    public float sphereBounce = .2f;
    [Tooltip("Le velocità verso l'alto sotto questo valore (m/s) sono micro-saltelli delle giunture e vengono tolte; sopra sono rimbalzi veri e restano")]
    public float microBounceSpeed = 1f;

    [Header("Burattino (il vecchio motorino agganciato alla sfera)")]
    [Tooltip("In derapata il motorino ruota con il muso più dentro la curva: 0 = nessuna esagerazione")]
    public float driftVisualAngle = .4f;
    [Tooltip("Angolo massimo aggiunto in derapata")]
    public float maxDriftVisualAngle = 35f;
    [Tooltip("Sotto questa velocità (m/s) la rotazione della derapata diventa la direzione del motorino, invece di sparire di colpo")]
    public float driftSettleSpeed = 1.5f;
    [Tooltip("Quanto in fretta il carrello si allinea alla pendenza del terreno")]
    public float alignToGroundSpeed = 8f;
    [Tooltip("Spinta del vecchio controller (che fa impennare) in partenza, rispetto al suo valore: cala man mano che la sfera smette di accelerare")]
    public float wheelieStrength = 2f;
    [Tooltip("La spinta dell'impennata resta piena fino a questa frazione della velocità massima, poi cala fino a zero: più alto = impennata più lunga")]
    [Range(0f, .95f)]
    public float wheelieHoldUntil = .6f;
    [Tooltip("Spinta del vecchio controller in frenata (fa inchiodare), rispetto al suo valore: si spegne quando la sfera è quasi ferma")]
    public float stoppieStrength = 1.5f;
    [Tooltip("Forza con cui il burattino torna dritto in beccheggio, uguale in tutte le direzioni (il vecchio stabilizzatore per un bug valeva 75 andando verso nord/sud e 30 verso est/ovest)")]
    public float pitchStabilizer = 30f;
    [Tooltip("Fine corsa del beccheggio del burattino (gradi, sia impennata sia inchiodata)")]
    public float maxPuppetPitch = 45f;
    [Tooltip("Rimbalzo quando il burattino arriva a fine corsa")]
    [Range(0f, 1f)]
    public float puppetPitchLimitBounce = .3f;
    [Tooltip("Quanto stretto il burattino segue la sfera in orizzontale (frequenza, 1/s): più alto = meno ritardo")]
    public float puppetFollowFrequency = 20f;
    [Tooltip("Quanto stretto il burattino segue la direzione e resta dritto di lato (frequenza, 1/s)")]
    public float puppetTurnFrequency = 25f;
    [Tooltip("Quanto il burattino è legato in altezza alla sfera (frequenza, 1/s): basso = in altezza fa quasi tutto la fisica")]
    public float puppetHeightFrequency = 4f;
    [Tooltip("Il burattino tiene solo i contatti che lo sostengono dal basso: sotto questo valore (normale.y) il contatto è un muro e viene ignorato")]
    [Range(0f, 1f)]
    public float puppetFloorNormal = .6f;
    [Tooltip("Molla delle sospensioni delle ruote del burattino (nella scena: 10000)")]
    public float puppetSuspensionSpring = 10000f;
    [Tooltip("Smorzamento delle sospensioni del burattino (nella scena: 10, quasi nullo, oscillava all'infinito)")]
    public float puppetSuspensionDamping = 200f;
    [Tooltip("Oltre questa inclinazione (gradi) o distanza dal carrello (m) il burattino viene rimesso in posizione")]
    public float puppetResetAngle = 70f;
    public float puppetResetDistance = 2.5f;

    [Header("Suono")]
    [Tooltip("Se tenendo il gas suona il minimo ma si supera questa velocità (m/s), riparte il suono dell'accelerazione")]
    public float engineResumeSpeed = 2f;

    [Header("Sgommate")]
    [Tooltip("Altezza della scia sopra la strada (m): a 0 la scia si confonde con l'asfalto e compaiono puntini (z-fighting)")]
    public float skidMarkHeight = .02f;

    [Header("Debug")]
    public bool showDebugHud = true;
    [Tooltip("Scrive nella Console, ogni quarto di secondo, i valori dell'HUD (temporaneo, per l'analisi)")]
    public bool logDebugValues = true;
    private float debugLogTimer = 0f;

    private SuspensionBikeController puppet;
    private Rigidbody[] puppetBodies;
    private Vector3[] puppetLocalPositions;
    private Quaternion[] puppetLocalRotations;
    private float puppetMass;
    private float puppetBaseThrust;
    private float currentThrust;
    private AnimationController puppetAnimation;
    private bool puppetAudioPrioritized = false;
    private float lastCarrierYaw;
    private Vector3 lastSphereVelocity;
    private Rigidbody rb;
    private Renderer sphereRenderer;
    private float puppetPitch = 0f;
    private Vector3 carrierPosition;
    private Quaternion carrierRotation;

    //collider del burattino, letti dal thread della fisica in OnContactModify (solo lettura dopo Awake)
    private static readonly HashSet<EntityId> puppetColliders = new HashSet<EntityId>();
    private static float floorNormalThreshold = .6f;
    private GameManager gameManager;
    private float rollCompensation = 1f;

    private int throttleInput = 0;
    private int steerInput = 0;
    private int throttle = 0;
    private float steer = 0f;
    private float yaw;
    private float driftYaw = 0f;
    private float brakeHoldTime = 0f;
    private bool reversing = false;
    private bool grounded = false;
    private Vector3 groundNormal = Vector3.up;
    private Vector3 carrierUp = Vector3.up;

    //misure per l'HUD
    private float forwardSpeed = 0f;
    private float spinSpeed = 0f;
    private float slipAngle = 0f;

    //i comandi muovono la sfera e arrivano anche al burattino (spinta sotto il baricentro, suono, animazioni)
    public void Accellera() { throttleInput = 1; puppet.Accellera(); }
    public void Deaccellera() { throttleInput = 0; puppet.Deaccellera(); }
    public void Frena() { throttleInput = -1; puppet.Frena(); }
    public void SterzaDx() { steerInput = 1; puppet.SterzaDx(); }
    public void SterzaSx() { steerInput = -1; puppet.SterzaSx(); }
    public void Desterza() { steerInput = 0; puppet.Desterza(); }

    void Awake()
    {
        //Awake viene chiamato anche se il componente è disattivato
        if (!enabled)
            return;

        puppet = GetComponent<SuspensionBikeController>();

        //punto di contatto sotto le ruote e direzione iniziale
        Vector3 frontBottom = WheelBottom(puppet.frontWheelRb);
        Vector3 backBottom = WheelBottom(puppet.backWheelRb);
        Vector3 contact = (frontBottom + backBottom) / 2f;
        contact.y = Mathf.Min(frontBottom.y, backBottom.y);
        yaw = Quaternion.LookRotation(Vector3.ProjectOnPlane(frontBottom - backBottom, Vector3.up)).eulerAngles.y;

        CreateSphere(contact);
        InitPuppetPose(contact);
        TunePuppetSuspension();

        //la direzione e la tenuta laterale del burattino le decide la sfera: la coppia di sterzo e la forza laterale
        //del vecchio controller la combatterebbero. Il resto (spinta sotto il baricentro, stabilizzatore, suoni) resta
        puppet.turnVelocity = 0f;
        puppet.lateralForce = 0f;
        //il raddrizzamento del vecchio stabilizzatore dipende dalla direzione (scala l'asse Z del mondo, non quello del
        //motorino): lo si spegne e lo si rifà in CouplePuppet. Il suo smorzamento della rotazione resta attivo
        puppet.torqueStabilizer = 0f;
        puppetBaseThrust = puppet.accellerationForce;
        puppet.accellerationForce = 0f; //la spinta la applica UpdatePuppetThrust
        puppetAnimation = GetComponent<AnimationController>();
        puppetMass = 0f;
        foreach (Rigidbody part in puppetBodies)
            puppetMass += part.mass;
        FilterPuppetContacts();
    }

    //il burattino tiene solo i contatti che lo sostengono dal basso: gli urti contro muri e ostacoli li fa la sfera.
    //Se li facesse anche lui, bloccato in orizzontale, verrebbe spinto in verticale e lanciato via
    void FilterPuppetContacts()
    {
        puppetColliders.Clear();
        floorNormalThreshold = puppetFloorNormal;
        foreach (Collider puppetCollider in GetComponentsInChildren<Collider>(true))
        {
            if (puppetCollider.isTrigger)
                continue;
            puppetCollider.hasModifiableContacts = true;
            puppetColliders.Add(puppetCollider.GetEntityId());
        }
        Physics.ContactModifyEvent += OnContactModify;
        Physics.ContactModifyEventCCD += OnContactModify;
    }

    void OnDestroy()
    {
        Physics.ContactModifyEvent -= OnContactModify;
        Physics.ContactModifyEventCCD -= OnContactModify;
    }

    //chiamato dalla fisica, anche su altri thread: niente API di Unity qui dentro
    static void OnContactModify(PhysicsScene scene, NativeArray<ModifiableContactPair> pairs)
    {
        for (int p = 0; p < pairs.Length; p++)
        {
            ModifiableContactPair pair = pairs[p];
            if (!puppetColliders.Contains(pair.colliderEntityId) && !puppetColliders.Contains(pair.otherColliderEntityId))
                continue;

            for (int i = 0; i < pair.contactCount; i++)
            {
                if (Mathf.Abs(pair.GetNormal(i).y) < floorNormalThreshold)
                    pair.IgnoreContact(i);
            }
        }
    }

    static Vector3 WheelBottom(Rigidbody wheelRb)
    {
        SphereCollider wheelCollider = wheelRb.GetComponent<SphereCollider>();
        return wheelCollider.transform.TransformPoint(wheelCollider.center) - Vector3.up * wheelCollider.radius * wheelCollider.transform.lossyScale.y;
    }

    void CreateSphere(Vector3 contact)
    {
        GameObject body = puppet.bodyRb.gameObject;
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = "MotorinoSphere";
        sphere.tag = body.tag; //Player: serve a DetectCollision e Destructible
        sphere.layer = body.layer; //Player: serve ai raycast del traffico
        sphere.transform.position = contact + Vector3.up * (sphereRadius + .02f);
        sphere.transform.localScale = Vector3.one * sphereRadius * 2f;
        sphereRenderer = sphere.GetComponent<Renderer>();
        sphereRenderer.enabled = showSphere;
        sphereRenderer.material.mainTexture = CheckerTexture();

        SphereCollider sphereCollider = sphere.GetComponent<SphereCollider>();
        sphereCollider.material = new PhysicsMaterial("MotorinoSphere")
        {
            dynamicFriction = groundFriction,
            staticFriction = groundFriction,
            bounciness = sphereBounce,
            frictionCombine = PhysicsMaterialCombine.Maximum,
            bounceCombine = PhysicsMaterialCombine.Maximum
        };

        //la sfera e il burattino occupano lo stesso spazio: non devono urtarsi
        foreach (Collider puppetCollider in GetComponentsInChildren<Collider>(true))
            Physics.IgnoreCollision(sphereCollider, puppetCollider);

        rb = sphere.AddComponent<Rigidbody>();
        rb.mass = sphereMass;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.maxAngularVelocity = 200f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.maxDepenetrationVelocity = maxDepenetrationVelocity;

        //quando la rotazione aumenta, l'attrito ne trasforma solo una parte in velocità (il resto resta come rotazione):
        //si compensa così i valori di accelerazione e frenata sono quelli reali
        float inertia = rb.inertiaTensor.x;
        if (inertia <= 0f)
            inertia = .4f * sphereMass * sphereRadius * sphereRadius; //sfera piena
        rollCompensation = 1f + sphereMass * sphereRadius * sphereRadius / inertia;
    }

    //scacchi arancioni e bianchi, per vedere la sfera ruotare quando è visibile
    static Texture2D CheckerTexture()
    {
        const int columns = 16, rows = 8;
        Texture2D texture = new Texture2D(columns, rows) { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Repeat };
        Color orange = new Color(1f, .45f, 0f);
        for (int x = 0; x < columns; x++)
            for (int y = 0; y < rows; y++)
                texture.SetPixel(x, y, (x + y) % 2 == 0 ? orange : Color.white);
        texture.Apply();
        return texture;
    }

    //il "carrello" è la posa (punto di contatto sotto la sfera, direzione) a cui è legato il burattino:
    //si salva dove sta ogni pezzo del burattino rispetto a lui
    void InitPuppetPose(Vector3 contact)
    {
        carrierPosition = contact;
        carrierRotation = Quaternion.Euler(0f, yaw, 0f);
        lastCarrierYaw = yaw;

        puppetBodies = new[] { puppet.bodyRb, puppet.frontWheelRb, puppet.backWheelRb };
        puppetLocalPositions = new Vector3[puppetBodies.Length];
        puppetLocalRotations = new Quaternion[puppetBodies.Length];
        Quaternion toCarrier = Quaternion.Inverse(carrierRotation);
        for (int i = 0; i < puppetBodies.Length; i++)
        {
            puppetLocalPositions[i] = toCarrier * (puppetBodies[i].position - carrierPosition);
            puppetLocalRotations[i] = toCarrier * puppetBodies[i].rotation;
        }
    }

    //le molle delle ruote nella scena sono quasi senza smorzamento: il burattino oscillava mentre la sfera no
    void TunePuppetSuspension()
    {
        foreach (Rigidbody wheel in new[] { puppet.frontWheelRb, puppet.backWheelRb })
        {
            ConfigurableJoint wheelJoint = wheel.GetComponent<ConfigurableJoint>();
            if (wheelJoint == null)
                continue;
            JointDrive drive = wheelJoint.yDrive;
            drive.positionSpring = puppetSuspensionSpring;
            drive.positionDamper = puppetSuspensionDamping;
            wheelJoint.yDrive = drive;
        }
    }

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        grounded = Physics.Raycast(rb.position, Vector3.down, out RaycastHit hit, sphereRadius + groundCheckDistance, groundLayers, QueryTriggerInteraction.Ignore);
        groundNormal = grounded ? hit.normal : Vector3.up;

        bool inPlay = gameManager.IsInPlay();
        throttle = inPlay ? throttleInput : 0;

        Vector3 forward = Vector3.ProjectOnPlane(Quaternion.Euler(0f, yaw, 0f) * Vector3.forward, groundNormal).normalized;
        Vector3 right = Vector3.Cross(groundNormal, forward);
        //girando attorno a questo asse la sfera rotola in avanti
        Vector3 rollAxis = right;
        Vector3 velocity = rb.linearVelocity;
        Vector3 spin = rb.angularVelocity;

        forwardSpeed = Vector3.Dot(velocity, forward);
        spinSpeed = Vector3.Dot(spin, rollAxis) * sphereRadius;

        if (grounded)
        {
            //anti-rimbalzo: nessuna velocità che stacca dal terreno, più una spinta verso di esso
            //anti-rimbalzo solo sui micro-saltelli: i rimbalzi veri (atterraggi, dossi presi forte) restano
            float away = Vector3.Dot(velocity, groundNormal);
            if (away > 0f && away < microBounceSpeed)
                velocity -= groundNormal * away;
            velocity -= groundNormal * stickToGround * dt;

            //il giocatore controlla la rotazione: gas = la sfera gira più in fretta, freno = gira più piano
            float spinAccel = SpinAcceleration(dt, out bool mayCrossZero) * rollCompensation;
            if (!mayCrossZero && spinAccel * spinSpeed < 0f && Mathf.Abs(spinAccel) * dt > Mathf.Abs(spinSpeed))
                spinAccel = -spinSpeed / dt;
            spin += rollAxis * (spinAccel / sphereRadius) * dt;

            //grip: la rotazione rimasta dalla direzione precedente si riallinea al muso poco a poco,
            //intanto la sfera continua a rotolare di traverso -> derapata
            float drift = Mathf.Abs(steer) * Mathf.Clamp01((Mathf.Abs(forwardSpeed) - driftMinSpeed) / Mathf.Max(.01f, maxSpeed - driftMinSpeed));
            float currentGrip = Mathf.Lerp(grip, driftGrip, drift);
            Vector3 aroundNormal = groundNormal * Vector3.Dot(spin, groundNormal);
            Vector3 sideways = spin - rollAxis * Vector3.Dot(spin, rollAxis) - aroundNormal;
            spin -= sideways * (1f - Mathf.Exp(-currentGrip * dt));
            spin -= aroundNormal; //la trottola su se stessa non muove niente, la si toglie

            //arresto completo sotto una certa velocità, anche in pendenza
            if (throttle == 0 && Vector3.ProjectOnPlane(velocity, groundNormal).magnitude < stopSpeed && Mathf.Abs(spinSpeed) < stopSpeed)
            {
                velocity -= Vector3.ProjectOnPlane(velocity, groundNormal);
                spin = Vector3.zero;
            }
        }
        else
        {
            velocity += Physics.gravity * (airGravityMultiplier - 1f) * dt;
        }

        rb.linearVelocity = velocity;
        rb.angularVelocity = spin;

        //sterzo: nullo da fermi, inverso in retromarcia, niente sterzo in aria
        steer = Mathf.MoveTowards(steer, inPlay ? steerInput : 0, steerResponse * dt);
        if (grounded)
        {
            float speed01 = Mathf.Clamp01(Mathf.Abs(forwardSpeed) / maxSpeed);
            yaw += steer * maxYawRate * steerBySpeed.Evaluate(speed01) * Mathf.Sign(forwardSpeed) * dt;
        }

        Vector3 planarVelocity = Vector3.ProjectOnPlane(velocity, groundNormal);
        //angolo tra la direzione di marcia e il muso (o la coda, in retromarcia): così resta piccolo anche andando indietro
        Vector3 travelForward = forwardSpeed < 0f ? -forward : forward;
        slipAngle = planarVelocity.magnitude > 1f ? Vector3.SignedAngle(travelForward, planarVelocity, groundNormal) : 0f;

        UpdatePuppetThrust();
        MoveCarrier(dt);
    }

    //la spinta che fa impennare o inchiodare: stessa forza, punto e direzione di quella del vecchio controller
    //(in avanti, sotto il baricentro), ma applicata da qui:
    //- segue l'accelerazione vera: forte in partenza e in frenata, cala man mano che la sfera smette di accelerare
    //  o rallentare (con una spinta costante il burattino trovava un equilibrio di circa 5° e restava fermo lì);
    //- "a terra" lo decide la sfera: il vecchio controller lo controlla con un raggio di 1 m dal corpo, che oltre
    //  i 25-30° di impennata non arriva più a terra, e la spinta si spegneva proprio al culmine
    void UpdatePuppetThrust()
    {
        float strength = 0f;
        currentThrust = 0f;
        if (throttle > 0)
            strength = wheelieStrength * Mathf.Clamp01((1f - spinSpeed / maxSpeed) / (1f - wheelieHoldUntil));
        else if (throttle < 0 && !reversing)
            strength = -stoppieStrength * Mathf.Clamp01((spinSpeed - stopSpeed) / 2f);

        if (!grounded || strength == 0f)
            return;
        currentThrust = puppetBaseThrust * strength;
        Transform body = puppet.bodyRb.transform;
        puppet.bodyRb.AddForceAtPosition(body.forward * puppetBaseThrust * strength, body.position - body.up * puppet.applicationDeltaPoint);
    }

    //il carrello va dove sarà la sfera al prossimo passo fisico, così il burattino non resta indietro
    void MoveCarrier(float dt)
    {
        //in derapata il muso punta ancora più dentro la curva rispetto a dove scivola: la coda va fuori
        //quasi fermi la rotazione della derapata diventa la nuova direzione: il motorino resta orientato come lo si vede
        //e ripartendo va dove punta il muso, invece di raddrizzarsi di scatto
        if (Vector3.ProjectOnPlane(rb.linearVelocity, groundNormal).magnitude < driftSettleSpeed)
        {
            yaw += driftYaw;
            driftYaw = 0f;
        }
        else
        {
            float driftYawTarget = Mathf.Clamp(-slipAngle * driftVisualAngle, -maxDriftVisualAngle, maxDriftVisualAngle);
            driftYaw = Mathf.Lerp(driftYaw, driftYawTarget, 1f - Mathf.Exp(-8f * dt));
        }
        carrierUp = Vector3.Slerp(carrierUp, grounded ? groundNormal : Vector3.up, 1f - Mathf.Exp(-alignToGroundSpeed * dt));

        Vector3 nextSpherePosition = rb.position + rb.linearVelocity * dt;
        carrierPosition = nextSpherePosition - carrierUp * sphereRadius;
        carrierRotation = Quaternion.FromToRotation(Vector3.up, carrierUp) * Quaternion.Euler(0f, yaw + driftYaw, 0f);

        CouplePuppet(dt);
        ResetPuppetIfBroken();
    }

    //il burattino è un corpo libero tirato verso la sfera da forze, come da un ammortizzatore molto rigido:
    //niente posizioni imposte (rompevano la sua inerzia e lo rendevano scattoso) e niente joint (troppo elastici).
    //In orizzontale, in direzione e nel rollio segue la sfera; il beccheggio è tutto della fisica
    void CouplePuppet(float dt)
    {
        Rigidbody body = puppet.bodyRb;
        Vector3 up = carrierRotation * Vector3.up;
        Vector3 groundPoint = rb.position - carrierUp * sphereRadius;

        //accelerazione della sfera, data anche al burattino come anticipo così non resta indietro
        Vector3 sphereAcceleration = (rb.linearVelocity - lastSphereVelocity) / dt;
        lastSphereVelocity = rb.linearVelocity;

        //posizione: forza applicata al baricentro, quindi non aggiunge né toglie impennata
        Vector3 error = groundPoint + carrierRotation * puppetLocalPositions[0] - body.position;
        Vector3 relativeVelocity = rb.linearVelocity - body.linearVelocity;
        float follow = puppetFollowFrequency;
        Vector3 horizontal = Vector3.ProjectOnPlane(follow * follow * error + 2f * follow * relativeVelocity + sphereAcceleration, up);
        float height = puppetHeightFrequency;
        float vertical = Vector3.Dot(height * height * error + 2f * height * relativeVelocity, up);
        body.AddForce((horizontal + up * vertical) * puppetMass);

        //nei salti la sfera cade con la gravità extra: la stessa va data al burattino, o resterebbe sospeso
        if (!grounded)
        {
            foreach (Rigidbody part in puppetBodies)
                part.AddForce(Physics.gravity * (airGravityMultiplier - 1f), ForceMode.Acceleration);
        }

        //rotazione, nel sistema del carrello: x = beccheggio (libero), y = direzione, z = rollio
        Quaternion deviation = Quaternion.Inverse(carrierRotation) * body.rotation * Quaternion.Inverse(puppetLocalRotations[0]);
        Vector3 bodyForward = deviation * Vector3.forward;
        Vector3 bodyUp = deviation * Vector3.up;
        float yawError = Mathf.Atan2(bodyForward.x, bodyForward.z);
        float rollError = Vector3.SignedAngle(Vector3.ProjectOnPlane(Vector3.up, bodyForward), Vector3.ProjectOnPlane(bodyUp, bodyForward), bodyForward) * Mathf.Deg2Rad;
        puppetPitch = Mathf.Atan2(bodyForward.y, new Vector2(bodyForward.x, bodyForward.z).magnitude) * Mathf.Rad2Deg;

        //raddrizzamento in beccheggio, uguale in tutte le direzioni (stessa formula del vecchio stabilizzatore)
        body.AddTorque(carrierRotation * Vector3.right * (body.mass * pitchStabilizer * Mathf.Sin(puppetPitch * Mathf.Deg2Rad)));

        float carrierYaw = yaw + driftYaw;
        float targetYawRate = Mathf.DeltaAngle(lastCarrierYaw, carrierYaw) * Mathf.Deg2Rad / dt;
        lastCarrierYaw = carrierYaw;

        Vector3 spin = Quaternion.Inverse(carrierRotation) * body.angularVelocity;
        //fine corsa del beccheggio, con un piccolo rimbalzo (rotazione positiva attorno a x = muso giù)
        if ((puppetPitch > maxPuppetPitch && spin.x < 0f) || (puppetPitch < -maxPuppetPitch && spin.x > 0f))
        {
            spin.x = -spin.x * puppetPitchLimitBounce;
            body.angularVelocity = carrierRotation * spin;
        }

        float turn = puppetTurnFrequency;
        Vector3 angularAcceleration = new Vector3(
            0f,
            turn * turn * -yawError + 2f * turn * (targetYawRate - spin.y),
            turn * turn * -rollError + 2f * turn * -spin.z);
        body.AddTorque(carrierRotation * angularAcceleration, ForceMode.Acceleration);
    }

    //rete di sicurezza: se nonostante tutto il burattino finisce in una posa assurda, viene rimesso sul carrello
    void ResetPuppetIfBroken()
    {
        Rigidbody body = puppet.bodyRb;
        Vector3 bodyStart = carrierPosition + carrierRotation * puppetLocalPositions[0];
        float tilt = Vector3.Angle(body.rotation * (Quaternion.Inverse(puppetLocalRotations[0]) * Vector3.up), carrierRotation * Vector3.up);
        if (tilt < puppetResetAngle && Vector3.Distance(body.position, bodyStart) < puppetResetDistance)
            return;

        for (int i = 0; i < puppetBodies.Length; i++)
        {
            Rigidbody part = puppetBodies[i];
            Vector3 position = carrierPosition + carrierRotation * puppetLocalPositions[i];
            Quaternion rotation = carrierRotation * puppetLocalRotations[i];
            part.transform.SetPositionAndRotation(position, rotation);
            part.position = position;
            part.rotation = rotation;
            part.linearVelocity = rb.linearVelocity;
            part.angularVelocity = Vector3.zero;
        }
    }

    void Update()
    {
        sphereRenderer.enabled = showSphere;
        PrioritizePuppetAudio();
        ResumeEngineSound();

        //AnimationController fa girare la ruota dietro in base alla spinta del vecchio controller, che ora cala con la
        //velocità: si aggiunge la differenza così la ruota gira come prima invece di fermarsi andando forte
        if (puppetAnimation != null && puppetAnimation.enabled && puppetAnimation.backWheelMesh != null && gameManager.IsInPlay())
            puppetAnimation.backWheelMesh.Rotate(Vector3.right, (puppetBaseThrust * throttleInput - puppet.GetAccellerationForce()) * Time.deltaTime);

        debugLogTimer -= Time.unscaledDeltaTime;
        if (logDebugValues && Time.timeScale > 0f && gameManager.IsInPlay() && debugLogTimer <= 0f)
        {
            debugLogTimer = .25f;
            Debug.Log($"[BikeDebug] t={Time.time:F2} gas={throttleInput} steer={steer:F2} speed={forwardSpeed * 3.6f:F0}kmh " +
                $"spin={spinSpeed * 3.6f:F0}kmh sphereGround={grounded} pitch={puppetPitch:F1} " +
                $"puppetGround={puppet.IsGrounded()} thrust={currentThrust:F0} " +
                $"heading={Mathf.Repeat(yaw + driftYaw, 360f):F0} slip={slipAngle:F0}");
        }
    }

    //accelerazione della rotazione (in m/s² di rotolamento): gas, freno, retromarcia e rallentamento naturale
    float SpinAcceleration(float dt, out bool mayCrossZero)
    {
        mayCrossZero = false;

        if (throttle > 0)
        {
            brakeHoldTime = 0f;
            reversing = false;
            mayCrossZero = true;
            //se girava all'indietro prima frena, poi accelera
            return spinSpeed < 0f ? brakeDeceleration : acceleration * Mathf.Clamp01(1f - spinSpeed / maxSpeed);
        }

        if (throttle < 0)
        {
            if (!reversing && spinSpeed > stopSpeed)
            {
                brakeHoldTime = 0f;
                return -brakeDeceleration;
            }

            //fermo con il freno premuto: dopo reverseDelay parte la retromarcia
            brakeHoldTime += dt;
            if (brakeHoldTime >= reverseDelay)
                reversing = true;
            mayCrossZero = reversing;
            return reversing ? -reverseAcceleration * Mathf.Clamp01(1f + spinSpeed / maxReverseSpeed) : -spinSpeed / dt;
        }

        brakeHoldTime = 0f;
        reversing = false;
        return -Mathf.Sign(spinSpeed) * coastDeceleration;
    }

    //i circa 70 motori del traffico (in loop, udibili fino a 500 m) superano il limite di 32 suoni reali di Unity, che
    //silenzia quelli meno importanti: con la stessa priorità (128) poteva toccare al motore del motorino, soprattutto
    //durante i crossfade tra le clip quando il suo volume cala. Priorità massima a motore e sgommate del motorino.
    //Le AudioSource del motore le crea il vecchio controller nel suo Start, quindi si aspetta che esistano
    void PrioritizePuppetAudio()
    {
        if (puppetAudioPrioritized)
            return;
        AudioSource[] engine = puppet.bodyRb.GetComponents<AudioSource>();
        if (engine.Length == 0)
            return;
        foreach (AudioSource source in engine)
            source.priority = 0;
        foreach (Rigidbody wheel in new[] { puppet.frontWheelRb, puppet.backWheelRb })
            foreach (AudioSource source in wheel.GetComponents<AudioSource>())
                source.priority = 1;
        puppetAudioPrioritized = true;
    }

    //il vecchio controller cambia clip solo quando si preme/lascia un tasto o quando una clip non in loop finisce.
    //Tenendo il gas contro un muro, la clip di accelerazione finisce da fermi e parte il minimo in loop: ripartendo
    //senza lasciare il tasto restava il minimo. Se suona il minimo ma con il gas il motorino si muove davvero, si
    //rifà partire l'accelerazione come se il tasto fosse stato ripremuto (velocità reale, non la sfera che può
    //girare a vuoto contro il muro)
    void ResumeEngineSound()
    {
        if (throttleInput <= 0 || !gameManager.IsInPlay() || forwardSpeed < engineResumeSpeed)
            return;

        bool idling = false;
        foreach (AudioSource source in puppet.bodyRb.GetComponents<AudioSource>())
        {
            if (!source.isPlaying)
                continue;
            //accelerazione o velocità massima già in corso (anche in dissolvenza): niente da fare,
            //altrimenti durante il crossfade si richiamerebbe Accellera di continuo
            if (source.clip == puppet.accellerating || source.clip == puppet.topSpeed)
                return;
            if (source.clip == puppet.idle)
                idling = true;
        }
        if (idling)
            puppet.Accellera();
    }

    //la scia della vecchia Sgommata sta all'altezza del fondo della ruota, cioè sulla strada: le due superfici si
    //contendono i pixel e dentro la scia compaiono puntini. Ogni frame la si porta appena sopra il terreno
    void LateUpdate()
    {
        foreach (Rigidbody wheel in new[] { puppet.frontWheelRb, puppet.backWheelRb })
        {
            Sgommata skid = wheel.GetComponent<Sgommata>();
            if (skid == null || skid.trailPrefab == null)
                continue;
            SphereCollider wheelCollider = wheel.GetComponent<SphereCollider>();
            float reach = wheelCollider.radius * wheelCollider.transform.lossyScale.y + .3f;
            if (Physics.Raycast(wheel.transform.TransformPoint(wheelCollider.center), Vector3.down, out RaycastHit hit, reach, groundLayers, QueryTriggerInteraction.Ignore))
                skid.trailPrefab.transform.position = hit.point + hit.normal * skidMarkHeight;
        }
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        if (!showDebugHud)
            return;

        GUIStyle style = new GUIStyle(GUI.skin.label) { fontSize = 16 };
        GUI.Box(new Rect(10, 10, 360, 170), GUIContent.none);
        GUI.Label(new Rect(20, 15, 350, 165),
            $"Velocità: {forwardSpeed * 3.6f:F0} km/h\n" +
            $"Rotolamento: {spinSpeed * 3.6f:F0} km/h\n" +
            $"A terra: {(grounded ? "sì" : "no")}\n" +
            $"Derapata: {slipAngle:F0}°\n" +
            $"Impennata: {puppetPitch:F0}°\n" +
            $"Spinta impennata: {currentThrust:F0}\n" +
            $"Gas: {throttleInput}   Sterzo: {steer:F2}" +
            (reversing ? "\nRETROMARCIA" : ""), style);
    }
#endif
}
