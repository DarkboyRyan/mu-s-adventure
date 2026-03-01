using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HumanRoulette : MonoBehaviour
{
    [Header("Refs")]
    public RectTransform human;     
    public Button startButton;
    public Button stopButton;
    public TMP_Text resultText;
    public MinigameCameraSwitcher switcher;

    [Header("Spin")]
    public float minSpeed = 360f;    // deg/sec
    public float maxSpeed = 900f;    // deg/sec
    public float decel = 1200f;      // deg/sec^2

    [Header("Angle Calibration")]
    public float headOffsetDeg = 0f;

    [Header("Green Sector (Success)")]
    public float greenCenterDeg = 90f;
    public float greenHalfWidthDeg = 8f;

    public bool hasSecondGreenAtBottom = true;

    enum State { Idle, Spinning, Decel }
    State state = State.Idle;

    float speed;

    void Awake()
    {
        startButton.onClick.AddListener(StartSpin);
        stopButton.onClick.AddListener(StopSpin);

        SetButtons(true);
        if (resultText) resultText.text = "";
    }

    void Update()
    {
        if (human == null) return;

        if (state == State.Spinning)
        {
            RotateHuman(speed);
        }
        else if (state == State.Decel)
        {
            speed = Mathf.Max(0f, speed - decel * Time.deltaTime);
            RotateHuman(speed);

            if (speed <= 0.01f)
            {
                speed = 0f;
                state = State.Idle;
                SetButtons(true);
                Judge();
            }
        }
    }

    void RotateHuman(float degPerSec)
    {
        human.Rotate(0f, 0f, -degPerSec * Time.deltaTime);
    }

    public void StartSpin()
    {
        if (state != State.Idle) return;

        speed = UnityEngine.Random.Range(minSpeed, maxSpeed);
        state = State.Spinning;
        SetButtons(false);
        if (resultText) resultText.text = "Spinning...";
    }

    public void StopSpin()
    {
        if (state != State.Spinning) return;

        state = State.Decel;
        if (resultText) resultText.text = "Stopping...";
    }

    void Judge()
    {
        Vector2 dir = human.up;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // -180~180
        angle = Normalize(angle + headOffsetDeg);               // 0~360

        bool success = InGreen(angle);

        if (resultText)
            resultText.text = success ? $"Success! angle={angle:F1}°" : $"Fail. angle={angle:F1}°";

        if (success && switcher != null)
        {
            switcher.ExitMinigameOnSuccess();
        }
    }

    bool InGreen(float angle)
    {
        bool top = InRangeAngle(angle, greenCenterDeg, greenHalfWidthDeg);

        if (!hasSecondGreenAtBottom) return top;

        float bottomCenter = Normalize(greenCenterDeg + 180f);
        bool bottom = InRangeAngle(angle, bottomCenter, greenHalfWidthDeg);

        return top || bottom;
    }

    bool InRangeAngle(float angle, float center, float halfWidth)
    {
        float d = Mathf.DeltaAngle(angle, center); // -180~180
        return Mathf.Abs(d) <= halfWidth;
    }

    float Normalize(float a)
    {
        a %= 360f;
        if (a < 0f) a += 360f;
        return a;
    }

    void SetButtons(bool idle)
    {
        startButton.interactable = idle;
        stopButton.interactable = !idle;
    }
}