using Mirror;
using UnityEngine.UI;

public class MapOptionsUI : NetworkBehaviour
{
    public Slider sizeSlider, boxsSlider;
    public Button startGameButton;
    public bool localPlayerWanted;

    public void Init()
    {
        if (isServer)
        {
            startGameButton.gameObject.SetActive(true);
            startGameButton.onClick.AddListener(StartGame);
        }
    }
    private void StartGame()
    {
        //GameManager.instance.levelBuilder.CreateMap();
        NetworkServer.SendToAll(new GameStartedMessage { });

    }
        
}
