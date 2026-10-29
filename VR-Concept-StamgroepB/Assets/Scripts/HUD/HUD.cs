using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("Tracking")]
    [SerializeField] private Transform leftHandAnchor;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Vector3 localPosition = new Vector3(0.05f, -0.02f, -0.17f);
    [SerializeField] private Vector3 localEulerAngles = new Vector3(0f, 0f, 270f);

    [Header("Gaze")]
    [SerializeField, Range(1f, 90f)] private float gazeAngle = 32f;
    [SerializeField] private float maximumVisibleDistance = 3f;
    [SerializeField] private float fadeSpeed = 12f;

    [Header("Display")]
    [SerializeField] private string ammo = "04/04";
    [SerializeField] private string score = "00000";

    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private TextMeshProUGUI timeText;
    private float targetAlpha;
    private Transform displayTransform;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateForScene()
    {
        if (Object.FindFirstObjectByType<HUD>() == null)
            new GameObject("Smartwatch HUD").AddComponent<HUD>();
    }

    private void Awake()
    {
        BuildDisplay();
    }

    private void Update()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (leftHandAnchor == null)
            leftHandAnchor = FindLeftHandAnchor();

        if (displayTransform != null && leftHandAnchor != null)
        {
            displayTransform.SetParent(leftHandAnchor, false);
            displayTransform.localPosition = localPosition;
            displayTransform.localRotation = Quaternion.Euler(localEulerAngles);
        }

        targetAlpha = ShouldShow() ? 1f : 0f;
        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
        UpdateDisplayText();
    }

    private bool ShouldShow()
    {
        if (playerCamera == null || leftHandAnchor == null || displayTransform == null)
            return false;

        Vector3 toDisplay = displayTransform.position - playerCamera.transform.position;
        float distance = toDisplay.magnitude;
        if (distance > maximumVisibleDistance || distance < 0.01f)
            return false;

        float viewAngle = Vector3.Angle(playerCamera.transform.forward, toDisplay);
        bool faceIsVisible = Vector3.Dot(displayTransform.forward, -toDisplay.normalized) > 0.05f;
        return viewAngle <= gazeAngle && faceIsVisible;
    }

    private void BuildDisplay()
    {
        GameObject display = new GameObject("Smartwatch Panel", typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup));
        displayTransform = display.transform;
        displayTransform.SetParent(leftHandAnchor, false);
        displayTransform.localPosition = localPosition;
        displayTransform.localRotation = Quaternion.Euler(localEulerAngles);
        displayTransform.localScale = Vector3.one * 0.001f;

        canvas = display.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = playerCamera;
        canvas.sortingOrder = 20;

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(420f, 260f);

        canvasGroup = display.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        GameObject background = new GameObject("Panel Background", typeof(RectTransform), typeof(Image));
        background.transform.SetParent(displayTransform, false);
        RectTransform backgroundRect = background.GetComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;
        background.GetComponent<Image>().color = new Color(0.55f, 0.55f, 0.55f, 0.88f);

        timeText = CreateText("TIME: 00:00", new Vector2(24f, -24f));
        CreateText("AMMO: " + ammo, new Vector2(24f, -86f));
        CreateText("SCORE: " + score, new Vector2(24f, -148f));
    }

    private TextMeshProUGUI CreateText(string value, Vector2 position)
    {
        GameObject textObject = new GameObject(value, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(displayTransform, false);
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(370f, 48f);

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = 31f;
        text.fontStyle = FontStyles.Bold;
        text.color = new Color(0.65f, 1f, 0f, 1f);
        text.alignment = TextAlignmentOptions.Left;
        text.raycastTarget = false;
        return text;
    }

    private void UpdateDisplayText()
    {
        if (timeText != null)
            timeText.text = "TIME: " + Mathf.FloorToInt(Time.time / 60f).ToString("00") + ":" + (Mathf.FloorToInt(Time.time) % 60).ToString("00");
    }

    private Transform FindLeftHandAnchor()
    {
        string[] names = { "Left Controller", "LeftHand Controller", "Left Hand", "LeftHand", "Hand Controller Left" };
        Transform[] transforms = Object.FindObjectsOfType<Transform>();
        foreach (string name in names)
        {
            foreach (Transform candidate in transforms)
            {
                if (candidate.name == name)
                    return candidate;
            }
        }

        return null;
    }
}
