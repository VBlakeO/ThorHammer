using UnityEngine.Playables;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private GameObject player = null;
    [SerializeField] private GameObject hud = null;

    private PlayableDirector cutscene = null;
    private bool end = false;

    private void Awake()
    {
        cutscene = GetComponent<PlayableDirector>();
    }

    private void Update()
    {
        if(cutscene.time >= cutscene.duration - 0.02f)
        {
            if(!end)
            {
                ActivatePlayer();
                end = true;
            }
        }
    }

    public void ActivatePlayer()
    {
        player.SetActive(true);
        hud.SetActive(true);
    }
}
