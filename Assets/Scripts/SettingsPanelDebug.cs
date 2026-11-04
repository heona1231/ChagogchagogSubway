using UnityEngine;

public class SettingsPanelDebug : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.LogWarning(
            $"¡Ú¡Ú¡Ú¡Ú¡Ú SettingsPanel ÄÑÁü ¡Ú¡Ú¡Ú¡Ú¡Ú " +
            $"frame={Time.frameCount}, time={Time.time}"
        );
    }

    private void OnDisable()
    {
        Debug.LogError(
            $"¡Ú¡Ú¡Ú¡Ú¡Ú SettingsPanel ²¨Áü ¡Ú¡Ú¡Ú¡Ú¡Ú " +
            $"frame={Time.frameCount}, time={Time.time}\n" +
            System.Environment.StackTrace
        );
    }
}