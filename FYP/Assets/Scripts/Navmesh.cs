using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NavMeshAgent))]
public class Navmesh : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private TMP_Text liftText;
    [SerializeField] GameObject[] stairBlockers;
    [SerializeField] private Toggle stairsToggle;

    public TMP_Dropdown startDropdown, endDropdown;
    public GameObject settingsPanel;
    public Sprite[] pausePlayImages;
    public Image image;
    public Button settingsButton, forwardButton, backButton, playPauseButton;
    public Button x0, x1, x2, x4;

    NavMeshAgent agent;
    NavMeshPath path = null;
    Vector3 start, end;
    float speedMultiplier = 1;

    Vector3[] points;
    Vector3 agentCurrentPositon;
    private int index = 0;
    private bool moving = false;
    private bool paused = true;

    private void Awake()
    {
        agent = this.GetComponent<NavMeshAgent>();
        settingsPanel.SetActive(false);
        image.sprite = pausePlayImages[0];
        line.startColor = Color.gray;
        liftText.text = "";

        if (!PlayerPrefs.HasKey("stairs"))
        {
            if (stairsToggle.isOn)
            {
                PlayerPrefs.SetInt("stairs", 1);
            }
            else
            {
                PlayerPrefs.SetInt("stairs", 0);
            }

        }
        else
        {
            stairsToggle.isOn = PlayerPrefs.GetInt("stairs") != 0;
        }

        if (PlayerPrefs.HasKey("multiplier"))
        {
            speedMultiplier = PlayerPrefs.GetFloat("multiplier");
        }
        else
        {
            PlayerPrefs.SetFloat("multiplier", 1);
            speedMultiplier = 1;
        }
        PlayerPrefs.Save();

        if (PlayerPrefs.GetInt("stairs") == 1)
        {
            for (int i = 0; i < stairBlockers.Length; i++)
            {
                stairBlockers[i].SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < stairBlockers.Length; i++)
            {
                stairBlockers[i].SetActive(false);
            }
        }

        playPauseButton.interactable = false;
        forwardButton.interactable = false;
        backButton.interactable = false;
        HighlightSpeed(PlayerPrefs.GetFloat("multiplier"));
    }

    private void Start()
    {
        agent.SetDestination(end);
        path = agent.path;
        agent.CalculatePath(agent.destination, path);
    }

    private void Update()
    {
        if (moving) return;

        DrawPath();
        CheckDestination();
    }

    public void MoveAgent(int moveIndex)
    {
        if (!paused) return;

        moving = true;
        index += moveIndex;
        if (index <= -1)
        {
            index = 0;
        }
        else if (index >= points.Length)
        {
            index = points.Length - 1;
        }
        agent.Warp(points[index]);
    }

    public void OnValueChange()
    {
        if (startDropdown.value != -1)
        {
            start = GameObject.FindGameObjectWithTag(startDropdown.options[startDropdown.value].text).transform.position;
            agent.Warp(start);
        }
        if (endDropdown.value != -1)
        {
            end = GameObject.FindGameObjectWithTag(endDropdown.options[endDropdown.value].text).transform.position;
            agent.SetDestination(end);
        }
        if (startDropdown.value >= 0 && endDropdown.value >= 0)
        {
            playPauseButton.interactable = true;
            forwardButton.interactable = true;
            backButton.interactable = true;
        }
        moving = false;
        DrawPath();
    }

    public void ToggleStairs()
    {
        if (!paused) return;

        if (stairsToggle.isOn)
        {
            PlayerPrefs.SetInt("stairs", 1);
        }
        else
        {
            PlayerPrefs.SetInt("stairs", 0);
        }

        PlayerPrefs.Save();

        if (stairsToggle.isOn)
        {
            for (int i = 0; i < stairBlockers.Length; i++)
            {
                stairBlockers[i].SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < stairBlockers.Length; i++)
            {
                stairBlockers[i].SetActive(false);
            }
        }

        DrawPath();
    }

    public void OnStart()
    {
        if (moving)
        {
            moving = false;
            agent.ResetPath();
            if (startDropdown.value != -1)
            {
                start = GameObject.FindGameObjectWithTag(startDropdown.options[startDropdown.value].text).transform.position;
                if (agentCurrentPositon != start)
                {
                    //start = agentCurrentPositon;
                    agent.Warp(agentCurrentPositon);
                }

            }
            if (endDropdown.value != -1)
            {
                end = GameObject.FindGameObjectWithTag(endDropdown.options[endDropdown.value].text).transform.position;
                agent.SetDestination(end);
            }
        }

        ToggleSpeed();
    }

    public void ToggleSettings()
    {
        if (!paused) return;

        settingsPanel.SetActive(!settingsPanel.activeSelf);

        if (settingsPanel.activeSelf)
        {
            playPauseButton.interactable = false;
            forwardButton.interactable = false;
            backButton.interactable = false;
            return;
        }
        else if (!settingsPanel.activeSelf && (startDropdown.value < 0 || endDropdown.value < 0))
        {
            playPauseButton.interactable = false;
            forwardButton.interactable = false;
            backButton.interactable = false;
            return;
        }
        else
        {
            playPauseButton.interactable = true;
            forwardButton.interactable = true;
            backButton.interactable = true;
        }
    }

    public void SetMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
        PlayerPrefs.SetFloat("multiplier", speedMultiplier);
        PlayerPrefs.Save();
        HighlightSpeed(PlayerPrefs.GetFloat("multiplier"));

        if (agent.speed != 0)
        {
            agent.speed = 1 * speedMultiplier;
        }
    }

    public void RunTutorial()
    {
        PlayerPrefs.SetInt("Tutorial", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene(0);
    }

    private void HighlightSpeed(float speed)
    {
        UnhighlightSpeed();

        switch (speed)
        {
            case 0.5f:
                x0.interactable = false;
                break;
            case 1f:
                x1.interactable = false;
                break;
            case 1.5f:
                x2.interactable = false;
                break;
            case 2f:
                x4.interactable = false;
                break;
            default:
                break;
        }
    }

    private void UnhighlightSpeed()
    {
        x0.interactable = true;
        x1.interactable = true;
        x2.interactable = true;
        x4.interactable = true;
    }

    private void ToggleSpeed()
    {
        liftText.color = Color.blue;
        if (agent.speed > 0)
        {
            endDropdown.interactable = true;
            startDropdown.interactable = true;
            settingsButton.interactable = true;
            forwardButton.interactable = true;
            backButton.interactable = true;
            paused = true;
            agent.speed = 0;
            image.sprite = pausePlayImages[0];
            line.startColor = Color.gray;
            line.endColor = line.startColor;


            return;
        }
        endDropdown.interactable = false;
        startDropdown.interactable = false;
        settingsButton.interactable = false;
        forwardButton.interactable = false;
        backButton.interactable = false;
        paused = false;
        agent.speed = 1 * speedMultiplier;
        image.sprite = pausePlayImages[1];
        line.startColor = Color.blue;
        line.endColor = line.startColor;
    }

    private void CheckDestination()
    {
        if (Vector3.Distance(agent.destination, agent.transform.position) < 1f)
        {
            if (agent.speed > 0)
            {
                ToggleSpeed();
            }
        }
    }

    private void DrawPath()
    {
        path = agent.path;
        points = path.corners;
        line.positionCount = points.Length;
        line.SetPositions(points);
        agentCurrentPositon = agent.nextPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Lift"))
        {
            if (agent.speed > 0)
            {
                ToggleSpeed();
            }

            if (endDropdown.value >= 0 && endDropdown.value < 9)
            {
                liftText.text = "Take elevator to ground floor";
            }
            else if (endDropdown.value >= 9 && endDropdown.value < 20)
            {
                liftText.text = "Take elevator to 1st floor";
            }
            else if (endDropdown.value >= 20 && endDropdown.value < 35)
            {
                liftText.text = "Take elevator to 2nd floor";
            }
            else if (endDropdown.value >= 35 && endDropdown.value < 49)
            {
                liftText.text = "Take elevator to 3rd floor";
            }
            else
            {
                liftText.text = "";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Lift"))
        {
            liftText.text = "";
        }
    }
}