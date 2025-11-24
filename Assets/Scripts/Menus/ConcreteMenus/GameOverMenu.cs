using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : BaseMenu
{
    public TMP_Text gameOverText;
    public Button RestartBtn;
    public Button QuitBtn;
    public override void Init(MenuController contex)
    {
        base.Init(contex);
        state = MenuStates.GameOver;

        gameOverText.text = "Game Over!";

        if (RestartBtn) RestartBtn.onClick.AddListener(() => SceneManager.LoadScene("level"));
        QuitBtn.onClick.AddListener(() => QuitGame());
    }
}
