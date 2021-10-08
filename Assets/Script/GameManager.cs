using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject ristorantiParent;
    public GameObject clientiParent;

    private List<GameObject> ristoranti = new List<GameObject>();
    private List<GameObject> clienti = new List<GameObject>();

    /*
    public class Luogo
    {
        protected string Food { get; set; }
        protected string Name { get; set; }

        public Luogo(string name, string food)
        {
            this.Name= name;
            this.Food = food;
        }
    }

    public class Ristorante : Luogo
    {
        private GameObject Destination { set; get; }

        public Ristorante(string name, string food, GameObject destination)
        {
            this.Name = name;
            this.Food = food;
            this.Destination = destination;
        }
    }
    */
    // Start is called before the first frame update
    void Start()
    {
        foreach (var ristorante in ristorantiParent.GetComponentsInChildren<Transform>())
        {
            if(ristorante.gameObject.layer == 12)
                ristoranti.Add(ristorante.gameObject);
        }
        //ristoranti.RemoveAt(0);

        foreach(var cliente in clientiParent.GetComponentsInChildren<Transform>())
        {
            if(cliente.gameObject.layer == 12)
                clienti.Add(cliente.gameObject);
        }
        //clienti.RemoveAt(0);

        foreach(GameObject ristorante in ristoranti)
        {
            ClienteAssegnato scriptCliente = ristorante.GetComponent<ClienteAssegnato>();

            if (clienti.Count != 0)
            {
                int i = Mathf.RoundToInt(Random.Range(0, clienti.Count - 1));
                scriptCliente.SetCliente(clienti[i]);
                scriptCliente.CalculateColor();
                clienti.RemoveAt(i);
            }

            if(scriptCliente.GetCliente() == null)
            {
                ristorante.SetActive(false);
            }

        }

        foreach(var ristorante in ristoranti)
        {
            if(ristorante.GetComponent<ClienteAssegnato>().GetCliente() != null)
                Debug.Log(ristorante.ToString() + " ha come cliente: " + ristorante.GetComponent<ClienteAssegnato>().GetCliente().ToString());
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
