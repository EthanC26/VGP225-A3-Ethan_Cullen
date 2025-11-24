using UnityEngine;

public enum MenuStates
{
    MainMenu, Credits, Pause, InGame, GameOver, Victory, Continue
}
public class BaseMenu : MonoBehaviour
{
    [HideInInspector]
    public MenuStates state;

    protected MenuController contex;

    public virtual void Init(MenuController contex) => this.contex = contex;
    public virtual void EnterState() { }
    public virtual void ExitState() { }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void JumpBack() => contex.JumpBack();
    public void SetNextMenu(MenuStates newState) => contex.SetActiveState(newState);

}

