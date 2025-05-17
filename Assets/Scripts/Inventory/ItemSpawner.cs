using UnityEngine;

// Este script representa um ponto no mundo que dá um item ao player quando ele entra no trigger.
// A coleta é ativada por um clique do player no spawner que habilita o trigger.
[RequireComponent(typeof(Collider))] // Garante que há um Collider (para o trigger)
public class ItemSpawner : MonoBehaviour
{
    [Header("Configuração do Item Spawner")]
    [Tooltip("O ItemData que este spawner dá ao jogador.")]
    [SerializeField] private ItemData itemToGive; // Qual ItemData este spawner dá

    // --- Controle de Interação (Baseado em Clique) ---
    [Tooltip("Indica se a interação por trigger está habilitada (ativada por clique).")]
    public bool enable = false, isPlayerInsideTrigger = false; // Flag: Habilita o trigger check em OnTriggerEnter após clique.

    // Usaremos a Layer do próprio GameObject para o Raycast
    private LayerMask thisObjectLayerMask; // NOVO: LayerMask gerada a partir da layer deste objeto.
    private DogMovement dogMovement;
    private PlayerInventory playerInventory;

    void Awake() // Use Awake para garantir que a layer seja obtida antes de Start de outros scripts
    {
        // Obtém o índice da layer deste GameObject
        int thisLayerIndex = gameObject.layer;

        // Converte o índice da layer para uma LayerMask
        // Uma LayerMask é um bitmask. O bit correspondente à layer é 1.
        thisObjectLayerMask = 1 << thisLayerIndex;
        dogMovement = FindObjectOfType<DogMovement>();

        playerInventory = FindObjectOfType<PlayerInventory>();

    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            SelectObjective();
        }
    }


    private void OnTriggerEnter(Collider other)
    {

        if (enable && other.CompareTag("Player"))
        {

            enable = false;
            isPlayerInsideTrigger = true; // Define a flag para indicar que o player está dentro do trigger


            if (itemToGive.id != playerInventory.GetItemID())
            {
                playerInventory.SetItem(itemToGive);
                if (itemToGive.id == "Racao")
                {
                    dogMovement.RequestFollowPlayer();
                }
                else if (itemToGive.id == "Coleira")
                {
                    dogMovement.RequestFlee();
                }
            }
            else
            {
                playerInventory.RemoveItem();
            }


        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = false; // Reseta a flag quando o player sai do trigger
        }
    }
    private void SelectObjective()
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, thisObjectLayerMask))
        {

            if (hit.collider.gameObject == gameObject)
            {
                enable = true; // Habilita a flag para que OnTriggerEnter possa iniciar o minigame

                if (isPlayerInsideTrigger) // Verifica a nova flag
                {
                    CheckAndRemoveItem();
                }


            }


        }

    }

    private void CheckAndRemoveItem()
    {
        if (playerInventory.GetItem() != null && playerInventory.GetItem().id == itemToGive.id)
        {
            playerInventory.RemoveItem();
        }
    }
}