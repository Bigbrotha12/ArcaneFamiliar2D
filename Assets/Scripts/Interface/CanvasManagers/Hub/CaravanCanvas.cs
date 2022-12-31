using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Characters;
using Environment;
using TMPro;

public class CaravanCanvas : UICanvasBase
{
    [SerializeField] private TransporterSO _driverController;

    public override void OnEnable()
    {
        base.OnEnable();
        _driverController.Greet();
    }

     // index 0: Travel, index 1: Map, index 2: Exit, index 3: Disable map canvas, index 4: Disable planner canvas
    protected override void HandleButtonSelect(int index)
    {
        switch(index)
        {
            case 0:
                OpenMapPanel();
                break;
            case 1:
                
                break;
            case 2:
                ActivateCanvas(false);
                break;
            case 3:
                CloseMapPanel();
                break;
            case 4:
                CancelPlanTrip();
                break;
            default:
                Debug.Log("Invalid button index.");
                break;
        }
    }

    [SerializeField] private GameObject travelMapPanel;
    [SerializeField] private GameObject travelPlanner;
    [SerializeField] private GameObject tripPlannerConfirmButton;
    [SerializeField] private Transform locationContainer;
    [SerializeField] private GameObject locationButtonPrefab;
    [SerializeField] private GameObject currentLocationDescriptor;
    [SerializeField] private Transform locationMap;
    [SerializeField] private GameObject mapPin;
    [SerializeField] private GameObject routeLine;
    [SerializeField] private Sprite defaultIcon;

    private void OpenMapPanel()
    {
        travelMapPanel.SetActive(true);
        RefreshDestinations();
    }

    private void RefreshDestinations()
    {
        Debug.Log("Running.");
        foreach (Transform container in locationContainer)
        {
            Destroy(container.gameObject);
        }

        foreach (Transform container in locationMap)
        {
            Destroy(container.gameObject);
        }

        Debug.Log("Adding locations.");
        string[] options = { "Plan Trip", "Cancel" };
        foreach (LocationSO location in GameManager.Instance.Player.Atlas.LocationsKnown)
        {
            // Add to side list
            GameObject container = GameObject.Instantiate(locationButtonPrefab, locationContainer);
            container.transform.Find("LocationLabel").GetComponent<TMP_Text>().text = location.ObjectName;
            container.GetComponent<Button>().onClick.AddListener(() => { GameManager.Instance.UInterface.QueueObject(location, options, PlanTrip); });
            container.AddComponent<LocationTooltip>().SetTooltip
            (
                location,
                defaultIcon,
                currentLocationDescriptor.transform.Find("LocationName").GetComponent<TMP_Text>(),
                currentLocationDescriptor.transform.Find("LocationDescription").GetComponent<TMP_Text>(),
                currentLocationDescriptor.transform.Find("Border").Find("LocationIcon").GetComponent<Image>()
            );
            
            // Add map pins
            GameObject pin = GameObject.Instantiate(mapPin, locationMap);
            pin.transform.localPosition = new Vector3(location.MapCoordinate.x, location.MapCoordinate.y, 0);
            pin.GetComponent<Button>().onClick.AddListener(() => { GameManager.Instance.UInterface.QueueObject(location, options, PlanTrip); });
            pin.AddComponent<LocationTooltip>().SetTooltip
            (
                location,
                defaultIcon,
                currentLocationDescriptor.transform.Find("LocationName").GetComponent<TMP_Text>(),
                currentLocationDescriptor.transform.Find("LocationDescription").GetComponent<TMP_Text>(),
                currentLocationDescriptor.transform.Find("Border").Find("LocationIcon").GetComponent<Image>()
            );
        }
    }


    private void PlanTrip(IObjectHeader destination, int index)
    {
        if (index == 1 || destination is not LocationSO) { return; }
        
        LocationSO[] route = _driverController.FindRoute(destination as LocationSO);
        int travelCost = _driverController.RouteCost(route);
        
        // Update canvas
        travelPlanner.SetActive(true);
        travelPlanner.transform.Find("Border").Find("LocationPanel").Find("IconFrame").Find("LocationIcon").GetComponent<Image>().sprite = destination.Icon;
        travelPlanner.transform.Find("Border").Find("LocationPanel").Find("Name").GetComponent<TMP_Text>().text = destination.ObjectName;
        travelPlanner.transform.Find("Border").Find("LocationPanel").Find("Descriptor").GetComponent<TMP_Text>().text = destination.Description;
        tripPlannerConfirmButton.GetComponent<Button>().onClick.AddListener(() => { _driverController.StartTravel(destination as LocationSO, travelCost); });
        travelPlanner.transform.Find("Border").Find("TravelCost").Find("TravelCostText").GetComponent<TMP_Text>().text = travelCost.ToString() + " G";

        // Draw route on map 
        if(route is not null)
        {
            for (int i = 0; i < route.Length; i++)
            {
                Transform map = travelPlanner.transform.Find("Border").Find("MapPanel");
                GameObject pin = GameObject.Instantiate(mapPin, map);
                float scaleFactor = 0.8f;
                pin.transform.localPosition = new Vector3(route[i].MapCoordinate.x * scaleFactor, route[i].MapCoordinate.y * scaleFactor, 0);

                // if this is not destination, draw route line.
                if(i + 1 < route.Length)
                {
                    DrawLine(route[i].MapCoordinate, route[i + 1].MapCoordinate, map.Find("Lines"), scaleFactor);
                }
            }
        }
    }

    public void CancelPlanTrip()
    {
        tripPlannerConfirmButton.GetComponent<Button>().onClick.RemoveAllListeners();
        travelPlanner.SetActive(false);
    }

    private void CloseMapPanel()
    {
        travelMapPanel.SetActive(false);
    }

    private void DrawLine(Vector2 start, Vector2 end, Transform container, float scaleFactor = 1.0f)
    {
        // Find distance between two vectors
        Vector2 scaledStart = new Vector2(start.x * scaleFactor, start.y * scaleFactor);
        Vector2 scaledEnd = new Vector2(end.x * scaleFactor, end.y * scaleFactor);
        float distance = Vector2.Distance(scaledStart, scaledEnd);

        // Find angle between points
        float angle = Vector2.Angle(scaledStart, scaledEnd);

        // Draw line
        GameObject line = GameObject.Instantiate(routeLine, container);
        line.transform.localPosition = new Vector3(scaledStart.x, scaledStart.y, 0);
        line.GetComponent<RectTransform>().sizeDelta = new Vector2(distance, 3);

        line.transform.position += new Vector3(scaledEnd.x - scaledStart.x, scaledEnd.y - scaledStart.y, 0).normalized * distance;
        line.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}