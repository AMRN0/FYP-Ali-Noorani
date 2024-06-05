using UnityEngine;
using UnityEngine.AI;
using TMPro;

[RequireComponent(typeof(NavMeshAgent))]
public class Navmesh : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private LineRenderer line;
    public TMP_Dropdown startDropdown, endDropdown;
    NavMeshPath path = null;

    Vector3 start, end;
    private void Awake()
    {
        agent = this.GetComponent<NavMeshAgent>();
        start = GameObject.FindGameObjectWithTag(startDropdown.options[startDropdown.value].text).transform.position;
        end = GameObject.FindGameObjectWithTag(endDropdown.options[endDropdown.value].text).transform.position;

    }

    void Start()
    {
        agent.transform.position = start;
        agent.SetDestination(end);
        path = agent.path;
        agent.CalculatePath(agent.destination, path);
    }

    private void Update()
    {
        DrawPath();
        CheckDestination();
    }

    private void CheckDestination()
    {
        if (Vector3.Distance(agent.destination, agent.transform.position) < 1f)
        {
            agent.speed = 0;
        }
    }

    public void OnStartValueChange()
    {
        start = GameObject.FindGameObjectWithTag(startDropdown.options[startDropdown.value].text).transform.position;
        agent.Warp(start);
        DrawPath();
    }

    public void OnEndValueChange()
    {
        end = GameObject.FindGameObjectWithTag(endDropdown.options[endDropdown.value].text).transform.position;
        agent.SetDestination(end);
        DrawPath();
    }

    public void OnStart()
    {
        ToggleSpeed();
    }

    public void OpenLink()
    {
        Application.OpenURL("https://forms.office.com/e/HQq8Pm8B0G");
    }

    private void ToggleSpeed()
    {
        if (agent.speed > 0)
        {
            agent.speed = 0;
            return;
        }
        agent.speed = 1;
    }

    private void DrawPath()
    {
        agent.CalculatePath(agent.destination, path);
        path = agent.path;
        Vector3[] points = path.corners;
        line.positionCount = points.Length;
        line.SetPositions(points);
    }
}