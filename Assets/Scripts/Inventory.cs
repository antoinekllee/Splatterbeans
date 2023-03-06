using UnityEngine; 
using UnityEngine.UI; 

public class Inventory : MonoBehaviour
{
    [SerializeField]
    GameObject playerModel; 

    [SerializeField]
    GameObject inventoryMenu; 
    [SerializeField]
    GameObject mainMenu; 

    [SerializeField]
    public Material[] classes; 
    public static Material[] otherClasses; 
    public static Material currentClass; 

    [SerializeField]
    GameObject[] helms;
    public static GameObject equippedHelm; 
    
    [SerializeField]
    GameObject[] props;
    public static GameObject equippedProp; 

    [SerializeField]
    GameObject classMenu;
    [SerializeField]
    GameObject helmMenu;
    [SerializeField]
    GameObject propMenu;

    public static int GetClassIndex()
    {
        int index = 0;

        for (int i = 0; i < otherClasses.Length; i++)
        {
            if (otherClasses[i] == currentClass)
            {
                index = i;
                break;
            }
        }

        return index;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.SetInt("IsFirstSetup", 0);
        }
    }

    void Start() 
    {
        otherClasses = classes; 

        mainMenu.SetActive(true); 
        inventoryMenu.SetActive(false);

        if (PlayerPrefs.GetInt("IsFirstSetup") == 0)
        {
            PlayerPrefs.SetInt("ClassID", 0);
            PlayerPrefs.SetInt("HelmID", 0);
            PlayerPrefs.SetInt("PropID", 0);

            PlayerPrefs.SetInt("IsFirstSetup", 1); 
        }

        currentClass = classes[PlayerPrefs.GetInt("ClassID")];
        playerModel.GetComponent<MeshRenderer>().material = currentClass;

        equippedHelm = helms[PlayerPrefs.GetInt("HelmID")]; 
        equippedHelm.SetActive(true); 

        equippedProp = props[PlayerPrefs.GetInt("PropID")]; 
        equippedProp.SetActive(true); 
    }

    public void ToggleInventoryMenu()
    {
        inventoryMenu.SetActive(!inventoryMenu.activeSelf);
        mainMenu.SetActive(!mainMenu.activeSelf);
    }

    public void ToggleClassMenu()
    {
        classMenu.SetActive(true); 
        helmMenu.SetActive(false); 
        propMenu.SetActive(false); 
    }
    public void ToggleHelmMenu()
    {
        classMenu.SetActive(false);
        helmMenu.SetActive(true);
        propMenu.SetActive(false);
    }
    public void TogglePropMenu()
    {
        classMenu.SetActive(false);
        helmMenu.SetActive(false);
        propMenu.SetActive(true);
    }
    
    public void EquipClass(Material materialToEquip)
    {
        for (int i = 0; i < classes.Length; i++)
        {
            if (classes[i] == materialToEquip)
            {
                playerModel.GetComponent<MeshRenderer>().material = materialToEquip;
                currentClass = materialToEquip; 
                PlayerPrefs.SetInt("ClassID", i); 
                break; 
            }
        }
    }
    public void EquipHelm(GameObject helmToEquip)
    {
        for (int i = 0; i < helms.Length; i++)
        {
            if (helms[i] == helmToEquip)
            {
                equippedHelm.SetActive(false); 
                helmToEquip.SetActive(true);

                equippedHelm = helmToEquip;
                PlayerPrefs.SetInt("HelmID", i);

                break;
            }
        }
    }
    public void EquipProp(GameObject propToEquip)
    {
        for (int i = 0; i < props.Length; i++)
        {
            if (props[i] == propToEquip)
            {
                equippedProp.SetActive(false);
                propToEquip.SetActive(true);

                equippedProp = propToEquip;
                PlayerPrefs.SetInt("PropID", i);

                break;
            }
        }
    }
}
