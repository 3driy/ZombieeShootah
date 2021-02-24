using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSaveData{

    public bool fromStart;
    public int savedShot;
    public int savedAmmo;
    public float savedHealth;
    public float[] savedPosition;
    public int currentSceneIndex;
    public int savedScore;
    public int highScore;
    public bool died;

    public PlayerSaveData(SceneControll sceneControll)
    {
        died = sceneControll.died;
        highScore = sceneControll.highScore;
        savedScore = sceneControll.savedKilledInGame;
        fromStart = sceneControll.fromStart;
        currentSceneIndex = sceneControll.currentSceneIndex;
        savedShot = sceneControll.savedShot;
        savedHealth = sceneControll.savedHealth;
        savedAmmo = sceneControll.savedAmmo;
        savedPosition = new float[3];
        savedPosition[0] = sceneControll.savedPlayerPos.x;
        savedPosition[1] = sceneControll.savedPlayerPos.y;
        savedPosition[2] = sceneControll.savedPlayerPos.z;
    }
}
