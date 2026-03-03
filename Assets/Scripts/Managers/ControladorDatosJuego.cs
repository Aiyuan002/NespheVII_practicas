using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;


public class ControladorDatosJuego : MonoBehaviour
{

    public GameObject jugador;
    public GameObject UI;
    public UIController scriptUI;
    public string archivoDeGuardado;
    public DatosJuego datosJuego = new DatosJuego();

    OnTriggerEnterFuego scriptFuego;
    public GameObject fuego;

    public bool primerCargado = false;

    public float lifes, currentHealth, currentEnergy;
    private void Awake()
    {
        archivoDeGuardado = Application.dataPath + "/datos juego.json";

        jugador = GameObject.FindGameObjectWithTag("Player");

       // GuardarDatos();
    }

    private void Start()
    {
        scriptFuego = fuego.GetComponent<OnTriggerEnterFuego>();

        lifes = UI.GetComponent<UIController>().lifes;
        currentEnergy = UI.GetComponent<UIController>().currentEnergy;
        currentHealth = UI.GetComponent<UIController>().currentHealth;
        scriptUI = UI.GetComponent<UIController>();
           // CargarDatos(); 
    }

    private void Update()
    {
        if (scriptFuego.guardar == true)
        {
            GuardarDatos();
        }
    }

    private void CargarDatos() 
    {
        if (File.Exists(archivoDeGuardado))
        {
            string contenido = File.ReadAllText(archivoDeGuardado);
            datosJuego = JsonUtility.FromJson<DatosJuego>(contenido);

            Debug.Log("Posición jugador : " + datosJuego.posicion);

            jugador.transform.position = datosJuego.posicion;
            scriptUI.lifes = datosJuego.vida;
            scriptUI.currentEnergy = datosJuego.energy;
            scriptUI.currentHealth = datosJuego.health;
            scriptUI.healthSlider.value = datosJuego.health;
            scriptUI.energySlider.value = datosJuego.energy;
            scriptUI.haGuardado = datosJuego.guardado;
        }
        else
        {
            Debug.Log("No existe");
        }
    }
    public void GuardarDatos()
    {
        UI.GetComponent<UIController>().haGuardado = true;
        DatosJuego nuevosDatos = new DatosJuego()
        {
            posicion = jugador.transform.position,

            //vida = lifes,
            //energy = currentEnergy,
            //health = currentHealth,
            vida = UI.GetComponent<UIController>().lifes,
            energy = UI.GetComponent<UIController>().currentEnergy,
            health = UI.GetComponent<UIController>().currentHealth,
            guardado = UI.GetComponent<UIController>().haGuardado,
        };
        Debug.Log(UI.GetComponent<UIController>().currentHealth);
        
        string cadenaJSON = JsonUtility.ToJson(nuevosDatos);

        File.WriteAllText(archivoDeGuardado, cadenaJSON);

        primerCargado = true;

        Debug.Log("Archivo Guardado");
    }
}
