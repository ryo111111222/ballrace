using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// タイトル画面、ルール・操作説明画面、ゲームモード選択（1P vs AI / 1P vs 2P）を管理するマネージャースクリプト
/// </summary>
public class TitleScreenManager : MonoBehaviour
{
    public enum GameMode
    {
        PvAI, // 1P vs AI (1人プレイ)
        PvP   // 1P vs 2P (2人対戦)
    }

    // 他のシーンやスクリプトから選択したモードを取得できる静的変数
    public static GameMode SelectedMode = GameMode.PvAI;

    [Header("--- UI Panels ---")]
    [Tooltip("タイトル画面のメインパネル")]
    public GameObject titlePanel;

    [Tooltip("ルール・操作説明のパネル")]
    public GameObject rulePanel;

    [Tooltip("モード選択パネル（1P vs AI / 1P vs 2P）")]
    public GameObject modeSelectPanel;

    [Tooltip("同一シーンで遊ぶ場合のレース用HUDパネル（任意）")]
    public GameObject gameHudPanel;

    [Header("--- Scene Transition Settings ---")]
    [Tooltip("trueならSceneManagerでシーン遷移、falseなら同一シーン内でUIを切り替えてゲーム開始")]
    public bool useSceneTransition = false;

    [Tooltip("遷移先のゲームシーン名（SceneManagerを使う場合）")]
    public string gameSceneName = "SampleScene";

    [Header("--- In-Game Players (同一シーン実行時用) ---")]
    [Tooltip("1PのPlayerController2")]
    public PlayerController2 player1;

    [Tooltip("2PのPlayerController2（AIまたは2P操作）")]
    public PlayerController2 player2;

    [Header("--- Audio Settings (任意) ---")]
    public AudioSource audioSource;
    public AudioClip buttonClickSE;
    public AudioClip startSE;

    void Start()
    {
        ShowTitle();

        // 同一シーンで管理する場合、初期状態ではプレイヤーの動きを止めておく
        if (!useSceneTransition && (player1 != null || player2 != null))
        {
            SetPlayersActive(false);
        }
    }

    /// <summary>
    /// タイトル画面を表示
    /// </summary>
    public void ShowTitle()
    {
        PlayClickSound();
        if (titlePanel != null) titlePanel.SetActive(true);
        if (rulePanel != null) rulePanel.SetActive(false);
        if (modeSelectPanel != null) modeSelectPanel.SetActive(false);
        if (gameHudPanel != null) gameHudPanel.SetActive(false);
    }

    /// <summary>
    /// ルール・操作説明画面を表示
    /// </summary>
    public void ShowRule()
    {
        PlayClickSound();
        if (titlePanel != null) titlePanel.SetActive(false);
        if (rulePanel != null) rulePanel.SetActive(true);
        if (modeSelectPanel != null) modeSelectPanel.SetActive(false);
    }

    /// <summary>
    /// モード選択画面を表示
    /// </summary>
    public void ShowModeSelect()
    {
        PlayClickSound();
        if (titlePanel != null) titlePanel.SetActive(false);
        if (rulePanel != null) rulePanel.SetActive(false);
        if (modeSelectPanel != null) modeSelectPanel.SetActive(true);
    }

    /// <summary>
    /// 1人プレイ (1P vs AI) でゲームを開始
    /// </summary>
    public void StartPvAIMode()
    {
        SelectedMode = GameMode.PvAI;
        StartGame();
    }

    /// <summary>
    /// 2人対戦 (1P vs 2P) でゲームを開始
    /// </summary>
    public void StartPvPMode()
    {
        SelectedMode = GameMode.PvP;
        StartGame();
    }

    /// <summary>
    /// ゲーム開始共通処理
    /// </summary>
    public void StartGame()
    {
        if (audioSource != null && startSE != null)
        {
            audioSource.PlayOneShot(startSE);
        }

        if (useSceneTransition)
        {
            // 別シーンへのロード
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            // 同一シーンでUIを切り替えてゲーム開始
            if (titlePanel != null) titlePanel.SetActive(false);
            if (rulePanel != null) rulePanel.SetActive(false);
            if (modeSelectPanel != null) modeSelectPanel.SetActive(false);
            if (gameHudPanel != null) gameHudPanel.SetActive(true);

            // プレイヤーとAIの設定を適用して有効化
            ApplyGameModeSettings();
            SetPlayersActive(true);
        }
    }

    private void ApplyGameModeSettings()
    {
        if (player1 != null)
        {
            player1.ID = 1;
            // isAI は PlayerController2 のインスペクター設定を優先する。
        }

        if (player2 != null)
        {
            player2.ID = 2;
            // isAI は PlayerController2 のインスペクター設定を優先する。
            // SelectedMode はUI上の選択状態として保持するが、車の操作種別は変更しない。
        }
    }

    private void SetPlayersActive(bool active)
    {
        if (player1 != null) player1.enabled = active;
        if (player2 != null) player2.enabled = active;
    }

    /// <summary>
    /// ゲーム終了（ビルド版用）
    /// </summary>
    public void QuitGame()
    {
        PlayClickSound();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void PlayClickSound()
    {
        if (audioSource != null && buttonClickSE != null)
        {
            audioSource.PlayOneShot(buttonClickSE);
        }
    }
}
