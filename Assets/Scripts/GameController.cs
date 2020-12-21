using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;

public class GameController : MonoBehaviour {

    #region Singleton

    public static GameController gameController;

    void Awake()
    {
        if (gameController != null)
        {
            return;
        }
        gameController = this;
    }

    #endregion

    public TextMeshProUGUI victoryText;
    public GameObject teleportEffect;
    public RectTransform LoadingScreen;
    public Slider progressBar;
    public Slider BossHP;
    public PlayerController pController;
    public PlayerMovement pMovement;
    public CharacterAnimator pAnimator;
    public NavMeshAgent pAgent;

    public GameObject bossPlatform;
    public bool characterTransfered = false;
    public bool gameReady = false;
    public bool transferStarted = false;

    // Use this for initialization
    void Start () {
        pController = GameObject.Find("Player").GetComponent<PlayerController>();
        pMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        pAnimator = GameObject.Find("Player").GetComponent<CharacterAnimator>();
        pAgent = GameObject.Find("Player").GetComponent<NavMeshAgent>();

        StartCoroutine(WaitForLoading());
    }
	
	// Update is called once per frame
	void Update () {

		if (progressBar.value >= 100 && !transferStarted)
        {
            transferStarted = true;
            StartCoroutine(TransferToBoss());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(TransferToBoss());
        }
	}

    private IEnumerator WaitForLoading()
    {
        while (!gameReady) {
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(LoadingScreen.gameObject);
    }

    private IEnumerator TransferToBoss()
    {
        Instantiate(teleportEffect, pController.transform);
        pController.enabled = false;
        pAgent.enabled = false;
        pMovement.enabled = false;
        pAnimator.enabled = false;

        progressBar.GetComponent<Slider>().gameObject.SetActive(false);
        progressBar.enabled = false;
        BossHP.GetComponent<Slider>().gameObject.SetActive(true);
        BossHP.enabled = true;

        yield return new WaitForSeconds(1f);
        MoveChar();
        yield return new WaitUntil(() => characterTransfered == true);

        pController.enabled = true;
        pAgent.enabled = true;
        pMovement.enabled = true;
        pAnimator.enabled = true;
    }

    private void MoveChar()
    {
        // animation
        pController.transform.position = new Vector3(4995, pController.transform.position.y, 4995);
        GameObject.Find("Boss-platform").GetComponentInChildren<Spawning>().SpawnBoss();
        GameObject.Find("Main Camera").GetComponent<CameraController>().repositionCamera();
        characterTransfered = true;
    }
}
