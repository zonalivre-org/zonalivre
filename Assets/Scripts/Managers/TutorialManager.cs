using System.Diagnostics;
using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private PetInteract petInteract;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private ChangeScene changeScene;
    [SerializeField] private PopUp popUp;
    [SerializeField] private GameObject[] minigames;
    [SerializeField] private GameObject[] minigameIndicators;
    [SerializeField] private GameObject[] petItems;
    [SerializeField] private GameObject[] statusIcons;

    #region Tutorial Variables
    [SerializeField] private int currentStep = 0;
    [SerializeField] private int timesMoved = 0;
    [SerializeField] private int popUpsClosed = 0;
    [SerializeField] private int minigameCompleted = 0;
    [SerializeField] private int petMinigamesCompleted = 0;
    [SerializeField] private int itensCollected = 0;
    [SerializeField] private int petItensCollected = 0;

    #endregion

    void Start()
    {
        gameManager.rules = false;
        gameManager.currentHappyness = gameManager.happyness / 2;
        gameManager.currentStamina = gameManager.stamina / 2;
        gameManager.currentHealth = gameManager.health / 2;

        petInteract.canHeal = false;
        petInteract.canFeed = false;
        petInteract.canPet = false;

        for (int i = 0; i < statusIcons.Length; i++)
        {
            statusIcons[i].SetActive(false);
        }

        for (int i = 0; i < minigameIndicators.Length; i++)
        {
            minigameIndicators[i].SetActive(false);
        }

        // Subscribe to the OnDestinationReached Action
        playerController.OnDestinationReached += OnDestinationReached;

        // Subscribe to the OnPopUpClosed Action
        popUp.OnPopUpClosed += OnPopUpClosed;

        // Subscribe to the OnMinigameCompleted Action
        MiniGameBase.OnMiniGameComplete += OnMinigameCompleted;

        playerInventory.OnItemChanged += OnItemCollected;

        petInteract.OnMinigameComplete += OnPetMinigameCompleted;
    }

    void Update()
    {
        switch (currentStep)
        {
            case 0:
                popUp.SetPopUp("Tutorial", "Bem vindo ao Tutorial! Aqui você vai aprender como se joga o jogo. Você pode rever a ultima lição clicando no botão <sprite=20> no canto superior direito da tela. Clique no <sprite=19> para continuar.");
                popUp.SetVideoPlayer("TutorialButton.mp4");
                currentStep++;
                break;

            case 1:
                if (popUpsClosed >= 1)
                {
                    currentStep++;
                    popUp.SetPopUp(
                        "Tempo",
                        "O jogo se baseia em realizar tarefas em um determinado tempo, indicado pelo <sprite=16> no topo da tela. Se o tempo acabar, você perde o jogo. Mas não se preocupe, pois no tutorial o tempo não abaixa."
                    );
                    popUp.SetVideoPlayer("TimeRunning.mp4");
                }
                break;

            case 2:
                if (popUpsClosed >= 2)
                {
                    ShowInitialPopUp();
                    minigames[0].GetComponent<BoxCollider>().enabled = false; // Disable the mango minigame collision
                    petInteract.gameObject.GetComponent<CapsuleCollider>().enabled = false; // Disable the pet collider object
                    currentStep++;
                    popUp.SetPopUp(
                        "Como se movimentar",
                        "Ótimo! Agora que você já sabe como o tempo funciona, vamos aprender a se movimentar. Para conseguir se movimentar pelo mapa, basta tocar/clicar em algum lugar do mapa. Feche essa janela e tente se movimentar 3 vezes."
                    );
                    popUp.SetVideoPlayer("WalkingVideo.mp4");
                }
                break;

            case 3:
                if (timesMoved >= 4)
                {
                    currentStep++;
                    minigames[0].GetComponent<BoxCollider>().enabled = true; // Enable the mango minigame collision
                    minigames[1].GetComponent<BoxCollider>().enabled = false; // Disable the leaves minigame collision
                    minigames[1].GetComponent<ObjectiveInteract>().taskItem.gameObject.SetActive(false); // Disable the leaves minigame task
                    minigameIndicators[0].SetActive(true); // Activate the Mango indicator
                    minigameIndicators[1].SetActive(false); // Disable the Leaves indicator
                    popUp.SetPopUp(
                        "Interação com tarefas",
                        "Muito bem! Seu objetivo é realizar tarefas pelo mapa. Para iniciar uma tarefa, basta andar até ela. Tente colher algumas mangas naquele pé de manga."
                    );
                }
                break;

            case 4:
                if (minigameCompleted >= 1)
                {
                    currentStep++;
                    minigames[2].SetActive(true); // Activate the Moskito Screen object
                    minigameIndicators[4].SetActive(true); // Activate the Moskito Screen indicator
                    popUp.SetPopUp(
                        "Interação com tarefas usando itens",
                        "Ótimo! Agora você já sabe como interagir com as tarefas. No entanto, algumas tarefas necessitam de itens para serem feitas. Colete o item TELA <Icone da tela> no mapa. Basta encostar nele."
                    );
                }
                break;

            case 5:
                if (itensCollected >= 1 && playerInventory.GetItemID() == "Tela")
                {
                    currentStep++;
                    minigameIndicators[2].SetActive(true); // Activate the Window indicator
                    minigameIndicators[4].SetActive(false); // Disable the Moskito Screen indicator
                    minigames[4].GetComponent<BoxCollider>().enabled = true; // Activate the Window collider object
                    popUp.SetPopUp(
                        "Interação com tarefas usando itens",
                        "Excelente! Agora que você coletou o item TELA <Icone da tela>, leve-o para o OBJETIVO JANELA <Icone da janela> para aplicar a TELA."
                    );
                }
                break;

            case 6:
                if (minigameCompleted >= 2)
                {
                    currentStep++;
                    minigameIndicators[4].SetActive(false); // Disable the Moskito Screen indicator
                    popUp.SetPopUp(
                        "Cuidando do seu pet",
                        "Muito bem! Entendido como os objetivos funcionam. Agora, vamos cuidar do seu pet! "
                        + "Ele possui três necessidades: Saúde <sprite=10>, Fome <sprite=5> e Felicidade <sprite=3>. Você pode ver essas informações na parte superior esquerda da tela."
                    );
                }
                break;

            case 7:
                if (popUpsClosed >= 7)
                {
                    currentStep++;
                    popUp.SetPopUp(
                        "Cuidando do seu pet",
                        "Cuidado! Fora do tutorial, essas necessidades diminuem com o tempo. Então você precisa tirar tempo para cuidar das necessidades do seu pet. Caso uma delas chegue a zero, você perde o jogo."
                    );
                }
                break;

            case 8:
                if (popUpsClosed >= 8)
                {
                    currentStep++;
                    petInteract.gameObject.GetComponent<CapsuleCollider>().enabled = true; // Activate the pet collider object
                    minigameIndicators[3].SetActive(true); // Activate the Pet indicator
                    minigameIndicators[5].SetActive(true); // Activate the Coleira indicator
                    petItems[0].SetActive(true);
                    popUp.SetPopUp(
                        "Cuidando do seu pet",
                        "Vamos começar levando ele ao veterinário. A saúde é representada pelo ícone <Icone da Saude>. Para aumentar ela, colete a coleira <Icone da coleira> no mapa e vá até o pet."
                    );
                    statusIcons[0].SetActive(true);
                    petInteract.canHeal = true;
                }
                break;

            case 9:
                if (petMinigamesCompleted >= 1)
                {
                    currentStep++;
                    //minigameBlock[5].GetComponent<CapsuleCollider>().enabled = false; // Disable the pet collider object
                    petItems[1].SetActive(true);
                    minigameIndicators[5].SetActive(false); // Disable the Coleira indicator
                    minigameIndicators[6].SetActive(true); // Activate the Ration Bag indicator
                    popUp.SetPopUp(
                        "Cuidando do seu pet",
                        "Ótimo! É sempre importante levar seu pet ao veterinário. Agora, vamos alimentá-lo. A fome é representada pelo ícone <Icone da comida>. "
                        + "Para isso, colete o saco de ração <Icone da comida> no mapa e vá até o pet."
                    );
                    statusIcons[1].SetActive(true);
                    petInteract.canFeed = true;
                    petInteract.canHeal = false;
                }
                break;
            case 10:
                if (petMinigamesCompleted >= 2)
                {
                    currentStep++;
                    minigameIndicators[6].SetActive(false); // Disable the Ration Bag indicator
                    popUp.SetPopUp(
                        "Cuidando do seu pet",
                        "Excelente! Agora que ele está alimentado e saudável, faça carinho nele. A felicidade é representada pelo ícone <Icone da felicidade>. Para aumentar ela, basta ir até ele sem carregar nenhum item."
                    );
                    statusIcons[2].SetActive(true);
                    petInteract.canPet = true;
                    petInteract.canFeed = false;
                }
                break;

            case 11:
                if (popUpsClosed >= 11)
                {
                    currentStep++;
                    popUp.SetPopUp(
                        "Mexendo com itens",
                        "Se precisar largar um item da mão, existem duas formas: 1. Você pode devolver o item para o mesmo lugar que você pegou. 2. Ao concluir ou fechar um objetivo."
                    );
                }
                break;

            case 12:
                if (petMinigamesCompleted >= 3)
                {
                    currentStep++;
                    popUp.SetPopUp(
                        "Mão na massa",
                        "Muito Bem! Agora, para finalizar, complete as tarefas restantes para acabarmos as lições. "
                    );
                    minigames[1].GetComponent<BoxCollider>().enabled = true; // Activate the leaves minigame collision
                    minigameIndicators[1].SetActive(true); // Activate the Leaves indicator
                    minigameIndicators[4].SetActive(true); // Activate the Moskito Screen indicator

                    minigames[4].SetActive(true); // Activate the Window Screen object
                    minigames[4].GetComponent<ObjectiveInteract>().taskItem.gameObject.SetActive(true); // Activate the Window Screen task
                    minigames[1].GetComponent<ObjectiveInteract>().taskItem.gameObject.SetActive(true); // Activate the leaves minigame task

                }
                break;

            case 13:
                if (popUpsClosed >= 13)
                {
                    currentStep++;
                    popUp.SetPopUp(
                        "Tarefas",
                        "Você pode checar todas as tarefas da fase no botão <Icone do botão>. "
                    );
                }
                break;
            case 14:
                if (CheckCompleteMinigames())
                {
                    currentStep++;
                    popUp.SetPopUp(
                        "Finalmente acabou!",
                        "Parabéns! Agora que você viu como o jogo funciona, você pode começar a jogar de verdade. Sempre que quiser, o tutorial poderá ser repetido na seleção de níveis. Feche essa janela para começar a jogar."
                    );

                }
                break;
            case 15:
                if (popUpsClosed >= 15)
                {
                    changeScene.ChangeToSceneMusic(1);
                    changeScene.LoadSceneDelay(1);
                }
                break;
        }

    }

    private void ShowInitialPopUp()
    {
        popUp.SetPopUp(
            "Como se movimentar",
            "Bem vindo ao Tutorial! Para conseguir se movimentar pelo mapa, basta tocar/clicar em algum lugar do mapa. Feche essa janela e tente se movimentar 3 vezes."
        );
    }

    private void OnDestinationReached()
    {
        timesMoved++;
    }

    private void OnPopUpClosed()
    {
        popUpsClosed++;
    }

    private void OnMinigameCompleted()
    {
        minigameCompleted++;
    }

    private void OnItemCollected(ItemData item)
    {
        itensCollected++;
    }

    private void OnPetMinigameCompleted()
    {
        petMinigamesCompleted++;
    }

    private bool CheckCompleteMinigames()
    {
        int minigameCount = 0;
        foreach (GameObject item in taskManager.GetTaskListItems())
        {
            if (item.GetComponent<TaskItem>().isComplete)
            {
                minigameCount++;
            }
        }

        if (minigameCount == taskManager.GetTaskListItems().Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the events to avoid memory leaks
        playerController.OnDestinationReached -= OnDestinationReached;
        popUp.OnPopUpClosed -= OnPopUpClosed;
    }
}
