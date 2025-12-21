using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : BaseMenu
{
    public TMP_Text MainMenuText;
    public Button playBtn;
    public Button QuitBtn;


    public override void Init(MenuController contex)
    {
        base.Init(contex);
        state = MenuStates.MainMenu;
        MainMenuText.text = "OH NO! OLD MAN!";
        if (playBtn) playBtn.onClick.AddListener(() => SceneManager.LoadScene("level"));
        QuitBtn.onClick.AddListener(() => QuitGame());
    }
}
