using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class Inimigo : MonoBehaviour
{
    [Header("Spawner(Apenas para spawners)")]
    public GameObject Tipo;
    public int quantidade = 3;

    //É possivel usar Scriptable Objects para essas variaveis
    [Header("Inimigo(Apenas para as prefabs dos inimigos)")]
    public int vidaMax = 30;
    [Range(0,10)]
    public int velocidade = 4;
    public int dano = 1;
    [Range(0,20f)]
    public float cooldownTimer = 5f;
    public Material atordoadoMaterial;
    public float TempoPraAtirar = .5f;
    public GameObject Projetil;
    public float ProjForc = 3f;
    public AudioSource audioAcertado;
    public AudioSource audioAtirar;
    public AudioSource audioAtordoado;
    public bool AtacarBool = true;
    
    private float projTemp;
    private bool somTocando;
    private bool atordoado;
    private int vidaAtual;
    private Transform playerPos;
    private Renderer renderer;
    private Material defaultMaterial;
    private NavMeshAgent NavMeshAgente;
    private float temp;
    private bool Spawnado = false;
    public static Inimigo _instance;
    

    private void Awake()
    {
        _instance = this;
    }
    private void Start()
    {
        //Encontrar o jogador
        var gObj = GameObject.FindWithTag("Track");
        playerPos = gObj.transform;
        

        //Aplicar vida máxima a atual
        vidaAtual = vidaMax;

        //Pegar os componentes do IA para depois executar o PathFinding
        if(this.gameObject.tag == "Inimigo")
        {
            NavMeshAgente = this.GetComponent<NavMeshAgent>();
            NavMeshAgente.speed = velocidade;
            defaultMaterial = this.GetComponent<Renderer>().material;
            renderer = this.GetComponent<Renderer>();
        }
        GerarInimigo();
    }

    private void Update()
    {
        
        Movimentar();
        AtacarPersonagem();
        Atordoar(cooldownTimer);
        AtordoarSom();

    }

    //Instanciando o inimigo em lugares aleatorios proximos ao GameObject
    private void GerarInimigo()
    {
        if(this.gameObject.tag != "Inimigo")
        {
            for(int i = 0; i < quantidade; i++)
            {
                Vector3 InimigoPosition = new Vector3(Random.Range(-5, 5) + this.transform.position.x, this.transform.position.y + 5, this.transform.position.z + Random.Range(-5, 5));
                Instantiate(Tipo, InimigoPosition, Quaternion.identity);
                
            }
        }
    }

    //Movimentar até o jogador caso esteja com vida senão função Atordoar é chamada
    private void Movimentar()
    {
        if(this.gameObject.tag == "Inimigo")
        {
            if (vidaAtual > 0 && playerPos != null && AtacarBool)
            {
                NavMeshAgente.SetDestination(playerPos.position);
            }
            else
            {
                Atordoar(cooldownTimer);
            }

        }
        
    }

    private void AtacarPersonagem()
    {
        if(this.gameObject.tag == "Inimigo" && Projetil != null && this.vidaAtual > 0 && playerPos != null && AtacarBool)
        {
            projTemp += Time.deltaTime;
            
            this.gameObject.transform.LookAt(playerPos.transform);
            var angle = transform.rotation.eulerAngles;
            angle.x = 0;
            angle.z = 0;
            transform.rotation = Quaternion.Euler(angle);
            
            if(projTemp > TempoPraAtirar)
            {
                audioAtirar.Play();
                GameObject InimigoProjetil = Instantiate(Projetil, transform.position, this.gameObject.transform.rotation);
                Rigidbody Rb = InimigoProjetil.GetComponent<Rigidbody>();
                Rb.AddForce(transform.forward * ProjForc, ForceMode.VelocityChange);
                projTemp = 0;
                Destroy(InimigoProjetil,2f);
            }
            
        }
        
    }

    //Mudar o material e a vida atual baseado em quanto tempo o inimigo está atordoado
    private void Atordoar(float cooldown)
    {
        if(this.gameObject.tag == "Inimigo" && this.vidaAtual <= 0)
        {   
            atordoado = true;
            renderer.material = atordoadoMaterial;
            NavMeshAgente.SetDestination(transform.position);
            if (cooldown > temp)
            {
                temp += Time.deltaTime;
            }
            else
            {
                atordoado = false;
                this.vidaAtual = vidaMax;
                renderer.material = defaultMaterial;
                temp = 0;
            }

        }
        
    }

    // Aplicar dano a vida atual quando chamado em qualquer script
    public void TomarDano(int dano)
    {
        this.vidaAtual -= dano;
        audioAcertado.Play();
    }

    private void AtordoarSom()
    {
        
        if(atordoado && !somTocando){
            somTocando = true;
            audioAtordoado.Play();
        }
        if(!atordoado){
            somTocando = false;
        }
    }
}