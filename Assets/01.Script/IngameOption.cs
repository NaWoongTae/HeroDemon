using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameOption : MonoBehaviour
{
    [SerializeField] GameObject menu;
    [SerializeField] GameObject optionWin;
    [SerializeField] GameObject exit_Game;
    [SerializeField] GameObject exit_Program;

    BaseManager _BSM;

    enum Win { menu, option, exGame, exProgram }

    float timeSpeed = 0.0f;

    void Start()
    {
        _BSM = PrefabPool.instance.GetManager(Managers.BaseManager).GetComponent<BaseManager>();
    }

    // 메뉴창 -----------------------------------------------------------------

    // 오픈 메뉴
    public void SetOpen()
    {
        timeSpeed = Time.timeScale;
        Time.timeScale = 0;
    }

    // 프로젝트 exit 창
    public void exitProgramWin()
    {
        OpenWin(Win.exProgram);
    }

    // 게임 exit 창
    public void exitGameWin()
    {
        OpenWin(Win.exGame);
    }

    // 옵션창
    public void option()
    {
        OpenWin(Win.option);
    }

    // exit game -----------------------------------------------------------------

    public void exitGame()
    {
        Time.timeScale = 1;
        _BSM._loadScene(Managers.LobbyManager, "BattleScene");
    }

    // exit program --------------------------------------------------------------

    public void exitProgram()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    // 메뉴로 돌아가기
    public void returnToMenu()
    {
        OpenWin(Win.menu);
    }

    // 게임으로
    public void returnToGame()
    {
        Time.timeScale = timeSpeed;
        gameObject.SetActive(false);
    }

    // 창을 끄고키는
    void OpenWin(Win win)
    {
        menu.SetActive(false);
        optionWin.SetActive(false);
        exit_Game.SetActive(false);
        exit_Program.SetActive(false);

        switch (win)
        {
            case Win.menu:
                menu.SetActive(true);
                break;
            case Win.option:
                optionWin.SetActive(true);
                break;
            case Win.exGame:
                exit_Game.SetActive(true);
                break;
            case Win.exProgram:
                exit_Program.SetActive(true);
                break;
            default:
                break;
        }
    }
}
