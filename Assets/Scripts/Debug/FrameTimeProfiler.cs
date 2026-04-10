using System;
using TMPro;
using UnityEngine;

public class FrameTimeProfiler : MonoBehaviour
{
    private const int BufferSize = 512; // желательно степень двойки
    private readonly float[] _buffer = new float[BufferSize];

    private int _index;
    private int _count;

    private float _time;

    [Header("Settings")]
    [SerializeField] private float _interval = 2f;
    [SerializeField] private float _warmupTime = 3f;
    [SerializeField] private TextMeshProUGUI _text;

    void Awake()
    {
        // отключаем ограничения FPS
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;
    }

    void Update()
    {
        // пропускаем прогрев
        if (Time.time < _warmupTime)
            return;

        float ms = Time.unscaledDeltaTime * 1000f;

        _buffer[_index] = ms;
        _index = (_index + 1) & (BufferSize - 1);

        if (_count < BufferSize)
            _count++;

        _time += Time.unscaledDeltaTime;

        if (_time >= _interval)
        {
            CalculateAndDisplay();
            _time = 0f;
        }
    }

    private void CalculateAndDisplay()
    {
        if (_count == 0)
            return;

        // временный массив (1 аллокация раз в интервал — это ок)
        float[] temp = new float[_count];
        Array.Copy(_buffer, temp, _count);

        Array.Sort(temp);

        float sum = 0f;
        for (int i = 0; i < _count; i++)
            sum += temp[i];

        float avg = sum / _count;
        float p95 = temp[(int)(_count * 0.95f)];
        float p99 = temp[(int)(_count * 0.99f)];
        float max = temp[_count - 1];

        float fps = 1000f / avg;

        if (_text != null)
        {
            _text.text =
                $"FPS: {fps:F1}\n" +
                $"avg: {avg:F2} ms\n" +
                $"p95: {p95:F2} ms\n" +
                $"p99: {p99:F2} ms\n" +
                $"max: {max:F2} ms";
        }

        // лог для сравнения билдов
        //Debug.Log($"[FrameStats] avg:{avg:F2} p95:{p95:F2} p99:{p99:F2} max:{max:F2}");
    }
}
