using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SerializableVector3 = ItemsManager.SerializableVector3;

public class CablesNetwork : MonoBehaviour {

    public class CNetworkElementInfos
    {
        public float[][][] elementsInfos = new float[][][]//this defines data relative to Cables Networks Elements
        {
            new float[][]//cables
            {
                new float[]//transfer speed
                {
                    1,
                    10,
                    100,
                },
                new float[]//transfer efficiency (applyed to each element)
                {
                    0.5f,
                    0.75f,
                    1f,
                },
            }
        };
    }

    public class CNetworkElement
    {
        public int id;//Id of the element (cable, input, output, ...)
        public int type;//type of element (iron cable, copper cable, gold cable, ..), this define it's properties
        public GameObject elementObj;//only used for non-linking elements (machines)
        public SerializableVector3 position;//global (world-relative) position of the element
        public List<CNetworkElementLink> links = new List<CNetworkElementLink>();//list of elements linked to this one
        public int[] linksBinary = new int[6] { 0, 0, 0, 0, 0, 0 };//used to define mesh (display link or not)
        public List<CNetworkElement> neighBorsElements = new List<CNetworkElement>();//list of elements that could be linked to this one (neighbors elements)

        public CNetworkElement(int _id, int _type, SerializableVector3 pos)
        {
            id = _id;
            type = _type;
            position = pos;
        }
    }

    public class CNetworkElementLink
    {
        public CNetworkElement linkedTo;//linked element
        public int linkType;//linked as ? (input, output) | this determines the direction of the transfer
    }

    public class CNetworkInputElement
    {
        public bool continuousInput = false;
        public float nextInputTime = Mathf.Infinity;
        public int inputType;
        public float inputAmount;
        public CNetworkElement element;

        public CNetworkInputElement(bool continuous, float delay, int type, float amount, CNetworkElement _element)
        {
            continuousInput = continuous;
            nextInputTime = delay;
            inputType = type;
            inputAmount = amount;
            element = _element;
        }
    }

    public class CNetworkOutputElement
    {
        public bool continuousOutput = false;
        public float nextOutputTime = Mathf.Infinity;
        public int outputType;
        public float outputAmount;
        public CNetworkElement element;

        public CNetworkOutputElement(bool continuous, float delay, int type, float amount, CNetworkElement _element)
        {
            continuousOutput = continuous;
            nextOutputTime = delay;
            outputType = type;
            outputAmount = amount;
            element = _element;
        }
    }

    public class CNetwork
    {
        public List<CNetworkElement> networkElements;//network elements 
        public List<CNetworkInputElement>[] networkInputElements;//network input elements groupms (energy, fluid, ressource, information) | 0=electricity
        public List<CNetworkOutputElement>[] networkOutputElements;//network output elements 
        public List<CNetworkElement> networkLinkingElements;//network linking elements 

        public CNetwork ()//initialize network variables
        {
            networkElements = new List<CNetworkElement>();
            networkInputElements = new List<CNetworkInputElement>[1];
            networkOutputElements = new List<CNetworkOutputElement>[1];
            networkLinkingElements = new List<CNetworkElement>();
        }
    }

    public void SplitNetwork(CNetwork _network)//called when a linking element is removed
    {

    }

    public void FusionNetworks(CNetwork _network1, CNetwork _network2)//called when a DIRECT link between 2 networks is created (remote links will be managed by an external class)
    {

    }

    public void AddElementToNetwork(CNetworkElement _element, CNetwork _network)
    {
        _network.networkElements.Add(_element);
        if(_element.id == 1)//test machine1
        {
            CNetworkOutputElement outputElement = new CNetworkOutputElement(true, 0, 0, 0.05f, _element);
            _network.networkOutputElements[0].Add(outputElement);
        }
        if (_element.id == 2)//test machine2
        {
            CNetworkInputElement inputElement = new CNetworkInputElement(true, 0, 0, 0.05f, _element);
            _network.networkInputElements[0].Add(inputElement);
        }
    }

    public void RemoveElementToNetwork(CNetworkElement _element, CNetwork _network)
    {

    }

    public void ChangeLink(CNetworkElement _element, CNetworkElementLink _linkedElement)//called to define an element's link
    {

    }

    public void GenNetworkMesh(CNetwork _network)//generate mesh and render it
    {
        if (GetComponent<MeshFilter>())
        {
            DestroyImmediate(GetComponent<MeshFilter>());
        }
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        Mesh mesh = filter.mesh;
        mesh.Clear();

        //gen center cube
        //gen each link that exist as a rectangle of width=2cubewidth, height=depth=cubewidth
    }

    public void Input(CNetworkElement _element, CNetwork _network, int inputType, float amount, bool continuous = false)
    {
        if (_element.id == 1)
        {
            _element.elementObj.GetComponent<Machine>().Output(inputType, amount);
        }
    }

    public void Output(CNetworkElement _element, CNetwork _network, int outputType, float amount, bool continuous = false)
    {
        if(_element.id == 1)
        {
            _element.elementObj.GetComponent<Machine>().Input(outputType, amount);
        }
    }

    ///////////////////// Remove when tests are done

    float nextOutputTime = 10000;
    CNetworkElement temp;
    CNetwork net;

    private void Start()
    {
        net = new CNetwork();
        if(GameObject.Find("TESTmachine1"))
        {
            temp = new CNetworkElement(1, 0, Vector3.zero);
            temp.elementObj = GameObject.Find("TESTmachine1");
            AddElementToNetwork(temp, net);
        }
    }

    private void Update()
    {
        for (int i = 0; i < net.networkInputElements.Length; i++)
        {
            int divider = net.networkInputElements[i].Count;
            foreach (CNetworkInputElement inputElement in net.networkInputElements[i])
            {

            }
        }

        if(Time.time >= nextOutputTime)
        {
            Output(temp, net, 0, 12f);
        }
    }
}
