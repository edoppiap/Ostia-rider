using UnityEngine;

//comandi da tastiera per giocare dall'Editor (e da PC): usano la stessa interfaccia dei pulsanti a schermo
//W/S o frecce su/giù: accelera/frena, A/D o frecce sinistra/destra: sterza, Invio: inizia, Esc: pausa
public class KeyboardInput : MonoBehaviour
{
#if UNITY_EDITOR || UNITY_STANDALONE
    private GameManager gameManager;
    private SuspensionBikeController controller;
    private PlayfabManager playfabManager;
    private int lastVertical = 0;
    private int lastHorizontal = 0;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        controller = FindFirstObjectByType<SuspensionBikeController>();
        playfabManager = gameManager.GetComponent<PlayfabManager>();
    }

    static int Axis(KeyCode positive, KeyCode positiveAlt, KeyCode negative, KeyCode negativeAlt)
    {
        int value = 0;
        if (Input.GetKey(positive) || Input.GetKey(positiveAlt)) value++;
        if (Input.GetKey(negative) || Input.GetKey(negativeAlt)) value--;
        return value;
    }

    void Update()
    {
        //i metodi del controller vengono chiamati solo quando lo stato dei tasti cambia,
        //così i pulsanti a schermo continuano a funzionare quando la tastiera non è usata
        int vertical = Axis(KeyCode.W, KeyCode.UpArrow, KeyCode.S, KeyCode.DownArrow);
        if (vertical != lastVertical)
        {
            if (vertical > 0) controller.Accellera();
            else if (vertical < 0) controller.Frena();
            else controller.Deaccellera();
            lastVertical = vertical;
        }

        int horizontal = Axis(KeyCode.D, KeyCode.RightArrow, KeyCode.A, KeyCode.LeftArrow);
        if (horizontal != lastHorizontal)
        {
            if (horizontal > 0) controller.SterzaDx();
            else if (horizontal < 0) controller.SterzaSx();
            else controller.Desterza();
            lastHorizontal = horizontal;
        }

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) &&
            !gameManager.IsInPlay() && gameManager.startCanvas.activeInHierarchy &&
            !playfabManager.nameWindow.activeInHierarchy)
        {
            gameManager.StartGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && gameManager.IsInPlay() && !gameManager.isGameEnded())
        {
            if (gameManager.pauseCanvas.activeSelf)
                gameManager.Resume();
            else
                gameManager.Pause();
        }
    }
#endif
}
